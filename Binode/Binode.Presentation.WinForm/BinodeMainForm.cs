using Binode.Data.Model;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Binode.Presentation.WinForm
{
    public partial class BinodeMainForm : Form
    {
        private TreeNode _rightClicknode;

        public BinodeMainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var kategoriler = DemoData.DemoKategoriGetir();
            KategoriyiTreeviewAEkle(kategoriler, null);
        }

        private void KategoriyiTreeviewAEkle(List<Kategori> kategoriler, TreeNode node)
        {
            foreach (var kategori in kategoriler)
            {
                if (node is null)
                {
                    var nnode = new TreeNode(kategori.Isim);
                    nnode.ContextMenuStrip = contextMenuStrip1;
                    treeKategori.Nodes.Add(nnode);
                    nnode.Tag = kategori;

                    if (kategori.AltKategori != null)
                    {
                        KategoriyiTreeviewAEkle(kategori.AltKategori, nnode);
                    }
                }
                else
                {
                    var nnode = node.Nodes.Add(kategori.Isim);
                    nnode.ContextMenuStrip = contextMenuStrip1;

                    nnode.Tag = kategori;
                    if (kategori.AltKategori != null)
                    {
                        KategoriyiTreeviewAEkle(kategori.AltKategori, nnode);
                    }
                }
            }

        }

        private void treeKategori_AfterSelect(object sender, TreeViewEventArgs e)
        {
            listView1.Items.Clear();

            ListViewIcerigeGoreDoldur(e.Node);
        }

        private void ListViewDoldur(TreeNode node)
        {
            var kategori = node.Tag as Kategori;

            //Hatalı olabilir
            if (kategori?.Icerik?.Count == null)
            {
                return;
            }

            var group = new ListViewGroup();
            group.Name = kategori.Isim;
            group.Header = kategori.Isim;

            listView1.Groups.Add(group);
            foreach (var icerik in kategori.Icerik)
            {
                var li = new ListViewItem(new[] { icerik.Isim, kategori.Isim });
                li.Group = group;
                li.Tag = icerik;
                li.ImageKey = icerik.Tip.ToString().ToLower();
                listView1.Items.Add(li);
            }

            //listView1.Groups.Add(group);

            if (node.Nodes != null)
            {
                foreach (TreeNode subNode in node.Nodes)
                {
                    ListViewDoldur(subNode);
                }
            }
        }

        private void ListViewIcerigeGoreDoldur(TreeNode node)
        {
            var kategori = node.Tag as Kategori;

            //Hatalı olabilir
            if (kategori?.Icerik?.Count == null)
            {
                return;
            }

            var groupMetin = new ListViewGroup();
            groupMetin.Name = IcerikTipi.Metin.ToString();
            groupMetin.Header = IcerikTipi.Metin.ToString();

            var groupPdf = new ListViewGroup();
            groupPdf.Name = IcerikTipi.Pdf.ToString();
            groupPdf.Header = IcerikTipi.Pdf.ToString();

            var groupVideo = new ListViewGroup();
            groupVideo.Name = IcerikTipi.Video.ToString();
            groupVideo.Header = IcerikTipi.Video.ToString();

            var groupSes = new ListViewGroup();
            groupSes.Name = IcerikTipi.Ses.ToString();
            groupSes.Header = IcerikTipi.Ses.ToString();

            foreach (var icerik in kategori.Icerik)
            {
                if (icerik.Tip == IcerikTipi.Metin)
                {
                    var li = new ListViewItem(new[] { icerik.Isim, kategori.Isim }, icerik.Tip.ToString().ToLower(), groupMetin);
                    li.Tag = icerik;
                    listView1.Items.Add(li);
                }
                else if (icerik.Tip == IcerikTipi.Pdf)
                {
                    var li = new ListViewItem(new[] { icerik.Isim, kategori.Isim }, icerik.Tip.ToString().ToLower(), groupPdf);
                    li.Tag = icerik;
                    listView1.Items.Add(li);
                }
                else if (icerik.Tip == IcerikTipi.Video)
                {
                    var li = new ListViewItem(new[] { icerik.Isim, kategori.Isim }, icerik.Tip.ToString().ToLower(), groupVideo);
                    li.Tag = icerik;
                    listView1.Items.Add(li);
                }
                if (icerik.Tip == IcerikTipi.Ses)
                {
                    var li = new ListViewItem(new[] { icerik.Isim, kategori.Isim }, icerik.Tip.ToString().ToLower(), groupSes);
                    li.Tag = icerik;
                    listView1.Items.Add(li);
                }
            }

            //listView1.Groups.Add(group);

            if (node.Nodes != null)
            {
                foreach (TreeNode subNode in node.Nodes)
                {
                    ListViewIcerigeGoreDoldur(subNode);
                }
            }

            listView1.Groups.Add(groupMetin);
            listView1.Groups.Add(groupPdf);
            listView1.Groups.Add(groupVideo);
            listView1.Groups.Add(groupSes);
        }

        private void treeKategori_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _rightClicknode = e.Node;
            }

            //e.Node.BeginEdit();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //Debug.WriteLine(_rightClicknode.Text);
            _rightClicknode.BeginEdit();

        }

        private void treeKategori_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            e.Node.Text = e.Label;
            var kategori = e.Node.Tag as Kategori;

            kategori.Isim = e.Label;
            RefreshListView();
        }

        private void RefreshListView()
        {
            listView1.Items.Clear();
            ListViewIcerigeGoreDoldur(treeKategori.SelectedNode);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            string kategoriAdi = Interaction.InputBox("Kategori adını giriniz.");
            var anaKategori = _rightClicknode.Tag as Kategori;
            var yeniKAtegori = new Kategori
            {
                Isim = kategoriAdi,
                UstKategori = anaKategori,
                EklenmeTarihi = DateTime.Now,
                //AltKategori = new List<Kategori>()
            };

            anaKategori.AltKategori.Add(yeniKAtegori);
            _rightClicknode.Nodes.Add(new TreeNode
            {
                Text = kategoriAdi,
                Tag = yeniKAtegori,
                ContextMenuStrip = contextMenuStrip1
            });
        }

        private void metinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var textContentForm = new AddTextContentForm();
            textContentForm.SelectedKategori = _rightClicknode.Tag as Kategori;
            textContentForm.ShowDialog();
            RefreshListView();
        }

        private void pdfToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFileContent("PDF | *.pdf", IcerikTipi.Pdf);
        }

        private void AddFileContent(string filter, IcerikTipi tip)
        {
            openFileDialog1.Filter = filter;
            openFileDialog1.ShowDialog();

            var category = _rightClicknode.Tag as Kategori;

            var content = new Icerik
            {
                Isim = openFileDialog1.SafeFileName,
                Content = openFileDialog1.FileName,
                Tip = tip,
                EklenmeTarihi = DateTime.Now,
                Kategori = category
            };

            category.Icerik.Add(content);

            RefreshListView();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            var listView = sender as ListView;
            var content = listView.SelectedItems[0].Tag as Icerik;

            switch (content.Tip)
            {
                case IcerikTipi.Metin:
                    ShowTextContentViewForm(content);
                    break;
                case IcerikTipi.Pdf:
                    break;
                case IcerikTipi.Ses:
                    break;
                case IcerikTipi.Video:
                    ShowVideoContentPlayerForm();
                    break;
                default:
                    break;
            }
        }

        private void ShowTextContentViewForm(Icerik content)
        {
            var textContentViewverForm = new TextContentViewForm();
            textContentViewverForm.Icerik = content;
            textContentViewverForm.ShowDialog();
        }

        private void ShowVideoContentPlayerForm()
        {
            var videoContentPlayerForm = new VideoContentPlayerForm();
            videoContentPlayerForm.ShowDialog();
        }

        private void videoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFileContent("MP4 | *.mp4", IcerikTipi.Video);
        }

        private void sesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddFileContent("WAV | *.wav", IcerikTipi.Ses);
        }
    }
}
