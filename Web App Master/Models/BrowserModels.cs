using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;

namespace Web_App_Master.Models
{
    public class BindingFileList : BindingList<FileFolderInfo>, IEnumerable
    {
        public BindingFileList() { }
        public BindingFileList(List<System.IO.FileInfo> list)
        {
            foreach (var item in list)
            {
                this.Add(new FileFolderInfo(item));
            }
        }
    }
    public enum IconType
    {
        Folder, File
    }
    public class FileFolderInfo
    {
        public FileFolderInfo(string dir)
        {
            Url = "javascript:void(0);";
            Title = dir;
            Id = Convert.ToBase64String(Encoding.UTF8.GetBytes(dir));
            ToolTip = dir;
            IconType = IconType.Folder;
            // var tmp = System.IO.Path.GetDirectoryName(dir);
            //var split = tmp.Split(new string[] { "\\" }, StringSplitOptions.None).ToList();
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(dir));
            OnClick = "dirNodeClick('" + encoded + "');";
        }
        public FileFolderInfo(System.IO.DirectoryInfo dir)
        {
            Url = "javascript:void(0);";
            Title = dir.Name;
            Id = Convert.ToBase64String(Encoding.UTF8.GetBytes(dir.FullName));
            ToolTip = dir.Name;
            IconType = IconType.Folder;
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(dir.FullName));
            OnClick = "dirNodeClick('" + encoded + "');";
        }
        public FileFolderInfo(System.IO.FileInfo file)
        {

            var name = file.Name;
            if (file.Name.Contains("_-_"))
            {
                name = name.Split(new string[] { "_-_" }, StringSplitOptions.None)[1];
            }
            Title = name;
            Id = Convert.ToBase64String(Encoding.UTF8.GetBytes(file.FullName));
            ToolTip = file.Name;
            IconType = IconType.File;
            Size = file.Length.ToString() + " bytes";
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(file.FullName));
            OnClick = "fileNodeClick('" + encoded + "');";
            Url = "un.l0x?f=" + encoded;
        }

        public IconType IconType = IconType.File;
        public string Id { get; set; }
        public string ToolTip { get; set; }
        public string Image
        {
            get
            {
                switch (this.IconType)
                {
                    case IconType.Folder:
                        return "images/icons/icons8-folder-40.png";
                    case IconType.File:
                        return "images/icons/icons8-file-40.png";
                    default:
                        return "images/icons/icons8-file-40.png";
                }

            }
        }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Size { get; set; }
        public string OnClick { get; set; }
    }
    public class DirectoryTree : IEnumerable<DirectoryTreeNode>
    {
        public void Add(DirectoryTreeNode node)
        {
            Nodes.Add(node);
        }
        public void Clear()
        {
            Nodes = new BindingList<DirectoryTreeNode>();
        }

        public IEnumerator<DirectoryTreeNode> GetEnumerator()
        {
            return ((IEnumerable<DirectoryTreeNode>)Nodes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<DirectoryTreeNode>)Nodes).GetEnumerator();
        }

        public DirectoryTree()
        {
            Name = "";
            Nodes = new BindingList<DirectoryTreeNode>();
            ImgDir = "images/";
        }
        public string Name { get; set; }
        public BindingList<DirectoryTreeNode> Nodes { get; set; }
        public string ImgDir { get; set; }
    }
    public class DirectoryTreeNode
    {
        public void Clear()
        {
            Nodes = new BindingList<DirectoryTreeNode>();
        }
        public DirectoryTreeNode(string name)
        {
            Type = NodeType.directory;
            Text = name;
            Name = name;
            Url = "#";
            ImgUrl = "images/transparent.png";
            Size = "0";
            Nodes = new BindingList<DirectoryTreeNode>();
        }
        public NodeType Type { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }
        public Func<object, object> Action { get; set; }
        public string Url { get; set; }
        public string ImgUrl { get; set; }
        public string Size { get; set; }
        public BindingList<DirectoryTreeNode> Nodes { get; set; }
    }
    public enum NodeType
    {
        file, directory, root
    }

    public class G_JSTree
    {
        public G_JsTreeAttribute attr;
        public G_JSTree[] children;
        public string data
        {
            get;
            set;
        }
        public int IdServerUse
        {
            get;
            set;
        }
        public string icons
        {
            get;
            set;
        }
        public string state
        {
            get;
            set;
        }
    }

    public class G_JsTreeAttribute
    {
        public string id;
        public bool selected;
    }
}