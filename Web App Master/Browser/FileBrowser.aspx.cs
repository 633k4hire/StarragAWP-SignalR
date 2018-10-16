using Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using Web_App_Master.Models;

namespace Web_App_Master.Browser
{
    public partial class App : System.Web.UI.Page
    {
        public string UID { get { return ECDHAES256s.AES.Sha256(User.Identity.Name); } }
        public string TreeHtml = "";
        public DirectoryTree _Tree;
        string[] _goodFileTypes = new string[] { ".gif", ".pdf", ".png", ".jpg", ".jpeg", ".doc", ".docx", ".xls", ".xlsx", ".psd", ".eps", ".zip", ".ai", ".ppt", ".pptx" };
        public string UserPath { get; set; }
        protected List<KeyValuePair<string, string>> CutList
        {
            get { return Session["cut"] as List<KeyValuePair<string, string>>; }
            set { Session["cut"] = value; }
        }
        protected List<KeyValuePair<string, string>> CopyList
        {
            get { return Session["copy"] as List<KeyValuePair<string, string>>; }
            set { Session["copy"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            DirTree.ViewStateMode = ViewStateMode.Disabled;
            DirectoryTreeUpdatePanel.ViewStateMode = ViewStateMode.Disabled;
            FileBoxUpdatePanel.ViewStateMode = ViewStateMode.Disabled;
            FileRepeater.ViewStateMode = ViewStateMode.Disabled;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Init += new EventHandler(Page_Init);
            if (!User.Identity.IsAuthenticated)
            {
                //Response.Redirect("/Account/Login");
            }
            var userid = ECDHAES256s.AES.Sha256(User.Identity.Name);
            if (Application[userid + "pass"] != null)
            {
                //var script = @"$(document).ready(function (){ HideDiv('SignInModal');});";
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "testScript", script, true);

            }
            else
            {
                if (!IsPostBack)
                {
                    //var script = @"$(document).ready(function (){ ShowDiv('SignInModal');});";
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "testScript2", script, true);
                }

                // var salty = ECDHAES256s.AES.GetSalt();

            }
            DirTree.Attributes.Add("onclick", "return treeClick(event)");

            UserPath = "/Upload/Files/" + userid + "/";
            //_Tree = BuildTree(UserPath);

            if (!IsPostBack)
            {
                Session["CurrentView"] = "icon";
                if (!Directory.Exists(Server.MapPath(UserPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(UserPath));
                }
                Session["Files"] = GetFiles();
                Session["ShowList"] = false;
                SetGlobalSessions();

                DirTree.ExpandDepth = 1;

            }

            BuildTree(UserPath);
            UpdateView("icon", Session["Files"] as BindingFileList);
        }


        protected void SetGlobalSessions()
        {
            Session["cut"] = new List<KeyValuePair<string, string>>();
            Session["copy"] = new List<KeyValuePair<string, string>>();

        }


        public bool isRoot = true;

        private void BuildTree(string relativePath)
        {
            TreeNodeCollection tree = null;
            if (Directory.Exists(Server.MapPath(relativePath)))
            {
                DirTree.Nodes.Clear();
                var rootDirectoryInfo = new DirectoryInfo(Server.MapPath(relativePath));
                var root = CreateDirectoryNode(rootDirectoryInfo);
                foreach (TreeNode node in root.ChildNodes)
                {
                    TreeNode n = new TreeNode(node.Text);
                    DirTree.Nodes.Add(n);
                }

            }


        }

        private TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            TreeNode directoryNode = new TreeNode(directoryInfo.Name);

            foreach (var directory in directoryInfo.GetDirectories())
                directoryNode.ChildNodes.Add(CreateDirectoryNode(directory));

            foreach (var file in directoryInfo.GetFiles())
            {
                var name = file.Name;
                if (file.Name.Contains("_-_"))
                {
                    name = name.Split(new string[] { "_-_" }, StringSplitOptions.None)[1];
                }
                var sub = new TreeNode(name);

                var enc = Convert.ToBase64String(Encoding.UTF8.GetBytes(file.FullName));

                sub.NavigateUrl = "javascript:fileNodeClick('" + enc + "');";
                directoryNode.ChildNodes.Add(sub);
            }
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(directoryInfo.FullName));
            directoryNode.NavigateUrl = "javascript:dirNodeClick('" + encoded + "');";
            return directoryNode;
        }

