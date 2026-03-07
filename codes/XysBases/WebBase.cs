using Azure.Core;
using Microsoft.Data.SqlClient;
using SkyNet;
using SkyNet.ToolKit;
using ASPNETCoreWeb.codes.XysPages;
using System.Data;
using System.Drawing;
using System.Globalization;

namespace ASPNETCoreWeb.codes.XysBases
{
    public struct References
    {
        public static string ProjectTitle = WebCore.AppName;

        public struct Htmls
        {
            public const string Email_SignUp = "Email_SignUp";
            public const string Email_PassReset = "Email_PassReset";
            public const string Email_PassChanged = "Email_PassChanged";
            public const string Email_ResetDevice = "Email_ResetDevice";
        }

        public struct Keys
        {
            public static string SignUp_User = WebParams.GlobalPrefix + ProjectTitle + ".TempUser";
            public static string AppKey = WebParams.GlobalPrefix + ProjectTitle + ".AppKey";
            public static string PageKey = WebParams.GlobalPrefix + ProjectTitle + ".Page.{0}";
        }

        public struct Elements
        {
            public const string PageContents = "pagecontents";
            public const string ContentWrap = "contentwrap";
            public const string ElmBox = "elmBox";
        }

        public struct Pages
        {
            public const string XysSignupExpired = "XysSignupExpired";
            public const string XysUnAuthorized = "XysUnAuthorized";
            public const string XysAuth = "XysAuth";
            public const string XysVerify = "XysVerify";
            public const string XysSignup = "XysSignup";
            public const string XysSignin = "XysSignin";
            public const string XysSent = "XysSent";
            public const string XysPassReset = "XysPassReset";
            public const string XysPassChange = "XysPassChange";
            public const string XysPermission = "XysPermission";
            public const string XysPass = "XysPass";
            public const string XysRole = "XysRole";
            public const string XysRoleMV = "XysRoleMV";
            public const string XysRoleEV = "XysRoleEV";
            public const string XysUser = "XysUser";
            public const string XysUserMV = "XysUserMV";
            public const string XysUserEV = "XysUserEV";
            public const string XysHome = "XysHome";
            public const string XysSettings = "XysSettings";
            public const string XysPage = "XysPage";
            public const string XysPageEV = "XysPageEV";
            public const string XysPageMV = "XysPageMV";
            public const string XysLevel = "XysLevel";
            public const string XysLevelEV = "XysLevelEV";
            public const string XysLevelMV = "XysLevelMV";
            public const string XysFile = "XysFile";
            public const string XysFileMV = "XysFileMV";
            public const string XysMenu = "XysMenu";
            public const string XysMenuMV = "XysMenuMV";
            public const string XysMenuEV = "XysMenuEV";
            public const string XysProfile = "XysProfile";
            public const string XysProPass = "XysProPass";
            public const string XysCloseAcct = "XysCloseAcct";
            public const string XysReport = "XysReport";
            public const string XysBulletin = "XysBulletin";
            public const string XysBulletinEV = "XysBulletinEV";
            public const string XysBulletinMV = "XysBulletinMV";
            public const string XysDict = "XysDict";
            public const string XysDictMV = "XysDictMV";
            public const string XysDictEV = "XysDictEV";
            public const string XysLang = "XysLang";
            public const string XysLangMV = "XysLangMV";
            public const string XysLangEV = "XysLangEV";
            public const string XysOption = "XysOption";
            public const string XysOptionMV = "XysOptionMV";
            public const string XysOptionEV = "XysOptionEV";
            public const string XysData = "XysData";
            public const string XysDataMV = "XysDataMV";
            public const string XysDataEV = "XysDataEV";
            public const string XysDataNV = "XysDataNV";
            public const string XysDataCode = "XysDataCode";
            public const string XysDataPrvw = "XysDataPrvw";
            public const string XysForm = "XysForm";
            public const string XysFormMV = "XysFormMV";
            public const string XysFormEV = "XysFormEV";
            public const string XysFormNV = "XysFormNV";
            public const string XysFormCode = "XysFormCode";
            public const string XysFormPrvw = "XysFormPrvw";

            public const string Support = "Support";
            public const string SupportMV = "SupportMV";
        }
    }

    public class AppKey
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public string UserPic { get; set; } = string.Empty;
        public string UserRef { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
    }

    public class WebBase : WebPage
    {
        public AppKey AppKey = null;
        public ViewPart ViewPart = new ViewPart();

        public CultureInfo LocalCulture;
        public string LocalDateTimeFormat = string.Empty;
        public string LocalDateFormat = string.Empty;
        public string StdDateFormat = "yyyy-MM-dd";

