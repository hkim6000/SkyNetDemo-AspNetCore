using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysPermission : WebBase
    {
        public XysPermission()
        {
            ViewPart.Fields.AddRange(new NameValueFlag[] {
            new NameValueFlag { name = "RoleName", flag = true }
        });
        }

        public override void InitialViewData()
        {
            string SSQL = " Select RoleName From XysRole Where  RoleId = '" + PartialData + "'";
            ViewPart.BindData(SSQL);
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(ViewPart.Data == null ? new string[] { "save" } : null);

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = Translator.Format("rolepermission");

            FilterSection filter = new();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "90%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            HtmlElementBox elmBox = new HtmlElementBox();
            elmBox.SetAttribute(HtmlAttributes.id, References.Elements.ElmBox);
            elmBox.SetStyle(HtmlStyles.width, "90%");
            elmBox.SetStyle(HtmlStyles.margin, "auto");
            elmBox.SetStyle(HtmlStyles.marginTop, "8px");
            elmBox.SetStyle(HtmlStyles.marginBottom, "80px");
            elmBox.Wrap.SetStyle(HtmlStyles.margin, "50px 30px 30px 50px");
            elmBox.Wrap.SetStyle(HtmlStyles.overflow, "auto");

            elmBox.AddItem(SwitchUI(), 1);

            string ViewHtml = filter.HtmlText + elmBox.HtmlText;
            return ViewHtml;
        }

        private object SwitchUI()
        {
            string roleid = PartialData;
            string rolename = SQLData.SQLFieldValue("SELECT dbo.XF_RoleName(N'" + roleid + "')");

            string rlt = string.Empty;
            string sSql = " declare @RoleId NVARCHAR(50); set @RoleId = N'" + roleid + "'; " +
                          " select t1, case when t2='' and t3 <> '' then 'Common' else t2 end as t2,t3,t4,tck,tint from dbo.XF_RolePermission(@RoleId) order by tord;";
            DataTable dt = SQLData.SQLDataTable(sSql, ref rlt);

            Grid _tablex = new Grid();
            if (string.IsNullOrEmpty(rlt))
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    _tablex.Table.SetAttribute(HtmlAttributes.@class, "table");
                    _tablex.Table.SetStyle(HtmlStyles.fontSize, "14px");
                    _tablex.Table.SetStyle(HtmlStyles.width, "97%");
                    _tablex.Table.SetStyle(HtmlStyles.border, "1px solid #ddd");
                    _tablex.DataSource(dt);

                    for (int i = 0; i < _tablex.Headers.Count; i++)
                    {
                        switch (i)
                        {
                            case 0:
                            case 5:
                                _tablex.Headers[i].SetStyle(HtmlStyles.display, "none");
                                _tablex.SetColumnStyle(i, HtmlStyles.display, "none");
                                break;
                            case 1:
                                _tablex.Headers[i].InnerText = Translator.Format("area");
                                _tablex.SetColumnStyle(i, HtmlStyles.textAlign, "center");
                                _tablex.SetColumnStyle(i, HtmlStyles.fontWeight, "bold");
                                break;
                            case 2:
                                _tablex.Headers[i].InnerText = Translator.Format("page");
                                _tablex.SetColumnStyle(i, HtmlStyles.textAlign, "left");
                                _tablex.SetColumnStyle(i, HtmlStyles.fontWeight, "bold");
                                break;
                            case 3:
                                _tablex.Headers[i].InnerText = Translator.Format("function");
                                _tablex.SetColumnStyle(i, HtmlStyles.textAlign, "left");
                                break;
                            case 4:
                                _tablex.Headers[i].InnerText = Translator.Format("status");
                                _tablex.SetColumnStyle(i, HtmlStyles.textAlign, "center");
                                break;
                        }
                        _tablex.SetColumnStyle(i, HtmlStyles.whiteSpace, "nowrap");
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string rowid = dt.Rows[i][0].ToString();
                        string rowstatus = dt.Rows[i][4].ToString();
                        string rowtype = dt.Rows[i][5].ToString();

                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            if (j == 4)
                            {
                                string jsEvent = "SetRoleRange('XysPermission/SetRoleRange','" + roleid + "','" + rowid + "','" + rowtype + "',this.checked)";

                                Switch switchCtrl = new Switch();
                                switchCtrl.Id = rowid;
                                switchCtrl.Name = "switch";
                                switchCtrl.Attributes = "onclick:" + jsEvent;
                                switchCtrl.Size = 50;
                                if (Common.Val(rowstatus) == 1)
                                    switchCtrl.Checked = true;

                                _tablex.Rows[i].Columns[j].InnerText = switchCtrl.HtmlText;
                            }
                        }
                    }
                }
            }

            return _tablex;
        }

        public ApiResponse SetRoleRange()
        {
            string roleid = GetDataValue("d");
            string key = GetDataValue("c");
            string keytyp = GetDataValue("t");
            string keyvlu = GetDataValue("s");

            List<string> SQL = new List<string>();
            SQL.Add(" DECLARE @RoleId NVARCHAR(50),@PageId NVARCHAR(50),@Key NVARCHAR(50),@KeyTyp INT,@KeyVlu INT,@SYSDTE datetime,@SYSUSR nvarchar(100) " +
                     " SET @RoleId = N'" + roleid + "' " +
                     " SET @PageId = N'' " +
                     " SET @Key = N'" + key + "' " +
                     " SET @KeyTyp = " + Common.Val(keytyp).ToString() + " " +
                     " SET @KeyVlu = " + Common.Val(keyvlu).ToString() + " " +
                     " SET @SYSDTE = getdate() " +
                     " SET @SYSUSR = N'" + AppKey.UserId + "' " +
                     " IF @KeyTyp = 0 " +
                     " BEGIN " +
                     "      DELETE FROM XysRolePage WHERE RoleId=@RoleId AND PageId =@Key " +
                     "      if @KeyVlu = 1 " +
                     "      begin " +
                     "          INSERT INTO XysRolePage(RoleId,PageId,SYSDTE,SYSUSR) values(@RoleId,@Key,@SYSDTE,@SYSUSR) " +
                     "      end " +
                     "      else " +
                     "      begin " +
                     "          DELETE FROM XysRoleMenu WHERE RoleId=@RoleId AND MenuId in (select MenuId from XysMenu where PageId =@Key)  " +
                     "      end " +
                     " END " +
                     " ELSE " +
                     " BEGIN " +
                     "      SET @PageId = isnull((select PageId from XysMenu where MenuId = @Key),'') " +
                     "      DELETE FROM XysRoleMenu WHERE RoleId=@RoleId AND MenuId =@Key " +
                     "      if @KeyVlu = 1 " +
                     "      begin " +
                     "          INSERT INTO XysRoleMenu(RoleId,MenuId,SYSDTE,SYSUSR) values(@RoleId,@Key,@SYSDTE,@SYSUSR) " +
                     "          if not exists(select * from XysRolePage where RoleId=@RoleId and PageId = (select PageId from XysMenu where MenuId = @Key))  " +
                     "          begin " +
                     "              if @PageId <> '' " +
                     "              begin " +
                     "                  INSERT INTO XysRolePage(RoleId,PageId,SYSDTE,SYSUSR) values(@RoleId,@PageId,@SYSDTE,@SYSUSR) " +
                     "              end " +
                     "          end " +
                     "      end " +
                     "      else " +
                     "      begin " +
                     "          if @PageId <> '' " +
                     "          begin " +
                     "              if not exists(select * from XysRoleMenu where RoleId=@RoleId and MenuId in (select MenuId from XysMenu where PageId = @PageId))  " +
                     "              begin " +
                     "                  delete from XysRolePage where RoleId=@RoleId and PageId=@PageId " +
                     "              end " +
                     "          end " +
                     "      end " +
                     " END ");

            string rlt = PutData(SQL);

            ApiResponse _ApiResponse = new ApiResponse();
            if (!string.IsNullOrEmpty(rlt))
            {
                DialogBox dialogBox = new DialogBox(rlt);
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
            }

            string SSQL = string.Empty;
            if (Common.Val(keytyp) == 0)
            {
                SSQL = " DECLARE @RoleId NVARCHAR(50) " +
                       " SET @RoleId = N'" + roleid + "' " +
                       " select t1,tck from dbo.XF_RolePermission(@RoleId) where t1 in " +
                       " ( select PageId from XysPage where PageId = N'" + key + "' " +
                       "   union all " +
                       "   select MenuId from XysMenu where PageId = N'" + key + "' ) ";
            }
            else
            {
                SSQL = " DECLARE @RoleId NVARCHAR(50) " +
                       " SET @RoleId = N'" + roleid + "' " +
                       " select t1,tck from dbo.XF_RolePermission(@RoleId) where t1 in " +
                       " ( select PageId from XysPage where PageId = (select PageId from XysMenu where MenuId = N'" + key + "') " +
                       "   union all " +
                       "   select MenuId from XysMenu where MenuId = N'" + key + "' ) ";
            }

            DataTable dt = SQLData.SQLDataTable(SSQL, ref rlt);
            if (dt != null && dt.Rows.Count > 0)
            {
                string script = " var elem = document.getElementsByName('switch'); ";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string tid = dt.Rows[i][0].ToString();
                    string tbool = Convert.ToDouble(string.IsNullOrEmpty(dt.Rows[i][1].ToString()) ? "0" : dt.Rows[i][1].ToString()) == 0 ? "false" : "true";
                    script += " for (var i = 0, j = elem.length; i < j; i++) { " +
                              "    if (elem[i].id == '" + tid + "'){ " +
                              "       elem[i].checked = " + tbool + "; " +
                              "    } " +
                              " } ";
                }

                _ApiResponse.ExecuteScript(script);
            }

            return _ApiResponse;
        }

        public ApiResponse ItemSelected()
        {
            string t = GetDataValue("t");

            ApiResponse _ApiResponse = new ApiResponse();
            HtmlDocument Html = PartialDocument(References.Pages.XysRoleEV, t);
            _ApiResponse.SetElementContents(References.Elements.PageContents, Html);
            return _ApiResponse;
        }
    }

}
