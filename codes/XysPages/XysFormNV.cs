using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysFormNV : WebBase
    {
        private UIControl MyControl { get; set; } = new UIControl();
        public XysFormNV()
        {
            string InitValue1 = OptionValues.EnumToOptionalized(typeof(UIForm.Papers), "0");
            string InitValue2 = OptionValues.EnumToOptionalized(typeof(UIForm.Orientations), "0");

            MyControl.Set(new UIControl.Item[] {
                new UIControl.Item() { Name = "FormId", ValueType = ValueTypes.GUID, IsKey = true, IsVisible = false, LineSpacing = 1 },
                new UIControl.Item() { Name = "Title", Styles = "width:474px;", IsRequired = true, LineSpacing = 1 },
                new UIControl.Item() { Name = "IsTitle", UIType = UITypes.Checkbox, InitialValues = "{1|Yes|*}" },
                new UIControl.Item() { Name = "FormFlag", UIType = UITypes.Checkbox, InitialValues = "{1|Yes|*}" },
                new UIControl.Item() { Name = "FormRef", WrapStyles = "margin-left:40px;", Styles = "width:272px;", LineSpacing = 10 },
                new UIControl.Item() { Name = "FormDesc", Styles = "width:480px;", LineSpacing = 10 },
                new UIControl.Item() { Name = "TitleAttributes", Styles = "width:480px;", LineSpacing = 1 },
                new UIControl.Item() { Name = "TitleStyles", Styles = "width:480px;", LineSpacing = 1 },
                new UIControl.Item() { Name = "WrapAttributes", Styles = "width:480px;", LineSpacing = 1 },
                new UIControl.Item() { Name = "WrapStyles", Styles = "width:480px;", LineSpacing = 20 },
                new UIControl.Item() { Name = "Paper", Styles = "width:100px;", UIType = UITypes.Dropdown, InitialValues = InitValue1 },
                new UIControl.Item() { Name = "Orientation", Styles = "width:100px;", UIType = UITypes.Dropdown, InitialValues = InitValue2 },
                new UIControl.Item() { Name = "MarginTop", WrapStyles = "margin-left:20px;", Styles = "width:80px;text-align:center;", UIType = UITypes.Number, ValueType = ValueTypes.Defalt, InitialValues = "10" },
                new UIControl.Item() { Name = "MarginLeft", Styles = "width:80px;text-align:center;", UIType = UITypes.Number, ValueType = ValueTypes.Defalt, InitialValues = "20", LineSpacing = 20 },
                new UIControl.Item() { Name = "IsBorderLine", UIType = UITypes.Checkbox, InitialValues = "{1|Yes|*}" },
                new UIControl.Item() { Name = "IsAttached", UIType = UITypes.Checkbox, InitialValues = "{1|Yes|*}", LineSpacing = 20 }
            });

            for (int i = 0; i < MyControl.Items.Count; i++)
            {
                MyControl.Items[i].Label = Translator.Format(MyControl.Items[i].Name);
            }
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(new string[] { });

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = Translator.Format("newform");

            FilterSection filter = new FilterSection();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "92%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            HtmlElementBox elmBtns = new HtmlElementBox();
            elmBtns.DefaultStyle = false;
            elmBtns.AddItem(ViewButtons, 20);

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "92%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "30px 10px 30px 40px");

            elmBox.AddItem(MyControl, 30);
            elmBox.AddItem(elmBtns);

            string ViewHtml = filter.HtmlText + elmBox.HtmlText;

            HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TreeScript);
            return ViewHtml;
        }

        public ApiResponse SaveData()
        {
            string FormId = NewID();
            string FormTitle = ParamValue("Title");

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(FormTitle))
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
            }
            else
            {
                string rlt = PutSaveData(FormId);
                if (string.IsNullOrEmpty(rlt))
                {
                    _ApiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(References.Pages.XysFormEV, FormId));
                }
                else
                {
                    _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
                }
            }

            return _ApiResponse;
        }

        private string PutSaveData(string FormId)
        {
            XForm xForm = new XForm()
            {
                FormId = FormId,
                FormRef = ParamValue("FormRef"),
                FormFlag = (int)Convert.ToDouble(ParamValue("FormFlag")),
                Title = ParamValue("Title"),
                IsTitle = (int)Convert.ToDouble(ParamValue("IsTitle")),
                Description = ParamValue("FormDesc"),
                TitleAttributes = ParamValue("TitleAttributes"),
                TitleStyles = ParamValue("TitleStyles"),
                WrapAttributes = ParamValue("WrapAttributes"),
                WrapStyles = ParamValue("WrapStyles"),
                Paper = (int)Convert.ToDouble(ParamValue("Paper")),
                Orientation = (int)Convert.ToDouble(ParamValue("Orientation")),
                MarginTop = (int)Convert.ToDouble(ParamValue("MarginTop")),
                MarginLeft = (int)Convert.ToDouble(ParamValue("MarginLeft")),
                IsBorderLine = (int)Convert.ToDouble(ParamValue("IsBorderLine")),
                IsAttached = (int)Convert.ToDouble(ParamValue("IsAttached"))
            };

            string formModel = SerializeObject(xForm, typeof(XForm));

            List<string> SQL = new List<string>();
            SQL.Add(" Insert into XysForm(FormId, FormTitle,FormDesc, FormRef,FormFlag,FormModel,SYSDTE,SYSUSR) " +
                    " values (@FormId,  @FormTitle, @FormDesc,@FormRef,@FormFlag,@FormModel, getdate(), @SYSUSR) ");

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
    }

}
