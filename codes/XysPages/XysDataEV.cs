using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;
using System.Text.RegularExpressions;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysDataEV : WebBase
    {
        private XData xData = null;
        private string XDataKey = "xdata";

        public XysDataEV()
        {
            ViewPart.Fields.AddRange(new NameValueFlag[] {
            new NameValueFlag { name = "DataId", flag = true },
            new NameValueFlag { name = "DataName", flag = true },
            new NameValueFlag { name = "DataDesc", flag = true },
            new NameValueFlag { name = "DataModel", flag = true }
        });
        }

        public override void InitialViewData()
        {
            string ssql = " select DataId, DataName, DataDesc,DataModel from XysData where DataId = N'" + PartialData + "' ";

            ViewPart.BindData(ssql);

            xData = new XData
            {
                DataId = ViewPart.Field("DataId").value,
                DataName = ViewPart.Field("DataName").value,
                DataDesc = ViewPart.Field("DataDesc").value,
                DataElements = (List<XDataElement>)DeserializeObject(ViewPart.Field("DataModel").value, typeof(List<XDataElement>))
            };
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons();

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = Translator.Format("editdata");

            FilterSection filter = new FilterSection();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "92%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            TextArea text2 = new TextArea(Translator.Format(XDataKey), XDataKey);
            text2.Required = true;
            text2.Wrap.SetStyle(HtmlStyles.display, "none");
            text2.Text.SetStyle(HtmlStyles.width, "100%");
            text2.Text.SetStyle(HtmlStyles.height, "100px");
            text2.Text.InnerText = xData == null ? string.Empty : SerializeObjectEnc(xData, typeof(XData));
            text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            HtmlElementBox elmBtns = new HtmlElementBox();
            elmBtns.DefaultStyle = false;
            elmBtns.AddItem(ViewButtons, 15);
            elmBtns.SetStyle(HtmlStyles.borderBottom, "2px dashed #ccc");

            Stacker elmStacker = new Stacker();
            elmStacker.WrapDefaultStyle = false;
            elmStacker.Wrap.SetStyle(HtmlStyles.width, "");
            elmStacker.AddColumn(TreeViewHtml(), " border:none;  width:Calc(100% - 380px);  ", "id:" + ViewTree + ";");
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

        private string ViewTree = "viewtree";
        private string ViewProp = "viewprop";
        private string TreeId = "TreeId";

        private enum TreeGroup
        {
            Base,
            Element
        }

        private string TreeViewHtml()
        {
            string rtnvlu = string.Empty;

            if (xData == null) return rtnvlu;

            TreeView2.TreeItem TreeItem = new TreeView2.TreeItem() { Id = ViewPart.Field("DataId").value, ParentId = string.Empty };
            TreeItem.Item.InnerText = xData.DataDesc;
            TreeItem.Item.SetAttribute(HtmlAttributes.data("tag"), TreeGroup.Base.ToString());
            TreeItem.Item.SetStyle(HtmlStyles.border, "2px solid #1C698C");
            TreeItem.Item.SetStyle(HtmlStyles.minWidth, "120px");

            if (xData != null && xData.DataElements != null && xData.DataElements.Count > 0)
            {
                TreeItem.AddSubItem(TreeSubItems(xData.DataId), "max-width:400px;white-space:normal;padding:12px;", HtmlAttributes.data("tag") + ":" + TreeGroup.Element.ToString());
            }

            List<TreeView2.TreeItem> TreeItems = new List<TreeView2.TreeItem>();
            TreeItems.Add(TreeItem);

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

            List<XDataElement> OrdElms = xData.DataElements.OrderBy(x => x.Seq).ToList();

            for (int i = 0; i < OrdElms.Count; i++)
            {
                string id = OrdElms[i].ElementId;
                string parentid = xData.DataId;
                string label = OrdElms[i].Label != string.Empty ? OrdElms[i].Label : "[" + OrdElms[i].Name + "]";
                string seq = OrdElms[i].Seq.ToString();
                string tag = OrdElms[i].Name;
                int newline = OrdElms[i].LineSpacing;
                UITypes uitype = (UITypes)OrdElms[i].UIType;

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
            string LocY = GetDataValue("y");
            string LocX = GetDataValue("x");

            string TreeMenuPopUp = TreeItemMenu(itmid, (int)Convert.ToDouble(LocY), (int)Convert.ToDouble(LocX));

            ApiResponse _ApiResponse = new ApiResponse();
            if (TreeMenuPopUp != string.Empty)
            {
                _ApiResponse.PopUpMenu(TreeMenuPopUp);
            }

            return _ApiResponse;
        }

        private string TreeItemMenu(string itmid, int top, int left)
        {
            string rtnvlu = string.Empty;

            string xdataVlu = ParamValue(XDataKey);
            if (string.IsNullOrEmpty(xdataVlu) == false)
            {
                xData = (XData)DeserializeObjectEnc(xdataVlu, typeof(XData));

                string LocX = GetDataValue("x");

                MenuList MenuItems = new MenuList() { Alignment = Alignments.Vertical };
                MenuItems.SetVirticalFixed("treemenu", top, left);

                if (itmid == xData.DataId)
                {
                    MenuItems.Add(Translator.Format("addelement"), string.Empty, "onclick:AddElementPop('" + this.GetType().Name + "','" + itmid + "')");
                    MenuItems.Add(new HtmlTag());
                    MenuItems.Add(Translator.Format("dbelements"), string.Empty, "onclick:DbElementPop('" + this.GetType().Name + "','" + itmid + "')");
                }
                else
                {
                    List<XDataElement> _DataElements = xData.DataElements.OrderBy(x => x.Seq).ToList();
                    XDataElement DataElement = xData.DataElements.Find(x => x.ElementId == itmid);
                    if (DataElement != null)
                    {
                        if (xData.DataElements.Count > 1)
                        {
                            if (_DataElements[0].ElementId != itmid)
                            {
                                MenuItems.Add(Translator.Format("moveup"), string.Empty, "onclick:MoveUpElement('" + this.GetType().Name + "','" + itmid + "')");
                            }
                            if (_DataElements[_DataElements.Count - 1].ElementId != itmid)
                            {
                                MenuItems.Add(Translator.Format("movedown"), string.Empty, "onclick:MoveDownElement('" + this.GetType().Name + "','" + itmid + "')");
                            }
                        }
                    }
                    MenuItems.Add(new HtmlTag());
                    MenuItems.Add(Translator.Format("delete"), string.Empty, "onclick:DeleteElement('" + this.GetType().Name + "','" + itmid + "')");
                }
                rtnvlu = MenuItems.HtmlText;
            }

            return rtnvlu;
        }

        public ApiResponse DbElementPop()
        {
            string itmid = GetDataValue("o");

            Label lbl = new Label();
            lbl.Wrap.InnerText = Translator.Format("dbelements");
            lbl.Wrap.SetStyles("margin-left:6px; font-size:24px;");

            Texts txt = new Texts();
            txt.Required = true;
            txt.Label.InnerText = Translator.Format("dbtable");
            txt.Label.SetStyle(HtmlStyles.fontSize, "14px");
            txt.Text.SetStyle(HtmlStyles.fontSize, "16px");
            txt.Text.SetStyle(HtmlStyles.width, "260px");
            txt.Text.SetAttribute(HtmlEvents.onblur, "$objblur(event, this)");
            txt.Text.SetId("dbtable");

            Button btn = new Button();
            btn.SetStyle(HtmlStyles.width, "100px");
            btn.SetAttribute(HtmlAttributes.@class, "button");
            btn.SetAttribute(HtmlAttributes.value, Translator.Format("add"));
            btn.SetAttribute(HtmlEvents.onclick, "DbElement('" + this.GetType().Name + "','" + itmid + "')");

            Button btn1 = new Button();
            btn1.SetStyle(HtmlStyles.width, "100px");
            btn1.SetAttribute(HtmlAttributes.@class, "button1");
            btn1.SetAttribute(HtmlAttributes.value, Translator.Format("cancel"));
            btn1.SetAttribute(HtmlEvents.onclick, "$PopOff()");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetStyle(HtmlStyles.padding, "16px");
            elmBox.AddItem(lbl, 20);
            elmBox.AddItem(txt, 10);
            elmBox.AddItem(btn);
            elmBox.AddItem(btn1);

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.PopUpWindow(elmBox.HtmlText);
            return _ApiResponse;
        }

        public ApiResponse DbElement()
        {
            string itemid = GetDataValue("o");
            string dbtable = GetDataValue("n");
            string xdataVlu = ParamValue(XDataKey);

            ApiResponse _ApiResponse = new ApiResponse();

            if (string.IsNullOrEmpty(xdataVlu) == false)
            {
                if (string.IsNullOrEmpty(dbtable))
                {
                    _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
                }
                else
                {
                    DataTable dt = SQLData.SQLTableSchema(dbtable);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        xData = (XData)DeserializeObjectEnc(xdataVlu, typeof(XData));
                        if (xData.DataElements == null) xData.DataElements = new List<XDataElement>();

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            XDataElement xde = new XDataElement();
                            xde.ElementId = NewID();
                            xde.Name = dt.Rows[i][0].ToString();
                            xde.Label = xde.Name;
                            xde.Seq = xData.DataElements.Count + 1;

                            xData.DataElements.Add(xde);
                        }

                        _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                        _ApiResponse.SetElementContents(XDataKey, SerializeObjectEnc(xData, typeof(XData)));
                    }
                    else
                    {
                        DialogBox dialogBox = new DialogBox(Translator.Format("msg_notfound"));
                        dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                        _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
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
            txt1.Required = true;
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
            string xdataVlu = ParamValue(XDataKey);

            ApiResponse _ApiResponse = new ApiResponse();

            if (string.IsNullOrEmpty(xdataVlu) == false)
            {
                if (string.IsNullOrEmpty(elmnm) || string.IsNullOrEmpty(elmlbl) || IsAnySpecialChar(elmnm) == true)
                {
                    _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
                }
                else
                {
                    xData = (XData)DeserializeObjectEnc(xdataVlu, typeof(XData));
                    if (xData.DataElements == null) xData.DataElements = new List<XDataElement>();

                    XDataElement xde = new XDataElement();
                    xde.ElementId = NewID();
                    xde.Name = elmnm.Replace(" ", "_");
                    xde.Label = elmlbl;
                    xde.Seq = xData.DataElements.Count + 1;

                    xData.DataElements.Add(xde);

                    _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                    _ApiResponse.SetElementContents(XDataKey, SerializeObjectEnc(xData, typeof(XData)));
                }
            }

            return _ApiResponse;
        }

        public ApiResponse DeleteElement()
        {
            string itmid = GetDataValue("o");
            string xdataVlu = ParamValue(XDataKey);

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(xdataVlu) == false)
            {
                xData = (XData)DeserializeObjectEnc(xdataVlu, typeof(XData));
                if (xData != null)
                {
                    XDataElement DataElement = xData.DataElements.Find(x => x.ElementId == itmid);
                    if (DataElement != null)
                    {
                        xData.DataElements.Remove(DataElement);

                        List<XDataElement> _DataElements = xData.DataElements.OrderBy(x => x.Seq).ToList();
                        for (int i = 0; i < _DataElements.Count; i++)
                        {
                            _DataElements[i].Seq = i + 1;
                        }

                        _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                        _ApiResponse.SetElementContents(XDataKey, SerializeObjectEnc(xData, typeof(XData)));
                    }
                }
            }

            return _ApiResponse;
        }

        public ApiResponse MoveUpElement()
        {
            string itmid = GetDataValue("o");
            string xdataVlu = ParamValue(XDataKey);

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(xdataVlu) == false)
            {
                xData = (XData)DeserializeObjectEnc(xdataVlu, typeof(XData));
                if (xData != null)
                {
                    XDataElement DataElement = xData.DataElements.Find(x => x.ElementId == itmid);
                    if (DataElement != null && DataElement.Seq > 1)
                    {
                        int origin = DataElement.Seq;
                        int target = DataElement.Seq - 1;

                        XDataElement DataElement1 = xData.DataElements.Find(x => x.Seq == target);
                        DataElement1.Seq = origin;
                        DataElement.Seq = target;

                        _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                        _ApiResponse.SetElementContents(XDataKey, SerializeObjectEnc(xData, typeof(XData)));
                    }
                }
            }

            return _ApiResponse;
        }

        public ApiResponse MoveDownElement()
        {
            string itmid = GetDataValue("o");
            string xdataVlu = ParamValue(XDataKey);

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(xdataVlu) == false)
            {
                xData = (XData)DeserializeObjectEnc(xdataVlu, typeof(XData));
                if (xData != null)
                {
                    XDataElement DataElement = xData.DataElements.Find(x => x.ElementId == itmid);
                    if (DataElement != null && DataElement.Seq < xData.DataElements.Count)
                    {
                        int origin = DataElement.Seq;
                        int target = DataElement.Seq + 1;

                        XDataElement DataElement1 = xData.DataElements.Find(x => x.Seq == target);
                        DataElement1.Seq = origin;
                        DataElement.Seq = target;

                        _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                        _ApiResponse.SetElementContents(XDataKey, SerializeObjectEnc(xData, typeof(XData)));
                    }
                }
            }

            return _ApiResponse;
        }

        public ApiResponse SwitchElements()
        {
            string srcid = GetDataValue("s");
            string trgid = GetDataValue("t");
            string xdataVlu = ParamValue(XDataKey);

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(xdataVlu) == false)
            {
                xData = (XData)DeserializeObjectEnc(xdataVlu, typeof(XData));
                if (xData != null)
                {
                    XDataElement src = xData.DataElements.Find(x => x.ElementId == srcid);
                    XDataElement trg = xData.DataElements.Find(x => x.ElementId == trgid);
                    if (src != null && trg != null)
                    {
                        int trgseq = trg.Seq;
                        List<XDataElement> Elms = xData.DataElements.FindAll(x => x.Seq >= trgseq).ToList();

                        for (int i = 0; i < Elms.Count; i++)
                        {
                            Elms[i].Seq += 1;
                        }
                        src.Seq = trgseq;

                        List<XDataElement> OrdElms = xData.DataElements.OrderBy(x => x.Seq).ToList();
                        for (int i = 0; i < OrdElms.Count; i++)
                        {
                            OrdElms[i].Seq = i + 1;
                        }

                        _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                        _ApiResponse.SetElementContents(XDataKey, SerializeObjectEnc(xData, typeof(XData)));
                    }
                }
            }

            return _ApiResponse;
        }

        public ApiResponse TreeClicked()
        {
            string itemid = GetDataValue("o").Split('_').Last();

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.SetElementContents(ViewProp, ViewPropHtml(itemid));
            _ApiResponse.SetElementStyle(itemid, HtmlStyles.borderColor, "#ff6666");
            return _ApiResponse;
        }

        private string ViewPropHtml(string itmid)
        {
            string rtnvlu = string.Empty;
            string xdataVlu = ParamValue(XDataKey);
            if (string.IsNullOrEmpty(xdataVlu) == false)
            {
                xData = (XData)DeserializeObjectEnc(xdataVlu, typeof(XData));
                if (itmid != xData.DataId)
                {
                    rtnvlu = PropRow(itmid, TreeGroup.Element);
                }
                else
                {
                    rtnvlu = PropRow(itmid, TreeGroup.Base);
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

            switch (group)
            {
                case TreeGroup.Base:
                    name = TypeFieldNames(typeof(XData))[1]; //DataName
                    value = UIInput(group, itmid, "text", name, xData.DataName);
                    gridX.AddRow(new string[] { Translator.Format(name.ToLower()), value });

                    name = TypeFieldNames(typeof(XData))[2]; //DataDesc
                    value = UIInput(group, itmid, "text", name, xData.DataDesc);
                    gridX.AddRow(new string[] { Translator.Format(name.ToLower()), value });
                    break;
                case TreeGroup.Element:
                    List<string> Fields = TypeFieldNames(typeof(XDataElement));
                    XDataElement Element = xData.DataElements.Find(x => x.ElementId == itmid);
                    if (Element != null)
                    {
                        gridLabel.Wrap.InnerText += " - " + Element.Seq.ToString();

                        for (int i = 0; i < Fields.Count; i++)
                        {
                            name = Translator.Format(Fields[i].ToLower());
                            value = string.Empty;
                            object elmobjvlu = TypeFieldGetValue(typeof(XDataElement), Fields[i], Element);
                            elmvlu = Convert.ToString(elmobjvlu);

                            switch (Fields[i].ToLower())
                            {
                                case "elementid":
                                case "seq":
                                case "value":
                                    name = string.Empty;
                                    break;
                                case "name":
                                case "label":
                                    value = UIInput(group, itmid, "text", Fields[i], elmvlu);
                                    break;
                                case "initialvalues":
                                    value = UIInput(group, itmid, "textarea", Fields[i], elmvlu);
                                    break;
                                case "uitype":
                                    OptionValues OptionValues = new OptionValues(typeof(UITypes), elmvlu);
                                    value = UIInput(group, itmid, "dropdown", Fields[i], elmvlu, OptionValues.OptionsHtml());
                                    break;
                                case "styles":
                                case "attributes":
                                case "labelstyles":
                                case "labelattributes":
                                case "wrapstyles":
                                    value = UIInput(group, itmid, "textarea", Fields[i], elmvlu);
                                    break;
                                case "iskey":
                                case "isvisible":
                                case "isreadonly":
                                case "isrequired":
                                case "isfixedvalue":
                                case "isgridcolumn":
                                    value = UIInput(group, itmid, "checkbox", Fields[i], elmvlu);
                                    break;
                                case "linespacing":
                                    value = UIInput(group, itmid, "number", Fields[i], elmvlu);
                                    break;
                                case "valuetype":
                                    OptionValues ValOptionValues = new OptionValues(typeof(ValueTypes), elmvlu);
                                    value = UIInput(group, itmid, "dropdown", Fields[i], elmvlu, ValOptionValues.OptionsHtml());
                                    break;
                            }
                            if (string.IsNullOrEmpty(name) == false)
                            {
                                gridX.AddRow(new string[] { Translator.Format(name.ToLower()), value });
                            }
                        }
                    }
                    break;
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
                case "none":
                    // No action
                    break;
                case "button":
                case "reset":
                case "submit":
                    // No action
                    break;
                case "textarea":
                    HtmlTag inputT = new HtmlTag(HtmlTags.textarea);
                    inputT.SetAttribute(HtmlAttributes.id, colnm);
                    inputT.SetStyle(HtmlStyles.border, "1px solid #999");
                    inputT.SetStyle(HtmlStyles.borderRadius, "4px");
                    inputT.SetStyle(HtmlStyles.fontFamily, "sans-serif !important");
                    inputT.SetStyle(HtmlStyles.resize, "vertical");
                    inputT.SetStyle(HtmlStyles.fontSize, "14px !important");
                    inputT.SetStyle(HtmlStyles.padding, "4px !important");
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
                    rtnvlu = inputI.Html();
                    break;
                case "checkbox":
                    HtmlTag input2 = new HtmlTag(HtmlTags.input, HtmlTag.Types.Empty);
                    input2.SetStyle(HtmlStyles.border, "1px solid #999");
                    input2.SetStyle(HtmlStyles.fontSize, "14px !important");
                    input2.SetStyle(HtmlStyles.padding, "4px !important");
                    input2.SetAttribute(HtmlAttributes.id, colnm);
                    input2.SetAttribute(HtmlAttributes.type, UIType);
                    input2.SetAttribute(HtmlEvents.onclick, "SaveProperty('" + procedure + "','" + group.ToString() + "','" + ItemId + "','" + colnm + "',this)");
                    if (Convert.ToDouble(value) == 1)
                    {
                        input2.SetAttribute(HtmlAttributes.@checked, "checked");
                    }
                    rtnvlu = input2.HtmlText;
                    break;
                case "dropdown":
                    HtmlTag input3 = new HtmlTag(HtmlTags.@select);
                    input3.SetAttribute(HtmlAttributes.id, colnm);
                    input3.SetStyle(HtmlStyles.border, "1px solid #999");
                    input3.SetStyle(HtmlStyles.borderRadius, "4px");
                    input3.SetStyle(HtmlStyles.width, "100%");
                    input3.SetStyle(HtmlStyles.fontSize, "14px !important");
                    input3.SetStyle(HtmlStyles.padding, "4px !important");
                    input3.SetAttribute(HtmlEvents.onchange, "SaveProperty('" + procedure + "','" + group.ToString() + "','" + ItemId + "','" + colnm + "',this)");
                    input3.InnerText = optvalues;
                    rtnvlu = input3.HtmlText;
                    break;
                default: // Catches text, password, email, etc.
                    HtmlTag input1 = new HtmlTag(HtmlTags.input, HtmlTag.Types.Empty);
                    input1.SetStyle(HtmlStyles.border, "1px solid #999");
                    input1.SetStyle(HtmlStyles.borderRadius, "4px");
                    input1.SetStyle(HtmlStyles.fontSize, "14px !important");
                    input1.SetStyle(HtmlStyles.padding, "4px !important");
                    input1.SetAttribute(HtmlAttributes.id, colnm);
                    input1.SetAttribute(HtmlAttributes.type, UIType);
                    input1.SetAttribute(HtmlAttributes.value, value);
                    input1.SetAttribute(HtmlEvents.onblur, "SaveProperty('" + procedure + "','" + group.ToString() + "','" + ItemId + "','" + colnm + "',this)");
                    input1.SetStyle(HtmlStyles.width, "94%");
                    rtnvlu = input1.HtmlText;
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
            string xdataVlu = ParamValue(XDataKey);

            ApiResponse _ApiResponse = new ApiResponse();

            if (string.IsNullOrEmpty(xdataVlu) == false)
            {
                if (!(colnm.ToLower() == "name" && colvlu == string.Empty))
                {
                    if (colnm.ToLower() == "name") colvlu = colvlu.Replace(" ", "_");

                    Match match = Regex.Match(colvlu, @"\{(\d+)\}");
                    if (match.Success == true)
                    {
                        colvlu = colvlu.Replace(match.Value, "{Invalid Expression}");
                    }

                    xData = (XData)DeserializeObjectEnc(xdataVlu, typeof(XData));

                    if (group == TreeGroup.Base.ToString())
                    {
                        bool cFlag = false;
                        if (colnm == TypeFieldNames(typeof(XData))[1]) //DataName
                        {
                            if (xData.DataName != colvlu)
                            {
                                cFlag = true;
                                xData.DataName = colvlu;
                            }
                        }
                        else if (colnm == TypeFieldNames(typeof(XData))[2]) //DataDesc
                        {
                            if (xData.DataDesc != colvlu)
                            {
                                cFlag = true;
                                xData.DataDesc = colvlu;
                            }
                        }

                        if (cFlag == true)
                        {
                            _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                            _ApiResponse.SetElementContents(XDataKey, SerializeObjectEnc(xData, typeof(XData)));
                        }
                    }
                    else if (group == TreeGroup.Element.ToString())
                    {
                        XDataElement Element = xData.DataElements.Find(x => x.ElementId == itmid);
                        if (Element != null)
                        {
                            string oldvlu = TypeFieldGetValue(typeof(XDataElement), colnm, Element).ToString();
                            if (oldvlu != colvlu)
                            {
                                TypeFieldSetValue(typeof(XDataElement), colnm, Element, colvlu);
                                _ApiResponse.SetElementContents(ViewTree, TreeViewHtml());
                                _ApiResponse.SetElementContents(XDataKey, SerializeObjectEnc(xData, typeof(XData)));
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
            string page = References.Pages.XysDataCode;
            string xdataVlu = ParamValue(XDataKey);

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(page, xdataVlu));
            _ApiResponse.ExecuteScript("$ScrollToTop()");
            return _ApiResponse;
        }

        public ApiResponse SaveData()
        {
            string xdataVlu = ParamValue(XDataKey);

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(xdataVlu))
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
            List<string> SQL = new List<string>();

            xData = (XData)DeserializeObjectEnc(ParamValue(XDataKey), typeof(XData));

            SQL.Add(" Update XysData set " +
                    " DataName = @DataName, DataDesc = @DataDesc, DataModel = @DataModel, " +
                    " SYSDTE = getdate(), SYSUSR = @SYSUSR Where DataId = @DataId ");

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@DataId", Value = xData.DataId, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@DataName", Value = xData.DataName, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@DataDesc", Value = xData.DataDesc, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@DataModel", Value = SerializeObject(xData.DataElements, typeof(List<XDataElement>)), SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        public ApiResponse DeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.PopUpWindow(DialogQstDelete("XysDataEV/ConfirmDeleteData"), References.Elements.PageContents);
            return _ApiResponse;
        }

        public ApiResponse ConfirmDeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            string rlt = PutDeleteData();
            if (string.IsNullOrEmpty(rlt))
            {
                _ApiResponse.PopUpWindow(DialogMsgDeleted("m=XysDataMV"), References.Elements.PageContents);
            }
            else
            {
                _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
            }
            return _ApiResponse;
        }

        private string PutDeleteData()
        {
            xData = (XData)DeserializeObjectEnc(ParamValue(XDataKey), typeof(XData));

            List<string> SQL = new List<string>() {
            " delete from XysData Where DataId = @DataId "
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@DataId", Value = xData.DataId, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        public ApiResponse Preview()
        {
            xData = (XData)DeserializeObjectEnc(ParamValue(XDataKey), typeof(XData));

            string xparam = EncryptString(xData.DataId);
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.NewWindow(References.Pages.XysDataPrvw + "?x=" + xparam);
            return _ApiResponse;
        }
    }

}
