using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysPageWz : WebBase
    {
        public XysPageWz()
        {
            ViewPart.Fields.AddRange(new NameValueFlag[] {
            new NameValueFlag { name = "PageName" },
            new NameValueFlag { name = "PageGroup" },
            new NameValueFlag { name = "PageDesc" },
            new NameValueFlag { name = "PageOrder" },
            new NameValueFlag { name = "PageSource" }
        });
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

            CheckBox chk1 = new CheckBox(Translator.Format("source"), ViewPart.Field("PageSource").name);
            chk1.Checks.AddItem(ViewPart.Field("PageSource").name, "1");
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
            elmBox.AddItem(text3, 10);
            elmBox.AddItem(chk1, 40);
            elmBox.AddItem(ViewButtons, 20);

            return filter.HtmlText + elmBox.HtmlText;
        }

        public ApiResponse SaveData()
        {
            string PageName = ViewPart.Field("PageName").value;
            string PageGroup = ViewPart.Field("PageGroup").value;
            string PageDesc = ViewPart.Field("PageDesc").value;
            string PageOrder = ViewPart.Field("PageOrder").value;
            string PageSource = ViewPart.Field("PageSource").value;

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
                    if (Convert.ToDouble(string.IsNullOrEmpty(PageSource) ? "0" : PageSource) == 1)
                    {
                        string tmpPagePath = ConfigFolder + "TempPage.txt";
                        string tmpPageEVPath = ConfigFolder + "TempPageEV.txt";
                        string tmpPageMVPath = ConfigFolder + "TempPageMV.txt";

                        string codePath = CodeFolder + PageName + ".vb";
                        string codePathEV = CodeFolder + PageName + "EV.vb";
                        string codePathMV = CodeFolder + PageName + "MV.vb";

                        if (File.Exists(tmpPagePath) && File.Exists(tmpPageEVPath) && File.Exists(tmpPageMVPath))
                        {
                            string pgTxt = File.ReadAllText(tmpPagePath).Replace("{TempPage}", PageName);
                            string pgTxtev = File.ReadAllText(tmpPageEVPath).Replace("{TempPage}", PageName);
                            string pgTxtmv = File.ReadAllText(tmpPageMVPath).Replace("{TempPage}", PageName);
                            File.WriteAllText(codePath, pgTxt);
                            File.WriteAllText(codePathEV, pgTxtev);
                            File.WriteAllText(codePathMV, pgTxtmv);
                        }
                    }

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

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@PageName", Value = ViewPart.Field("PageName").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@PageGroup", Value = ViewPart.Field("PageGroup").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@PageDesc", Value = ViewPart.Field("PageDesc").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@PageOrder", Value = Common.Val(ViewPart.Field("PageOrder").value).ToString(), SqlDbType = SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });

            string pgid = NewID();
            string pgnm = ViewPart.Field("PageName").value;
            string pgord = Common.Val(ViewPart.Field("PageOrder").value).ToString();
            SQL.Add(" Insert into XysPage( PageId,PageName,PageGroup,PageDesc,PageOrder,PageMenu,PageUse,SYSDTE,SYSUSR) " +
                    " values( N'" + pgid + "', N'" + pgnm + "', @PageGroup, @PageDesc, " + pgord + ", 1,1, getdate(), @SYSUSR) ");

            // MV ===============================================================
            pgid = NewID();
            pgnm = ViewPart.Field("PageName").value + "MV";
            pgord = (Common.Val(ViewPart.Field("PageOrder").value) + 3).ToString();
            SQL.Add(" Insert into XysPage( PageId,PageName,PageGroup,PageDesc,PageOrder,PageMenu,PageUse,SYSDTE,SYSUSR) " +
                    " values( N'" + pgid + "',N'" + pgnm + "', @PageGroup, @PageDesc + ' - View', " + pgord + ", 0,1, getdate(), @SYSUSR) ");

            string mntag = "create";
            string mnmethod = "PartialView";
            string mnparams = "m=" + ViewPart.Field("PageName").value + "EV";
            string mnstyleclass = string.Empty;
            string mnctl = SerializeObject(MenuElement(mntag, mnmethod, mnparams, mnstyleclass), typeof(HtmlTag));
            SQL.Add(" Insert into XysMenu( MenuId,PageId,MenuDesc,MenuArea,MenuTag,MenuMethod,MenuParams,MenuCtl,MenuType,MenuClass,MenuOrder,MenuUse,SYSDTE,SYSUSR) " +
                    " values ( lower(newid()), N'" + pgid + "', N'Create', N'M', N'" + mntag + "', N'" + mnmethod + "',N'" + mnparams + "', N'" + EscQuote(mnctl) + "', N'skylite.HtmlTag', N'" + mnstyleclass + "',0,1, getdate(), @SYSUSR)");

            mntag = "home";
            mnmethod = "Navigate";
            mnparams = "m=XysHome";
            mnstyleclass = string.Empty;
            mnctl = SerializeObject(MenuElement(mntag, mnmethod, mnparams, mnstyleclass), typeof(HtmlTag));
            SQL.Add(" Insert into XysMenu( MenuId,PageId,MenuDesc,MenuArea,MenuTag,MenuMethod,MenuParams,MenuCtl,MenuType,MenuClass,MenuOrder,MenuUse,SYSDTE,SYSUSR) " +
                    " values ( lower(newid()), N'" + pgid + "', N'Back to Home', N'M', N'" + mntag + "', N'" + mnmethod + "',N'" + mnparams + "', N'" + EscQuote(mnctl) + "', N'skylite.HtmlTag', N'" + mnstyleclass + "',1,1, getdate(), @SYSUSR)");

            mntag = "edit";
            mnmethod = "PartialView";
            mnparams = "m=" + ViewPart.Field("PageName").value + "EV&t={0}";
            mnstyleclass = string.Empty;
            mnctl = string.Empty;
            SQL.Add(" Insert into XysMenu( MenuId,PageId,MenuDesc,MenuArea,MenuTag,MenuMethod,MenuParams,MenuCtl,MenuType,MenuClass,MenuOrder,MenuUse,SYSDTE,SYSUSR) " +
                    " values ( lower(newid()), N'" + pgid + "', N'Edit', N'X', N'" + mntag + "', N'" + mnmethod + "',N'" + mnparams + "', N'" + EscQuote(mnctl) + "', N'', N'" + mnstyleclass + "',0,1, getdate(), @SYSUSR)");


            // EV ===============================================================
            pgid = NewID();
            pgnm = ViewPart.Field("PageName").value + "EV";
            pgord = (Common.Val(ViewPart.Field("PageOrder").value) + 6).ToString();
            SQL.Add(" Insert into XysPage( PageId,PageName,PageGroup,PageDesc,PageOrder,PageMenu,PageUse,SYSDTE,SYSUSR) " +
                    " values( N'" + pgid + "', N'" + pgnm + "' , @PageGroup, @PageDesc + ' - Edit', " + pgord + ", 0,1, getdate(), @SYSUSR) ");

            mntag = "save";
            mnmethod = ViewPart.Field("PageName").value + "EV/SaveData";
            mnparams = string.Empty;
            mnstyleclass = "button";
            mnctl = SerializeObject(BtnElement(mntag, mnmethod, mnparams, mnstyleclass), typeof(Button));
            SQL.Add(" Insert into XysMenu( MenuId,PageId,MenuDesc,MenuArea,MenuTag,MenuMethod,MenuParams,MenuCtl,MenuType,MenuClass,MenuOrder,MenuUse,SYSDTE,SYSUSR) " +
                    " values ( lower(newid()), N'" + pgid + "', N'Save', N'B', N'" + mntag + "', N'" + mnmethod + "',N'" + mnparams + "', N'" + EscQuote(mnctl) + "', N'skylite.ToolKit.Button', N'" + mnstyleclass + "',0,1, getdate(), @SYSUSR)");

            mntag = "delete";
            mnmethod = ViewPart.Field("PageName").value + "EV/DeleteData";
            mnparams = string.Empty;
            mnstyleclass = "button1";
            mnctl = SerializeObject(BtnElement(mntag, mnmethod, mnparams, mnstyleclass), typeof(Button));
            SQL.Add(" Insert into XysMenu( MenuId,PageId,MenuDesc,MenuArea,MenuTag,MenuMethod,MenuParams,MenuCtl,MenuType,MenuClass,MenuOrder,MenuUse,SYSDTE,SYSUSR) " +
                    " values ( lower(newid()), N'" + pgid + "', N'Delete', N'B', N'" + mntag + "', N'" + mnmethod + "',N'" + mnparams + "', N'" + EscQuote(mnctl) + "', N'skylite.ToolKit.Button', N'" + mnstyleclass + "',1,1, getdate(), @SYSUSR)");

            mntag = "backtomain";
            mnmethod = "PartialView";
            mnparams = "m=" + ViewPart.Field("PageName").value + "MV";
            mnstyleclass = string.Empty;
            mnctl = SerializeObject(MenuElement(mntag, mnmethod, mnparams, mnstyleclass), typeof(HtmlTag));
            SQL.Add(" Insert into XysMenu( MenuId,PageId,MenuDesc,MenuArea,MenuTag,MenuMethod,MenuParams,MenuCtl,MenuType,MenuClass,MenuOrder,MenuUse,SYSDTE,SYSUSR) " +
                    " values ( lower(newid()), N'" + pgid + "', N'Back to List', N'M', N'" + mntag + "', N'" + mnmethod + "',N'" + mnparams + "', N'" + EscQuote(mnctl) + "', N'skylite.HtmlTag', N'" + mnstyleclass + "',0,1, getdate(), @SYSUSR)");

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
