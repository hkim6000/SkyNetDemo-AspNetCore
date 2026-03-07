using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;

namespace ASPNETCoreWeb.codes.XysPages
{
    public class XysDictEV : WebBase
    {
        public XysDictEV()
        {
            ViewPart.Fields.AddRange(new NameValueFlag[] {
            new NameValueFlag { name = "Target", flag = true },
            new NameValueFlag { name = "IsoCode", flag = true },
            new NameValueFlag { name = "KeyWord", flag = true },
            new NameValueFlag { name = "Translated" }
        });
        }

        public override void InitialViewData()
        {
            string SSQL = " Select Target,IsoCode,KeyWord,Translated   " +
                          " from XysDict where Target+IsoCode+KeyWord = N'" + PartialData + "'";

            ViewPart.BindData(SSQL);
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(ViewPart.Data == null ? new string[] { "save" } : null);

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = ViewPart.Data == null ? Translator.Format("newdict") : Translator.Format("editdict");

            FilterSection filter = new();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "90%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Label lbl = new Label(Translator.Format("target"));
            lbl.Wrap.SetStyles("padding-left:8px;color:#777;");
            Label Vlbl = new Label(ViewPart.Field("Target").value);
            Vlbl.Wrap.SetStyles("padding-left:8px;font-weight:bold;font-size:20px;");

            Label lbl1 = new Label(Translator.Format("isocode"));
            lbl1.Wrap.SetStyles("padding-left:8px;color:#777;");
            Label Vlbl1 = new Label(ViewPart.Field("IsoCode").value);
            Vlbl1.Wrap.SetStyles("padding-left:8px;font-weight:bold;font-size:20px;");

            Label lbl2 = new Label(Translator.Format("keyword"));
            lbl2.Wrap.SetStyles("padding-left:8px;color:#777;");
            Label Vlbl2 = new Label(ViewPart.Field("KeyWord").value);
            Vlbl2.Wrap.SetStyles("padding-left:8px;font-weight:bold;font-size:20px;");

            Texts text = new Texts(Translator.Format("translated"), ViewPart.Field("Translated").name, TextTypes.text);
            text.Required = true;
            text.Text.SetStyle(HtmlStyles.width, "200px");
            text.Text.SetAttribute(HtmlAttributes.maxlength, "200");
            text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("Translated").value);
            text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "90%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");

            elmBox.AddItem(lbl, 10);
            elmBox.AddItem(Vlbl, 20);
            elmBox.AddItem(lbl1, 10);
            elmBox.AddItem(Vlbl1, 20);
            elmBox.AddItem(lbl2, 10);
            elmBox.AddItem(Vlbl2, 20);
            elmBox.AddItem(text, 40);
            elmBox.AddItem(ViewButtons, 20);

            string ViewHtml = filter.HtmlText + elmBox.HtmlText;
            return ViewHtml;
        }

        public ApiResponse SaveData()
        {
            string Translated = ViewPart.Field("Translated").value;

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(Translated))
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
            }
            else
            {
                string rlt = PutSaveData();
                if (string.IsNullOrEmpty(rlt))
                {
                    _ApiResponse.PopUpWindow(DialogMsgSaved("m=XysDictMV"), References.Elements.PageContents);
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
                SQL.Add(" Insert into XysDict(Target,IsoCode,KeyWord,Translated) " +
                        " values( @Target,@IsoCode,@KeyWord,@Translated) ");
            }
            else
            {
                SQL.Add(" Update XysDict set " +
                        " Translated = @Translated " +
                        " WHERE Target+IsoCode+KeyWord = @KeyData");
            }

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@KeyData", Value = PartialData, SqlDbType = System.Data.SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@IsoCode", Value = ViewPart.Field("IsoCode").value, SqlDbType = System.Data.SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@KeyWord", Value = ViewPart.Field("KeyWord").value, SqlDbType = System.Data.SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@Translated", Value = ViewPart.Field("Translated").value, SqlDbType = System.Data.SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        public ApiResponse DeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.PopUpWindow(DialogQstDelete("XysDictEV/ConfirmDeleteData"), References.Elements.PageContents);
            return _ApiResponse;
        }

        public ApiResponse ConfirmDeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            string rlt = PutDeleteData();
            if (string.IsNullOrEmpty(rlt))
            {
                _ApiResponse.PopUpWindow(DialogMsgDeleted("m=XysDictMV"), References.Elements.PageContents);
            }
            else
            {
                _ApiResponse.PopUpWindow(DialogMsg(rlt), References.Elements.PageContents);
            }
            return _ApiResponse;
        }

        private string PutDeleteData()
        {
            List<string> SQL = new List<string>
        {
            " delete from XysDict where Target+IsoCode+KeyWord = @KeyData "
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@KeyData", Value = PartialData, SqlDbType = System.Data.SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }
    }
}
