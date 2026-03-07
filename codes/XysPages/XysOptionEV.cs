using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysOptionEV : WebBase
    {
        public XysOptionEV()
        {
            ViewPart.Fields.AddRange(new NameValueFlag[] {
            new NameValueFlag { name = "CODE", flag = false },
            new NameValueFlag { name = "SNO" },
            new NameValueFlag { name = "SD01" },
            new NameValueFlag { name = "SD02" },
            new NameValueFlag { name = "SD03" },
            new NameValueFlag { name = "SD04" },
            new NameValueFlag { name = "SD05" },
            new NameValueFlag { name = "SD06" },
            new NameValueFlag { name = "SD07" }
        });
        }

        public override void InitialViewData()
        {
            string SSQL = " Select CODE, SNO, SD01, SD02, SD03, SD04, SD05, SD06, SD07  From XysOption   " +
                          " where CODE+ SNO = N'" + PartialData + "'";

            ViewPart.BindData(SSQL);
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(ViewPart.Data == null ? new string[] { "save" } : null);

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = ViewPart.Data == null ? Translator.Format("newopt") : Translator.Format("editopt");

            FilterSection filter = new();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "90%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Texts text = new Texts(Translator.Format("code"), ViewPart.Field("CODE").name, TextTypes.text);
            text.Wrap.SetStyle(HtmlStyles.display, "none");
            text.Text.SetAttribute(HtmlAttributes.value, "OPTION");
            text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text1 = new Texts(Translator.Format("no."), ViewPart.Field("SNO").name, TextTypes.text);
            text1.Required = true;
            text1.Text.SetStyle(HtmlStyles.width, "100px");
            text1.Text.SetAttribute(HtmlAttributes.maxlength, "10");
            text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SNO").value);
            if (ViewPart.Data != null) text1.Text.SetAttribute(HtmlAttributes.disabled, "disabled");
            text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text2 = new Texts(Translator.Format("sd01"), ViewPart.Field("SD01").name, TextTypes.text);
            text2.Required = true;
            text2.Text.SetStyle(HtmlStyles.width, "300px");
            text2.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD01").value);
            text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text3 = new Texts(Translator.Format("sd02"), ViewPart.Field("SD02").name, TextTypes.text);
            text3.Text.SetStyle(HtmlStyles.width, "300px");
            text3.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text3.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD02").value);
            text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text4 = new Texts(Translator.Format("sd03"), ViewPart.Field("SD03").name, TextTypes.text);
            text4.Text.SetStyle(HtmlStyles.width, "300px");
            text4.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text4.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD03").value);
            text4.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text5 = new Texts(Translator.Format("sd04"), ViewPart.Field("SD04").name, TextTypes.text);
            text5.Text.SetStyle(HtmlStyles.width, "300px");
            text5.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text5.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD04").value);
            text5.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text6 = new Texts(Translator.Format("sd05"), ViewPart.Field("SD05").name, TextTypes.text);
            text6.Text.SetStyle(HtmlStyles.width, "300px");
            text6.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text6.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD05").value);
            text6.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text7 = new Texts(Translator.Format("sd06"), ViewPart.Field("SD06").name, TextTypes.text);
            text7.Text.SetStyle(HtmlStyles.width, "300px");
            text7.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text7.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD06").value);
            text7.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text8 = new Texts(Translator.Format("sd07"), ViewPart.Field("SD07").name, TextTypes.text);
            text8.Text.SetStyle(HtmlStyles.width, "300px");
            text8.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text8.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("SD07").value);
            text8.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "90%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");

            elmBox.AddItem(text);
            elmBox.AddItem(text1, 20);
            elmBox.AddItem(text2, 1);
            elmBox.AddItem(text3, 1);
            elmBox.AddItem(text4, 1);
            elmBox.AddItem(text5, 1);
            elmBox.AddItem(text6, 1);
            elmBox.AddItem(text7, 1);
            elmBox.AddItem(text8, 50);
            elmBox.AddItem(ViewButtons, 20);

            return filter.HtmlText + elmBox.HtmlText;
        }

        public ApiResponse SaveData()
        {
            string CODE = ViewPart.Field("CODE").value;
            string SNO = ViewPart.Field("SNO").value;
            string SD01 = ViewPart.Field("SD01").value;

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(CODE) || string.IsNullOrEmpty(SNO) || string.IsNullOrEmpty(SD01))
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
            }
            else
            {
                string rlt = PutSaveData();
                if (string.IsNullOrEmpty(rlt))
                {
                    _ApiResponse.PopUpWindow(DialogMsgSaved("m=XysOptionMV"), References.Elements.PageContents);
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
                SQL.Add(" insert into XysOption(CODE, SNO, SD01, SD02, SD03, SD04, SD05, SD06, SD07) " +
                    " values(@CODE,@SNO,@SD01,@SD02,@SD03,@SD04,@SD05,@SD06,@SD07) ");
            }
            else
            {
                SQL.Add(" Update XysOption set " +
                        "   SD01 = @SD01, " +
                        "   SD02 = @SD02, " +
                        "   SD03 = @SD03, " +
                        "   SD04 = @SD04, " +
                        "   SD05 = @SD05, " +
                        "   SD06 = @SD06, " +
                        "   SD07 = @SD07 " +
                        " Where  CODE = @CODE and SNO = @SNO  ");
            }

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@CODE", Value = ViewPart.Field("CODE").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SNO", Value = ViewPart.Field("SNO").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SD01", Value = ViewPart.Field("SD01").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SD02", Value = ViewPart.Field("SD02").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SD03", Value = ViewPart.Field("SD03").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SD04", Value = ViewPart.Field("SD04").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SD05", Value = ViewPart.Field("SD05").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SD06", Value = ViewPart.Field("SD06").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SD07", Value = ViewPart.Field("SD07").value, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }
    }

}
