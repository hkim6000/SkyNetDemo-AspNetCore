using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysLevelEV : WebBase
    {
        public XysLevelEV()
        {
            ViewPart.Fields.AddRange(new NameValueFlag[] {
            new NameValueFlag { name = "LevelCode" },
            new NameValueFlag { name = "LevelName" },
            new NameValueFlag { name = "LevelDesc" },
            new NameValueFlag { name = "LevelFlag" }
        });
        }

        public override void InitialViewData()
        {
            string SSQL = " Select LevelCode,LevelName,LevelDesc,LevelFlag  From XysLevel   " +
                          " where LevelCode = N'" + PartialData + "'";
            ViewPart.BindData(SSQL);
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(ViewPart.Data == null ? new string[] { "save" } : null);

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = ViewPart.Data == null ? Translator.Format("newlevel") : Translator.Format("editlevel");

            FilterSection filter = new();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "90%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Texts text = new Texts(Translator.Format("code"), ViewPart.Field("LevelCode").name, TextTypes.text);
            text.Required = true;
            text.Text.SetStyle(HtmlStyles.width, "200px");
            text.Text.SetAttribute(HtmlAttributes.maxlength, "200");
            text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("LevelCode").value);
            if (ViewPart.Data != null) text.Text.SetAttribute(HtmlAttributes.disabled, "disabled");
            text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text1 = new Texts(Translator.Format("name"), ViewPart.Field("LevelName").name, TextTypes.text);
            text1.Required = true;
            text1.Text.SetStyle(HtmlStyles.width, "200px");
            text1.Text.SetAttribute(HtmlAttributes.maxlength, "50");
            text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("LevelName").value);
            text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text2 = new Texts(Translator.Format("desc"), ViewPart.Field("LevelDesc").name, TextTypes.text);
            text2.Text.SetStyle(HtmlStyles.width, "300px");
            text2.Text.SetAttribute(HtmlAttributes.maxlength, "200");
            text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("LevelDesc").value);
            text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            CheckBox chk1 = new CheckBox(Translator.Format("flag"), ViewPart.Field("LevelFlag").name);
            bool isChecked = Convert.ToDouble(ViewPart.Field("LevelFlag").value ?? "0") == 1;
            chk1.Checks.AddItem(ViewPart.Field("LevelFlag").name, "1", isChecked);
            chk1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "90%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");

            elmBox.AddItem(text, 1);
            elmBox.AddItem(text1, 1);
            elmBox.AddItem(text2, 20);
            elmBox.AddItem(chk1, 50);
            elmBox.AddItem(ViewButtons, 20);

            return filter.HtmlText + elmBox.HtmlText;
        }

        public ApiResponse SaveData()
        {
            string LevelCode = ViewPart.Field("LevelCode").value;
            string LevelName = ViewPart.Field("LevelName").value;

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(LevelCode) || string.IsNullOrEmpty(LevelName))
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
            }
            else
            {
                string rlt = PutSaveData();
                if (string.IsNullOrEmpty(rlt))
                {
                    _ApiResponse.PopUpWindow(DialogMsgSaved("m=XysLevelMV"), References.Elements.PageContents);
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

            if (ViewPart.Data == null)
            {
                SQL.Add(" Insert into XysLevel( LevelCode,LevelName,LevelDesc,LevelFlag ,SYSDTE,SYSUSR) " +
                        " values( @LevelCode, @LevelName, @LevelDesc, @LevelFlag, getdate(), @SYSUSR) ");
            }
            else
            {
                SQL.Add(" Update XysLevel set " +
                        " LevelName = @LevelName, LevelDesc = @LevelDesc, LevelFlag = @LevelFlag, " +
                        " SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                        " WHERE LevelCode = @LevelCode");
            }

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@LevelCode", Value = ViewPart.Field("LevelCode").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@LevelName", Value = ViewPart.Field("LevelName").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@LevelDesc", Value = ViewPart.Field("LevelDesc").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@LevelFlag", Value = Common.Val(ViewPart.Field("LevelFlag").value).ToString(), SqlDbType = SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        public ApiResponse DeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.PopUpWindow(DialogQstDelete("XysLevelEV/ConfirmDeleteData"), References.Elements.PageContents);
            return _ApiResponse;
        }

        public ApiResponse ConfirmDeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            string rlt = PutDeleteData();
            if (string.IsNullOrEmpty(rlt))
            {
                _ApiResponse.PopUpWindow(DialogMsgDeleted("m=XysLevelMV"), References.Elements.PageContents);
            }
            else
            {
                _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
            }
            return _ApiResponse;
        }

        private string PutDeleteData()
        {
            List<string> SQL = new List<string> {
            " delete from XysLevel where LevelCode = @LevelCode "
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@LevelCode", Value = ViewPart.Field("LevelCode").value, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }
    }

}