        public WebBase() : base()
        {
            try
            {
                LocalCulture = new CultureInfo(ClientLanguage);
            }
            catch
            {
                LocalCulture = CultureInfo.InvariantCulture;
            }

            LocalDateFormat = LocalCulture.DateTimeFormat.ShortDatePattern;
            LocalDateTimeFormat = LocalCulture.DateTimeFormat.ShortDatePattern + " " + ClientCulture.DateTimeFormat.ShortTimePattern;

            try
            {
                string AppKeyVlu = CookieValue(References.Keys.AppKey, true);
                AppKey = (AppKey)DeserializeObjectEnc(AppKeyVlu, typeof(AppKey));
            }
            catch
            {
                AppKey = null;
            }

            HtmlTranslator.Add(GetPageDict(this.GetType().Name));
        }

        public override void OnResponse(ref ApiResponse _ApiResponse)
        {
            base.OnResponse(ref _ApiResponse);
        }

        public override ApiResponse OnRequest(string Method = "")
        {
            string SerializedViewPart = ParamValue(References.Keys.PageKey.Replace("{0}", this.GetType().Name));
            if (SerializedViewPart != string.Empty)
            {
                ViewPart = (ViewPart)DeserializeObjectEnc(SerializedViewPart, typeof(ViewPart));
            }

            if (ViewPart != null)
            {
                for (int i = 0; i < ViewPart.Fields.Count; i++)
                {
                    if (ViewPart.Fields[i].flag == false)
                    {
                        ViewPart.Fields[i].value = ParamValue(ViewPart.Fields[i].name);
                    }
                }
            }

            ApiResponse _ApiResponse = null;
            if (IsMethodName(Method) == false)
            {
                _ApiResponse = new ApiResponse();
                DialogBox dialogBox = new DialogBox(Translator.Format("accessdenied"));
                dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
            }
            return _ApiResponse;
        }

        protected internal bool IsMethodName(string MethodName)
        {
            bool rtnvlu = false;
            List<ViewMethod> Methods = ViewMethods("");

            if (Methods == null) return true;
            if (Methods.Find(x => x.Method.EndsWith(MethodName, StringComparison.OrdinalIgnoreCase)) == null) return true;

            for (int i = 0; i < Methods.Count; i++)
            {
                string compMethod = !Methods[i].Method.Contains("/") ? MethodName : this.GetType().Name + "/" + MethodName;
                if (Methods[i].Method.Trim().Equals(compMethod.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    rtnvlu = true;
                }
            }
            return rtnvlu;
        }

        public override void OnInitialized()
        {
            InitialViewData();

            string SerializedViewPart = SerializeObjectEnc(ViewPart, typeof(ViewPart));
            HtmlDoc.InitialScripts.StoreLocalValue(References.Keys.PageKey.Replace("{0}", this.GetType().Name), SerializedViewPart);

            if (AppKey != null)
            {
                HtmlDoc.InitialScripts.SetCookie(References.Keys.AppKey, SerializeObjectEnc(AppKey, typeof(AppKey)));
            }

            HtmlDoc.AddJsFile("WebScript.js");
            HtmlDoc.AddCSSFile("WebStyle.css");
            HtmlDoc.SetTitle(Translator.Format("title"));

            bool _ViewAccess = ViewAccess();
            HtmlDoc.HtmlBodyAddOn = _ViewAccess ? InitialViewHtml() : PartialPage(References.Pages.XysUnAuthorized);
        }

        public virtual void InitialViewData() { }

        public virtual string InitialViewHtml()
        {
            return string.Empty;
        }

        public override void OnAfterRender()
        {
            base.OnAfterRender();
        }

        protected internal List<Translator.DictionaryEntry> GetPageDict(string pagename)
        {
            List<Translator.DictionaryEntry> rlt = new List<Translator.DictionaryEntry>();
            string SSQL = $" declare @pageid nvarchar(50),@isocode nvarchar(10) " +
                          $" set @pageid = N'{pagename}' " +
                          $" set @isocode = N'{ClientLanguage}' ";

            if (ClientLanguage.Contains("-"))
            {
                SSQL += " if exists(select * from XysDict where Isocode = @isocode) " +
                        " begin " +
                        "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                        "  Where (Target = @pageid or Target = '*') and (Isocode = '*' or Isocode = @isocode ) " +
                        "  order by KeyWord " +
                        " end " +
                        " else " +
                        " begin " +
                        "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                        "  Where (Target = @pageid or Target = '*') and (Isocode = '*' or Isocode = 'en-US' ) " +
                        "  order by KeyWord " +
                        " end ";
            }
            else
            {
                SSQL += " if exists(select * from XysDict where left(Isocode,2) = @isocode) " +
                        " begin " +
                        "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                        "  Where (Target = @pageid or Target = '*') and (Isocode = '*' or Isocode = @isocode ) " +
                        "  order by KeyWord " +
                        " end " +
                        " else " +
                        " begin " +
                        "  Select Target,IsoCode,KeyWord,Translated from XysDict " +
                        "  Where (Target = @pageid or Target = '*') and (Isocode = '*' or Isocode = 'en-US' ) " +
                        "  order by KeyWord " +
                        " end ";
            }

            string emsg = string.Empty;
            DataTable dt = SQLData.SQLDataTable(SSQL, ref emsg);
            if (string.IsNullOrEmpty(emsg) && dt != null && dt.Rows.Count != 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Translator.DictionaryEntry _Dict = new Translator.DictionaryEntry();
                    _Dict.IsoCode = dt.Rows[i][1].ToString();
                    _Dict.DicKey = dt.Rows[i][2].ToString();
                    _Dict.DicWord = dt.Rows[i][3].ToString();
                    rlt.Add(_Dict);
                }
            }
            return rlt;
        }

