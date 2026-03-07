using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysPageEV : WebBase
    {
        public XysPageEV()
        {
            ViewPart.Fields.AddRange(new NameValueFlag[] {
            new NameValueFlag { name = "PageId", flag = true },
            new NameValueFlag { name = "PageName" },
            new NameValueFlag { name = "PageGroup" },
            new NameValueFlag { name = "PageDesc" },
            new NameValueFlag { name = "PageOrder" },
            new NameValueFlag { name = "PageMenu" },
            new NameValueFlag { name = "PageUse" }
        });
        }

        public override void InitialViewData()
        {
            string SSQL = " Select PageId,PageName,PageGroup,PageDesc,PageOrder,PageMenu,PageUse From XysPage   " +
                          " where PageId = N'" + PartialData + "'";

            ViewPart.BindData(SSQL);
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(ViewPart.Data == null ? new string[] { "save" } : null);

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = ViewPart.Data == null ? Translator.Format("newpage") : Translator.Format("editpage");

            FilterSection filter = new();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "90%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Texts text = new Texts(Translator.Format("name"), ViewPart.Field("PageName").name, TextTypes.text);
            text.Required = true;
            text.Text.SetStyle(HtmlStyles.width, "200px");
            text.Text.SetAttribute(HtmlAttributes.maxlength, "200");
            text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("PageName").value);
            text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text1 = new Texts(Translator.Format("group"), ViewPart.Field("PageGroup").name, TextTypes.text);
            text1.Text.SetStyle(HtmlStyles.width, "200px");
            text1.Text.SetAttribute(HtmlAttributes.maxlength, "50");
            text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("PageGroup").value);
            text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text2 = new Texts(Translator.Format("desc"), ViewPart.Field("PageDesc").name, TextTypes.text);
            text2.Required = true;
            text2.Text.SetStyle(HtmlStyles.width, "300px");
            text2.Text.SetAttribute(HtmlAttributes.maxlength, "200");
            text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("PageDesc").value);
            text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text3 = new Texts(Translator.Format("order"), ViewPart.Field("PageOrder").name, TextTypes.text);
            text3.Text.SetStyle(HtmlStyles.width, "50px");
            text3.Text.SetAttribute(HtmlAttributes.maxlength, "4");
            text3.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("PageOrder").value);
            text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            CheckBox chk1 = new CheckBox(Translator.Format("menu"), ViewPart.Field("PageMenu").name);
            chk1.Checks.AddItem(ViewPart.Field("PageMenu").name, "1", Convert.ToDouble(ViewPart.Field("PageMenu").value ?? "0") == 1);
            chk1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            CheckBox chk2 = new CheckBox(Translator.Format("use"), ViewPart.Field("PageUse").name);
            chk2.Checks.AddItem(ViewPart.Field("PageUse").name, "1", Convert.ToDouble(ViewPart.Field("PageUse").value ?? "0") == 1);
            chk2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

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
            elmBox.AddItem(text3, 20);
            elmBox.AddItem(chk1, 1);
            elmBox.AddItem(chk2, 50);
            elmBox.AddItem(ViewButtons, 20);

            return filter.HtmlText + elmBox.HtmlText;
        }

        public ApiResponse SaveData()
        {
            string PageName = ViewPart.Field("PageName").value;
            string PageDesc = ViewPart.Field("PageDesc").value;

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(PageName) || string.IsNullOrEmpty(PageDesc))
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
            }
            else
            {
                string rlt = PutSaveData();
                if (string.IsNullOrEmpty(rlt))
                {
                    _ApiResponse.PopUpWindow(DialogMsgSaved("m=XysPageMV"), References.Elements.PageContents);
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
                ViewPart.Field("PageId").value = NewID();
                SQL.Add(" Insert into XysPage( PageId,PageName,PageGroup,PageDesc,PageOrder,PageMenu,PageUse,SYSDTE,SYSUSR) " +
                        " values( @PageId, @PageName, @PageGroup, @PageDesc, @PageOrder, @PageMenu,@PageUse, getdate(), @SYSUSR) ");
            }
            else
            {
                SQL.Add(" Update XysPage set " +
                    " PageName = @PageName, PageGroup = @PageGroup, PageDesc = @PageDesc, " +
                    " PageOrder = @PageOrder, PageMenu = @PageMenu, PageUse= @PageUse, SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                    " WHERE PageId = @PageId");
            }

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@PageId", Value = ViewPart.Field("PageId").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@PageName", Value = ViewPart.Field("PageName").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@PageGroup", Value = ViewPart.Field("PageGroup").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@PageDesc", Value = ViewPart.Field("PageDesc").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@PageOrder", Value = Common.Val(ViewPart.Field("PageOrder").value).ToString(), SqlDbType = SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@PageMenu", Value = Common.Val(ViewPart.Field("PageMenu").value).ToString(), SqlDbType = SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@PageUse", Value = Common.Val(ViewPart.Field("PageUse").value).ToString(), SqlDbType = SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        public ApiResponse DeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.PopUpWindow(DialogQstDelete("XysPageEV/ConfirmDeleteData"), References.Elements.PageContents);
            return _ApiResponse;
        }

        public ApiResponse ConfirmDeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            string rlt = PutDeleteData();
            if (string.IsNullOrEmpty(rlt))
            {
                _ApiResponse.PopUpWindow(DialogMsgDeleted("m=XysPageMV"), References.Elements.PageContents);
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
            " delete from XysRoleMenu where MenuId in (select MenuId from XysMenu where Pageid = @PageId) ",
            " delete from XysMenu where Pageid = @PageId ",
            " delete from XysRolePage where Pageid = @PageId ",
            " delete from XysPage where Pageid = @PageId "
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@PageId", Value = ViewPart.Field("PageId").value, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }
    }

}
