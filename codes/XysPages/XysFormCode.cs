namespace ASPNETCoreWeb.codes.XysPages
{
    using SkyNet;
    using SkyNet.ToolKit;
    using ASPNETCoreWeb.codes.XysBases;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class XysFormCode : WebBase
    {
        private XForm xForm = null;
        private string xFormKey = "xForm";

        public XysFormCode()
        {
        }

        public override void InitialViewData()
        {
            xForm = (XForm)DeserializeObjectEnc(PartialData, typeof(XForm));
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons();

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = Translator.Format("viewcode");

            FilterSection filter = new FilterSection();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "92%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Label labelT = new Label();
            labelT.Wrap.SetStyles("font-weight:700;font-size:22px; margin-left:10px; ");
            labelT.Wrap.InnerText = xForm.Title;

            TextArea text2 = new TextArea();
            text2.Text.SetAttribute(HtmlAttributes.spellcheck, "false");
            text2.Text.SetAttribute(HtmlAttributes.@readonly, "readonly");
            text2.Text.SetStyle(HtmlStyles.width, "98%");
            text2.Text.SetStyle(HtmlStyles.padding, "12px !important");
            text2.Text.SetStyle(HtmlStyles.height, "400px");
            text2.Text.SetStyle(HtmlStyles.fontFamily, "Times New Roman");
            text2.Text.SetStyle(HtmlStyles.fontSize, "14px");
            text2.Text.SetStyle(HtmlStyles.color, "#fff");
            text2.Text.SetStyle(HtmlStyles.backgroundColor, "#555");
            text2.LabelWrap.SetStyle(HtmlStyles.display, "none");

            TextArea text4 = new TextArea();
            text4.Text.SetStyles(text2.Text.GetStyles());
            text4.Text.SetAttributes(text2.Text.GetAttributes());

            text2.Text.InnerText = GenerateCode();
            text4.Text.InnerText = GenerateCodeCS();

            Hidden text3 = new Hidden(xFormKey);
            text3.SetAttribute(HtmlAttributes.value, PartialData);

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "92%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "30px 10px 30px 20px");

            elmBox.AddItem(labelT, 6);
            elmBox.AddItem(text2, 20);
            elmBox.AddItem(text4, 20);
            elmBox.AddItem(text3);

            string ViewHtml = filter.HtmlText + elmBox.HtmlText;

            HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TreeScript);
            return ViewHtml;
        }

        public string GenerateCodeCS()
        {
            if (xForm == null) return string.Empty;

            string codes = Environment.NewLine + "UIForm MyForm = new UIForm() {" + Environment.NewLine;

            List<string> FormFields = TypeFieldNames(typeof(XForm));
            for (int i = 0; i < FormFields.Count; i++)
            {
                if (FormFields[i].ToLower() != "sections" && IsTypeProperty(typeof(UIForm), FormFields[i]))
                {
                    string elmvlu = TypeFieldGetValue(typeof(XForm), FormFields[i], xForm).ToString();
                    FieldInfo elmfld = TypeFieldInfo(typeof(XForm), FormFields[i]);
                    switch (elmfld.FieldType.Name)
                    {
                        case "String":
                        case "Char":
                            codes += FormFields[i] + " = \"" + elmvlu + "\", ";
                            break;
                        default:
                            codes += FormFields[i] + " = " + elmvlu + ", ";
                            break;
                    }
                }
            }
            codes += "Margin = new Location(" + xForm.MarginTop.ToString() + "," + xForm.MarginLeft.ToString() + "), ";
            codes = codes.Substring(0, codes.Length - 2) + "};" + Environment.NewLine + Environment.NewLine;

            for (int i = 0; i < xForm.Sections.Count; i++)
            {
                codes += "UIForm.UISection " + xForm.Sections[i].Name + " = new UIForm.UISection() {" + Environment.NewLine;

                List<string> SectionFields = TypeFieldNames(typeof(XFormSection));
                for (int j = 0; j < SectionFields.Count; j++)
                {
                    if (SectionFields[j].ToLower() != "elements" && IsTypeProperty(typeof(UIForm.UISection), SectionFields[j]))
                    {
                        string elmvlu = TypeFieldGetValue(typeof(XFormSection), SectionFields[j], xForm.Sections[i]).ToString();
                        FieldInfo elmfld = TypeFieldInfo(typeof(XFormSection), SectionFields[j]);
                        switch (elmfld.FieldType.Name)
                        {
                            case "String":
                            case "Char":
                                codes += SectionFields[j] + " = \"" + elmvlu + "\", ";
                                break;
                            default:
                                codes += SectionFields[j] + " = " + elmvlu + ", ";
                                break;
                        }
                    }
                }
                codes = codes.Substring(0, codes.Length - 2) + "};" + Environment.NewLine + Environment.NewLine;

                if (xForm.Sections[i].Elements.Count > 0)
                {
                    codes += xForm.Sections[i].Name + ".Elements.AddRange(new List<UIForm.Element>(){" + Environment.NewLine;
                    for (int k = 0; k < xForm.Sections[i].Elements.Count; k++)
                    {
                        codes += "\t new UIForm.Element() {";

                        List<string> ElementFields = TypeFieldNames(typeof(XFormElement));
                        for (int m = 0; m < ElementFields.Count; m++)
                        {
                            if (IsTypeProperty(typeof(UIForm.Element), ElementFields[m]))
                            {
                                string elmvlu = TypeFieldGetValue(typeof(XFormElement), ElementFields[m], xForm.Sections[i].Elements[k]).ToString();
                                FieldInfo elmfld = TypeFieldInfo(typeof(XFormElement), ElementFields[m]);
                                switch (elmfld.FieldType.Name)
                                {
                                    case "String":
                                    case "Char":
                                        codes += ElementFields[m] + " = \"" + elmvlu + "\", ";
                                        break;
                                    default:
                                        codes += ElementFields[m] + " = " + elmvlu + ", ";
                                        break;
                                }
                            }
                        }
                        codes = codes.Substring(0, codes.Length - 2) + "}" + (k != xForm.Sections[i].Elements.Count - 1 ? ", " : " ") + Environment.NewLine;
                    }
                    codes = codes.Substring(0, codes.Length - 1) + "});" + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                }
            }

            codes += Environment.NewLine;
            for (int i = 0; i < xForm.Sections.Count; i++)
            {
                codes += "MyForm.UISections.Add( " + xForm.Sections[i].Name + " );" + Environment.NewLine;
            }

            return codes;
        }

        public string GenerateCode()
        {
            if (xForm == null) return string.Empty;

            string codes = Environment.NewLine + "Dim MyForm As New UIForm With {" + Environment.NewLine;

            List<string> FormFields = TypeFieldNames(typeof(XForm));
            for (int i = 0; i < FormFields.Count; i++)
            {
                if (FormFields[i].ToLower() != "sections" && IsTypeProperty(typeof(UIForm), FormFields[i]))
                {
                    string elmvlu = TypeFieldGetValue(typeof(XForm), FormFields[i], xForm).ToString();
                    FieldInfo elmfld = TypeFieldInfo(typeof(XForm), FormFields[i]);
                    switch (elmfld.FieldType.Name)
                    {
                        case "String":
                        case "Char":
                            codes += "." + FormFields[i] + " = \"" + elmvlu + "\", ";
                            break;
                        default:
                            codes += "." + FormFields[i] + " = " + elmvlu + ", ";
                            break;
                    }
                }
            }
            codes += ".Margin = New Location(" + xForm.MarginTop.ToString() + "," + xForm.MarginLeft.ToString() + "), ";
            codes = codes.Substring(0, codes.Length - 2) + "}" + Environment.NewLine + Environment.NewLine;

            for (int i = 0; i < xForm.Sections.Count; i++)
            {
                codes += "Dim " + xForm.Sections[i].Name + " As New UIForm.UISection With {" + Environment.NewLine;

                List<string> SectionFields = TypeFieldNames(typeof(XFormSection));
                for (int j = 0; j < SectionFields.Count; j++)
                {
                    if (SectionFields[j].ToLower() != "elements" && IsTypeProperty(typeof(UIForm.UISection), SectionFields[j]))
                    {
                        string elmvlu = TypeFieldGetValue(typeof(XFormSection), SectionFields[j], xForm.Sections[i]).ToString();
                        FieldInfo elmfld = TypeFieldInfo(typeof(XFormSection), SectionFields[j]);
                        switch (elmfld.FieldType.Name)
                        {
                            case "String":
                            case "Char":
                                codes += "." + SectionFields[j] + " = \"" + elmvlu + "\", ";
                                break;
                            default:
                                codes += "." + SectionFields[j] + " = " + elmvlu + ", ";
                                break;
                        }
                    }
                }
                codes = codes.Substring(0, codes.Length - 2) + "}" + Environment.NewLine + Environment.NewLine;

                if (xForm.Sections[i].Elements.Count > 0)
                {
                    codes += xForm.Sections[i].Name + ".Elements.AddRange({" + Environment.NewLine;
                    for (int k = 0; k < xForm.Sections[i].Elements.Count; k++)
                    {
                        codes += "\t New UIForm.Element With {";

                        List<string> ElementFields = TypeFieldNames(typeof(XFormElement));
                        for (int m = 0; m < ElementFields.Count; m++)
                        {
                            if (IsTypeProperty(typeof(UIForm.Element), ElementFields[m]))
                            {
                                string elmvlu = TypeFieldGetValue(typeof(XFormElement), ElementFields[m], xForm.Sections[i].Elements[k]).ToString();
                                FieldInfo elmfld = TypeFieldInfo(typeof(XFormElement), ElementFields[m]);
                                switch (elmfld.FieldType.Name)
                                {
                                    case "String":
                                    case "Char":
                                        codes += "." + ElementFields[m] + " = \"" + elmvlu + "\", ";
                                        break;
                                    default:
                                        codes += "." + ElementFields[m] + " = " + elmvlu + ", ";
                                        break;
                                }
                            }
                        }
                        codes = codes.Substring(0, codes.Length - 2) + "}" + (k != xForm.Sections[i].Elements.Count - 1 ? ", " : " ") + Environment.NewLine;
                    }
                    codes = codes.Substring(0, codes.Length - 1) + "})" + Environment.NewLine + Environment.NewLine + Environment.NewLine;
                }
            }

            codes += Environment.NewLine;
            for (int i = 0; i < xForm.Sections.Count; i++)
            {
                codes += "MyForm.UISections.Add( " + xForm.Sections[i].Name + " )" + Environment.NewLine;
            }

            return codes;
        }

        public ApiResponse BackDesign()
        {
            string page = References.Pages.XysFormEV;
            xForm = (XForm)DeserializeObjectEnc(ParamValue(xFormKey), typeof(XForm));

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(page, xForm.FormId));
            _ApiResponse.ExecuteScript("$ScrollToTop()");
            return _ApiResponse;
        }

        public ApiResponse Preview()
        {
            xForm = (XForm)DeserializeObjectEnc(ParamValue(xFormKey), typeof(XForm));

            string xparam = EncryptString(xForm.FormId);
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.NewWindow(References.Pages.XysFormPrvw + "?x=" + xparam);
            return _ApiResponse;
        }
    }
}