        public string PutData(List<string> SQL)
        {
            string emsg = string.Empty;
            SQLData.SQLDataPut(SQL, ref emsg);
            WriteXysLog(string.Join("|", SQL), ref emsg);
            return emsg;
        }

        public void WriteXysLog(string logTxt, ref string msg)
        {
            string userid = (AppKey != null) ? AppKey.UserId : string.Empty;

            List<string> SQL = new List<string> {
            "insert into XysLog(LogId,ClientIp,UserId,LogTxt,JobRlt,SysDte) " +
            "values(@LogId,@ClientIp,@UserId,@LogTxt,@JobRlt,GETDATE())"
        };

            List<SqlParameter> SqlParams = new List<SqlParameter>
        {
            new SqlParameter { ParameterName = "@LogId", Value = NewID(), SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@ClientIp", Value = ClientIPAddress, SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@UserId", Value = userid, SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@LogTxt", Value = EscQuote(logTxt.Trim()), SqlDbType = SqlDbType.NVarChar },
            new SqlParameter { ParameterName = "@JobRlt", Value = EscQuote(msg.Trim()), SqlDbType = SqlDbType.NVarChar }
        };

            string emsg = string.Empty;
            SQLData _SqlData = new SQLData();
            _SqlData.DataPut(SqlWithParams(SQL, SqlParams), ref emsg);
        }

        public string SendEmail(string Subject, string bodyHtml, string[] ToAddr)
        {
            Mail mail = new Mail();
            mail.Subject = Subject;
            mail.ToAddr = ToAddr;
            mail.Body = bodyHtml;
            return mail.SendMail();
        }

        protected internal TitleSection2 PageTitle(bool showTimer = true)
        {
            SkyNet.ToolKit.Timer tmr = new(){ InnerText = "00:00" };
            tmr.SetAttribute(HtmlAttributes.id, "tmr");
            tmr.SetStyle(HtmlStyles.top, "38px");
            tmr.SetStyle(HtmlStyles.right, "90px");

            //string imgfile = PhysicalFolder + "photos\\" + AppKey.UserId + ".jpg";
            //if (!File.Exists(imgfile))
            //{
            //    imgfile = ImagePath + "img_fakeuser.jpg";
            //}
            //else
            //{
            //    //imgfile = GetPhotoData(imgfile);
            //    }
            //}

            string imgfile = VirtualPath + "photos//" + AppKey.UserId + ".jpg";
            if (!File.Exists(imgfile))
            {
                imgfile = ImagePath + "img_fakeuser.jpg";
            }

            TitleSection2 tSector = new TitleSection2();
            tSector.Title.Caption.InnerText = WebAppName;
            tSector.Title.Page.InnerText = Translator.Format("title");
            if (showTimer) tSector.ContentExtra.InnerText = tmr.HtmlText;
            tSector.Title.LogoImage.SetAttribute(HtmlEvents.onclick, $"$navi('{References.Pages.XysHome}')");
            tSector.UserIcon.Icon.SetAttribute(HtmlAttributes.src, imgfile);
            tSector.UserIcon.Menu.AddItem(Translator.Format("profile"), string.Empty, "onclick:Profile();");
            tSector.UserIcon.Menu.AddItem(Translator.Format("signout"), string.Empty, "onclick:SignOut();");
            tSector.FooterSection.AddMenu(Translator.Format("support"), string.Empty, "onclick:" + ByPassCall("Navigate", "m=support"));
            return tSector;
        }

        private bool ViewAccess()
        {
            if (AppKey == null) return false;
            string SSQL = $" declare @userid nvarchar(50),@pagename nvarchar(100) " +
                          $" set @userid = N'{AppKey.UserId}' " +
                          $" set @pagename = N'{this.GetType().Name}' " +
                          $" select count(*) from XysPage where PageUse=1 and dbo.XF_UserPage(@userid,PageId) = 1 and PageName = @pagename ";

            string tcnt = SQLData.SQLFieldValue(SSQL);
            return Convert.ToDouble(tcnt) != 0;
        }

        protected internal List<ViewMethod> ViewMethods(string Area, string PageType = "")
        {
            PageType = string.IsNullOrEmpty(PageType) ? this.GetType().Name : PageType;
            if (AppKey == null) return null;
            string SSQL = $" declare @userid nvarchar(50),@pagename nvarchar(100),@area nvarchar(2) " +
                          $" set @userid = N'{AppKey.UserId}' " +
                          $" set @pagename = N'{PageType}' " +
                          $" set @area = N'{Area}' " +
                          $" if @area = N'' " +
                          " begin " +
                          "  select MenuArea Area,MenuTag Tag,MenuMethod Method,MenuParams Params,MenuCtl Control,MenuType ControlType,MenuObjInner ObjInner,MenuObjStyle ObjStyle,MenuObjAttr ObjAttr " +
                          "  from XysMenu where MenuUse=1 and PageId = dbo.XF_PageIdByName(@pagename) and dbo.XF_UserMenu(@userid,MenuId) = 1 order by MenuOrder " +
                          " end " +
                          " else " +
                          " begin " +
                          "  select MenuArea Area,MenuTag Tag,MenuMethod Method,MenuParams Params,MenuCtl Control,MenuType ControlType,MenuObjInner ObjInner,MenuObjStyle ObjStyle,MenuObjAttr ObjAttr  " +
                          "  from XysMenu where MenuUse=1 and PageId = dbo.XF_PageIdByName(@pagename) and MenuArea = @area and dbo.XF_UserMenu(@userid,MenuId) = 1 order by MenuOrder " +
                          " end ";

            return DataTableListT<ViewMethod>(SQLData.SQLDataTable(SSQL));
        }

        protected ViewMethod GetViewMethod(string MethodTag)
        {
            ViewMethod rtnvlu = null;
            List<ViewMethod> Methods = ViewMethods("X");
            if (Methods != null && Methods.Count > 0)
            {
                foreach (var method in Methods)
                {
                    if (method.Tag.Equals(MethodTag, StringComparison.OrdinalIgnoreCase))
                    {
                        rtnvlu = method;
                    }
                }
            }
            return rtnvlu;
        }

        protected MenuList GetViewMenuItems(string[] selectedItemTags = null)
        {
            MenuList menulist = new MenuList();
            menulist.Wrap.SetStyle(HtmlStyles.@float, "right");

            List<ViewMethod> Methods = ViewMethods("M");
            if (Methods != null)
            {
                for (int i = 0; i < Methods.Count; i++)
                {
                    bool isFiltered = selectedItemTags != null && selectedItemTags.Length > 0;
                    bool matchFound = false;

                    if (isFiltered)
                    {
                        string tags = string.Join(",", selectedItemTags);
                        if (tags.IndexOf(Methods[i].Tag, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            matchFound = true;
                        }
                    }

                    if (!isFiltered || matchFound)
                    {
                        //string mnuctl = RollbackQuote(Methods[i].Control).Replace("{params}", string.Empty);
                        //string mnutyp = Methods[i].ControlType;
                        //HtmlTag mnuobj = (HtmlTag)DeserializeObject(mnuctl, AssemblyType(mnutyp));

                        HtmlTag mnuobj = new();
                        mnuobj.SetStyles((string)DeserializeObject(Methods[i].ObjStyle, typeof(string)));
                        mnuobj.SetAttributes((string)DeserializeObject(Methods[i].ObjAttr, typeof(string)));
                        mnuobj.InnerText = (string)DeserializeObject(Methods[i].ObjInner, typeof(string));

                        if (mnuobj != null && mnuobj.Attributes != null)
                        {
                            for (int j = 0; j < mnuobj.Attributes.Count; j++)
                            {
                                string val = mnuobj.Attributes[j].value;
                                if (!string.IsNullOrEmpty(val))
                                {
                                    mnuobj.Attributes[j].value = val.Replace("%partialdata%", PartialData)
                                                                    .Replace("%requestdata%", RequestData);
                                }
                            }
                        }
                        menulist.Add(mnuobj);
                    }
                }
            }
            return menulist;
        }
        protected Wrap GetViewButtons(string[] selectedButtonTags = null, string WrapStyles = "")
        {
            Wrap BtnWrap = new Wrap();
            if (!string.IsNullOrEmpty(WrapStyles))
            {
                BtnWrap.SetStyles(WrapStyles);
            }
            else
            {
                BtnWrap.SetStyles("display:flex; justify-Content:flex-start;");
            }

            string BtnHtml = string.Empty;
            List<ViewMethod> Methods = ViewMethods("B");

            if (Methods != null)
            {
                for (int i = 0; i < Methods.Count; i++)
                {
                    bool isFiltered = selectedButtonTags != null && selectedButtonTags.Length > 0;
                    bool matchFound = false;

                    if (isFiltered)
                    {
                        string tags = string.Join(",", selectedButtonTags);
                        if (tags.IndexOf(Methods[i].Tag, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            matchFound = true;
                        }
                    }

                    if (!isFiltered || matchFound)
                    {
                        //string mnuctl = RollbackQuote(Methods[i].Control).Replace("{params}", string.Empty);
                        //string mnutyp = Methods[i].ControlType;
                        //Button mnuobj = (Button)DeserializeObject(mnuctl, AssemblyType(mnutyp));

                        Button mnuobj = new();
                        mnuobj.SetStyles((string)DeserializeObject(Methods[i].ObjStyle, typeof(string)));
                        mnuobj.SetAttributes((string)DeserializeObject(Methods[i].ObjAttr, typeof(string)));
                        mnuobj.InnerText = (string)DeserializeObject(Methods[i].ObjInner, typeof(string));

                        if (mnuobj != null && mnuobj.Attributes != null)
                        {
                            for (int j = 0; j < mnuobj.Attributes.Count; j++)
                            {
                                string val = mnuobj.Attributes[j].value;
                                if (!string.IsNullOrEmpty(val))
                                {
                                    mnuobj.Attributes[j].value = val.Replace("%partialdata%", PartialData)
                                                                    .Replace("%requestdata%", RequestData);
                                }
                            }
                            BtnHtml += mnuobj.HtmlText;
                        }
                    }
                }
            }

            BtnWrap.InnerText = BtnHtml;
            return BtnWrap;
        }

        #region Attached File Handler

        protected string UploadFile(string fileKey, string refId)
        {
            string rlt = string.Empty;
            if (HttpCurrent.Context.Request.Form.Files.Count == 0) return rlt;
            if (ViewPart.Data != null) DeleteFiles(refId);

            List<string> SQL = new List<string>();
            IFormFileCollection hfc = HttpCurrent.Context.Request.Form.Files;
            SQL.Add($" delete from XysFile where FileRefId = N'{refId}' ");

            var fileKeys = HttpCurrent.Context.Request.Form.Files.Select(f => f.Name).ToArray();
            for (int i = 0; i < hfc.Count; i++)
            {
                if (hfc[i].Name == fileKey)
                {
                    string flId = NewID(1);
                    string flRef = this.GetType().Name;
                    string flname = Path.GetFileName(hfc[i].FileName);
                    string flname2 = NewID(0, false) + (string.IsNullOrEmpty(Path.GetExtension(hfc[i].FileName)) ? "" : Path.GetExtension(hfc[i].FileName));
                    string flFolder = DataFolder + flname2;
                    string flPath = DataPath + flname2;

                    using (var stream = new FileStream(flFolder, FileMode.Create))
                    {
                        hfc[i].CopyToAsync(stream);
                    }

                    string ssql = $" Insert XysFile(FileId,FileRef,FileRefId,FileType,FileName,FileLink,FilePath,SYSDTE,SYSUSR) " +
                                  $" values(N'{flId}',N'{flRef}',N'{refId}',N'{hfc[i].ContentType}',N'{flname}',N'{flPath}',N'{flFolder}',Getdate(),N'{AppKey.UserId}')";
                    SQL.Add(ssql);
                }
            }
            return PutData(SQL);
        }

        private void DeleteFiles(string refId)
        {
            string ssql = $" select FilePath from XysFile where FileRefId = N'{refId}' ";
            DataTable dt = SQLData.SQLDataTable(ssql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string filepath = row[0].ToString();
                    if (File.Exists(filepath))
                    {
                        try { File.Delete(filepath); } catch { }
                    }
                }
            }
        }

        protected ApiResponse DeleteFile()
        {
            string fileid = GetDataValue("t");
            ApiResponse _ApiResponse = new ApiResponse();
            if (!string.IsNullOrEmpty(fileid))
            {
                DialogBox dialogBox = new DialogBox(Translator.Format("wannadeletefile"));
                dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
                dialogBox.AddButton(Translator.Format("yes"), string.Empty, "class:button1;onclick:" + ByPassCall("RemoveFileProcess", "t=" + fileid));
                dialogBox.AddButton(Translator.Format("no"), string.Empty, "onclick:$PopOff();class:button;");
                _ApiResponse.PopUpWindow(dialogBox.HtmlText, References.Elements.PageContents);
            }
            return _ApiResponse;
        }

        protected ApiResponse DeleteFileProcess()
        {
            string fileid = GetDataValue("t");
            ApiResponse _ApiResponse = new ApiResponse();
            string rlt = PutDeleteFile(fileid);
            if (string.IsNullOrEmpty(rlt))
            {
                DialogBox dialogBox = new DialogBox(Translator.Format("successdeletefile"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                dialogBox.AddButton(Translator.Format("ok"), string.Empty, "class:button1;onclick:" + ByPassCall("Refresh"));
                _ApiResponse.PopUpWindow(dialogBox.HtmlText);
            }
            else
            {
                _ApiResponse.PopUpWindow(DialogMsg(rlt));
            }
            return _ApiResponse;
        }

        private string PutDeleteFile(string FileId)
        {
            string filepath = SQLData.SQLFieldValue($" select FilePath from XysFile Where FileId = N'{FileId}' ");
            if (!string.IsNullOrEmpty(filepath) && File.Exists(filepath))
            {
                List<string> SQL = new List<string> { $"delete from XysFile where FileId = N'{FileId}'" };
                File.Delete(filepath);
                return PutData(SQL);
            }
            else
            {
                if (!filepath.ToLower().Contains("sqlerror") && !File.Exists(filepath))
                {
                    List<string> SQL = new List<string> { $"delete from XysFile where FileId = N'{FileId}'" };
                    PutData(SQL);
                }
                return Translator.Format("filenotfound");
            }
        }

        protected ApiResponse FileDownLoadNorm()
        {
            string encFileId = GetDataValue("FileId");
            ApiResponse _ApiResponse = new ApiResponse();
            DataTable dt = SQLData.SQLDataTable($"select FileName,FilePath from XysFile where FileId = N'{encFileId}' ");
            if (dt != null && dt.Rows.Count > 0)
            {
                string filename = dt.Rows[0][0].ToString();
                string filepath = dt.Rows[0][1].ToString();
                _ApiResponse.DownloadFile(DownLoadFileLink(filename, filepath));
            }
            else
            {
                DialogBox dialogBox = new DialogBox(Translator.Format("filenotfound"));
                dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                _ApiResponse.PopUpWindow(dialogBox.HtmlText);
            }
            return _ApiResponse;
        }

        protected ApiResponse FileDownLoadEnc()
        {
            string encFileId = GetDataValue("FileId");
            string decFileId = Encryptor.DecryptData(encFileId);
            ApiResponse _ApiResponse = new ApiResponse();

            if (!string.IsNullOrEmpty(decFileId) && decFileId.Split('|').Length >= 2)
            {
                string[] parts = decFileId.Split('|');
                if ((DateTime.Now - DateTime.Parse(parts[0])).TotalMinutes > 30)
                {
                    DialogBox dialogBox = new DialogBox(Translator.Format("filenotfound"));
                    dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
                    _ApiResponse.PopUpWindow(dialogBox.HtmlText);
                }
                else
                {
                    DataTable dt = SQLData.SQLDataTable($"select FileName,FilePath from XysFile where FileId = N'{parts[1]}' ");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        _ApiResponse.DownloadFile(DownLoadFileLink(dt.Rows[0][0].ToString(), dt.Rows[0][1].ToString()));
                    }
                    else
                    {
                        _ApiResponse.PopUpWindow(DialogMsg(Translator.Format("filenotfound")));
                    }
                }
            }
            return _ApiResponse;
        }

        public ApiResponse UpdatePhoto()
        {
            string imgid = GetDataValue("f");
            string inputid = GetDataValue("s");
            string userid = DecryptString(GetDataValue("p"));
            ApiResponse _ApiResponse = new ApiResponse();
            IFormFileCollection hfc = HttpCurrent.Context.Request.Form.Files;

            if (hfc.Count == 0 || Path.GetExtension(hfc[0].FileName).ToLower() != ".jpg")
            {
                _ApiResponse.SetElementValue(inputid, string.Empty);
                _ApiResponse.PopUpWindow(DialogMsg(Translator.Format("nouploadfilenojpg")));
            }
            else
            {
                string FileName = PhysicalFolder + "photos\\" + userid + ".jpg";
                try
                {
                    using (var stream = new FileStream(FileName, FileMode.Create))
                    {
                        hfc[0].CopyToAsync(stream);
                    }

                    _ApiResponse.SetElementAttribute(imgid, HtmlAttributes.src, GetPhotoData(FileName));
                }
                catch (Exception ex)
                {
                    _ApiResponse.SetElementValue(inputid, string.Empty);
                    _ApiResponse.PopUpWindow(DialogMsg(ex.Message));
                }
            }
            return _ApiResponse;
        }

        public string GetPhotoData(string FileName)
        {
            try
            {
                Stream stream = File.OpenRead(FileName);
                using (Bitmap bitmap = new(stream))
                {
                    return ImageHandler.ImageBase64(bitmap, 300, 380);
                }
            }
            catch (Exception)
            {
                Stream stream = File.OpenRead(ImageFolder + "noimage.jpg");
                using (Bitmap bitmap = new(stream))
                {
                    return ImageHandler.ImageBase64(bitmap, 300, 380);
                }

                // WebUrl
                //using (HttpClient client = new HttpClient())
                //using (Stream stream = client.GetStreamAsync(WebUrl).GetAwaiter().GetResult())
                //using (Bitmap bitmap = new(stream))
                //{
                //    return ImageHandler.ImageBase64(bitmap, 300, 380);
                //}
            }
        }

        #endregion

        #region Simple Dialogs

        public string DialogMsgRequred(string msg_required = "msg_required")
        {
            DialogBox dialogBox = new DialogBox(Translator.Format(msg_required));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            return dialogBox.HtmlText;
        }

        public string DialogMsgSaved(string paramsStr, string Funcs = "PartialView", string msg_saved = "msg_saved")
        {
            DialogBox dialogBox = new DialogBox(Translator.Format(msg_saved));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            string onclick = string.IsNullOrEmpty(paramsStr) ? "$PopOff();" : ByPassCall(Funcs, paramsStr) + "&$PopOff();";
            dialogBox.AddButton(Translator.Format("ok"), string.Empty, $"class:button;onclick:{onclick}");
            return dialogBox.HtmlText;
        }

        public string DialogQstDelete(string Funcs, string paramsStr = "", string msg_delete = "msg_delete")
        {
            DialogBox dialogBox = new DialogBox(Translator.Format(msg_delete));
            dialogBox.ContentsWrap.SetStyles("padding:30px;width:300px;height:60px;text-align:center;");
            dialogBox.AddButton(Translator.Format("yes"), string.Empty, "class:button1;onclick:" + ByPassCall(Funcs, paramsStr));
            dialogBox.AddButton(Translator.Format("no"), string.Empty, "onclick:$PopOff();class:button;");
            return dialogBox.HtmlText;
        }

        public string DialogMsgDeleted(string paramsStr, string Funcs = "PartialView", string msg_deleted = "msg_deleted")
        {
            DialogBox dialogBox = new DialogBox(Translator.Format(msg_deleted));
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            string onclick = string.IsNullOrEmpty(Funcs) ? "$PopOff();" : ByPassCall(Funcs, paramsStr) + "&$PopOff();";
            dialogBox.AddButton(Translator.Format("ok"), string.Empty, $"class:button;onclick:{onclick}");
            return dialogBox.HtmlText;
        }

        public string DialogMsg(string msg)
        {
            DialogBox dialogBox = new DialogBox(msg);
            dialogBox.ContentsWrap.SetStyles("width:300px;height:100px;text-align:center;");
            return dialogBox.HtmlText;
        }

        #endregion

        #region Dynamic Form/Data handler

        public UIForm UIFormFromXForm(XForm XForm, UIModes UIMode, List<NameValue> FormValues = null, string AttachedFiles = "")
        {
            UIForm _UIForm = new UIForm { UIMode = UIMode, AttachedFiles = AttachedFiles };
            List<string> FormFields = TypeFieldNames(typeof(XForm));

            foreach (var field in FormFields)
            {
                if (!field.Equals("sections", StringComparison.OrdinalIgnoreCase))
                {
                    TypePropertySetValue(_UIForm, field, TypeFieldGetValue(typeof(XForm), field, XForm));
                }
            }

            _UIForm.Margin = new Location(XForm.MarginTop, XForm.MarginLeft);

            foreach (var section in XForm.Sections)
            {
                UIForm.UISection _UISection = new UIForm.UISection();
                _UIForm.UISections.Add(_UISection);

                foreach (var sField in TypeFieldNames(typeof(XFormSection)))
                {
                    if (!sField.Equals("elements", StringComparison.OrdinalIgnoreCase))
                    {
                        TypePropertySetValue(_UISection, sField, TypeFieldGetValue(typeof(XFormSection), sField, section));
                    }
                }

                foreach (var element in section.Elements)
                {
                    string elmId = section.Name + "$" + element.Name;
                    UIForm.Element _UIElement = new UIForm.Element();
                    _UISection.Elements.Add(_UIElement);

                    if (FormValues != null)
                    {
                        var formValue = FormValues.Find(x => x.name == elmId);
                        if (formValue != null) element.Value = formValue.value;
                    }

                    foreach (var eField in TypeFieldNames(typeof(XFormElement)))
                    {
                        TypePropertySetValue(_UIElement, eField, TypeFieldGetValue(typeof(XFormElement), eField, element));
                    }
                }
            }
            return _UIForm;
        }

        public UIControl UIControlFromXDataElements(string UIGroupName, List<XDataElement> DataElements, UIModes UIMode, List<NameValueFlag> _UIData = null)
        {
            UIControl _UIControl = new UIControl { UIMode = UIMode, UIGroup = UIGroupName };
            if (DataElements != null)
            {
                var XElements = DataElements.OrderBy(x => x.Seq).ToList();
                var Fields = TypePropertyNames(typeof(UIControl.Item));

                foreach (var xElm in XElements)
                {
                    UIControl.Item _UIItem = new UIControl.Item();
                    foreach (var field in Fields)
                    {
                        string elmVlu = string.Empty;
                        if (field.Equals("value", StringComparison.OrdinalIgnoreCase) && _UIData != null)
                        {
                            var match = _UIData.Find(m => m.name == xElm.Name);
                            if (match != null) elmVlu = match.value;
                        }
                        else
                        {
                            elmVlu = TypeFieldGetValue(typeof(XDataElement), field, xElm).ToString();
                        }
                        TypePropertySetValue(_UIItem, field, elmVlu);
                    }
                    if (_UIControl.UIMode == UIModes.Edit && xElm.IsKey == 1) _UIItem.IsReadOnly = true;
                    _UIControl.Items.Add(_UIItem);
                }
            }
            return _UIControl;
        }

        public List<XDataElement> XDataElementsFromUIControl(UIControl _UIControl)
        {
            List<XDataElement> XDataElements = new List<XDataElement>();
            var Fields = TypePropertyNames(typeof(UIControl.Item));

            foreach (var item in _UIControl.Items)
            {
                XDataElement _DataItem = new XDataElement { ElementId = NewID(), Seq = XDataElements.Count + 1 };
                foreach (var field in Fields)
                {
                    TypeFieldSetValue(typeof(XDataElement), field, _DataItem, TypePropertyGetValue(item, field));
                }
                XDataElements.Add(_DataItem);
            }
            return XDataElements;
        }

        public List<XDataElement> XDataElementsFromDataName(string dataname)
        {
            string DataModel = SQLData.SQLFieldValue($"select DataModel from XysData where DataName = N'{dataname}'");
            var DataElements = (List<XDataElement>)DeserializeObject(DataModel, typeof(List<XDataElement>));
            return DataElements.OrderBy(x => x.Seq).ToList();
        }

        public UIGrid UIGridFromXDataElements(List<XDataElement> XDataElements, List<string> RemovedElements = null, bool TranslatorFormat = true)
        {
            List<UIGrid.Item> cols = new List<UIGrid.Item>();
            if (XDataElements != null)
            {
                foreach (var xElm in XDataElements)
                {
                    if (RemovedElements == null || !RemovedElements.Contains(xElm.Name))
                    {
                        if (xElm.UIType != (int)UITypes.DataList && xElm.UIType != (int)UITypes.Paragraph)
                        {
                            cols.Add(new UIGrid.Item
                            {
                                Name = xElm.Name,
                                Label = TranslatorFormat ? Translator.Format(xElm.Label) : xElm.Label,
                                Value = string.Empty,
                                IsKey = xElm.IsKey == 1
                            });
                        }
                    }
                }
            }
            return new UIGrid { Items = cols };
        }

        public UIGrid UIGridFromUIControl(UIControl _UIControl, List<string> RemovedItems = null, bool TranslatorFormat = true)
        {
            List<UIGrid.Item> cols = new List<UIGrid.Item>();
            if (_UIControl != null)
            {
                foreach (var item in _UIControl.Items)
                {
                    if (RemovedItems == null || !RemovedItems.Contains(item.Name))
                    {
                        if (item.UIType != UITypes.DataList && item.UIType != UITypes.Paragraph)
                        {
                            cols.Add(new UIGrid.Item
                            {
                                Name = item.Name,
                                Label = TranslatorFormat ? Translator.Format(item.Label) : item.Label,
                                Value = string.Empty,
                                IsKey = item.IsKey
                            });
                        }
                    }
                }
            }
            return new UIGrid { Items = cols };
        }
        #endregion
    }

    public class ViewPart
    {
        public DataTable Data { get; set; } = null;
        public List<NameValueFlag> Fields { get; set; } = new List<NameValueFlag>();
        public UIControl UIControl { get; set; } = null;

        public string ColunmValue(string ColumnName)
        {
            if (Data != null && Data.Columns.Contains(ColumnName) && Data.Rows.Count > 0)
            {
                return Data.Rows[0][ColumnName].ToString();
            }
            return string.Empty;
        }

        public void BindData(string sql)
        {
            if (string.IsNullOrEmpty(sql)) return;
            BindData(SQLData.SQLDataTable(sql));
        }

        public void BindData(DataTable dt)
        {
            Data = (dt != null && dt.Rows.Count > 0) ? dt : null;
            if (UIControl != null && UIControl.Items.Count > 0)
            {
                foreach (var item in UIControl.Items)
                {
                    item.Value = ColunmValue(item.Name);
                    Fields.Add(new NameValueFlag { name = item.Name, value = item.Value, flag = item.IsFixedValue });
                }
            }
            else
            {
                foreach (var field in Fields)
                {
                    field.value = ColunmValue(field.name);
                }
            }
        }

        public UIControl.Item Control(string Name)
        {
            return UIControl?.Items.Find(x => x.Name == Name);
        }

        public NameValueFlag Field(string Name)
        {
            return Fields.Find(x => x.name == Name);
        }
    }

    public class ViewMethod
    {
        public string Area { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public string Params { get; set; } = string.Empty;
        public string Control { get; set; } = string.Empty;
        public string ControlType { get; set; } = string.Empty;
        public string ObjInner { get; set; } = string.Empty;
        public string ObjStyle { get; set; } = string.Empty;
        public string ObjAttr { get; set; } = string.Empty;
        public int Sort { get; set; } = 0;
    }

 
}
