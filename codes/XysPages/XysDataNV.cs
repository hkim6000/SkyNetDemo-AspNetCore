using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysDataNV : WebBase
    {
        public XysDataNV()
        {
            ViewPart.Fields.AddRange(new NameValueFlag[] {
            new NameValueFlag { name = "DataId", flag = true },
            new NameValueFlag { name = "DataName" },
            new NameValueFlag { name = "DataDesc" },
            new NameValueFlag { name = "DataModel" }
            });
        }
        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(new string[] { });

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = Translator.Format("newdata");

            FilterSection filter = new FilterSection();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "92%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Texts text = new Texts(Translator.Format("dataname"), ViewPart.Field("DataName").name, TextTypes.text);
            text.Required = true;
            text.Text.SetStyle(HtmlStyles.width, "200px");
            text.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("DataName").value);
            text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text1 = new Texts(Translator.Format("datadesc"), ViewPart.Field("DataDesc").name, TextTypes.text);
            text1.Required = true;
            text1.Text.SetStyle(HtmlStyles.width, "400px");
            text1.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("DataDesc").value);
            text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            HtmlElementBox elmBtns = new HtmlElementBox();
            elmBtns.DefaultStyle = false;
            elmBtns.AddItem(ViewButtons, 20);

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "92%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "30px 10px 30px 20px");

            elmBox.AddItem(text, 20);
            elmBox.AddItem(text1, 40);
            elmBox.AddItem(elmBtns);

            string ViewHtml = filter.HtmlText + elmBox.HtmlText;

            HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TreeScript);
            return ViewHtml;
        }

        public ApiResponse SaveData()
        {
            string DataId = NewID();
            string DataName = ViewPart.Field("DataName").value;
            string DataDesc = ViewPart.Field("DataDesc").value;

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(DataName) || string.IsNullOrEmpty(DataDesc))
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
            }
            else
            {
                string rlt = PutSaveData(DataId);
                if (string.IsNullOrEmpty(rlt))
                {
                    _ApiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(References.Pages.XysDataEV, DataId));
                }
                else
                {
                    _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
                }
            }

            return _ApiResponse;
        }

        private string PutSaveData(string DataId)
        {
            List<string> SQL = new List<string>();

            SQL.Add(" Insert into XysData(DataId, DataName,DataDesc,DataModel,SYSDTE,SYSUSR) " +
                    " values (@DataId, @DataName, @DataDesc, @DataModel, getdate(), @SYSUSR) ");

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@DataId", Value = DataId, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@DataName", Value = ViewPart.Field("DataName").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@DataDesc", Value = ViewPart.Field("DataDesc").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@DataModel", Value = ViewPart.Field("DataModel").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

    }
}
