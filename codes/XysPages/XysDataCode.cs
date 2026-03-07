using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysDataCode : WebBase
    {

        private XData xData = null;
        private string XDataKey = "xdata";

        public XysDataCode()
        {
        }

        public override void InitialViewData()
        {
            xData = (XData)DeserializeObjectEnc(PartialData, typeof(XData));
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
            labelT.Wrap.InnerText = xData.DataName;

            Label labelT1 = new Label();
            labelT1.Wrap.SetStyles("font-size:14px;text-decoration:italic; margin-left:12px; color:#666; ");
            labelT1.Wrap.InnerText = "- " + xData.DataDesc;

            TextArea text2 = new TextArea();
            text2.Text.SetAttribute(HtmlAttributes.spellcheck, "false");
            text2.Text.SetAttribute(HtmlAttributes.@readonly, "readonly");
            text2.Text.SetStyle(HtmlStyles.width, "98%");
            text2.Text.SetStyle(HtmlStyles.padding, "12px !important");
            text2.Text.SetStyle(HtmlStyles.height, "300px");
            text2.Text.SetStyle(HtmlStyles.fontFamily, "sans-serif");
            text2.Text.SetStyle(HtmlStyles.fontSize, "14px");
            text2.Text.SetStyle(HtmlStyles.color, "#fff");
            text2.Text.SetStyle(HtmlStyles.backgroundColor, "#555");
            text2.LabelWrap.SetStyle(HtmlStyles.display, "none");

            TextArea text4 = new TextArea();
            text4.Text.SetStyles(text2.Text.GetStyles());
            text4.Text.SetAttributes(text2.Text.GetAttributes());

            TextArea text5 = new TextArea();
            text5.Text.SetAttributes("readonly:readonly;");
            text5.Text.SetStyles("width:98%; padding:12px; height:50px; font-size:14px;");

            text2.Text.InnerText = GenerateCode();
            text4.Text.InnerText = GenerateCodeCS();

            UIGrid UIGrid = UIGridFromXDataElements(xData.DataElements);
            text5.Text.InnerText = string.Join(",", UIGrid.Columns().ToArray());

            Hidden text3 = new Hidden(XDataKey);
            text3.SetAttribute(HtmlAttributes.value, PartialData);

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "92%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "30px 10px 30px 20px");

            elmBox.AddItem(labelT, 6);
            elmBox.AddItem(labelT1, 20);
            elmBox.AddItem(text5, 10);
            elmBox.AddItem(text2, 10);
            elmBox.AddItem(text4, 20);
            elmBox.AddItem(text3);

            string ViewHtml = filter.HtmlText + elmBox.HtmlText;

            HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TreeScript);
            return ViewHtml;
        }

        public string GenerateCodeCS()
        {
            if (xData == null || xData.DataElements == null || xData.DataElements.Count == 0)
            {
                return string.Empty;
            }

            string codes = Environment.NewLine + "UIControl MyControl = new UIControl();" + Environment.NewLine;
            codes += "MyControl.Set(new List<UIControl.Item>() { " + Environment.NewLine;

            var XDataElements = xData.DataElements.OrderBy(x => x.Seq).ToList();
            var Fields = TypeFieldNames(typeof(XDataElement));

            for (int i = 0; i < XDataElements.Count; i++)
            {
                codes += " new UIControl.Item() {";
                for (int j = 0; j < Fields.Count; j++)
                {
                    string elmVlu = TypeFieldGetValue(typeof(XDataElement), Fields[j], XDataElements[i]).ToString();

                    switch (Fields[j])
                    {
                        case "Name":
                            codes += Fields[j] + "=\"" + elmVlu + "\"";
                            break;
                        case "Label":
                        case "Value":
                        case "Styles":
                        case "Attributes":
                        case "LabelStyles":
                        case "LabelAttributes":
                        case "WrapStyles":
                        case "InitialValues":
                            if (elmVlu != string.Empty)
                            {
                                codes += ", " + Fields[j] + "=\"" + elmVlu.Replace("\n", " ") + "\"";
                            }
                            break;
                        case "UIType":
                            if (Common.Val(elmVlu) != (double)UITypes.Text)
                            {
                                codes += ", " + Fields[j] + "= UITypes." + EnumName(typeof(UITypes), Convert.ToInt16(elmVlu)) + "";
                            }
                            break;
                        case "ValueType":
                            if (Common.Val(elmVlu) != 0)
                            {
                                codes += ", " + Fields[j] + "= ValueTypes." + EnumName(typeof(ValueTypes), Convert.ToInt16(elmVlu)) + "";
                            }
                            break;
                        case "LineSpacing":
                            if (Common.Val(elmVlu) != 0)
                            {
                                codes += ", " + Fields[j] + "=" + Convert.ToDouble(elmVlu).ToString() + "";
                            }
                            break;
                        case "IsVisible":
                            bool tboolVis = Convert.ToDouble(elmVlu) == 1;
                            if (!tboolVis)
                            {
                                codes += ", " + Fields[j] + "= false";
                            }
                            break;
                        case "IsKey":
                        case "IsReadOnly":
                        case "IsRequired":
                        case "IsFixedValue":
                            bool tboolOther = Convert.ToDouble(elmVlu) == 1;
                            if (tboolOther)
                            {
                                codes += ", " + Fields[j] + "= true";
                            }
                            break;
                    }
                }
                codes += "}" + (i == XDataElements.Count - 1 ? string.Empty : ",") + Environment.NewLine;
            }
            codes += "});" + Environment.NewLine;

            return codes;
        }

        public string GenerateCode()
        {
            if (xData == null || xData.DataElements == null || xData.DataElements.Count == 0)
            {
                return string.Empty;
            }

            string codes = Environment.NewLine + "Dim MyControl as New UIControl()" + Environment.NewLine;
            codes += "MyControl.Set({ _" + Environment.NewLine;

            var XDataElements = xData.DataElements.OrderBy(x => x.Seq).ToList();
            var Fields = TypeFieldNames(typeof(XDataElement));

            for (int i = 0; i < XDataElements.Count; i++)
            {
                codes += " New UIControl.Item() With {";
                for (int j = 0; j < Fields.Count; j++)
                {
                    string elmVlu = TypeFieldGetValue(typeof(XDataElement), Fields[j], XDataElements[i]).ToString();

                    switch (Fields[j])
                    {
                        case "Name":
                            codes += "." + Fields[j] + "=\"" + elmVlu + "\"";
                            break;
                        case "Label":
                        case "Value":
                        case "Styles":
                        case "Attributes":
                        case "LabelStyles":
                        case "LabelAttributes":
                        case "WrapStyles":
                        case "InitialValues":
                            if (elmVlu != string.Empty)
                            {
                                codes += ", ." + Fields[j] + "=\"" + elmVlu.Replace("\n", " ") + "\"";
                            }
                            break;
                        case "UIType":
                            if (Convert.ToInt16(elmVlu) != (int)UITypes.Text)
                            {
                                codes += ", ." + Fields[j] + "= UITypes." + EnumName(typeof(UITypes), Convert.ToInt16(elmVlu)) + "";
                            }
                            break;
                        case "ValueType":
                            if (Convert.ToInt16(elmVlu) != 0)
                            {
                                codes += ", ." + Fields[j] + "= ValueTypes." + EnumName(typeof(ValueTypes), Convert.ToInt16(elmVlu)) + "";
                            }
                            break;
                        case "LineSpacing":
                            if (Convert.ToInt16(elmVlu) != 0)
                            {
                                codes += ", ." + Fields[j] + "=" + Convert.ToInt16(elmVlu).ToString() + "";
                            }
                            break;
                        case "IsVisible":
                            bool tboolVis = Convert.ToDouble(elmVlu) == 1;
                            if (!tboolVis)
                            {
                                codes += ", ." + Fields[j] + "= False";
                            }
                            break;
                        case "IsKey":
                        case "IsReadOnly":
                        case "IsRequired":
                        case "IsFixedValue":
                            bool tboolOther = Convert.ToDouble(elmVlu) == 1;
                            if (tboolOther)
                            {
                                codes += ", ." + Fields[j] + "= True";
                            }
                            break;
                    }
                }
                codes += "}" + (i == XDataElements.Count - 1 ? string.Empty : ",") + Environment.NewLine;
            }
            codes += "})" + Environment.NewLine;

            return codes;
        }

        public ApiResponse BackDesign()
        {
            string page = References.Pages.XysDataEV;
            xData = (XData)DeserializeObjectEnc(ParamValue(XDataKey), typeof(XData));

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(page, xData.DataId));
            _ApiResponse.ExecuteScript("$ScrollToTop()");
            return _ApiResponse;
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