        private bool CanViewFiles(DirectoryInfo dir)
        {
            return true;
            var config = WebConfigurationManager.OpenWebConfiguration(Server.MapPath(dir.FullName));
            if (config != null && config.HasFile)
            {
                var section = config.GetSection("system.web/authorization") as AuthorizationSection;
                if (section != null)
                {
                    foreach (AuthorizationRule rule in section.Rules)
                    {
                        if (rule.Action == AuthorizationRuleAction.Allow)
                        {
                            foreach (string role in rule.Roles)
                            {
                                if (HttpContext.Current.User.IsInRole(role))
                                {
                                    return true;
                                }
                            }

                            foreach (string user in rule.Users)
                            {
                                if (HttpContext.Current.User.Identity.Name == user)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    return false;
                }
            }
            return true;
        }



        protected void DirTree_SelectedNodeChanged(object sender, EventArgs e)
        {

        }

        protected void DirTree_TreeNodeCheckChanged(object sender, TreeNodeEventArgs e)
        {

        }


        protected void UpdateView(string view, object datasource = null)
        {
            if (datasource == null)
            { datasource = GetFiles().OrderBy(w => w.Title); }
            switch (view)
            {
                case "icon":
                    FileRepeater.ItemTemplate = Page.LoadTemplate("Templates/DefaultView.ascx");
                    FileRepeater.DataSource = datasource;
                    FileRepeater.DataBind();
                    Session["CurrentView"] = "icon";
                    break;
                case "detail":

                    break;
                case "list":
                    FileRepeater.ItemTemplate = Page.LoadTemplate("Templates/DefaultView.ascx");
                    FileRepeater.DataSource = datasource;
                    FileRepeater.DataBind();
                    Session["CurrentView"] = avSelectedView.Text;
                    break;
                default:
                    FileRepeater.ItemTemplate = Page.LoadTemplate("Templates/DefaultView.ascx");
                    FileRepeater.DataSource = datasource;
                    FileRepeater.DataBind();
                    Session["CurrentView"] = avSelectedView.Text;
                    break;
            }
            UpdateAllPanels();
        }

        private BindingFileList GetFiles(string relativePath = null)
        {
            var userid = ECDHAES256s.AES.Sha256(User.Identity.Name);
            if (relativePath == null)
            {
                UserPath = "/Upload/Files/" + userid + "/";
            }
            else
            {
                UserPath = relativePath;
                Upath = UserPath;
                Session["UserPath"] = relativePath;
            }
            BrowserMulti.ActiveViewIndex = 0;

            //get files
            List<FileFolderInfo> filesAndFolders = new List<FileFolderInfo>();
            var directories = Directory.GetDirectories(Server.MapPath(UserPath));
            foreach (var dir in directories)
            {
                var tmp = dir;
                var split = tmp.Split(new string[] { "\\" }, StringSplitOptions.None).ToList();
                var name = split.Last();
                filesAndFolders.Add(new FileFolderInfo(new DirectoryInfo(dir)));
            }
            var files = Directory.GetFiles(Server.MapPath(UserPath));
            foreach (var file in files)
            {
                var a = new System.IO.FileInfo(file);
                filesAndFolders.Add(new FileFolderInfo(a));
            }
            BindingFileList ds = new BindingFileList();
            filesAndFolders.ForEach(i => { ds.Add(i); });
            Session["UserPath"] = UserPath;
            Session["Files"] = ds;
            return ds;
        }

        protected void avChangeView_Click(object sender, EventArgs e)
        {

        }

        protected void DirClick_Click(object sender, EventArgs e)
        {
            var userid = ECDHAES256s.AES.Sha256(User.Identity.Name);
            UserPath = "/Upload/Files/" + userid + "/";
            BrowserMulti.ActiveViewIndex = 0;
            var dirId = DirID.Text;
            try
            {
                dirId = Encoding.UTF8.GetString(Convert.FromBase64String(dirId));

            }
            catch
            {
                dirId = DirID.Text;
            }
            if (dirId == userid || dirId == User.Identity.Name)
            {
                UserPath = "/Upload/Files/" + userid + "/";
            }
            else
            {
                //find actual path
                try
                {
                    var split = dirId.Split(new string[] { "\\Upload" }, StringSplitOptions.None);
                    var tmp = split.Last().Replace("\\", "/");
                    var bb = 0;
                    UserPath = "/Upload" + tmp;
                }
                catch
                {

                }
            }
            //get files
            List<FileFolderInfo> filesAndFolders = new List<FileFolderInfo>();
            var directories = Directory.GetDirectories(Server.MapPath(UserPath));
            foreach (var dir in directories)
            {
                var tmp = dir;
                var split = tmp.Split(new string[] { "\\" }, StringSplitOptions.None).ToList();
                var name = split.Last();
                filesAndFolders.Add(new FileFolderInfo(new DirectoryInfo(dir)));
            }
            var files = Directory.GetFiles(Server.MapPath(UserPath));
            foreach (var file in files)
            {
                var a = new System.IO.FileInfo(file);
                filesAndFolders.Add(new FileFolderInfo(a));
            }
            BindingFileList ds = new BindingFileList();
            filesAndFolders.ForEach(i => { ds.Add(i); });
            UpdateView("icon", ds);
            Session["UserPath"] = UserPath;
        }

        protected void FileClick_Click(object sender, EventArgs e)
        {
            BrowserMulti.ActiveViewIndex = 1;
            var fileId = FileID.Text;
        }
        // ITEM CREATED********
        protected void FileRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            ScriptManager scriptMan = ScriptManager.GetCurrent(this);
            LinkButton btn = e.Item.Controls[0].FindControl("IconLink") as LinkButton;
            if (btn != null)
            {
                btn.Click += FileFolder_Click;
                scriptMan.RegisterAsyncPostBackControl(btn);
            }
            var a = e.Item.Controls[0].FindControl("DownloadLink") as System.Web.UI.HtmlControls.HtmlGenericControl;
            if ((e.Item.DataItem as FileFolderInfo).IconType == IconType.Folder)
            {
                a.Visible = false;

            }
        }

        private void FileFolder_Click(object sender, EventArgs e)
        {
            var button = sender as LinkButton;
            var path = Encoding.UTF8.GetString(Convert.FromBase64String(button.CommandName));
            var task = button.CommandName;
            var split = path.Split(new string[] { "\\Upload" }, StringSplitOptions.None);
            path = split[1].Replace("\\", "/");
            path = "/Upload" + path;
            UserPath = path;
            Session["UserPath"] = UserPath;
            var ext = Path.GetExtension(UserPath);
            if (ext == "")
            {
                //browse folder
                UpdateView("icon", GetFiles(path));
                //BrowserFooter.Text = "<span class='fg-white'>"+Path.GetDirectoryName(path)+"</span>";
                FooterLabel.Text = path;
                FileBoxUpdatePanel.Update();
                FooterUpdatePanel.Update();
            }
            if (ext.Contains("."))
            {
                //browse file
                BrowseFile(Encoding.UTF8.GetString(Convert.FromBase64String(button.CommandName)));
            }


        }
        protected void BrowseFile(string path)
        {

            //*********************


        }

        protected void FileRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

        }

        protected void DeepCheck(TreeNode node, bool check = true)
        {
            node.Checked = check;
            foreach (TreeNode sub in node.ChildNodes)
            {
                DeepCheck(sub);
            }
        }

        protected void SelectAllBtn_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in DirTree.Nodes[0].ChildNodes)
            {
                DeepCheck(node);
            }
            foreach (RepeaterItem item in FileRepeater.Items)
            {
                var cb = item.Controls[0].Controls[3] as CheckBox;
                cb.Checked = true;
            }
            DirTree.ExpandDepth = 1;
        }

        protected string Upath
        {
            get { return Session["UserPath"] as string; }
            set { Session["UserPath"] = value; }
        }

        protected void PasteContent(string target)
        {
            var currentPath = Session["UserPath"] as string;
            foreach (var cut in CutList)
            {
                if (Path.GetExtension(cut.Value) == "")
                {
                    PasteDir(target, cut, true);
                }
                else
                {
                    PasteFile(target, cut, true);
                }
            }
            foreach (var copy in CopyList)
            {
                if (Path.GetExtension(copy.Value) == "")
                {
                    PasteDir(target, copy);
                }
                else
                {
                    PasteFile(target, copy);
                }

            }
            Session["cut"] = new List<KeyValuePair<string, string>>();
            Session["copy"] = new List<KeyValuePair<string, string>>();
        }

        protected void PasteFile(string target, KeyValuePair<string, string> file, bool cut = false)
        {

            try
            {
                var a = Session["UserPath"] as string;
                if (!a.EndsWith("/"))
                {
                    if (a.EndsWith(target))
                    {
                        a = a.TrimEnd(target.ToArray());
                    }
                    a = a + "/";
                }

                var dest = Server.MapPath(a + target + "/" + file.Value);
                var source = Server.MapPath(file.Key + "/" + file.Value);
                if (cut)
                {
                    File.Move(source, dest);
                }
                else
                {
                    File.Copy(source, dest);
                }
            }
            catch (Exception ex)
            {

            }

        }

        protected void PasteDir(string target, KeyValuePair<string, string> file, bool cut = false)
        {
            try
            {
                var a = Session["UserPath"] as string;
                if (!a.EndsWith("/"))
                {
                    if (a.EndsWith(target))
                    {
                        a = a.TrimEnd(target.ToArray());
                    }
                    a = a + "/";
                }
                var dest = Server.MapPath(a + target + "/" + file.Value);
                var b = file.Key;
                if (!b.EndsWith("/")) b = b + "/";
                var source = Server.MapPath(b + file.Value);

                if (cut)
                {
                    Directory.Move(source, dest);
                }
                else
                {
                    DirectoryCopy(source, dest, true);
                }
            }
            catch
            {
            }

        }

        protected void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        protected void RenameFile(string target, string name)
        {
            var a = Session["UserPath"] as string;
            if (!a.EndsWith("/"))
            {
                a = a + "/";
            }
            try
            {
                var path = Server.MapPath(a + target);
                File.Move(path, Server.MapPath(a + name));
            }
            catch
            {

            }

            UpdateAll();
        }

        protected void RenameDir(string target, string name)
        {
            var a = Session["UserPath"] as string;
            if (!a.EndsWith("/"))
            {
                a = a + "/";
            }
            try
            {
                var path = Server.MapPath(a + target);
                Directory.Move(path, Server.MapPath(a + name));
            }
            catch
            {
                Page.SiteMaster().ShowError("");
            }
            UpdateAll();

        }

        //SUPER BUTTON*********************************
        protected void SuperButton_Click(object sender, EventArgs e)
        {
            var u = UserPath = Session["UserPath"] as string;
            var args = SuperButtonArg.Text.Split(',');
            var command = args[0];
            var target = args[1].Replace("\r\n", "").TrimStart(' ').TrimEnd(' ');
            var go = 0;
            switch (command)
            {
                case "cut":
                    if (!CutList.Contains(new KeyValuePair<string, string>(Upath, target)))
                        CutList.Add(new KeyValuePair<string, string>(Upath, target));
                    CutBtn_Click(this, new EventArgs());
                    break;
                case "copy":
                    if (!CopyList.Contains(new KeyValuePair<string, string>(Upath, target)))
                        CopyList.Add(new KeyValuePair<string, string>(Upath, target));
                    CopyBtn_Click(this, new EventArgs());
                    break;
                case "paste":
                    PasteContent(target);
                    break;
                case "inputaccept":
                    InputModal.Style.Clear();
                    InputModal.Style.Add("display", "none");
                    var name = RenameInputBox.Text;
                    //find file and rename it
                    target = Session["Rename"] as string;
                    if (Path.GetExtension(target) == "")
                    {
                        RenameDir(target, name);
                    }
                    else
                    {
                        RenameFile(target, name);
                    }

                    UpdateAllPanels();
                    break;
                case "rename":
                    InputModal.Style.Clear();
                    InputModal.Style.Add("display", "inline-block");
                    Session["Rename"] = target;
                    RenameInputBox.Text = target;
                    RenameInputBox.Focus();
                    UpdateAllPanels();

                    break;
                case "delete":
                    if (Path.GetExtension(target) == "")
                    {
                        DeleteDir(target);
                    }
                    else
                    {
                        DeleteFile(target);
                    }
                    DeleteBtn_Click(this, new EventArgs());
                    break;
                case "New Folder":
                    var newpath = Server.MapPath(UserPath) + "New Folder";
                    Directory.CreateDirectory(newpath);
                    break;
                default:
                    break;

            }
            var aa = Session["UserPath"] as string;
            UpdateFileLists(aa);
            UpdateAllPanels();
        }
        protected void DeleteDir(string target)
        {
            var upath = Session["UserPath"] as string;
            if (!upath.EndsWith("/"))
            {
                upath = upath + "/";
            }
            var path = Server.MapPath(upath + target);
            Directory.Delete(path, true);

        }
        protected string GetUserID()
        {
            return ECDHAES256s.AES.Sha256(User.Identity.Name);
        }
        protected void DeleteFile(string target)
        {
            //var path = Server.MapPath("/Upload/Files/" + GetUserID());
            var userid = ECDHAES256s.AES.Sha256(User.Identity.Name);
            var path = Session["UserPath"] as string;
            foreach (var file in Directory.GetFiles(Server.MapPath(path)))
            {
                var info = new FileInfo(file);
                if (info.Name.Equals(target))
                {
                    File.Delete(file);
                }
            }
        }
        protected void UpdateFileLists(string relativePath = null)
        {
            var userid = ECDHAES256s.AES.Sha256(User.Identity.Name);
            var path = Session["UserPath"] as string;
            if (relativePath != null)
            {
                path = relativePath;
            }
            BuildTree("/Upload/Files/" + userid + "/");
            var files = GetFiles(path);
            Session["Files"] = files;
            UpdateView("icon", files);
            DirTree.ExpandDepth = 1;
            UpdateAllPanels();
        }

        protected void DirectoryTreeUpdatePanel_PreRender(object sender, EventArgs e)
        {
            DirTree.ExpandDepth = 1;
        }

        protected void ApplyActionToSelectedFiles(Action<RepeaterItem, string> action, string actionCommand)
        {
            foreach (RepeaterItem item in FileRepeater.Items)
            {
                var cb = item.Controls[0].Controls[3] as CheckBox;
                if (cb.Checked == true)
                {
                    action.Invoke(item, actionCommand);
                }
            }
        }
        protected void CutBtn_Click(object sender, EventArgs e)
        {
            var upath = Session["UserPath"] as string;
            foreach (RepeaterItem item in FileRepeater.Items)
            {
                try
                {
                    var cb = item.Controls[0].Controls[3] as CheckBox;
                    var a = item.Controls[0].Controls[0] as System.Web.UI.DataBoundLiteralControl;
                    var html = a.Text;
                    var split = html.Split(new string[] { "title=" }, StringSplitOptions.None);
                    var title = split[1].Replace("'", "").Replace(">\r\n", "").TrimStart(' ').TrimEnd(' ');
                    var ext = Path.GetExtension(title);
                    if (cb.Checked)
                    {
                        if (!CutList.Contains(new KeyValuePair<string, string>(upath, title)))
                            CutList.Add(new KeyValuePair<string, string>(upath, title));
                    }
                }
                catch { }

            }
            UpdateAll();
            //create cut list for pasting

        }

        protected void PasteBtn_Click(object sender, EventArgs e)
        {
            var aa = Session["UserPath"] as string;
            var idx = aa.LastIndexOf('/');
            var sub = aa.Substring(idx);
            PasteContent(sub);
            UpdateAll();
        }

        protected void DeleteBtn_Click(object sender, EventArgs e)
        {
            var upath = Session["UserPath"] as string;
            foreach (RepeaterItem item in FileRepeater.Items)
            {
                try
                {
                    var cb = item.Controls[0].Controls[3] as CheckBox;
                    var a = item.Controls[0].Controls[0] as System.Web.UI.DataBoundLiteralControl;
                    var html = a.Text;
                    var split = html.Split(new string[] { "title=" }, StringSplitOptions.None);
                    var title = split[1].Replace("'", "").Replace(">\r\n", "").TrimStart(' ').TrimEnd(' ');
                    var ext = Path.GetExtension(title);
                    if (cb.Checked)
                    {
                        if (ext == "")
                            DeleteDir(title);
                        else
                            DeleteFile(title);
                    }
                }
                catch { }

            }
            UpdateFileLists(Session["UserPath"] as string);
            UpdateAllPanels();
        }

        protected void CopyBtn_Click(object sender, EventArgs e)
        {
            var upath = Session["UserPath"] as string;
            foreach (RepeaterItem item in FileRepeater.Items)
            {
                try
                {
                    var cb = item.Controls[0].Controls[3] as CheckBox;
                    var a = item.Controls[0].Controls[0] as System.Web.UI.DataBoundLiteralControl;
                    var html = a.Text;
                    var split = html.Split(new string[] { "title=" }, StringSplitOptions.None);
                    var title = split[1].Replace("'", "").Replace(">\r\n", "").TrimStart(' ').TrimEnd(' ');
                    var ext = Path.GetExtension(title);
                    if (cb.Checked)
                    {
                        if (!CopyList.Contains(new KeyValuePair<string, string>(upath, title)))
                            CopyList.Add(new KeyValuePair<string, string>(upath, title)); ;
                    }
                }
                catch { }

            }
            UpdateAll();
        }

        protected void NavBack_Click(object sender, EventArgs e)
        {
            string t = "";
            var currentPath = Session["UserPath"] as string;
            t = currentPath;

            var idx = currentPath.LastIndexOf('/');
            var path = currentPath.Substring(0, idx);
            if (!path.Contains(UID))
            {
                path = t;
            }
            FooterLabel.Text = path;
            UpdateView("icon", GetFiles(path));
        }

        protected void NavHome_Click(object sender, EventArgs e)
        {
            FooterLabel.Text = "/Upload/Files/" + UID;
            UpdateView("icon", GetFiles());
        }

        protected void UpdateAll()
        {
            var aa = Session["UserPath"] as string;
            UpdateFileLists(aa);
            UpdateAllPanels();
        }

        protected void UpdateAllPanels()
        {
            FooterUpdatePanel.Update();
            DirectoryTreeUpdatePanel.Update();
            FileBoxUpdatePanel.Update();
        }

        protected void ShowListBtn_Click(object sender, EventArgs e)
        {
            // bool b = (bool)Session["ShowList"];
            // b = !b;
            //  DirectoryTreeUpdatePanel.Visible = b;
            //  if (b)
            //  {
            //       DirectoryTreeUpdatePanel.Update();
            //   }
            //   Session["ShowList"] = b;

        }

        protected void RenameInputBox_TextChanged(object sender, EventArgs e)
        {

        }

        protected void NewFolderBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var newpath = Server.MapPath(Session["UserPath"] as string) + "\\New Folder";
                Directory.CreateDirectory(newpath);
            }
            catch { }
            UpdateAll();
        }

        protected void DownloadZipBtn_Click(object sender, EventArgs e)
        {
            var arc = CreateArchive();
            if (Context.Session["temp"] == null)
                Context.Session["temp"] = new List<string>();
            var list = Context.Session["temp"] as List<string>;
            list.Add(arc);
            UpdateAll();
            PushArchiveToClient(arc, false);

        }

        protected string CreateArchive()
        {
            Session["copy"] = new List<KeyValuePair<string, string>>();
            var root = "/Upload/Files/l0x/";
            var dirname = Guid.NewGuid().ToString();
            var zipname = dirname + ".zip";

            var dest = root + zipname;
            var source = root + dirname;
            //List<string> files = new List<string>();

            //create directory
            Directory.CreateDirectory(Server.MapPath(source));

            //copy all selected
            CopyBtn_Click(this, new EventArgs());

            //paste files to temp dir
            if (CopyList.Count == 0)
                return "";
            else
                CopyFilesToArchive(CopyList, source);



            Session["copy"] = new List<KeyValuePair<string, string>>();

            //zip
            source = Server.MapPath(source);
            dest = Server.MapPath(dest);
            ZipFile.CreateFromDirectory(source, dest, CompressionLevel.Fastest, false);
            Directory.Delete(source, true);
            if (File.Exists(dest))
            {

            }
            else
            {
                Page.SiteMaster().ShowError("Problem Creating Zip File");
            }
            //send
            return dest;
        }

        protected void CopyFilesToArchive(List<KeyValuePair<string, string>> list, string relativeTargetDirectory)
        {
            foreach (var copy in list)
            {
                var source = copy.Key;
                source = source.Terminate() + copy.Value;
                if (Path.GetExtension(copy.Value) == "")
                {

                    DecryptAndPasteDir(source, relativeTargetDirectory);
                }
                else
                {
                    DecryptAndPasteFile(source, relativeTargetDirectory);
                }
            }

        }

        protected string DecryptAndPasteFile(string file, string relativeDest)
        {
            var pass = Application[UID + "pass"] as string;
            var path = file;
            path = Server.MapPath(path);
            var fileInfo = new FileInfo(path);
            using (FileStream reader = new FileStream(path, FileMode.Open))
            {
                try
                {
                    if (!relativeDest.EndsWith(fileInfo.Name))
                    {
                        relativeDest = relativeDest.Terminate() + fileInfo.Name;
                    }
                    var dest = Server.MapPath(relativeDest);
                    using (FileStream writer = new FileStream(dest, FileMode.Create))
                    {
                        var os = ECDHAES256s.AES.Decrypt(reader, pass);
                        os.Position = 0;
                        os.CopyTo(writer);
                    }
                }
                catch
                {

                }
            }
            return path;
        }

        //recursive decrypt
        protected string DecryptAndPasteDir(string directory, string relativeDest)
        {


            var dest = "";
            try
            {
                var pass = Application[UID + "pass"] as string;

                //relativeDest = relativeDest.Terminate();

                //set dest
                var a = relativeDest.Terminate().Replace("\\", "/");
                var idx = directory.LastIndexOf('/');
                var b = directory.Substring(idx);


                dest = a + b;
                dest = dest.Map();
                relativeDest = relativeDest.Terminate();
                //create dest dir
                if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);

                //get source
                var source = directory;

                source = source.Terminate().Map();

                DirectoryInfo dirInfo = new DirectoryInfo(source);

                foreach (var d in Directory.GetDirectories(source))
                {
                    var rs = d.Replace("\\", "/").Split(new string[] { "/Upload/Files" }, StringSplitOptions.None).Last();
                    var bb = "/Upload/Files" + rs;
                    var cc = a + b;
                    var target = cc;
                    DecryptAndPasteDir(bb.Replace("//", "/"), target.Replace("//", "/"));
                }
                foreach (var d in Directory.GetFiles(source))
                {
                    //DecryptAndPasteFile(file, (relativeDest + directory));
                    var rs = d.Replace("\\", "/").Split(new string[] { "/Upload/Files" }, StringSplitOptions.None).Last();
                    var src = "/Upload/Files" + rs;
                    var cc = a + b.Replace("/", "");
                    var target = Path.GetFileName(src);
                    var tmp = "";
                    target = cc.Terminate() + target;
                    tmp = "";
                    DecryptAndPasteFile(src.Replace("//", "/"), target.Replace("//", "/"));

                }

            }
            catch
            {
            }
            return dest;

        }

        protected void PushArchiveToClient(string file, bool deleteAfter = false)
        {
            try
            {
                HttpContext.Current.Response.ClearContent();

                HttpContext.Current.Response.ClearHeaders();

                HttpContext.Current.Response.AddHeader("Content-Disposition", "inline; filename=" + Path.GetFileName(file));

                HttpContext.Current.Response.ContentType = "application/zip";

                HttpContext.Current.Response.WriteFile(file);
                if (deleteAfter)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {

                    }
                }
                HttpContext.Current.Response.End();
            }
            catch { }
        }
    }
}