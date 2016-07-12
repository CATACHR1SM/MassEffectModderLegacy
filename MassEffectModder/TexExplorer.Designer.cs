/*
 * MassEffectModder
 *
 * Copyright (C) 2014-2016 Pawel Kolodziejski <aquadran at users.sourceforge.net>
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.

 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.

 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 *
 */

namespace MassEffectModder
{
    partial class TexExplorer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TexExplorer));
            this.contextMenuStripTextures = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.replaceTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.infoTextureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listViewMods = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStripMods = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.applyModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteModToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listViewResults = new System.Windows.Forms.ListView();
            this.columnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.treeViewPackages = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listViewTextures = new System.Windows.Forms.ListView();
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            this.richTextBoxInfo = new System.Windows.Forms.RichTextBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.MODsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sTARTModdingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eNDModdingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMODsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearMODsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.byCRCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeEmptyMipmapsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTextures.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextMenuStripMods.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStripTextures
            // 
            this.contextMenuStripTextures.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.replaceTextureToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.contextMenuStripTextures.Name = "contextMenuStripTextures";
            this.contextMenuStripTextures.Size = new System.Drawing.Size(198, 48);
            // 
            // replaceTextureToolStripMenuItem
            // 
            this.replaceTextureToolStripMenuItem.Name = "replaceTextureToolStripMenuItem";
            this.replaceTextureToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.replaceTextureToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.replaceTextureToolStripMenuItem.Text = "Replace Texture";
            this.replaceTextureToolStripMenuItem.Click += new System.EventHandler(this.replaceTextureToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.previewTextureToolStripMenuItem,
            this.infoTextureToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // previewTextureToolStripMenuItem
            // 
            this.previewTextureToolStripMenuItem.Name = "previewTextureToolStripMenuItem";
            this.previewTextureToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.previewTextureToolStripMenuItem.Text = "Preview";
            this.previewTextureToolStripMenuItem.Click += new System.EventHandler(this.previewToolStripMenuItem_Click);
            // 
            // infoTextureToolStripMenuItem
            // 
            this.infoTextureToolStripMenuItem.Name = "infoTextureToolStripMenuItem";
            this.infoTextureToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.infoTextureToolStripMenuItem.Text = "Info";
            this.infoTextureToolStripMenuItem.Click += new System.EventHandler(this.infoToolStripMenuItem_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Folder.png");
            this.imageList.Images.SetKeyName(1, "Folder Open.png");
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewMods);
            this.splitContainer1.Panel1.Controls.Add(this.listViewResults);
            this.splitContainer1.Panel1.Controls.Add(this.treeViewPackages);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(1140, 462);
            this.splitContainer1.SplitterDistance = 338;
            this.splitContainer1.TabIndex = 4;
            // 
            // listViewMods
            // 
            this.listViewMods.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewMods.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listViewMods.ContextMenuStrip = this.contextMenuStripMods;
            this.listViewMods.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listViewMods.Location = new System.Drawing.Point(0, 28);
            this.listViewMods.Name = "listViewMods";
            this.listViewMods.ShowGroups = false;
            this.listViewMods.Size = new System.Drawing.Size(338, 431);
            this.listViewMods.TabIndex = 2;
            this.listViewMods.UseCompatibleStateImageBehavior = false;
            this.listViewMods.View = System.Windows.Forms.View.List;
            this.listViewMods.SelectedIndexChanged += new System.EventHandler(this.listViewMods_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 500;
            // 
            // contextMenuStripMods
            // 
            this.contextMenuStripMods.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.applyModToolStripMenuItem,
            this.deleteModToolStripMenuItem});
            this.contextMenuStripMods.Name = "contextMenuStripTextures";
            this.contextMenuStripMods.Size = new System.Drawing.Size(136, 48);
            // 
            // applyModToolStripMenuItem
            // 
            this.applyModToolStripMenuItem.Name = "applyModToolStripMenuItem";
            this.applyModToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.applyModToolStripMenuItem.Text = "Apply Mod";
            this.applyModToolStripMenuItem.Click += new System.EventHandler(this.applyModToolStripMenuItem_Click);
            // 
            // deleteModToolStripMenuItem
            // 
            this.deleteModToolStripMenuItem.Name = "deleteModToolStripMenuItem";
            this.deleteModToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.deleteModToolStripMenuItem.Text = "Delete Mod";
            this.deleteModToolStripMenuItem.Click += new System.EventHandler(this.deleteModToolStripMenuItem_Click);
            // 
            // listViewResults
            // 
            this.listViewResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader});
            this.listViewResults.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listViewResults.Location = new System.Drawing.Point(0, 28);
            this.listViewResults.MultiSelect = false;
            this.listViewResults.Name = "listViewResults";
            this.listViewResults.ShowGroups = false;
            this.listViewResults.Size = new System.Drawing.Size(338, 431);
            this.listViewResults.TabIndex = 1;
            this.listViewResults.UseCompatibleStateImageBehavior = false;
            this.listViewResults.View = System.Windows.Forms.View.List;
            this.listViewResults.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewResults_MouseDoubleClick);
            // 
            // columnHeader
            // 
            this.columnHeader.Width = 500;
            // 
            // treeViewPackages
            // 
            this.treeViewPackages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewPackages.ImageKey = "Folder.png";
            this.treeViewPackages.ImageList = this.imageList;
            this.treeViewPackages.Location = new System.Drawing.Point(3, 28);
            this.treeViewPackages.Name = "treeViewPackages";
            this.treeViewPackages.SelectedImageIndex = 1;
            this.treeViewPackages.Size = new System.Drawing.Size(336, 431);
            this.treeViewPackages.TabIndex = 0;
            this.treeViewPackages.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeViewPackages_AfterCollapse);
            this.treeViewPackages.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeViewPackages_AfterExpand);
            this.treeViewPackages.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewPackages_AfterSelect);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listViewTextures);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.pictureBoxPreview);
            this.splitContainer2.Panel2.Controls.Add(this.richTextBoxInfo);
            this.splitContainer2.Size = new System.Drawing.Size(798, 462);
            this.splitContainer2.SplitterDistance = 334;
            this.splitContainer2.TabIndex = 0;
            // 
            // listViewTextures
            // 
            this.listViewTextures.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewTextures.ContextMenuStrip = this.contextMenuStripTextures;
            this.listViewTextures.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listViewTextures.Location = new System.Drawing.Point(3, 28);
            this.listViewTextures.MultiSelect = false;
            this.listViewTextures.Name = "listViewTextures";
            this.listViewTextures.Size = new System.Drawing.Size(328, 431);
            this.listViewTextures.TabIndex = 0;
            this.listViewTextures.UseCompatibleStateImageBehavior = false;
            this.listViewTextures.View = System.Windows.Forms.View.List;
            this.listViewTextures.SelectedIndexChanged += new System.EventHandler(this.listViewTextures_SelectedIndexChanged);
            this.listViewTextures.DoubleClick += new System.EventHandler(this.listViewTextures_DoubleClick);
            this.listViewTextures.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.listViewTextures_KeyPress);
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxPreview.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pictureBoxPreview.Location = new System.Drawing.Point(3, 30);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(457, 429);
            this.pictureBoxPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxPreview.TabIndex = 1;
            this.pictureBoxPreview.TabStop = false;
            // 
            // richTextBoxInfo
            // 
            this.richTextBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBoxInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxInfo.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBoxInfo.Location = new System.Drawing.Point(3, 28);
            this.richTextBoxInfo.Name = "richTextBoxInfo";
            this.richTextBoxInfo.Size = new System.Drawing.Size(457, 432);
            this.richTextBoxInfo.TabIndex = 0;
            this.richTextBoxInfo.Text = "";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MODsToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.removeEmptyMipmapsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1140, 24);
            this.menuStrip.TabIndex = 5;
            this.menuStrip.Text = "menuStrip1";
            // 
            // MODsToolStripMenuItem
            // 
            this.MODsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sTARTModdingToolStripMenuItem,
            this.eNDModdingToolStripMenuItem,
            this.loadMODsToolStripMenuItem,
            this.clearMODsToolStripMenuItem});
            this.MODsToolStripMenuItem.Name = "MODsToolStripMenuItem";
            this.MODsToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.MODsToolStripMenuItem.Text = "MODs";
            // 
            // sTARTModdingToolStripMenuItem
            // 
            this.sTARTModdingToolStripMenuItem.Name = "sTARTModdingToolStripMenuItem";
            this.sTARTModdingToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.sTARTModdingToolStripMenuItem.Text = "START Modding";
            this.sTARTModdingToolStripMenuItem.Click += new System.EventHandler(this.sTARTModdingToolStripMenuItem_Click);
            // 
            // eNDModdingToolStripMenuItem
            // 
            this.eNDModdingToolStripMenuItem.Name = "eNDModdingToolStripMenuItem";
            this.eNDModdingToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.eNDModdingToolStripMenuItem.Text = "END Modding";
            this.eNDModdingToolStripMenuItem.Click += new System.EventHandler(this.eNDModdingToolStripMenuItem_Click);
            // 
            // loadMODsToolStripMenuItem
            // 
            this.loadMODsToolStripMenuItem.Name = "loadMODsToolStripMenuItem";
            this.loadMODsToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.loadMODsToolStripMenuItem.Text = "Load MODs";
            this.loadMODsToolStripMenuItem.Click += new System.EventHandler(this.loadMODsToolStripMenuItem_Click);
            // 
            // clearMODsToolStripMenuItem
            // 
            this.clearMODsToolStripMenuItem.Name = "clearMODsToolStripMenuItem";
            this.clearMODsToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.clearMODsToolStripMenuItem.Text = "Clear MODs";
            this.clearMODsToolStripMenuItem.Click += new System.EventHandler(this.clearMODsToolStripMenuItem_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.byNameToolStripMenuItem,
            this.byCRCToolStripMenuItem});
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.searchToolStripMenuItem.Text = "Search Texture";
            // 
            // byNameToolStripMenuItem
            // 
            this.byNameToolStripMenuItem.Name = "byNameToolStripMenuItem";
            this.byNameToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.byNameToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.byNameToolStripMenuItem.Text = "By name";
            this.byNameToolStripMenuItem.Click += new System.EventHandler(this.byNameToolStripMenuItem_Click);
            // 
            // byCRCToolStripMenuItem
            // 
            this.byCRCToolStripMenuItem.Name = "byCRCToolStripMenuItem";
            this.byCRCToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.byCRCToolStripMenuItem.Text = "By CRC";
            this.byCRCToolStripMenuItem.Click += new System.EventHandler(this.byCRCToolStripMenuItem_Click);
            // 
            // removeEmptyMipmapsToolStripMenuItem
            // 
            this.removeEmptyMipmapsToolStripMenuItem.Name = "removeEmptyMipmapsToolStripMenuItem";
            this.removeEmptyMipmapsToolStripMenuItem.Size = new System.Drawing.Size(152, 20);
            this.removeEmptyMipmapsToolStripMenuItem.Text = "Remove empty mipmaps";
            this.removeEmptyMipmapsToolStripMenuItem.Click += new System.EventHandler(this.removeEmptyMipmapsToolStripMenuItem_Click);
            // 
            // TexExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 462);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.splitContainer1);
            this.MinimumSize = new System.Drawing.Size(1024, 500);
            this.Name = "TexExplorer";
            this.Text = "Texture Explorer";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TexExplorer_FormClosed);
            this.contextMenuStripTextures.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextMenuStripMods.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTextures;
        private System.Windows.Forms.ToolStripMenuItem replaceTextureToolStripMenuItem;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ListView listViewTextures;
        private System.Windows.Forms.RichTextBox richTextBoxInfo;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
        public System.Windows.Forms.TreeView treeViewPackages;
        private System.Windows.Forms.ListView listViewResults;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previewTextureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem infoTextureToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem byCRCToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.ToolStripMenuItem removeEmptyMipmapsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MODsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sTARTModdingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eNDModdingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMODsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearMODsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMods;
        private System.Windows.Forms.ToolStripMenuItem applyModToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteModToolStripMenuItem;
        private System.Windows.Forms.ListView listViewMods;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}