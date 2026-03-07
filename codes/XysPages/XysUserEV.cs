using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysUserEV : WebBase
    {
        public XysUserEV()
        {
            ViewPart.Fields.AddRange(new NameValueFlag[] {
            new NameValueFlag { name = "UserId", flag = true },
            new NameValueFlag { name = "UserRoles" },
            new NameValueFlag { name = "UserPwd" },
            new NameValueFlag { name = "UserOTP" },
            new NameValueFlag { name = "UserStatus" },
            new NameValueFlag { name = "LevelCode" },
            new NameValueFlag { name = "UserEmail" },
            new NameValueFlag { name = "UserName" },
            new NameValueFlag { name = "UserPhone" }
        });
        }

        public override void InitialViewData()
        {
            string SSQL = " select a.UserId,a.UserPwd,a.UserOTP, a.UserStatus, a.LevelCode, b.UserEmail,b.UserName,b.UserPhone " +
                          " from XysUser a inner join XysUserInfo b on a.UserId = b.UserId " +
                          " where a.UserId = N'" + PartialData + "'";

            ViewPart.BindData(SSQL);
            ViewPart.Field("UserPwd").value = Encryptor.DecryptData(ViewPart.Field("UserPwd").value);
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(ViewPart.Data == null ? new string[] { "save" } : null);

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = ViewPart.Data == null ? Translator.Format("newuser") : Translator.Format("edituser");

            FilterSection filter = new();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "110px");
            filter.Wrap.SetStyle(HtmlStyles.width, "90%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Texts text = new Texts(Translator.Format("email"), ViewPart.Field("UserEmail").name, TextTypes.text);
            text.Required = true;
            text.Text.SetStyle(HtmlStyles.width, "300px");
            text.Text.SetAttribute(HtmlAttributes.maxlength, "200");
            text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserEmail").value);
            text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text1 = new Texts(Translator.Format("name"), ViewPart.Field("UserName").name, TextTypes.text);
            text1.Required = true;
            text1.Text.SetStyle(HtmlStyles.width, "200px");
            text1.Text.SetAttribute(HtmlAttributes.maxlength, "10");
            text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserName").value);
            text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text2 = new Texts(Translator.Format("phone"), ViewPart.Field("UserPhone").name, TextTypes.text);
            text2.Text.SetStyle(HtmlStyles.width, "200px");
            text2.Text.SetAttribute(HtmlAttributes.maxlength, "16");
            text2.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserPhone").value);
            text2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            CheckBox roles = new CheckBox(Translator.Format("roles"), "UserRoles");
            roles.Required = true;
            roles.CheckWrap.SetStyles("max-width:350px; padding:4px;");
            roles.Checks.AddTextItems("sql@declare @userid nvarchar(50) " +
                                      " set @userid = N'" + PartialData + "' " +
                                      " select RoleId,RoleName,dbo.XF_UserRole(@userid,x.RoleId) " +
                                      " from XysRole x order by RoleOrder");
            roles.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text3 = new Texts(Translator.Format("pwd"), ViewPart.Field("UserPwd").name, TextTypes.text);
            text3.Required = true;
            text3.Text.SetStyle(HtmlStyles.width, "200px");
            text3.Text.SetAttribute(HtmlAttributes.maxlength, "32");
            text3.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserPwd").value);
            text3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            CheckBox chk1 = new CheckBox(Translator.Format("mfa"), ViewPart.Field("UserOTP").name);
            chk1.Checks.AddItem(ViewPart.Field("UserOTP").name, "1", Convert.ToDouble(ViewPart.Field("UserOTP").value ?? "0") == 1);
            chk1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Dropdown sel3 = new Dropdown(Translator.Format("levelcode"), ViewPart.Field("LevelCode").name);
            sel3.SelBox.SetStyle(HtmlStyles.width, "216px");
            sel3.SelOptions = new OptionValues("SQL@select LevelCode, LevelName from XysLevel order by LevelCode", ViewPart.Field("LevelCode").value);
            sel3.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Dropdown sel2 = new Dropdown(Translator.Format("status"), ViewPart.Field("UserStatus").name);
            sel2.SelBox.SetStyle(HtmlStyles.width, "216px");
            sel2.SelOptions = new OptionValues("{0|Normal}{8|Suspended}{9|Terminated}", ViewPart.Field("UserStatus").value);
            sel2.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            HtmlWrapper elmWrap = new HtmlWrapper();
            elmWrap.AddContents(text, 1);
            elmWrap.AddContents(text1, 1);
            elmWrap.AddContents(text2, 10);
            elmWrap.AddContents(text3);
            elmWrap.AddContents(chk1, 10);
            elmWrap.AddContents(sel3, 10);
            elmWrap.AddContents(sel2, 16);
            elmWrap.AddContents(roles, 36);
            elmWrap.AddContents(GetViewButtons(), 20);

            Wrap col = new Wrap();
            col.SetStyle(HtmlStyles.marginLeft, "40px");
            col.InnerText = elmWrap.HtmlText;

            string imgfile = PhysicalFolder + "photos\\" + ViewPart.Field("UserId").value + ".jpg";
            if (!File.Exists(imgfile))
            {
                imgfile = ImagePath + "img_fakeuser.jpg";
            }
            else
            {
                imgfile = GetPhotoData(imgfile);
            }

            HtmlTag img = new HtmlTag(HtmlTags.img, HtmlTag.Types.Empty);
            img.SetAttribute(HtmlAttributes.id, "UserPic");
            img.SetAttribute(HtmlAttributes.src, imgfile);
            img.SetStyle(HtmlStyles.borderRadius, "6px");
            img.SetStyle(HtmlStyles.width, "220px");
            img.SetStyle(HtmlStyles.height, "250px");
            img.SetStyle(HtmlStyles.objectFit, "cover");

            HtmlTag imginput = new HtmlTag(HtmlTags.input, HtmlTag.Types.Empty);
            imginput.SetAttribute(HtmlAttributes.id, "UserFile");
            imginput.SetAttribute(HtmlAttributes.type, "file");
            imginput.SetAttribute(HtmlEvents.onchange, "UpdatePhoto('UserPic','UserFile','" + EncryptString(ViewPart.Field("UserId").value) + "')");
            imginput.SetStyles("left: 0px; top: 0px; width: 100%; height: 100%; color: transparent; position: absolute; cursor: pointer; opacity: 0;");

            HtmlTag imgbtnwrap = new HtmlTag();
            imgbtnwrap.SetStyles("overflow: hidden; display: inline-block; border-radius:4px; width:36px; height:36px; position:absolute; bottom:0px;right:-2px;background-repeat: no-repeat;background-size: contain;");
            imgbtnwrap.SetStyle(HtmlStyles.backgroundImage, "url('" + ImagePath + "changephoto.jpg')");
            imgbtnwrap.InnerText = imginput.HtmlText;

            Wrap imgWrap = new Wrap();
            imgWrap.SetStyle(HtmlStyles.position, "relative");
            imgWrap.SetStyle(HtmlStyles.width, "220px");
            imgWrap.SetStyle(HtmlStyles.height, "250px");
            imgWrap.InnerText = img.HtmlText + imgbtnwrap.HtmlText;

            Wrap col1 = new Wrap();
            col1.SetStyle(HtmlStyles.padding, "20px");
            col1.InnerText = imgWrap.HtmlText;

            Wrap colWrap = new Wrap();
            colWrap.SetStyle(HtmlStyles.width, "90%");
            colWrap.SetStyle(HtmlStyles.margin, "auto");
            colWrap.SetStyle(HtmlStyles.marginTop, "10px");
            colWrap.SetStyle(HtmlStyles.marginBottom, "80px");
            colWrap.SetStyle(HtmlStyles.borderRadius, "2px");
            colWrap.SetStyle(HtmlStyles.border, "1px solid #ddd");
            colWrap.SetStyle(HtmlStyles.boxShadow, "3px 4px 6px 1px rgba(0, 0, 0, 0.15)");
            colWrap.SetStyle(HtmlStyles.boxSizing, "border-box");
            colWrap.SetStyle(HtmlStyles.padding, "30px");
            colWrap.SetStyle(HtmlStyles.display, "flex");
            colWrap.InnerText = (ViewPart.Data == null ? string.Empty : col1.HtmlText) + col.HtmlText;

            return filter.HtmlText + colWrap.HtmlText;
        }

        public ApiResponse SaveData()
        {
            string UserRoles = ViewPart.Field("UserRoles").value;
            string UserPwd = ViewPart.Field("UserPwd").value;
            string UserEmail = ViewPart.Field("UserEmail").value;
            string UserName = ViewPart.Field("UserName").value;

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(UserPwd) || string.IsNullOrEmpty(UserEmail) || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(UserRoles))
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
            }
            else
            {
                string rlt = PutSaveData();
                if (string.IsNullOrEmpty(rlt))
                {
                    _ApiResponse.PopUpWindow(DialogMsgSaved("m=XysUserMV"), References.Elements.PageContents);
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
                ViewPart.Field("UserId").value = NewID();
                SQL.Add(" Insert into XysUser( UserId,UserPwd,UserOTP,UserStatus,LevelCode,PassChanged,Created,Closed,SYSDTE,SYSUSR) " +
                        " values ( @UserId, @UserPwd, @UserOTP, @UserStatus,@LevelCode, getdate(), getdate(), getdate(), getdate(), @SYSUSR) ");
                SQL.Add(" Insert into XysUserInfo( UserId,UserName,UserDesc,UserEmail,UserPhone,UserPic,UserRef) " +
                        " values ( @UserId, @UserName, N'', @UserEmail, @UserPhone, N'', N'') ");
                SQL.Add(" Insert into XysUserRole( UserId,RoleId,SYSDTE,SYSUSR) values ( @UserId, @RoleId, getdate(), @SYSUSR) ");
            }
            else
            {
                SQL.Add(" update XysUser set UserPwd=@UserPwd,UserOTP=@UserOTP,UserStatus=@UserStatus,LevelCode=@LevelCode, SYSDTE = getdate(),SYSUSR=@SYSUSR where UserId = @UserId ");
                SQL.Add(" update XysUserInfo set UserEmail = @UserEmail,UserName = @UserName,UserPhone=@UserPhone where UserId = @UserId ");
                SQL.Add(" delete from XysUserRole where UserId = @UserId ");
                SQL.Add(" Insert into XysUserRole( UserId,RoleId,SYSDTE,SYSUSR) values ( @UserId, @RoleId, getdate(), @SYSUSR) ");
            }

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@UserId", Value = ViewPart.Field("UserId").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@UserPwd", Value = Encryptor.EncryptData(ViewPart.Field("UserPwd").value), SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@UserOTP", Value = Common.Val(ViewPart.Field("UserOTP").value), SqlDbType = SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@UserStatus", Value = Common.Val(ViewPart.Field("UserStatus").value), SqlDbType = SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@LevelCode", Value = Common.Val(ViewPart.Field("LevelCode").value), SqlDbType = SqlDbType.Int });
            SqlParams.Add(new SqlParameter { ParameterName = "@UserEmail", Value = ViewPart.Field("UserEmail").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@UserName", Value = ViewPart.Field("UserName").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@UserPhone", Value = ViewPart.Field("UserPhone").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@SYSUSR", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });
            // Handling the split Roles array
            SqlParams.Add(new SqlParameter { ParameterName = "@RoleId", Value = ViewPart.Field("UserRoles").value.Split(','), SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }

        public ApiResponse DeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.PopUpWindow(DialogQstDelete("XysUserEV/ConfirmDeleteData"), References.Elements.PageContents);
            return _ApiResponse;
        }

        public ApiResponse ConfirmDeleteData()
        {
            ApiResponse _ApiResponse = new ApiResponse();
            string rlt = PutDeleteData();
            if (string.IsNullOrEmpty(rlt))
            {
                _ApiResponse.PopUpWindow(DialogMsgDeleted("m=XysUserMV"), References.Elements.PageContents);
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
            " delete from XysUserRole where UserId = @UserId ",
            " delete from XysUserReset where UserId = @UserId ",
            " delete from XysUserInfo where UserId = @UserId ",
            " delete from XysUser where UserId = @UserId "
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@UserId", Value = ViewPart.Field("UserId").value, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }
    }

}
