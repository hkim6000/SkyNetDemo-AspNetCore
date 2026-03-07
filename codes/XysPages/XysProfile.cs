using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysBases;
using System.Data;

namespace ASPNETCoreWeb.codes.XysPages
{

    public class XysProfile : WebBase
    {
        public XysProfile()
        {
            ViewPart.Fields.AddRange(new NameValueFlag[] {
                new NameValueFlag { name = "UserEmail", flag = true },
                new NameValueFlag { name = "UserName" },
                new NameValueFlag { name = "UserPhone" },
                new NameValueFlag { name = "UserRole", flag = true }
            });
        }

        public override void InitialViewData()
        {
            string SSQL = " select UserEmail,UserName,UserPhone,dbo.XF_UserRoleNames(UserId) UserRole from XysUserInfo where UserId = N'" + AppKey.UserId + "'";
            ViewPart.BindData(SSQL);
        }

        public override string InitialViewHtml()
        {
            MenuList ViewMenuItems = GetViewMenuItems();
            Wrap ViewButtons = GetViewButtons(new string[] { });

            Label label = new Label();
            label.Wrap.SetStyles("font-weight:700;font-size:22px;margin:12px;");
            label.Wrap.InnerText = Translator.Format("profile");

            FilterSection filter = new();
            filter.ModalWrap = true;
            filter.Wrap.SetStyle(HtmlStyles.marginTop, "100px");
            filter.Wrap.SetStyle(HtmlStyles.width, "95%");
            filter.Menu = ViewMenuItems;
            filter.FilterHtml = label.HtmlText;

            Label txtid = new Label();
            txtid.Wrap.InnerText = Translator.Format("email") + "&nbsp;<b>" + ViewPart.Field("UserEmail").value + "</b>";
            txtid.Wrap.SetStyle(HtmlStyles.fontSize, "18px");
            txtid.Wrap.SetStyle(HtmlStyles.color, "#444");
            txtid.Wrap.SetStyle(HtmlStyles.marginLeft, "8px");

            Label txtrole = new Label();
            //txtrole.Wrap.InnerText = Translator.Format("userrole") + "&nbsp;<b>" + ViewPart.Field("UserRole").value + "</b>";
            txtrole.Wrap.InnerText =  ViewPart.Field("UserRole").value;
            txtrole.Wrap.SetStyle(HtmlStyles.fontSize, "18px");
            txtrole.Wrap.SetStyle(HtmlStyles.fontWeight, "bold");
            txtrole.Wrap.SetStyle(HtmlStyles.marginLeft, "8px");

            Texts text = new Texts(Translator.Format("name"), ViewPart.Field("UserName").name, TextTypes.text);
            text.Required = true;
            text.Text.SetStyle(HtmlStyles.width, "200px");
            text.Text.SetAttribute(HtmlAttributes.maxlength, "100");
            text.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserName").value);
            text.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            Texts text1 = new Texts(Translator.Format("phone"), ViewPart.Field("UserPhone").name, TextTypes.text);
            text1.Text.SetStyle(HtmlStyles.width, "200px");
            text1.Text.SetAttribute(HtmlAttributes.maxlength, "15");
            text1.Text.SetAttribute(HtmlAttributes.value, ViewPart.Field("UserPhone").value);
            text1.Wrap.SetStyle(HtmlStyles.paddingLeft, "4px");

            HtmlWrapper elmWrap = new HtmlWrapper();
            elmWrap.AddContents(txtid, 30);
            elmWrap.AddContents(txtrole, 10);
            elmWrap.AddContents(text, 1);
            elmWrap.AddContents(text1, 46);
            elmWrap.AddContents(GetViewButtons());

            Wrap col = new Wrap();
            col.SetStyle(HtmlStyles.marginLeft, "40px");
            col.InnerText = elmWrap.HtmlText;

            string imgfile = VirtualPath + "photos//" + AppKey.UserId + ".jpg";
            if (!File.Exists(imgfile))
            {
                imgfile = ImagePath + "img_fakeuser.jpg";
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
            imginput.SetAttribute(HtmlEvents.onchange, "UpdatePhoto('UserPic','UserFile','" + EncryptString(AppKey.UserId) + "')");
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
            colWrap.SetAttribute(HtmlAttributes.id, "elmBox");
            colWrap.SetStyle(HtmlStyles.width, "95%");
            colWrap.SetStyle(HtmlStyles.margin, "auto");
            colWrap.SetStyle(HtmlStyles.marginTop, "10px");
            colWrap.SetStyle(HtmlStyles.marginBottom, "80px");
            colWrap.SetStyle(HtmlStyles.borderRadius, "2px");
            colWrap.SetStyle(HtmlStyles.border, "1px solid #ddd");
            colWrap.SetStyle(HtmlStyles.boxShadow, "3px 4px 6px 1px rgba(0, 0, 0, 0.15)");
            colWrap.SetStyle(HtmlStyles.boxSizing, "border-box");
            colWrap.SetStyle(HtmlStyles.padding, "30px");
            colWrap.SetStyle(HtmlStyles.display, "flex");
            colWrap.InnerText = col1.HtmlText + col.HtmlText;

            HtmlDoc.AddJsFile(WebEnv.HeaderScripts.UserIconScript);
            HtmlDoc.AddJsFile(WebEnv.HeaderScripts.TimerScript);
            HtmlDoc.InitialScripts.ExecuteScript("$StartTimer('tmr')");

            TitleSection2 PageLayout = PageTitle();
            PageLayout.ContentWrap.SetAttribute(HtmlAttributes.id, References.Elements.PageContents);
            PageLayout.ContentWrap.InnerText = filter.HtmlText + colWrap.HtmlText;

            return PageLayout.HtmlText;
        }

        public ApiResponse PartialView()
        {
            string m = GetDataValue("m");
            string t = GetDataValue("t");

            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.SetElementContents(References.Elements.PageContents, PartialDocument(m, t));
            _ApiResponse.ExecuteScript("$ScrollToTop()");
            return _ApiResponse;
        }

        public ApiResponse Navigate()
        {
            string m = GetDataValue("m");
            ApiResponse _ApiResponse = new ApiResponse();
            _ApiResponse.Navigate(m);
            return _ApiResponse;
        }

        public ApiResponse SaveData()
        {
            string UserName = ViewPart.Field("UserName").value;
            string UserPhone = ViewPart.Field("UserPhone").value;

            ApiResponse _ApiResponse = new ApiResponse();
            if (string.IsNullOrEmpty(UserName))
            {
                _ApiResponse.PopUpWindow(DialogMsgRequred(), References.Elements.PageContents);
            }
            else
            {
                string rlt = PutSaveData();
                if (string.IsNullOrEmpty(rlt))
                {
                    AppKey.UserName = UserName;
                    AppKey.UserPhone = UserPhone;
                    string SerializedString = SerializeObjectEnc(AppKey, typeof(AppKey));
                    _ApiResponse.SetCookie(References.Keys.AppKey, SerializedString);

                    _ApiResponse.PopUpWindow(DialogMsgSaved(string.Empty), References.Elements.PageContents);
                }
                else
                {
                    DialogBox dialogBox = new DialogBox(rlt);
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
                }
            }

            return _ApiResponse;
        }

        private string PutSaveData()
        {
            List<string> SQL = new List<string>
        {
            " update XysUserInfo set UserName = @UserName,UserPhone=@UserPhone where UserId = @UserId " +
            " update XysUser set SYSDTE = getdate(),SYSUSR=@UserId where UserId = @UserId "
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>();
            SqlParams.Add(new SqlParameter { ParameterName = "@UserId", Value = AppKey.UserId, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@UserName", Value = ViewPart.Field("UserName").value, SqlDbType = SqlDbType.NVarChar });
            SqlParams.Add(new SqlParameter { ParameterName = "@UserPhone", Value = ViewPart.Field("UserPhone").value, SqlDbType = SqlDbType.NVarChar });

            return PutData(SqlWithParams(SQL, SqlParams));
        }
    }

}
