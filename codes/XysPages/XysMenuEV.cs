using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysMenuEV : WebBase
    {
        public XysMenuEV()
        {
            ViewPart.Fields.AddRange(new NameValueFlag[] {
                new NameValueFlag { name = "MenuId", flag = true },
                new NameValueFlag { name = "MenuDesc" },
                new NameValueFlag { name = "MenuArea" },
                new NameValueFlag { name = "MenuTag" },
                new NameValueFlag { name = "MenuMethod" },
                new NameValueFlag { name = "MenuParams" },
                new NameValueFlag { name = "MenuCtl" },
                new NameValueFlag { name = "MenuType" },
                new NameValueFlag { name = "MenuClass" },
                new NameValueFlag { name = "MenuOrder" },
                new NameValueFlag { name = "MenuUse" },
                new NameValueFlag { name = "PageId" }
            });
        }

        public override void InitialViewData()
        {
            string SSQL = " Select MenuId,PageId,MenuDesc,MenuArea,MenuTag,MenuMethod,MenuParams,MenuCtl,MenuType,MenuClass,MenuOrder,MenuUse From XysMenu   " +
                          " where MenuId = N'" + PartialData + "'";
            ViewPart.BindData(SSQL);
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(ViewPart.Data == null ? new string[] { "save" } : null);

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = ViewPart.Data == null ? Translator.Format("newmenu") : Translator.Format("editmenu");

            FilterSection filter = new();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "90%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Dropdown sel1 = new Dropdown(Translator.Format("page"), ViewPart.Field("PageId").name);
            sel1.Required = true;
            sel1.SelBox.SetStyle(HtmlStyles.width, "414px");
            sel1.SelOptions = new OptionValues("sql@select PageId,PageName + case when PageName <> PageDesc then ' (' + PageDesc + ')' else '' end as PageName " +
                                               " from XysPage order by PageOrder,PageName", ViewPart.Field("PageId").value);
            sel1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Dropdown sel2 = new Dropdown(Translator.Format("area"), ViewPart.Field("MenuArea").name);
            sel2.Required = true;
            sel2.SelBox.SetStyle(HtmlStyles.width, "120px");
            sel2.SelOptions = new OptionValues("{X|Method}{M|Menu Item}{B|Button}", ViewPart.Field("MenuArea").value);
            sel2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text2 = new Texts(Translator.Format("desc"), ViewPart.Field("MenuDesc").name, TextTypes.text);
            text2.Required = true;
            text2.Text.SetStyle(HtmlStyles.width, "400px");
            text2.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuDesc").value);
            text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text = new Texts(Translator.Format("tag"), ViewPart.Field("MenuTag").name, TextTypes.text);
            text.Text.SetStyle(HtmlStyles.width, "268px");
            text.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuTag").value);
            text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text1 = new Texts(Translator.Format("method"), ViewPart.Field("MenuMethod").name, TextTypes.text);
            text1.Required = true;
            text1.Text.SetStyle(HtmlStyles.width, "400px");
            text1.Text.SetAttribute(HtmlAttributes.maxlength, "400");
            text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuMethod").value);
            text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text3 = new Texts(Translator.Format("params"), ViewPart.Field("MenuParams").name, TextTypes.text);
            text3.Text.SetStyle(HtmlStyles.width, "400px");
            text3.Text.SetStyle(HtmlStyles.fontSize, "14px");
            text3.Text.SetAttribute(HtmlAttributes.maxlength, "500");
            text3.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuParams").value);
            text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text4 = new Texts(Translator.Format("class"), ViewPart.Field("MenuClass").name, TextTypes.text);
            text4.Text.SetStyle(HtmlStyles.width, "200px");
            text4.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text4.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("MenuClass").value);
            text4.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Dropdown sel3 = new Dropdown(Translator.Format("order"), ViewPart.Field("MenuOrder").name);
            sel3.Required = true;
            sel3.SelBox.SetStyle(HtmlStyles.width, "80px");
            sel3.SelOptions = new OptionValues("{0|0}{1|1}{2|2}{3|3}{4|4}{5|5}{6|6}{7|7}{8|8}{9|9}{10|10}{11|11}{12|12}{13|13}{14|14}{15|15}", ViewPart.Field("MenuOrder").value);
            sel3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            CheckBox chk1 = new CheckBox(Translator.Format("use"), ViewPart.Field("MenuUse").name);
            if (ViewPart.Data != null)
            {
                bool isChecked = Convert.ToDouble(ViewPart.Field("MenuUse").value ?? "0") == 1;
                chk1.Checks.AddItem(ViewPart.Field("MenuUse").name, "1", isChecked);
            }
            else
            {
                chk1.Checks.AddItem(ViewPart.Field("MenuUse").name, "1", true);
            }
            chk1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "90%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");

            elmBox.AddItem(sel1, 1);
            elmBox.AddItem(text2, 1);
            elmBox.AddItem(sel2);
            elmBox.AddItem(text, 20);
            elmBox.AddItem(text1, 1);
            elmBox.AddItem(text3, 1);
            elmBox.AddItem(text4, 20);
            elmBox.AddItem(sel3, 20);
            elmBox.AddItem(chk1, 50);
            elmBox.AddItem(ViewButtons, 20);

            return filter.HtmlText + elmBox.HtmlText;
        }

        public ApiResponse SaveData()
        {
            string MenuDesc = ViewPart.Field("MenuDesc").value;
            string MenuMethod = ViewPart.Field("MenuMethod").value;
            string PageId = ViewPart.Field("PageId").value;

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(PageId) || string.IsNullOrEmpty(MenuMethod) || string.IsNullOrEmpty(MenuDesc))
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
            }
            else
            {
                string rlt = PutSaveData();
                if (string.IsNullOrEmpty(rlt))
                {
                    _ApiResponse.PopUpWindow(DialogMsgSaved("m=XysMenuMV"), References.Elements.PageContents);
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

            string tag = ViewPart.Field("MenuTag").value;
            string method = ViewPart.Field("MenuMethod").value;
            string paramsVal = ViewPart.Field("MenuParams").value;
            string styleclass = ViewPart.Field("MenuClass").value;

            switch (ViewPart.Field("MenuArea").value.ToLower())
            {
                case "x":
                    ViewPart.Field("MenuCtl").value = string.Empty;
                    ViewPart.Field("MenuType").value = string.Empty;
                    break;
                case "m":
                    string ctlObjM = SerializeObject(MenuElement(tag, method, paramsVal, styleclass), typeof(HtmlTag));
                    ViewPart.Field("MenuCtl").value = EscQuote(ctlObjM);
                    ViewPart.Field("MenuType").value = typeof(HtmlTag).FullName;
                    break;
                case "b":
                    string ctlObjB = SerializeObject(BtnElement(tag, method, paramsVal, styleclass), typeof(Button));
                    ViewPart.Field("MenuCtl").value = EscQuote(ctlObjB);
                    ViewPart.Field("MenuType").value = typeof(Button).FullName;
                    break;
            }

            if (ViewPart.Data == null)
            {
                ViewPart.Field("MenuId").value = NewID();
                SQL.Add(" Insert into XysMenu( MenuId,PageId,MenuDesc,MenuArea,MenuTag,MenuMethod,MenuParams,MenuCtl,MenuType,MenuClass,MenuOrder,MenuUse,SYSDTE,SYSUSR) " +
                        " values ( @MenuId, @PageId, @MenuDesc, @MenuArea, @MenuTag, @MenuMethod, @MenuParams, @MenuCtl, @MenuType, @MenuClass,@MenuOrder,@MenuUse, getdate(), @SYSUSR)");
            }
            else
            {
                SQL.Add(" Update XysMenu set " +
                        " PageId = @PageId, MenuDesc = @MenuDesc, MenuArea = @MenuArea, MenuTag = @MenuTag, MenuMethod = @MenuMethod, " +
                        " MenuParams = @MenuParams, MenuCtl = @MenuCtl,MenuType = @MenuType, MenuClass = @MenuClass, MenuOrder = @MenuOrder, MenuUse = @MenuUse, " +
                        " SYSDTE = getdate(), SYSUSR = @SYSUSR " +
                        " WHERE MenuId = @MenuId");
            }

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@MenuId", Value = ViewPart.Field("MenuId").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@PageId", Value = ViewPart.Field("PageId").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@MenuDesc", Value = ViewPart.Field("MenuDesc").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@MenuArea", Value = ViewPart.Field("MenuArea").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@MenuTag", Value = ViewPart.Field("MenuTag").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@MenuMethod", Value = ViewPart.Field("MenuMethod").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@MenuParams", Value = ViewPart.Field("MenuParams").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@MenuCtl", Value = ViewPart.Field("MenuCtl").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@MenuType", Value = ViewPart.Field("MenuType").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@MenuClass", Value = ViewPart.Field("MenuClass").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@MenuOrder", Value = Common.Val(ViewPart.Field("MenuOrder").value).ToString(), SqlDbType = SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@MenuUse", Value = Common.Val(ViewPart.Field("MenuUse").value).ToString(), SqlDbType = SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        public ApiResponse DeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.PopUpWindow(DialogQstDelete("XysMenuEV/ConfirmDeleteData"), References.Elements.PageContents);
            return _ApiResponse;
        }

        public ApiResponse ConfirmDeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            string rlt = PutDeleteData();
            if (string.IsNullOrEmpty(rlt))
            {
                _ApiResponse.PopUpWindow(DialogMsgDeleted("m=XysMenuMV"), References.Elements.PageContents);
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
            " delete from XysRoleMenu where MenuId = @MenuId ",
            " delete from XysMenu where Menuid = @MenuId "
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@MenuId", Value = ViewPart.Field("MenuId").value, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        private HtmlTag MenuElement(string tag, string method, string paramsVal, string styleclass)
        {
            paramsVal = !string.IsNullOrWhiteSpace(paramsVal) ? paramsVal.Trim() : "{params}";
            HtmlTag mnu = new HtmlTag();
            mnu.InnerText = Translator.Format(tag);
            mnu.SetAttribute(HtmlAttributes.@class, styleclass);
        mnu.SetAttribute(HtmlEvents.onclick, ByPassCall(method, paramsVal, false));
        return mnu;
    }

    private Button BtnElement(string tag, string method, string paramsVal, string styleclass)
        {
            paramsVal = !string.IsNullOrWhiteSpace(paramsVal) ? paramsVal.Trim() : "{params}";
            Button btn = new Button(Translator.Format(tag), Button.ButtonTypes.Button);
            btn.SetAttribute(HtmlAttributes.@class, styleclass);
            btn.SetAttribute(HtmlEvents.onclick, ByPassCall(method, paramsVal, false));
            btn.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");
            return btn;
        }
    }

}
