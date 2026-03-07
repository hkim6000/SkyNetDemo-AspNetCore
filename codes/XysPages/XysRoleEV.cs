using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysRoleEV : WebBase
    {
        public XysRoleEV()
        {
            ViewPart.Fields.AddRange(new NameValueFlag[] {
            new NameValueFlag { name = "RoleId", flag = true },
            new NameValueFlag { name = "RoleName" },
            new NameValueFlag { name = "RoleAlias" },
            new NameValueFlag { name = "RoleOrder" }
        });
        }

        public override void InitialViewData()
        {
            string SSQL = " Select RoleId,RoleName,RoleAlias,RoleOrder,SYSDTE,SYSUSR From XysRole Where  RoleId = '" + PartialData + "'";
            ViewPart.BindData(SSQL);
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems(ViewPart.Data == null ? new string[] { "backtomain" } : new string[] { });
            Wrap ViewButtons = GetViewButtons(ViewPart.Data == null ? new string[] { "save" } : null);

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = ViewPart.Data == null ? Translator.Format("newrole") : Translator.Format("editrole");

            FilterSection filter = new();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "90%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Texts text = new Texts(Translator.Format("name"), ViewPart.Field("RoleName").name, TextTypes.text);
            text.Required = true;
            text.Text.SetStyle(HtmlStyles.width, "200px");
            text.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("RoleName").value);
            text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text1 = new Texts(Translator.Format("alias"), ViewPart.Field("RoleAlias").name, TextTypes.text);
            text1.Required = true;
            text1.Text.SetStyle(HtmlStyles.width, "200px");
            text1.Text.SetAttribute(HtmlAttributes.maxlength, "10");
            text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("RoleAlias").value);
            text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Dropdown sel1 = new Dropdown(Translator.Format("order"), ViewPart.Field("RoleOrder").name);
            sel1.Required = true;
            sel1.SelBox.SetStyle(HtmlStyles.width, "216px");
            sel1.SelOptions = new OptionValues("{0|0}{1|1}{2|2}{3|3}{4|4}{5|5}{6|6}{7|7}{8|8}{9|9}{10|10}", ViewPart.Field("RoleOrder").value);
            sel1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "90%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");

            elmBox.AddItem(text, 1);
            elmBox.AddItem(text1, 20);
            elmBox.AddItem(sel1, 40);
            elmBox.AddItem(ViewButtons, 20);

            return filter.HtmlText + elmBox.HtmlText;
        }

        public ApiResponse SaveData()
        {
            string rolename = ViewPart.Field("RoleName").value;
            string rolealias = ViewPart.Field("RoleAlias").value;
            string roleorder = ViewPart.Field("RoleOrder").value;

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(rolename) || string.IsNullOrEmpty(rolealias) || string.IsNullOrEmpty(roleorder))
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
            }
            else
            {
                string rlt = PutSaveData();
                if (string.IsNullOrEmpty(rlt))
                {
                    _ApiResponse.PopUpWindow(DialogMsgSaved("m=XysRoleMV"), References.Elements.PageContents);
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
                ViewPart.Field("RoleId").value = NewID();
                SQL.Add(" Insert into XysRole( RoleId,RoleName,RoleAlias,RoleOrder,SYSDTE,SYSUSR) " +
                        " values ( @RoleId, @RoleName, @RoleAlias, @RoleOrder, getdate(), @SYSUSR) ");
            }
            else
            {
                SQL.Add(" Update XysRole set " +
                        " RoleName = @RoleName, RoleAlias = @RoleAlias, RoleOrder = @RoleOrder, " +
                        " SYSDTE = getdate(), SYSUSR = @SYSUSR Where RoleId = @RoleId ");
            }

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@RoleId", Value = ViewPart.Field("RoleId").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@RoleName", Value = ViewPart.Field("RoleName").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@RoleAlias", Value = ViewPart.Field("RoleAlias").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@RoleOrder", Value = ViewPart.Field("RoleOrder").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        public ApiResponse DeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();

            if (IfExists())
            {
                _ApiResponse.PopUpWindow(DialogMsg(Translator.Format("msg_exists")), References.Elements.PageContents);
            }
            else
            {
                _ApiResponse.PopUpWindow(DialogQstDelete("XysRoleEV/ConfirmDeleteData"), References.Elements.PageContents);
            }
            return _ApiResponse;
        }

        private bool IfExists()
        {
            bool rtnvlu = false;
            DataTable dt = SQLData.SQLDataTable(" select count(*) from XysUserInfo where RoleId = '" + ViewPart.Field("RoleId").value + "'");
            if (dt != null && dt.Rows.Count > 0)
            {
                if (Common.Val(dt.Rows[0][0].ToString()) > 0)
                {
                    rtnvlu = true;
                }
            }
            return rtnvlu;
        }

        public ApiResponse ConfirmDeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            string rlt = PutDeleteData();
            if (string.IsNullOrEmpty(rlt))
            {
                _ApiResponse.PopUpWindow(DialogMsgDeleted("m=XysRoleMV"), References.Elements.PageContents);
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
            " delete from XysRoleMenu where RoleId = @RoleId ",
            " delete from XysRolePage where RoleId = @RoleId ",
            " delete from XysRole where RoleId = @RoleId "
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@RoleId", Value = ViewPart.Field("RoleId").value, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }
    }

}
