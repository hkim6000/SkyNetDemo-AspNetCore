using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysFormEV : WebBase
    {
        private XForm xForm = null;
        private string XFormKey = "xform";

        private string ViewTree = "viewtree";
        private string ViewProp = "viewprop";
        private string TreeId = "TreeId";

        private enum TreeGroup
        {
            Base,
            Section,
            Element
        }

        public XysFormEV()
        {
            ViewPart.Fields.AddRange(new List<NameValueFlag>
            {
                new NameValueFlag { name = "FormId", flag = true },
                new NameValueFlag { name = "FormTitle", flag = true },
                new NameValueFlag { name = "FormRef", flag = true },
                new NameValueFlag { name = "FormFlag", flag = true },
                new NameValueFlag { name = "FormModel", flag = true }
            });
        }

        public override void InitialViewData()
        {
            string ssql = " select FormId, FormTitle,FormRef,FormFlag,FormModel from XysForm where FormId = N'" + PartialData + "' ";
            ViewPart.BindData(ssql);

            xForm = (XForm)DeserializeObject(ViewPart.Field("FormModel").value, typeof(XForm));
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons();

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = Translator.Format("editform");

            FilterSection filter = new FilterSection();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "92%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            TextArea text2 = new TextArea(Translator.Format(XFormKey), XFormKey);
            text2.Required = true;
            text2.Wrap.SetStyle(HtmlStyles.display, "none");
            text2.Text.SetStyle(HtmlStyles.width, "100%");
            text2.Text.SetStyle(HtmlStyles.height, "100px");
            text2.Text.InnerText = xForm == null ? string.Empty : SerializeObjectEnc(xForm, typeof(XForm));
            text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            HtmlElementBox elmBtns = new HtmlElementBox();
            elmBtns.DefaultStyle = false;
            elmBtns.AddItem(ViewButtons, 15);
            elmBtns.SetStyle(HtmlStyles.borderBottom, "2px dashed #ccc");

            Stacker elmStacker = new Stacker();
            elmStacker.WrapDefaultStyle = false;
            elmStacker.Wrap.SetStyle(HtmlStyles.width, "");
            elmStacker.AddColumn(TreeViewHtml(), " border:none; width:Calc(100% - 380px);   ", "id:" + ViewTree + ";");
            elmStacker.AddColumn(string.Empty, " border:none; padding:4px; padding-top:12px; padding-left:20px; width:360px;", "id:" + ViewProp + ";");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "92%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "30px 10px 30px 20px");

            elmBox.AddItem(elmBtns, 4);
            elmBox.AddItem(text2, 20);
            elmBox.AddItem(elmStacker, 20);

            string ViewHtml = filter.HtmlText + elmBox.HtmlText;

            HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TreeScript);
            return ViewHtml;
        }

        private string TreeViewHtml()
        {
            string rtnvlu = string.Empty;

            if (xForm == null) return rtnvlu;

            TreeView2.TreeItem TreeItem = new TreeView2.TreeItem() { Id = xForm.FormId, ParentId = string.Empty };
            TreeItem.Item.InnerText = xForm.Title == string.Empty ? "[No Title]" : xForm.Title;
            TreeItem.Item.SetAttribute(HtmlAttributes.data("tag"), TreeGroup.Base.ToString());
            TreeItem.Item.SetStyles("min-width:120px; border:2px solid #1C698C; ");

            List<TreeView2.TreeItem> TreeItems = new List<TreeView2.TreeItem>();
            TreeItems.Add(TreeItem);

            var OrdSecs = xForm.Sections.OrderBy(x => x.Seq).ToList();

            for (int i = 0; i < OrdSecs.Count; i++)
            {
                TreeView2.TreeItem SectionTreeItem = new TreeView2.TreeItem() { Id = OrdSecs[i].SectionId, ParentId = xForm.FormId };
                SectionTreeItem.Item.InnerText = OrdSecs[i].Label.Trim() == string.Empty ? "[" + OrdSecs[i].Name + "]" : OrdSecs[i].Label;
                SectionTreeItem.Item.SetAttribute(HtmlAttributes.data("tag"), TreeGroup.Section.ToString() + "." + OrdSecs[i].Seq.ToString());
                SectionTreeItem.Item.SetStyles("min-width:120px; border:2px solid #2db331; ");

                if (OrdSecs[i].Elements.Count > 0)
                {
                    SectionTreeItem.AddSubItem(TreeSubItems(OrdSecs[i].SectionId), "max-width:400px;white-space:normal;padding:12px;", HtmlAttributes.data("tag") + ":" + TreeGroup.Element.ToString());
                }

                TreeItems.Add(SectionTreeItem);
            }

            TreeView2 _TreeView = new TreeView2(TreeId)
            {
                TreeItems = TreeItems,
                TreeItemGap = 10,
                TreeItemClick = string.Empty,
                TreeItemRightClick = "TreeSelected('" + this.GetType().Name + "',args)",
                TreeSubItemClick = string.Empty,
                TreeSubItemRightClick = string.Empty
            };

            _TreeView.SetStyle(HtmlStyles.marginTop, "0px");
            _TreeView.SetStyle(HtmlStyles.marginBottom, "40px");
            rtnvlu = _TreeView.HtmlText;

            return rtnvlu;
        }

        private string TreeSubItems(string SuperId)
        {
            string rtnvlu = string.Empty;

            var OrdElms = xForm.Sections.Find(x => x.SectionId == SuperId).Elements.OrderBy(y => y.Seq).ToList();

            for (int i = 0; i < OrdElms.Count; i++)
            {
                string id = OrdElms[i].ElementId;
                string parentid = SuperId;
                string label = OrdElms[i].Label.Trim() == string.Empty ? "[" + OrdElms[i].Name + "]" : OrdElms[i].Label;
                string seq = OrdElms[i].Seq.ToString();
                string tag = OrdElms[i].Name;
                int newline = OrdElms[i].NewLineAfter;
                int uitypeInt = OrdElms[i].UIType;
                UITypes uitype = (UITypes)uitypeInt;

                if (SuperId == parentid)
                {
                    Button btn = new Button();
                    btn.SetStyle(HtmlStyles.display, "inline-block");

                    switch (uitype)
                    {
                        case UITypes.None:
                            btn.SetStyle(HtmlStyles.backgroundColor, "#e8e8e8");
                            break;
                        case UITypes.DataList:
                            btn.SetStyle(HtmlStyles.backgroundColor, "#eafcdf");
                            break;
                        default:
                            btn.SetStyle(HtmlStyles.backgroundColor, "#fffaeb");
                            break;
                    }

                    if (OrdElms[i].IsVisible == 0)
                    {
                        btn.SetStyle(HtmlStyles.fontStyle, "italic");
                        btn.SetStyle(HtmlStyles.color, "#aaa");
                    }

                    btn.SetStyle(HtmlStyles.border, "1px solid #d0d0d0");
                    btn.SetStyle(HtmlStyles.borderRadius, "4px");
                    btn.SetStyle(HtmlStyles.fontWeight, "bold");
                    btn.SetStyle(HtmlStyles.fontSize, "14px");
                    btn.SetStyle(HtmlStyles.width, string.Empty);
                    btn.SetStyle(HtmlStyles.maxWidth, "150px");
                    btn.SetStyle(HtmlStyles.margin, "4px");
                    btn.SetStyle(HtmlStyles.padding, "12px");
                    btn.SetStyle(HtmlStyles.boxShadow, "0 2px 4px 0 rgba(0, 0, 0, 0.2), 0 3px 10px 0 rgba(0, 0, 0, 0.19)");
                    btn.SetAttribute(HtmlEvents.oncontextmenu, "return false;");
                    btn.SetAttribute(HtmlEvents.onmouseup, "TreeSelected('" + this.GetType().Name + "',event, this)");
                    btn.SetAttribute(HtmlAttributes.id, id);
                    btn.SetAttribute(HtmlAttributes.name, tag);
                    btn.SetAttribute(HtmlAttributes.value, label);
                    btn.SetAttribute(HtmlAttributes.data_type, SuperId);
                    btn.SetAttribute(HtmlAttributes.data_name, this.GetType().Name);
                    btn.SetAttribute(HtmlAttributes.data("tag"), TreeGroup.Element.ToString() + "." + OrdElms[i].Seq.ToString());
                    btn.SetAttribute(HtmlEvents.ondrop, "drop(event)");
                    btn.SetAttribute(HtmlEvents.ondragstart, "drag(event)");
                    btn.SetAttribute(HtmlAttributes.draggable, "true");

                    rtnvlu += btn.HtmlText;
                    if (newline != 0)
                    {
                        LinePitch block = new LinePitch();
                        rtnvlu += block.HtmlText;
                    }
                }
            }
            return rtnvlu;
        }

        public ApiResponse TreeSelected()
        {
            string itmid = GetDataValue("o").Split('_').Last();
            string itmtag = GetDataValue("g");
            string LocY = GetDataValue("y");
            string LocX = GetDataValue("x");

            string TreeMenuPopUp = TreeItemMenu(itmid, itmtag, (int)Convert.ToDouble(LocY), (int)Convert.ToDouble(LocX));

            ApiResponse _ApiResponse = new ApiResponse();
            if (TreeMenuPopUp != string.Empty)
            {
                _ApiResponse.PopUpMenu(TreeMenuPopUp);
            }

            return _ApiResponse;
        }

        private string TreeItemMenu(string itmid, string itmtag, int top, int left)
        {
            string rtnvlu = string.Empty;
            string xformVlu = ParamValue(XFormKey);

            if (!string.IsNullOrEmpty(xformVlu))
            {
                xForm = (XForm)DeserializeObjectEnc(xformVlu, typeof(XForm));
                string LocX = GetDataValue("x");

                MenuList MenuItems = new MenuList();
                MenuItems.SetVirticalFixed("treemenu", top, left);

                string prefix = itmtag.Split('.').FirstOrDefault();

                if (prefix == TreeGroup.Base.ToString())
                {
                    MenuItems.Add(Translator.Format("addsection"), string.Empty, "onclick:AddSectionPop('" + this.GetType().Name + "','" + itmid + "')");
                }
                else if (prefix == TreeGroup.Section.ToString())
                {
                    MenuItems.Add(Translator.Format("addelement"), string.Empty, "onclick:AddElementPop('" + this.GetType().Name + "','" + itmid + "')");

                    var _FormSections = xForm.Sections.OrderBy(x => x.Seq).ToList();
                    if (_FormSections.Count > 1)
                    {
                        int acnt = 0;
                        if (_FormSections[0].SectionId != itmid) acnt += 1;
                        if (_FormSections[_FormSections.Count - 1].SectionId != itmid) acnt += 1;
                        if (acnt > 0) MenuItems.Add(new LinePitch());

                        if (_FormSections[0].SectionId != itmid)
                        {
                            MenuItems.Add(Translator.Format("moveup"), string.Empty, "onclick:MoveUpSection('" + this.GetType().Name + "','" + itmid + "')");
                        }
                        if (_FormSections[_FormSections.Count - 1].SectionId != itmid)
                        {
                            MenuItems.Add(Translator.Format("movedown"), string.Empty, "onclick:MoveDownSection('" + this.GetType().Name + "','" + itmid + "')");
                        }
                    }
                    MenuItems.Add(new LinePitch());
                    MenuItems.Add(Translator.Format("delete"), string.Empty, "onclick:DeleteSection('" + this.GetType().Name + "','" + itmid + "')");
                }
                else if (prefix == TreeGroup.Element.ToString())
                {
                    XFormSection _FormSection = xForm.Sections.FirstOrDefault(s => s.Elements.Any(x => x.ElementId == itmid));

                    if (_FormSection != null)
                    {
                        int acnt = 0;
                        var _FormElements = _FormSection.Elements.OrderBy(x => x.Seq).ToList();
                        if (_FormElements != null && _FormElements.Count > 1)
                        {
                            if (_FormElements[0].ElementId != itmid)
                            {
                                acnt += 1;
                                MenuItems.Add(Translator.Format("moveup"), string.Empty, "onclick:MoveUpElement('" + this.GetType().Name + "','" + itmid + "')");
                            }
                            if (_FormElements[_FormElements.Count - 1].ElementId != itmid)
                            {
                                acnt += 1;
                                MenuItems.Add(Translator.Format("movedown"), string.Empty, "onclick:MoveDownElement('" + this.GetType().Name + "','" + itmid + "')");
                            }
                        }
                        if (acnt > 0) MenuItems.Add(new LinePitch());
                        MenuItems.Add(Translator.Format("delete"), string.Empty, "onclick:DeleteElement('" + this.GetType().Name + "','" + itmid + "')");
                    }
                }
                rtnvlu = MenuItems.HtmlText;
            }
            return rtnvlu;
        }

        public ApiResponse AddSectionPop()
        {
            string itmid = GetDataValue("o");

            Label lbl = new Label();
            lbl.Wrap.InnerText = Translator.Format("addsection");
            lbl.Wrap.SetStyles("margin-left:6px; font-size:24px;");

            Texts txt = new Texts();
            txt.Required = true;
            txt.Label.InnerText = Translator.Format("sectionname");
            txt.Label.SetStyle(HtmlStyles.fontSize, "14px");
            txt.Text.SetStyle(HtmlStyles.fontSize, "16px");
            txt.Text.SetStyle(HtmlStyles.width, "260px");
            txt.Text.SetAttribute(HtmlEvents.onblur, "$objblur(event, this)");
            txt.Text.SetId("sectionname");

            Texts txt1 = new Texts();
            txt1.Label.InnerText = Translator.Format("sectionlabel");
            txt1.Label.SetStyle(HtmlStyles.fontSize, "14px");
            txt1.Text.SetStyle(HtmlStyles.fontSize, "16px");
            txt1.Text.SetStyle(HtmlStyles.width, "260px");
            txt1.Text.SetAttribute(HtmlEvents.onblur, "$objblur(event, this)");
            txt1.Text.SetId("sectionlabel");

            Button btn = new Button();
            btn.SetStyle(HtmlStyles.width, "100px");
            btn.SetAttribute(HtmlAttributes.@class, "button");
            btn.SetAttribute(HtmlAttributes.value, Translator.Format("add"));
            btn.SetAttribute(HtmlEvents.onclick, "AddSection('" + this.GetType().Name + "','" + itmid + "')");

            Button btn1 = new Button();
            btn1.SetStyle(HtmlStyles.width, "100px");
            btn1.SetAttribute(HtmlAttributes.@class, "button1");
            btn1.SetAttribute(HtmlAttributes.value, Translator.Format("cancel"));
            btn1.SetAttribute(HtmlEvents.onclick, "$PopOff()");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetStyle(HtmlStyles.padding, "16px");
            elmBox.AddItem(lbl, 20);
            elmBox.AddItem(txt, 10);
            elmBox.AddItem(txt1, 20);
            elmBox.AddItem(btn);
            elmBox.AddItem(btn1);

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.PopUpWindow(elmBox.HtmlText);
            return _ApiResponse;
        }

        public ApiResponse AddSection()
        {
            string itemid = GetDataValue("o");
            string secnm = GetDataValue("n");
            string seclbl = GetDataValue("m");
            string xformVlu = ParamValue(XFormKey);

            ApiResponse _ApiResponse = new ApiResponse();

            if (!string.IsNullOrEmpty(xformVlu))
            {
                if (string.IsNullOrEmpty(secnm) || IsAnySpecialChar(secnm))
                {
                    _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
                }
                else
                {
                    xForm = (XForm)DeserializeObjectEnc(xformVlu, typeof(XForm));
                    if (xForm.Sections == null) xForm.Sections = new List<XFormSection>();

                    XFormSection xfs = new XFormSection();
                    xfs.SectionId = NewID();
                    xfs.Name = secnm.Replace(" ", "_");
                    xfs.Label = seclbl;
                    xfs.Seq = xForm.Sections.Count + 1;

                    xForm.Sections.Add(xfs);

                    _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                    _ApiResponse.SetElementContents(XFormKey, SerializeObjectEnc(xForm, typeof(XForm)));
                }
            }
            return _ApiResponse;
        }

        public ApiResponse MoveUpSection()
        {
            string itmid = GetDataValue("o");
            string xformVlu = ParamValue(XFormKey);

            ApiResponse _ApiResponse = new ApiResponse();
            if (!string.IsNullOrEmpty(xformVlu))
            {
                xForm = (XForm)DeserializeObjectEnc(ParamValue(XFormKey), typeof(XForm));
                if (xForm != null)
                {
                    XFormSection cSection = xForm.Sections.Find(x => x.SectionId == itmid);
                    int csecseq = cSection.Seq - 1;

                    XFormSection xSection = xForm.Sections.Find(x => x.Seq == csecseq);
                    xSection.Seq = cSection.Seq;
                    cSection.Seq = csecseq;

                    _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                    _ApiResponse.SetElementContents(XFormKey, SerializeObjectEnc(xForm, typeof(XForm)));
                }
            }
            return _ApiResponse;
        }

        public ApiResponse MoveDownSection()
        {
            string itmid = GetDataValue("o");
            string xformVlu = ParamValue(XFormKey);

            ApiResponse _ApiResponse = new ApiResponse();
            if (!string.IsNullOrEmpty(xformVlu))
            {
                xForm = (XForm)DeserializeObjectEnc(ParamValue(XFormKey), typeof(XForm));
                if (xForm != null)
                {
                    XFormSection cSection = xForm.Sections.Find(x => x.SectionId == itmid);
                    int csecseq = cSection.Seq + 1;

                    XFormSection xSection = xForm.Sections.Find(x => x.Seq == csecseq);
                    xSection.Seq = cSection.Seq;
                    cSection.Seq = csecseq;

                    _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                    _ApiResponse.SetElementContents(XFormKey, SerializeObjectEnc(xForm, typeof(XForm)));
                }
            }
            return _ApiResponse;
        }

        public ApiResponse DeleteSection()
        {
            string itmid = GetDataValue("o");
            string xformVlu = ParamValue(XFormKey);

            ApiResponse _ApiResponse = new ApiResponse();
            if (!string.IsNullOrEmpty(xformVlu))
            {
                xForm = (XForm)DeserializeObjectEnc(ParamValue(XFormKey), typeof(XForm));
                if (xForm != null)
                {
                    XFormSection _FormSection = xForm.Sections.Find(x => x.SectionId == itmid);
                    if (_FormSection != null)
                    {
                        xForm.Sections.Remove(_FormSection);
                        _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                        _ApiResponse.SetElementContents(XFormKey, SerializeObjectEnc(xForm, typeof(XForm)));
                    }
                }
            }
            return _ApiResponse;
        }

        public ApiResponse AddElementPop()
        {
            string itmid = GetDataValue("o");

            Label lbl = new Label();
            lbl.Wrap.InnerText = Translator.Format("addelement");
            lbl.Wrap.SetStyles("margin-left:6px; font-size:24px;");

            Texts txt = new Texts();
            txt.Required = true;
            txt.Label.InnerText = Translator.Format("elementname");
            txt.Label.SetStyle(HtmlStyles.fontSize, "14px");
            txt.Text.SetStyle(HtmlStyles.fontSize, "16px");
            txt.Text.SetStyle(HtmlStyles.width, "260px");
            txt.Text.SetAttribute(HtmlEvents.onblur, "$objblur(event, this)");
            txt.Text.SetId("elementname");

            Texts txt1 = new Texts();
            txt1.Label.InnerText = Translator.Format("elementlabel");
            txt1.Label.SetStyle(HtmlStyles.fontSize, "14px");
            txt1.Text.SetStyle(HtmlStyles.fontSize, "16px");
            txt1.Text.SetStyle(HtmlStyles.width, "260px");
            txt1.Text.SetAttribute(HtmlEvents.onblur, "$objblur(event, this)");
            txt1.Text.SetId("elementlabel");

            Button btn = new Button();
            btn.SetStyle(HtmlStyles.width, "100px");
            btn.SetAttribute(HtmlAttributes.@class, "button");
            btn.SetAttribute(HtmlAttributes.value, Translator.Format("add"));
            btn.SetAttribute(HtmlEvents.onclick, "AddElement('" + this.GetType().Name + "','" + itmid + "')");

            Button btn1 = new Button();
            btn1.SetStyle(HtmlStyles.width, "100px");
            btn1.SetAttribute(HtmlAttributes.@class, "button1");
            btn1.SetAttribute(HtmlAttributes.value, Translator.Format("cancel"));
            btn1.SetAttribute(HtmlEvents.onclick, "$PopOff()");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetStyle(HtmlStyles.padding, "16px");
            elmBox.AddItem(lbl, 20);
            elmBox.AddItem(txt, 10);
            elmBox.AddItem(txt1, 20);
            elmBox.AddItem(btn);
            elmBox.AddItem(btn1);

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.PopUpWindow(elmBox.HtmlText);
            return _ApiResponse;
        }

        public ApiResponse AddElement()
        {
            string itemid = GetDataValue("o");
            string elmnm = GetDataValue("n");
            string elmlbl = GetDataValue("m");
            string xformVlu = ParamValue(XFormKey);

            ApiResponse _ApiResponse = new ApiResponse();

            if (!string.IsNullOrEmpty(xformVlu))
            {
                if (string.IsNullOrEmpty(elmnm) || IsAnySpecialChar(elmnm))
                {
                    _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
                }
                else
                {
                    xForm = (XForm)DeserializeObjectEnc(xformVlu, typeof(XForm));

                    XFormSection _FormSection = xForm.Sections.Find(x => x.SectionId == itemid);
                    if (_FormSection != null)
                    {
                        XFormElement xfe = new XFormElement();
                        xfe.ElementId = NewID();
                        xfe.Name = elmnm.Replace(" ", "_");
                        xfe.Label = elmlbl;
                        xfe.Seq = _FormSection.Elements.Count + 1;
                        _FormSection.Elements.Add(xfe);
                    }

                    _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                    _ApiResponse.SetElementContents(XFormKey, SerializeObjectEnc(xForm, typeof(XForm)));
                }
            }
            return _ApiResponse;
        }

        public ApiResponse MoveUpElement()
        {
            string itmid = GetDataValue("o");
            string xformVlu = ParamValue(XFormKey);

            ApiResponse _ApiResponse = new ApiResponse();
            if (!string.IsNullOrEmpty(xformVlu))
            {
                xForm = (XForm)DeserializeObjectEnc(ParamValue(XFormKey), typeof(XForm));
                if (xForm != null)
                {
                    XFormSection _FormSection = xForm.Sections.FirstOrDefault(s => s.Elements.Any(x => x.ElementId == itmid));
                    if (_FormSection != null)
                    {
                        XFormElement cElement = _FormSection.Elements.Find(x => x.ElementId == itmid);
                        int celmseq = cElement.Seq - 1;

                        XFormElement xElement = _FormSection.Elements.Find(x => x.Seq == celmseq);
                        xElement.Seq = cElement.Seq;
                        cElement.Seq = celmseq;

                        _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                        _ApiResponse.SetElementContents(XFormKey, SerializeObjectEnc(xForm, typeof(XForm)));
                    }
                }
            }
            return _ApiResponse;
        }

        public ApiResponse MoveDownElement()
        {
            string itmid = GetDataValue("o");
            string xformVlu = ParamValue(XFormKey);

            ApiResponse _ApiResponse = new ApiResponse();
            if (!string.IsNullOrEmpty(xformVlu))
            {
                xForm = (XForm)DeserializeObjectEnc(ParamValue(XFormKey), typeof(XForm));
                if (xForm != null)
                {
                    XFormSection _FormSection = xForm.Sections.FirstOrDefault(s => s.Elements.Any(x => x.ElementId == itmid));
                    if (_FormSection != null)
                    {
                        XFormElement cElement = _FormSection.Elements.Find(x => x.ElementId == itmid);
                        int celmseq = cElement.Seq + 1;

                        XFormElement xElement = _FormSection.Elements.Find(x => x.Seq == celmseq);
                        xElement.Seq = cElement.Seq;
                        cElement.Seq = celmseq;

                        _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                        _ApiResponse.SetElementContents(XFormKey, SerializeObjectEnc(xForm, typeof(XForm)));
                    }
                }
            }
            return _ApiResponse;
        }

        public ApiResponse DeleteElement()
        {
            string itmid = GetDataValue("o");
            string xformVlu = ParamValue(XFormKey);

            ApiResponse _ApiResponse = new ApiResponse();
            if (!string.IsNullOrEmpty(xformVlu))
            {
                xForm = (XForm)DeserializeObjectEnc(ParamValue(XFormKey), typeof(XForm));
                if (xForm != null)
                {
                    XFormSection _FormSection = xForm.Sections.FirstOrDefault(s => s.Elements.Any(x => x.ElementId == itmid));
                    if (_FormSection != null)
                    {
                        XFormElement FormElement = _FormSection.Elements.Find(x => x.ElementId == itmid);
                        if (FormElement != null)
                        {
                            _FormSection.Elements.Remove(FormElement);
                            var _FormElements = _FormSection.Elements.OrderBy(x => x.Seq).ToList();
                            for (int i = 0; i < _FormElements.Count; i++)
                            {
                                _FormElements[i].Seq = i + 1;
                            }

                            _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                            _ApiResponse.SetElementContents(XFormKey, SerializeObjectEnc(xForm, typeof(XForm)));
                        }
                    }
                }
            }
            return _ApiResponse;
        }

        public ApiResponse SwitchElements()
        {
            string srcid = GetDataValue("s");
            string trgid = GetDataValue("t");
            string xformVlu = ParamValue(XFormKey);

            ApiResponse _ApiResponse = new ApiResponse();
            if (!string.IsNullOrEmpty(xformVlu))
            {
                xForm = (XForm)DeserializeObjectEnc(xformVlu, typeof(XForm));
                if (xForm != null)
                {
                    XFormSection srcSec = xForm.Sections.FirstOrDefault(s => s.Elements.Any(x => x.ElementId == srcid));
                    XFormSection trgSec = xForm.Sections.FirstOrDefault(s => s.Elements.Any(x => x.ElementId == trgid));

                    XFormElement src = srcSec.Elements.Find(z => z.ElementId == srcid);
                    XFormElement trg = trgSec.Elements.Find(z => z.ElementId == trgid);
                    if (src != null && trg != null)
                    {
                        int srcseq = src.Seq;
                        int trgseq = trg.Seq;

                        src.Seq = trgseq;
                        trg.Seq = srcseq;

                        _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                        _ApiResponse.SetElementContents(XFormKey, SerializeObjectEnc(xForm, typeof(XForm)));
                    }
                }
            }
            return _ApiResponse;
        }

        public ApiResponse TreeClicked()
        {
            string itmid = GetDataValue("o").Split('_').Last();
            string itmtag = GetDataValue("g");

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.SetElementContents(ViewProp, ViewPropHtml(itmid, itmtag));
            _ApiResponse.SetElementStyle(itmid, HtmlStyles.borderColor, "#ff6666");

            return _ApiResponse;
        }

        private string ViewPropHtml(string itmid, string itmtag)
        {
            string rtnvlu = string.Empty;
            string xformVlu = ParamValue(XFormKey);
            if (!string.IsNullOrEmpty(xformVlu))
            {
                xForm = (XForm)DeserializeObjectEnc(ParamValue(XFormKey), typeof(XForm));

                string prefix = itmtag.Split('.').FirstOrDefault();
                if (prefix == TreeGroup.Base.ToString())
                {
                    rtnvlu = PropRow(itmid, TreeGroup.Base);
                }
                else if (prefix == TreeGroup.Section.ToString())
                {
                    rtnvlu = PropRow(itmid, TreeGroup.Section);
                }
                else if (prefix == TreeGroup.Element.ToString())
                {
                    rtnvlu = PropRow(itmid, TreeGroup.Element);
                }
            }
            return rtnvlu;
        }

        private string PropRow(string itmid, TreeGroup group)
        {
            string rtnvlu = string.Empty;

            Wrap wrap = new Wrap();
            wrap.SetStyle(HtmlStyles.boxSizing, "border-box");
            wrap.SetStyle(HtmlStyles.padding, "4px");
            wrap.SetStyle(HtmlStyles.paddingTop, "8px");
            wrap.SetStyle(HtmlStyles.paddingLeft, "6px");
            wrap.SetStyle(HtmlStyles.backgroundColor, "#fff");
            wrap.SetStyle(HtmlStyles.border, "1px solid #bbb");
            wrap.SetStyle(HtmlStyles.boxShadow, "0 2px 4px 0 rgba(0, 0, 0, 0.2), 0 3px 10px 0 rgba(0, 0, 0, 0.19)");

            Label gridLabel = new Label();
            gridLabel.Wrap.InnerText = EnumName(typeof(TreeGroup), (int)group);
            gridLabel.Wrap.SetStyle(HtmlStyles.color, "#666");
            gridLabel.Wrap.SetStyle(HtmlStyles.fontWeight, "bold");

            Grid gridX = new Grid();
            gridX.Table.SetAttribute(HtmlAttributes.@class, "tableX");
            gridX.Table.SetStyle(HtmlStyles.border, "1px solid #bbb");
            gridX.Table.SetStyle(HtmlStyles.marginTop, "4px");
            gridX.Table.SetStyle(HtmlStyles.marginBottom, "4px");
            gridX.Table.SetStyle(HtmlStyles.fontSize, "14px");
            gridX.AddColumn(Translator.Format("property"));
            gridX.AddColumn(Translator.Format("value"));

            gridX.SetColumnStyle(0, HtmlStyles.color, "#444");
            gridX.SetColumnStyle(0, HtmlStyles.textAlign, "left");
            gridX.SetColumnStyle(0, HtmlStyles.whiteSpace, "nowrap");
            gridX.SetColumnStyle(1, HtmlStyles.whiteSpace, "nowrap");
            gridX.SetColumnStyle(1, HtmlStyles.textAlign, "center");

            string name = string.Empty;
            string value = string.Empty;
            string elmvlu = string.Empty;

            if (group == TreeGroup.Base)
            {
                List<string> Fields = TypeFieldNames(typeof(XForm));
                for (int i = 0; i < Fields.Count; i++)
                {
                    if (Fields[i].ToLower() != "sections")
                    {
                        name = Translator.Format(Fields[i].ToLower());
                        value = string.Empty;
                        elmvlu = TypeFieldGetValue(typeof(XForm), Fields[i], xForm).ToString();

                        switch (Fields[i].ToLower())
                        {
                            case "formid":
                                name = string.Empty;
                                break;
                            case "formref":
                            case "title":
                                value = UIInput(group, itmid, "text", Fields[i], elmvlu);
                                break;
                            case "formflag":
                            case "istitle":
                            case "isborderline":
                            case "isattached":
                                value = UIInput(group, itmid, "checkbox", Fields[i], elmvlu);
                                break;
                            case "titleattributes":
                            case "titlestyles":
                            case "wrapattributes":
                            case "wrapstyles":
                            case "description":
                                value = UIInput(group, itmid, "textarea", Fields[i], elmvlu);
                                break;
                            case "paper":
                                OptionValues opP = new OptionValues(typeof(UIForm.Papers), elmvlu);
                                value = UIInput(group, itmid, "dropdown", Fields[i], elmvlu, opP.OptionsHtml());
                                break;
                            case "orientation":
                                OptionValues opO = new OptionValues(typeof(UIForm.Orientations), elmvlu);
                                value = UIInput(group, itmid, "dropdown", Fields[i], elmvlu, opO.OptionsHtml());
                                break;
                            case "margintop":
                            case "marginleft":
                                value = UIInput(group, itmid, "number", Fields[i], elmvlu);
                                break;
                        }
                        if (!string.IsNullOrEmpty(name)) gridX.AddRow(new string[] { name, value });
                    }
                }
            }
            else if (group == TreeGroup.Section)
            {
                List<string> Fields = TypeFieldNames(typeof(XFormSection));
                XFormSection Section = xForm.Sections.Find(x => x.SectionId == itmid);
                if (Section != null)
                {
                    gridLabel.Wrap.InnerText += " - " + Section.Seq.ToString();

                    for (int i = 0; i < Fields.Count; i++)
                    {
                        if (Fields[i].ToLower() != "elements")
                        {
                            name = Translator.Format(Fields[i].ToLower());
                            value = string.Empty;
                            elmvlu = TypeFieldGetValue(typeof(XFormSection), Fields[i], Section).ToString();

                            switch (Fields[i].ToLower())
                            {
                                case "sectionid":
                                case "seq":
                                    name = string.Empty;
                                    break;
                                case "name":
                                case "label":
                                case "refid":
                                    value = UIInput(group, itmid, "text", Fields[i], elmvlu);
                                    break;
                                case "labelattributes":
                                case "labelstyles":
                                case "wrapattributes":
                                case "wrapstyles":
                                    value = UIInput(group, itmid, "textarea", Fields[i], elmvlu);
                                    break;
                                case "columns":
                                    OptionValues opC = new OptionValues("{0|0}{1|1}{2|2}{3|3}{4|4}{5|5}{6|6}{7|7}{8|8}{9|9}", elmvlu);
                                    value = UIInput(group, itmid, "dropdown", Fields[i], elmvlu, opC.OptionsHtml());
                                    break;
                                case "flexiblerows":
                                    value = UIInput(group, itmid, "checkbox", Fields[i], elmvlu);
                                    break;
                                case "borderline":
                                    value = UIInput(group, itmid, "number", Fields[i], elmvlu);
                                    break;
                            }
                            if (!string.IsNullOrEmpty(name)) gridX.AddRow(new string[] { name, value });
                        }
                    }
                }
            }
            else if (group == TreeGroup.Element)
            {
                List<string> Fields = TypeFieldNames(typeof(XFormElement));
                XFormSection Section = xForm.Sections.FirstOrDefault(s => s.Elements.Any(x => x.ElementId == itmid));
                XFormElement Element = Section.Elements.Find(z => z.ElementId == itmid);

                if (Element != null)
                {
                    string SecLabel = Section.Label == string.Empty ? Section.Name : Section.Label;
                    gridLabel.Wrap.InnerText = SecLabel + " - " + gridLabel.Wrap.InnerText + " - " + Element.Seq.ToString();

                    for (int i = 0; i < Fields.Count; i++)
                    {
                        name = Translator.Format(Fields[i].ToLower());
                        value = string.Empty;
                        elmvlu = TypeFieldGetValue(typeof(XFormElement), Fields[i], Element).ToString();

                        switch (Fields[i].ToLower())
                        {
                            case "elementid":
                            case "seq":
                            case "value":
                                name = string.Empty;
                                break;
                            case "name":
                            case "label":
                            case "refid":
                                value = UIInput(group, itmid, "text", Fields[i], elmvlu);
                                break;
                            case "uitype":
                                OptionValues opU = new OptionValues(typeof(UITypes), elmvlu);
                                value = UIInput(group, itmid, "dropdown", Fields[i], elmvlu, opU.OptionsHtml());
                                break;
                            case "styles":
                            case "attributes":
                            case "wrapstyles":
                            case "wrapattributes":
                            case "labelstyles":
                            case "labelattributes":
                            case "initialvalues":
                                value = UIInput(group, itmid, "textarea", Fields[i], elmvlu);
                                break;
                            case "iskey":
                            case "isvisible":
                            case "isreadonly":
                            case "isrequired":
                            case "isfixedvalue":
                            case "newlineafter":
                                value = UIInput(group, itmid, "checkbox", Fields[i], elmvlu);
                                break;
                            case "rowspan":
                                OptionValues opR = new OptionValues("{0|0}{1|1}{2|2}{3|3}{4|4}{5|5}{6|6}{7|7}{8|8}{9|9}", elmvlu);
                                value = UIInput(group, itmid, "dropdown", Fields[i], elmvlu, opR.OptionsHtml());
                                break;
                            case "valuetype":
                                OptionValues opV = new OptionValues(typeof(ValueTypes), elmvlu);
                                value = UIInput(group, itmid, "dropdown", Fields[i], elmvlu, opV.OptionsHtml());
                                break;
                        }
                        if (!string.IsNullOrEmpty(name)) gridX.AddRow(new string[] { name, value });
                    }
                }
            }

            wrap.InnerText = gridLabel.HtmlText + gridX.HtmlText;
            rtnvlu = wrap.HtmlText;
            return rtnvlu;
        }

        private string UIInput(TreeGroup group, string ItemId, string UIType, string colnm, string value, string optvalues = "")
        {
            string rtnvlu = string.Empty;
            string procedure = this.GetType().Name;

            switch (UIType.ToLower())
            {
                case "textarea":
                    HtmlTag inputT = new HtmlTag(HtmlTags.textarea);
                    inputT.SetAttribute(HtmlAttributes.id, colnm);
                    inputT.SetStyle(HtmlStyles.border, "1px solid #999");
                    inputT.SetStyle(HtmlStyles.borderRadius, "4px");
                    inputT.SetStyle(HtmlStyles.fontFamily, "sans-serif !important");
                    inputT.SetStyle(HtmlStyles.fontSize, "14px !important");
                    inputT.SetStyle(HtmlStyles.padding, "4px !important");
                    inputT.SetStyle(HtmlStyles.resize, "vertical");
                    inputT.SetAttribute(HtmlAttributes.autocomplete, "off");
                    inputT.SetAttribute(HtmlAttributes.spellcheck, "false");
                    inputT.SetAttribute(HtmlEvents.onblur, "SaveProperty('" + procedure + "','" + group.ToString() + "','" + ItemId + "','" + colnm + "',this)");
                    inputT.SetStyle(HtmlStyles.width, "93%");
                    inputT.SetStyle(HtmlStyles.height, "35px");
                    inputT.InnerText = value;
                    rtnvlu = inputT.HtmlText;
                    break;
                case "color":
                case "date":
                case "datetime-local":
                case "email":
                case "file":
                case "hidden":
                case "image":
                case "month":
                case "number":
                case "password":
                case "range":
                case "search":
                case "tel":
                case "text":
                case "time":
                case "url":
                case "week":
                    HtmlTag inputI = new HtmlTag(HtmlTags.input, HtmlTag.Types.Empty);
                    inputI.SetStyle(HtmlStyles.border, "1px solid #999");
                    inputI.SetStyle(HtmlStyles.borderRadius, "4px");
                    inputI.SetStyle(HtmlStyles.fontSize, "14px !important");
                    inputI.SetStyle(HtmlStyles.padding, "4px !important");
                    inputI.SetAttribute(HtmlAttributes.id, colnm);
                    inputI.SetAttribute(HtmlAttributes.type, UIType);
                    inputI.SetAttribute(HtmlAttributes.value, value);
                    inputI.SetAttribute(HtmlEvents.onblur, "SaveProperty('" + procedure + "','" + group.ToString() + "','" + ItemId + "','" + colnm + "',this)");
                    inputI.SetStyle(HtmlStyles.width, "94%");
                    rtnvlu = inputI.HtmlText;
                    break;
                case "checkbox":
                    HtmlTag inputC = new HtmlTag(HtmlTags.input, HtmlTag.Types.Empty);
                    inputC.SetStyle(HtmlStyles.border, "1px solid #999");
                    inputC.SetAttribute(HtmlAttributes.id, colnm);
                    inputC.SetAttribute(HtmlAttributes.type, UIType);
                    inputC.SetAttribute(HtmlEvents.onclick, "SaveProperty('" + procedure + "','" + group.ToString() + "','" + ItemId + "','" + colnm + "',this)");
                    if (Convert.ToDouble(value) == 1)
                    {
                        inputC.SetAttribute(HtmlAttributes.@checked, "checked");
                    }
                    rtnvlu = inputC.HtmlText;
                    break;
                case "dropdown":
                    HtmlTag inputD = new HtmlTag(HtmlTags.select);
                    inputD.SetAttribute(HtmlAttributes.id, colnm);
                    inputD.SetStyle(HtmlStyles.border, "1px solid #999");
                    inputD.SetStyle(HtmlStyles.borderRadius, "4px");
                    inputD.SetStyle(HtmlStyles.width, "100%");
                    inputD.SetStyle(HtmlStyles.fontSize, "14px !important");
                    inputD.SetStyle(HtmlStyles.padding, "4px !important");
                    inputD.SetAttribute(HtmlEvents.onchange, "SaveProperty('" + procedure + "','" + group.ToString() + "','" + ItemId + "','" + colnm + "',this)");
                    inputD.InnerText = optvalues;
                    rtnvlu = inputD.HtmlText;
                    break;
                default:
                    HtmlTag inputE = new HtmlTag(HtmlTags.input, HtmlTag.Types.Empty);
                    inputE.SetStyle(HtmlStyles.border, "1px solid #999");
                    inputE.SetStyle(HtmlStyles.borderRadius, "4px");
                    inputE.SetStyle(HtmlStyles.fontSize, "14px !important");
                    inputE.SetStyle(HtmlStyles.padding, "4px !important");
                    inputE.SetAttribute(HtmlAttributes.id, colnm);
                    inputE.SetAttribute(HtmlAttributes.type, UIType);
                    inputE.SetAttribute(HtmlAttributes.value, value);
                    inputE.SetAttribute(HtmlEvents.onblur, "SaveProperty('" + procedure + "','" + group.ToString() + "','" + ItemId + "','" + colnm + "',this)");
                    inputE.SetStyle(HtmlStyles.width, "94%");
                    rtnvlu = inputE.HtmlText;
                    break;
            }
            return rtnvlu;
        }

        public ApiResponse SaveProperty()
        {
            string group = GetDataValue("g");
            string itmid = GetDataValue("m");
            string colnm = GetDataValue("c");
            string colvlu = GetDataValue("v");
            string xformVlu = ParamValue(XFormKey);

            ApiResponse _ApiResponse = new ApiResponse();

            if (!string.IsNullOrEmpty(xformVlu))
            {
                if (!(colnm.ToLower() == "name" && colvlu == string.Empty))
                {
                    if (colnm.ToLower() == "name") colvlu = colvlu.Replace(" ", "_");

                    xForm = (XForm)DeserializeObjectEnc(ParamValue(XFormKey), typeof(XForm));

                    if (group == TreeGroup.Base.ToString())
                    {
                        string oldvlu = TypeFieldGetValue(typeof(XForm), colnm, xForm).ToString();
                        if (oldvlu != colvlu)
                        {
                            TypeFieldSetValue(typeof(XForm), colnm, xForm, colvlu);
                            _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                            _ApiResponse.SetElementContents(XFormKey, SerializeObjectEnc(xForm, typeof(XForm)));
                        }
                    }
                    else if (group == TreeGroup.Section.ToString())
                    {
                        XFormSection Section = xForm.Sections.Find(x => x.SectionId == itmid);
                        if (Section != null)
                        {
                            if (colnm.ToLower() == "flexiblerows" && Convert.ToDouble(colvlu) == 1)
                            {
                                _ApiResponse.SetElementValue("Columns", "0");
                                TypeFieldSetValue(typeof(XFormSection), "Columns", Section, "0");
                            }
                            if (colnm.ToLower() == "columns" && Convert.ToDouble(colvlu) > 0)
                            {
                                _ApiResponse.SetElementValue("FlexibleRows", "0");
                                TypeFieldSetValue(typeof(XFormSection), "flexiblerows", Section, "0");
                            }

                            string oldvlu = TypeFieldGetValue(typeof(XFormSection), colnm, Section).ToString();
                            if (oldvlu != colvlu)
                            {
                                TypeFieldSetValue(typeof(XFormSection), colnm, Section, colvlu);
                                _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                                _ApiResponse.SetElementContents(XFormKey, SerializeObjectEnc(xForm, typeof(XForm)));
                            }
                        }
                    }
                    else if (group == TreeGroup.Element.ToString())
                    {
                        XFormSection Section = xForm.Sections.FirstOrDefault(s => s.Elements.Any(x => x.ElementId == itmid));
                        XFormElement Element = Section.Elements.Find(z => z.ElementId == itmid);
                        if (Element != null)
                        {
                            string oldvlu = TypeFieldGetValue(typeof(XFormElement), colnm, Element).ToString();
                            if (oldvlu != colvlu)
                            {
                                TypeFieldSetValue(typeof(XFormElement), colnm, Element, colvlu);
                                _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                                _ApiResponse.SetElementContents(XFormKey, SerializeObjectEnc(xForm, typeof(XForm)));
                                _ApiResponse.SetElementStyle(itmid, HtmlStyles.borderColor, "#ff6666");
                            }
                        }
                    }
                }
            }
            return _ApiResponse;
        }

        public ApiResponse ViewCode()
        {
            string page = References.Pages.XysFormCode;
            string xformVlu = ParamValue(XFormKey);

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(page, xformVlu));
            _ApiResponse.ExecuteScript("$ScrollToTop()");
            return _ApiResponse;
        }

        public ApiResponse SaveData()
        {
            string xformVlu = ParamValue(XFormKey);
            ApiResponse _ApiResponse = new ApiResponse();

            if (string.IsNullOrEmpty(xformVlu))
            {
                DialogBox dialogBox = new DialogBox(Translator.Format("msg_damaged"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
            }
            else
            {
                string rlt = PutSaveData();
                if (string.IsNullOrEmpty(rlt))
                {
                    _ApiResponse.PopUpWindow(DialogMsgSaved(string.Empty), References.Elements.PageContents);
                }
                else
                {
                    _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
                }
            }
            return _ApiResponse;
        }

        private string PutSaveData()
        {
            xForm = (XForm)DeserializeObjectEnc(ParamValue(XFormKey), typeof(XForm));
            string formModel = SerializeObject(xForm, typeof(XForm));

            List<string> SQL = new List<string>();
            SQL.Add(" update XysForm set " +
                    " FormTitle = @FormTitle, FormDesc = @FormDesc, FormRef = @FormRef, FormFlag = @FormFlag, FormModel = @FormModel, " +
                    " SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                    " Where FormId = @FormId ");

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@FormId", Value = xForm.FormId, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@FormTitle", Value = xForm.Title, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@FormDesc", Value = xForm.Description, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@FormRef", Value = xForm.FormRef, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@FormFlag", Value = xForm.FormFlag, SqlDbType = SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@FormModel", Value = formModel, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        public ApiResponse DeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.PopUpWindow(DialogQstDelete("XysFormEV/ConfirmDeleteData"), References.Elements.PageContents);
            return _ApiResponse;
        }

        public ApiResponse ConfirmDeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            string rlt = PutDeleteData();
            if (string.IsNullOrEmpty(rlt))
            {
                _ApiResponse.PopUpWindow(DialogMsgDeleted("m=XysFormMV"), References.Elements.PageContents);
            }
            else
            {
                _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
            }
            return _ApiResponse;
        }

        private string PutDeleteData()
        {
            xForm = (XForm)DeserializeObjectEnc(ParamValue(XFormKey), typeof(XForm));

            List<string> SQL = new List<string> { " delete from XysForm Where FormId = @FormId " };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@FormId", Value = xForm.FormId, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        public ApiResponse CopyForm()
        {
            string xformVlu = ParamValue(XFormKey);
            ApiResponse _ApiResponse = new ApiResponse();
            if (!string.IsNullOrEmpty(xformVlu))
            {
                string PopUp = CopyFormPop();
                if (PopUp != string.Empty)
                {
                    _ApiResponse.PopUpWindow(PopUp);
                }
            }
            return _ApiResponse;
        }

        private string CopyFormPop()
        {
            string itmid = GetDataValue("o");

            Label lbl = new Label();
            lbl.Wrap.InnerText = Translator.Format("copyto");
            lbl.Wrap.SetStyles("margin-left:6px; font-size:24px;");

            Texts txt = new Texts();
            txt.Required = true;
            txt.Label.InnerText = Translator.Format("formtitle");
            txt.Label.SetStyle(HtmlStyles.fontSize, "14px");
            txt.Text.SetStyle(HtmlStyles.fontSize, "16px");
            txt.Text.SetStyle(HtmlStyles.width, "260px");
            txt.Text.SetAttribute(HtmlEvents.onblur, "$objblur(event, this)");
            txt.Text.SetId("formtitle");

            Button btn = new Button();
            btn.SetStyle(HtmlStyles.width, "100px");
            btn.SetAttribute(HtmlAttributes.@class, "button");
            btn.SetAttribute(HtmlAttributes.value, Translator.Format("copy"));
            btn.SetAttribute(HtmlEvents.onclick, "CopyFormConfirm('" + this.GetType().Name + "')");

            Button btn1 = new Button();
            btn1.SetStyle(HtmlStyles.width, "100px");
            btn1.SetAttribute(HtmlAttributes.@class, "button1");
            btn1.SetAttribute(HtmlAttributes.value, Translator.Format("cancel"));
            btn1.SetAttribute(HtmlEvents.onclick, "$PopOff()");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetStyle(HtmlStyles.padding, "16px");
            elmBox.AddItem(lbl, 20);
            elmBox.AddItem(txt, 20);
            elmBox.AddItem(btn);
            elmBox.AddItem(btn1);

            return elmBox.HtmlText;
        }

        public ApiResponse CopyFormConfirm()
        {
            string formTitle = ParamValue("formtitle");
            ApiResponse _ApiResponse = new ApiResponse();

            if (string.IsNullOrEmpty(formTitle))
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
            }
            else
            {
                string rlt = PutCopyData();
                if (string.IsNullOrEmpty(rlt))
                {
                    DialogBox dialogBox = new DialogBox(Translator.Format("msg_copied"));
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                    dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button;onclick:" + ByPassCall("PartialView", "m=" + References.Pages.XysFormMV) + "&$PopOff();");
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
                }
                else
                {
                    _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
                }
            }
            return _ApiResponse;
        }

        private string PutCopyData()
        {
            string formTitle = ParamValue("formtitle");
            xForm = (XForm)DeserializeObjectEnc(ParamValue(XFormKey), typeof(XForm));
            string oldId = xForm.FormId;

            xForm.FormId = NewID();
            xForm.Title = formTitle;
            string formModel = SerializeObject(xForm, typeof(XForm));

            List<string> SQL = new List<string> {
                    " insert into XysForm(FormId,FormTitle,FormDesc,FormRef,FormFlag,FormModel,SYSDTE,SYSUSR ) " +
                    " select @FormId,@FormTitle,FormDesc,'',1,@FormModel,getdate(),@SYSUSR " +
                    " from XysForm where FormId = @OldId"
                };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@OldId", Value = oldId, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@FormId", Value = xForm.FormId, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@FormTitle", Value = xForm.Title, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@FormModel", Value = formModel, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        public ApiResponse Preview()
        {
            xForm = (XForm)DeserializeObjectEnc(ParamValue(XFormKey), typeof(XForm));
            string xparam = EncryptString(xForm.FormId);
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.NewWindow(References.Pages.XysFormPrvw + "?x=" + xparam);
            return _ApiResponse;
        }
    }
}
