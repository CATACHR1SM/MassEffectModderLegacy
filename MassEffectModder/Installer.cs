/*
 * MassEffectModder
 *
 * Copyright (C) 2016-2019 Pawel Kolodziejski
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

using StreamHelpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.Devices;
using System.Reflection;

namespace MassEffectModder
{
    public partial class Installer : Form
    {
        struct ModSelection
        {
            public List<string> files;
            public List<string> descriptions;
        }
        List<ModSelection> modsSelection;
        List<string> allMemMods;
        const uint MEMI_TAG = 0x494D454D;
        public bool exitToModder;
        ConfIni configIni;
        ConfIni installerIni;
        int gameId = 1;
        GameData gameData;
        public List<string> memFiles;
        List<FoundTexture> textures;
        MipMaps mipMaps;
        TreeScan treeScan;
        bool updateMode;
        bool unpackDLC;
        string errors = "";
        string log = "";
        int MeuitmVer;
        string softShadowsModPath;
        string splashDemiurge;
        string splashEA;
        string BlackCrushRemoval;
        string splashBitmapPath;
        string reshadePath;
        string indirectSoundPath;
        bool meuitmMode = false;
        bool OptionRepackVisible;
        bool OptionBlackCrushVisible;
        bool Option2kLimitVisible;
        bool OptionReshadeVisible;
        bool OptionBikVisible;
        bool mute = false;
        int stage = 1;
        int totalStages = 6;
        System.Media.SoundPlayer musicPlayer;
        CustomLabel customLabelDesc;
        CustomLabel customLabelCurrentStatus;
        CustomLabel customLabelFinalStatus;
        static public List<string> pkgsToRepack = null;
        static public List<string> pkgsToMarker = null;

        public Installer()
        {
            InitializeComponent();
            Text = "MEM Installer v" + Application.ProductVersion;
            mipMaps = new MipMaps();
            treeScan = new TreeScan();
            MipMaps.modsToReplace = new List<ModEntry>();

            // 
            // customLabelDesc
            // 
            customLabelDesc = new CustomLabel();
            customLabelDesc.Anchor = labelDesc.Anchor;
            customLabelDesc.BackColor = labelDesc.BackColor;
            customLabelDesc.FlatStyle = labelDesc.FlatStyle;
            customLabelDesc.Font = labelDesc.Font;
            customLabelDesc.ForeColor = labelDesc.ForeColor;
            customLabelDesc.Location = labelDesc.Location;
            customLabelDesc.Name = "customLabelDesc";
            customLabelDesc.Size = labelDesc.Size;
            customLabelDesc.TextAlign = labelDesc.TextAlign;
            customLabelDesc.Visible = false;
            Controls.Add(customLabelDesc);
            // 
            // customLabelCurrentStatus
            // 
            customLabelCurrentStatus = new CustomLabel();
            customLabelCurrentStatus.Anchor = labelCurrentStatus.Anchor;
            customLabelCurrentStatus.BackColor = labelCurrentStatus.BackColor;
            customLabelCurrentStatus.FlatStyle = labelCurrentStatus.FlatStyle;
            customLabelCurrentStatus.Font = labelCurrentStatus.Font;
            customLabelCurrentStatus.ForeColor = labelCurrentStatus.ForeColor;
            customLabelCurrentStatus.Location = labelCurrentStatus.Location;
            customLabelCurrentStatus.Name = "customLabelCurrentStatus";
            customLabelCurrentStatus.Size = labelCurrentStatus.Size;
            customLabelCurrentStatus.TextAlign = labelCurrentStatus.TextAlign;
            customLabelCurrentStatus.Visible = false;
            Controls.Add(customLabelCurrentStatus);
            // 
            // customLabelDesc
            // 
            customLabelFinalStatus = new CustomLabel();
            customLabelFinalStatus.Anchor = labelFinalStatus.Anchor;
            customLabelFinalStatus.BackColor = labelFinalStatus.BackColor;
            customLabelFinalStatus.FlatStyle = labelFinalStatus.FlatStyle;
            customLabelFinalStatus.Font = labelFinalStatus.Font;
            customLabelFinalStatus.ForeColor = labelFinalStatus.ForeColor;
            customLabelFinalStatus.Location = labelFinalStatus.Location;
            customLabelFinalStatus.Name = "customLabelFinalStatus";
            customLabelFinalStatus.Size = labelFinalStatus.Size;
            customLabelFinalStatus.TextAlign = labelFinalStatus.TextAlign;
            customLabelFinalStatus.Visible = false;
            Controls.Add(customLabelFinalStatus);
        }

        public bool Run(bool runAsAdmin)
        {
            if (runAsAdmin)
                MessageBox.Show("The Installer should be run as standard user to avoid (user account) issues.\n" +
                    "The installer will ask for administrative rights when necessary.");

            installerIni = new ConfIni(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "installer.ini"));
            string gameIdStr = installerIni.Read("GameId", "Main");
            if (gameIdStr.ToLowerInvariant() == "me1")
                gameId = 1;
            else if (gameIdStr.ToLowerInvariant() == "me2")
                gameId = 2;
            else if (gameIdStr.ToLowerInvariant() == "me3")
                gameId = 3;
            else
            {
                MessageBox.Show("Game ID not recognized in installer.ini, exiting...", "Installer");
                return false;
            }

            string baseModNameStr = installerIni.Read("BaseModName", "Main");
            if (baseModNameStr != "")
            {
                Text = "MEM Installer v" + Application.ProductVersion + " for " + baseModNameStr;
                if (gameId == 1 && baseModNameStr.Contains("MEUITM"))
                    meuitmMode = true;
            }
            else
                Text += " for ME" + gameId;

            if (runAsAdmin)
                Text += " (run as Administrator)";

            try
            {
                MeuitmVer = int.Parse(installerIni.Read("MeuitmVersion", "Main"));
            }
            catch (Exception)
            {
                MeuitmVer = 0;
            }

            string meuitm = installerIni.Read("MEUITM", "Main").ToLowerInvariant();
            if (gameId == 1 && (meuitm == "true" || MeuitmVer != 0))
                meuitmMode = true;
            if (meuitmMode && MeuitmVer == 0)
                MeuitmVer = 1;

            indirectSoundPath = installerIni.Read("IndirectSound", "Main").ToLowerInvariant();
            if (indirectSoundPath != "")
            {
                if (!File.Exists(indirectSoundPath) || Path.GetExtension(indirectSoundPath).ToLowerInvariant() != ".zip")
                {
                    indirectSoundPath = "";
                }
            }

            BlackCrushRemoval = installerIni.Read("BlackCrushCorrected", "Main").ToLowerInvariant();
            if (BlackCrushRemoval != "")
            {
                if (!File.Exists(BlackCrushRemoval) || Path.GetExtension(BlackCrushRemoval).ToLowerInvariant() != ".zip")
                {
                    BlackCrushRemoval = "";
                }
            }

            splashDemiurge = installerIni.Read("DemiurgeSplashVideo", "Main").ToLowerInvariant();
            if (splashDemiurge != "")
            {
                if (!File.Exists(splashDemiurge) || Path.GetExtension(splashDemiurge).ToLowerInvariant() != ".bik")
                {
                    splashDemiurge = "";
                }
            }

            splashEA = installerIni.Read("EASplashVideo", "Main").ToLowerInvariant();
            if (splashEA != "")
            {
                if (!File.Exists(splashEA) || Path.GetExtension(splashEA).ToLowerInvariant() != ".bik")
                {
                    splashEA = "";
                }
            }

            splashBitmapPath = installerIni.Read("SplashBitmap", "Main").ToLowerInvariant();
            if (splashBitmapPath != "")
            {
                if (!File.Exists(splashBitmapPath) || Path.GetExtension(splashBitmapPath).ToLowerInvariant() != ".bmp")
                {
                    splashBitmapPath = "";
                }
            }

            reshadePath = installerIni.Read("ReShade", "Main").ToLowerInvariant();
            if (reshadePath != "")
            {
                if (!File.Exists(reshadePath) || Path.GetExtension(reshadePath).ToLowerInvariant() != ".zip")
                {
                    reshadePath = "";
                }
            }

            comboBoxMod0.Visible = comboBoxMod1.Visible = comboBoxMod2.Visible = comboBoxMod3.Visible = false;
            comboBoxMod4.Visible = comboBoxMod5.Visible = comboBoxMod6.Visible = comboBoxMod7.Visible = false;
            comboBoxMod8.Visible = comboBoxMod9.Visible = comboBoxMod10.Visible = comboBoxMod11.Visible = false;
            comboBoxMod12.Visible = comboBoxMod13.Visible = comboBoxMod14.Visible = comboBoxMod15.Visible = false;
            comboBoxMod16.Visible = comboBoxMod17.Visible = comboBoxMod18.Visible = comboBoxMod19.Visible = false;
            comboBoxMod20.Visible = comboBoxMod21.Visible = comboBoxMod22.Visible = comboBoxMod23.Visible = false;
            comboBoxMod24.Visible = comboBoxMod25.Visible = comboBoxMod26.Visible = comboBoxMod27.Visible = false;
            comboBoxMod28.Visible = comboBoxMod29.Visible = comboBoxMod30.Visible = comboBoxMod31.Visible = false;
            comboBoxMod32.Visible = comboBoxMod33.Visible = comboBoxMod34.Visible = comboBoxMod35.Visible = false;
            comboBoxMod36.Visible = comboBoxMod37.Visible = comboBoxMod38.Visible = comboBoxMod39.Visible = false;
            comboBoxMod40.Visible = comboBoxMod41.Visible = comboBoxMod42.Visible = comboBoxMod43.Visible = false;
            comboBoxMod44.Visible = comboBoxMod45.Visible = comboBoxMod46.Visible = comboBoxMod47.Visible = false;
            comboBoxMod48.Visible = comboBoxMod49.Visible = comboBoxMod50.Visible = comboBoxMod51.Visible = false;
            comboBoxMod52.Visible = comboBoxMod53.Visible = comboBoxMod54.Visible = comboBoxMod55.Visible = false;
            comboBoxMod56.Visible = comboBoxMod57.Visible = comboBoxMod58.Visible = comboBoxMod59.Visible = false;

            allMemMods = new List<string>();
            modsSelection = new List<ModSelection>();
            for (int i = 1; i <= 60; i++)
            {
                ModSelection modSelect = new ModSelection();
                modSelect.files = new List<string>();
                modSelect.descriptions = new List<string>();
                for (int l = 1; l <= 10; l++)
                {
                    string file = installerIni.Read("File" + l, "Mod" + i).ToLowerInvariant();
                    string description = installerIni.Read("Label" + l, "Mod" + i);
                    if (file == "" || description == "")
                        continue;
                    modSelect.files.Add(file);
                    modSelect.descriptions.Add(description);
                }
                if (modSelect.files.Count < 2)
                {
                    modSelect.files.Clear();
                    modSelect.descriptions.Clear();
                    continue;
                }
                modsSelection.Add(modSelect);
            }
            for (int i = 1; i <= modsSelection.Count; i++)
            {
                ModSelection modSelect = modsSelection[i - 1];
                for (int l = 1; l <= modSelect.files.Count; l++)
                {
                    allMemMods.Add(modSelect.files[l - 1]);
                    string description = modSelect.descriptions[l - 1];
                    switch (i)
                    {
                        case 1:
                            comboBoxMod0.Items.Add(description);
                            comboBoxMod0.Visible = true;
                            comboBoxMod0.SelectedIndex = 0;
                            break;
                        case 2:
                            comboBoxMod1.Items.Add(description);
                            comboBoxMod1.Visible = true;
                            comboBoxMod1.SelectedIndex = 0;
                            break;
                        case 3:
                            comboBoxMod2.Items.Add(description);
                            comboBoxMod2.Visible = true;
                            comboBoxMod2.SelectedIndex = 0;
                            break;
                        case 4:
                            comboBoxMod3.Items.Add(description);
                            comboBoxMod3.Visible = true;
                            comboBoxMod3.SelectedIndex = 0;
                            break;
                        case 5:
                            comboBoxMod4.Items.Add(description);
                            comboBoxMod4.Visible = true;
                            comboBoxMod4.SelectedIndex = 0;
                            break;
                        case 6:
                            comboBoxMod5.Items.Add(description);
                            comboBoxMod5.Visible = true;
                            comboBoxMod5.SelectedIndex = 0;
                            break;
                        case 7:
                            comboBoxMod6.Items.Add(description);
                            comboBoxMod6.Visible = true;
                            comboBoxMod6.SelectedIndex = 0;
                            break;
                        case 8:
                            comboBoxMod7.Items.Add(description);
                            comboBoxMod7.Visible = true;
                            comboBoxMod7.SelectedIndex = 0;
                            break;
                        case 9:
                            comboBoxMod8.Items.Add(description);
                            comboBoxMod8.Visible = true;
                            comboBoxMod8.SelectedIndex = 0;
                            break;
                        case 10:
                            comboBoxMod9.Items.Add(description);
                            comboBoxMod9.Visible = true;
                            comboBoxMod9.SelectedIndex = 0;
                            break;
                        case 11:
                            comboBoxMod10.Items.Add(description);
                            comboBoxMod10.Visible = true;
                            comboBoxMod10.SelectedIndex = 0;
                            break;
                        case 12:
                            comboBoxMod11.Items.Add(description);
                            comboBoxMod11.Visible = true;
                            comboBoxMod11.SelectedIndex = 0;
                            break;
                        case 13:
                            comboBoxMod12.Items.Add(description);
                            comboBoxMod12.Visible = true;
                            comboBoxMod12.SelectedIndex = 0;
                            break;
                        case 14:
                            comboBoxMod13.Items.Add(description);
                            comboBoxMod13.Visible = true;
                            comboBoxMod13.SelectedIndex = 0;
                            break;
                        case 15:
                            comboBoxMod14.Items.Add(description);
                            comboBoxMod14.Visible = true;
                            comboBoxMod14.SelectedIndex = 0;
                            break;
                        case 16:
                            comboBoxMod15.Items.Add(description);
                            comboBoxMod15.Visible = true;
                            comboBoxMod15.SelectedIndex = 0;
                            break;
                        case 17:
                            comboBoxMod16.Items.Add(description);
                            comboBoxMod16.Visible = true;
                            comboBoxMod16.SelectedIndex = 0;
                            break;
                        case 18:
                            comboBoxMod17.Items.Add(description);
                            comboBoxMod17.Visible = true;
                            comboBoxMod17.SelectedIndex = 0;
                            break;
                        case 19:
                            comboBoxMod18.Items.Add(description);
                            comboBoxMod18.Visible = true;
                            comboBoxMod18.SelectedIndex = 0;
                            break;
                        case 20:
                            comboBoxMod19.Items.Add(description);
                            comboBoxMod19.Visible = true;
                            comboBoxMod19.SelectedIndex = 0;
                            break;
                        case 21:
                            comboBoxMod20.Items.Add(description);
                            comboBoxMod20.Visible = true;
                            comboBoxMod20.SelectedIndex = 0;
                            break;
                        case 22:
                            comboBoxMod21.Items.Add(description);
                            comboBoxMod21.Visible = true;
                            comboBoxMod21.SelectedIndex = 0;
                            break;
                        case 23:
                            comboBoxMod22.Items.Add(description);
                            comboBoxMod22.Visible = true;
                            comboBoxMod22.SelectedIndex = 0;
                            break;
                        case 24:
                            comboBoxMod23.Items.Add(description);
                            comboBoxMod23.Visible = true;
                            comboBoxMod23.SelectedIndex = 0;
                            break;
                        case 25:
                            comboBoxMod24.Items.Add(description);
                            comboBoxMod24.Visible = true;
                            comboBoxMod24.SelectedIndex = 0;
                            break;
                        case 26:
                            comboBoxMod25.Items.Add(description);
                            comboBoxMod25.Visible = true;
                            comboBoxMod25.SelectedIndex = 0;
                            break;
                        case 27:
                            comboBoxMod26.Items.Add(description);
                            comboBoxMod26.Visible = true;
                            comboBoxMod26.SelectedIndex = 0;
                            break;
                        case 28:
                            comboBoxMod27.Items.Add(description);
                            comboBoxMod27.Visible = true;
                            comboBoxMod27.SelectedIndex = 0;
                            break;
                        case 29:
                            comboBoxMod28.Items.Add(description);
                            comboBoxMod28.Visible = true;
                            comboBoxMod28.SelectedIndex = 0;
                            break;
                        case 30:
                            comboBoxMod29.Items.Add(description);
                            comboBoxMod29.Visible = true;
                            comboBoxMod29.SelectedIndex = 0;
                            break;
                        case 31:
                            comboBoxMod30.Items.Add(description);
                            comboBoxMod30.Visible = true;
                            comboBoxMod30.SelectedIndex = 0;
                            break;
                        case 32:
                            comboBoxMod31.Items.Add(description);
                            comboBoxMod31.Visible = true;
                            comboBoxMod31.SelectedIndex = 0;
                            break;
                        case 33:
                            comboBoxMod32.Items.Add(description);
                            comboBoxMod32.Visible = true;
                            comboBoxMod32.SelectedIndex = 0;
                            break;
                        case 34:
                            comboBoxMod33.Items.Add(description);
                            comboBoxMod33.Visible = true;
                            comboBoxMod33.SelectedIndex = 0;
                            break;
                        case 35:
                            comboBoxMod34.Items.Add(description);
                            comboBoxMod34.Visible = true;
                            comboBoxMod34.SelectedIndex = 0;
                            break;
                        case 36:
                            comboBoxMod35.Items.Add(description);
                            comboBoxMod35.Visible = true;
                            comboBoxMod35.SelectedIndex = 0;
                            break;
                        case 37:
                            comboBoxMod36.Items.Add(description);
                            comboBoxMod36.Visible = true;
                            comboBoxMod36.SelectedIndex = 0;
                            break;
                        case 38:
                            comboBoxMod37.Items.Add(description);
                            comboBoxMod37.Visible = true;
                            comboBoxMod37.SelectedIndex = 0;
                            break;
                        case 39:
                            comboBoxMod38.Items.Add(description);
                            comboBoxMod38.Visible = true;
                            comboBoxMod38.SelectedIndex = 0;
                            break;
                        case 40:
                            comboBoxMod39.Items.Add(description);
                            comboBoxMod39.Visible = true;
                            comboBoxMod39.SelectedIndex = 0;
                            break;
                        case 41:
                            comboBoxMod40.Items.Add(description);
                            comboBoxMod40.Visible = true;
                            comboBoxMod40.SelectedIndex = 0;
                            break;
                        case 42:
                            comboBoxMod41.Items.Add(description);
                            comboBoxMod41.Visible = true;
                            comboBoxMod41.SelectedIndex = 0;
                            break;
                        case 43:
                            comboBoxMod42.Items.Add(description);
                            comboBoxMod42.Visible = true;
                            comboBoxMod42.SelectedIndex = 0;
                            break;
                        case 44:
                            comboBoxMod43.Items.Add(description);
                            comboBoxMod43.Visible = true;
                            comboBoxMod43.SelectedIndex = 0;
                            break;
                        case 45:
                            comboBoxMod44.Items.Add(description);
                            comboBoxMod44.Visible = true;
                            comboBoxMod44.SelectedIndex = 0;
                            break;
                        case 46:
                            comboBoxMod45.Items.Add(description);
                            comboBoxMod45.Visible = true;
                            comboBoxMod45.SelectedIndex = 0;
                            break;
                        case 47:
                            comboBoxMod46.Items.Add(description);
                            comboBoxMod46.Visible = true;
                            comboBoxMod46.SelectedIndex = 0;
                            break;
                        case 48:
                            comboBoxMod47.Items.Add(description);
                            comboBoxMod47.Visible = true;
                            comboBoxMod47.SelectedIndex = 0;
                            break;
                        case 49:
                            comboBoxMod48.Items.Add(description);
                            comboBoxMod48.Visible = true;
                            comboBoxMod48.SelectedIndex = 0;
                            break;
                        case 50:
                            comboBoxMod49.Items.Add(description);
                            comboBoxMod49.Visible = true;
                            comboBoxMod49.SelectedIndex = 0;
                            break;
                        case 51:
                            comboBoxMod50.Items.Add(description);
                            comboBoxMod50.Visible = true;
                            comboBoxMod50.SelectedIndex = 0;
                            break;
                        case 52:
                            comboBoxMod51.Items.Add(description);
                            comboBoxMod51.Visible = true;
                            comboBoxMod51.SelectedIndex = 0;
                            break;
                        case 53:
                            comboBoxMod52.Items.Add(description);
                            comboBoxMod52.Visible = true;
                            comboBoxMod52.SelectedIndex = 0;
                            break;
                        case 54:
                            comboBoxMod53.Items.Add(description);
                            comboBoxMod53.Visible = true;
                            comboBoxMod53.SelectedIndex = 0;
                            break;
                        case 55:
                            comboBoxMod54.Items.Add(description);
                            comboBoxMod54.Visible = true;
                            comboBoxMod54.SelectedIndex = 0;
                            break;
                        case 56:
                            comboBoxMod55.Items.Add(description);
                            comboBoxMod55.Visible = true;
                            comboBoxMod55.SelectedIndex = 0;
                            break;
                        case 57:
                            comboBoxMod56.Items.Add(description);
                            comboBoxMod56.Visible = true;
                            comboBoxMod56.SelectedIndex = 0;
                            break;
                        case 58:
                            comboBoxMod57.Items.Add(description);
                            comboBoxMod57.Visible = true;
                            comboBoxMod57.SelectedIndex = 0;
                            break;
                        case 59:
                            comboBoxMod58.Items.Add(description);
                            comboBoxMod58.Visible = true;
                            comboBoxMod58.SelectedIndex = 0;
                            break;
                        case 60:
                            comboBoxMod59.Items.Add(description);
                            comboBoxMod59.Visible = true;
                            comboBoxMod59.SelectedIndex = 0;
                            break;

                    }
                }
            }

            if (modsSelection.Count == 0)
                labelModsSelection.Visible = false;

            configIni = new ConfIni();

            customLabelDesc.Text = customLabelCurrentStatus.Text = customLabelFinalStatus.Text = "";

            if (gameId == 2 || gameId == 3)
                OptionRepackVisible = checkBoxOptionRepack.Visible = labelOptionRepack.Visible = true;
            else
                OptionRepackVisible = checkBoxOptionRepack.Visible = labelOptionRepack.Visible = false;
            if (gameId == 2 && BlackCrushRemoval != "")
                OptionBlackCrushVisible = checkBoxOptionBlackCrush.Visible = labelOptionBlackCrush.Visible = true;
            else
                OptionBlackCrushVisible = checkBoxOptionBlackCrush.Visible = labelOptionBlackCrush.Visible = false;
            if (gameId == 1)
                Option2kLimitVisible = checkBoxOption2kLimit.Visible = labelOption2kLimit.Visible = true;
            else
                Option2kLimitVisible = checkBoxOption2kLimit.Visible = labelOption2kLimit.Visible = false;
            if (gameId == 1 && splashDemiurge != "")
                OptionBikVisible = checkBoxOptionBikInst.Visible = labelOptionBikinst.Visible = true;
            else
                OptionBikVisible = checkBoxOptionBikInst.Visible = labelOptionBikinst.Visible = false;
            if (gameId == 2 && splashEA != "")
                OptionBikVisible = checkBoxOptionBikInst.Visible = labelOptionBikinst.Visible = true;
            else
                OptionBikVisible = checkBoxOptionBikInst.Visible = labelOptionBikinst.Visible = false;
            if (gameId == 1 && reshadePath != "")
                OptionReshadeVisible = checkBoxOptionReshade.Visible = labelOptionReshade.Visible = true;
            else
                OptionReshadeVisible = checkBoxOptionReshade.Visible = labelOptionReshade.Visible = false;

            if (gameId == 2 && reshadePath != "")
                OptionReshadeVisible = checkBoxOptionReshade.Visible = labelOptionReshade.Visible = true;
            else
                OptionReshadeVisible = checkBoxOptionReshade.Visible = labelOptionReshade.Visible = false;

            if (gameId == 1)
            checkBoxOptionReshade.Checked = false;
            checkBoxOptionBikInst.Checked = true;

            if (gameId == 2)
            checkBoxOptionReshade.Checked = false;
            checkBoxOptionBikInst.Checked = true;

            buttonSTART.Visible = true;
            buttonNormal.Visible = true;

            customLabelDesc.Parent = pictureBoxBG;
            customLabelFinalStatus.Parent = pictureBoxBG;
            customLabelCurrentStatus.Parent = pictureBoxBG;
            labelOptions.Parent = pictureBoxBG;
            labelOptionRepack.Parent = pictureBoxBG;
            labelOptionBlackCrush.Parent = pictureBoxBG;
            labelOption2kLimit.Parent = pictureBoxBG;
            labelOptionReshade.Parent = pictureBoxBG;
            labelOptionBikinst.Parent = pictureBoxBG;
            checkBoxOptionRepack.Parent = pictureBoxBG;
            checkBoxOptionBlackCrush.Parent = pictureBoxBG;
            checkBoxOption2kLimit.Parent = pictureBoxBG;
            checkBoxOptionReshade.Parent = pictureBoxBG;
            checkBoxOptionBikInst.Parent = pictureBoxBG;
            labelModsSelection.Parent = pictureBoxBG;
            comboBoxMod0.Parent = comboBoxMod1.Parent = comboBoxMod2.Parent = comboBoxMod3.Parent = comboBoxMod4.Parent = pictureBoxBG;
            comboBoxMod5.Parent = comboBoxMod6.Parent = comboBoxMod7.Parent = comboBoxMod8.Parent = comboBoxMod9.Parent = pictureBoxBG;
            comboBoxMod10.Parent = comboBoxMod11.Parent = comboBoxMod12.Parent = comboBoxMod13.Parent = comboBoxMod14.Parent = pictureBoxBG;
            comboBoxMod15.Parent = comboBoxMod16.Parent = comboBoxMod17.Parent = comboBoxMod18.Parent = comboBoxMod19.Parent = pictureBoxBG;
            comboBoxMod20.Parent = comboBoxMod21.Parent = comboBoxMod22.Parent = comboBoxMod23.Parent = comboBoxMod24.Parent = pictureBoxBG;
            comboBoxMod25.Parent = comboBoxMod26.Parent = comboBoxMod27.Parent = comboBoxMod28.Parent = comboBoxMod29.Parent = pictureBoxBG;
            comboBoxMod30.Parent = comboBoxMod31.Parent = comboBoxMod32.Parent = comboBoxMod33.Parent = comboBoxMod34.Parent = pictureBoxBG;
            comboBoxMod35.Parent = comboBoxMod36.Parent = comboBoxMod37.Parent = comboBoxMod38.Parent = comboBoxMod39.Parent = pictureBoxBG;
            comboBoxMod40.Parent = comboBoxMod41.Parent = comboBoxMod42.Parent = comboBoxMod43.Parent = comboBoxMod44.Parent = pictureBoxBG;
            comboBoxMod45.Parent = comboBoxMod46.Parent = comboBoxMod47.Parent = comboBoxMod48.Parent = comboBoxMod49.Parent = pictureBoxBG;
            comboBoxMod50.Parent = comboBoxMod51.Parent = comboBoxMod52.Parent = comboBoxMod53.Parent = comboBoxMod54.Parent = pictureBoxBG;
            comboBoxMod55.Parent = comboBoxMod56.Parent = comboBoxMod57.Parent = comboBoxMod58.Parent = comboBoxMod59.Parent = pictureBoxBG;
            buttonMute.Parent = pictureBoxBG;

            labelOptions.Visible = OptionRepackVisible || Option2kLimitVisible || OptionReshadeVisible || OptionBikVisible || OptionBlackCrushVisible;

            string bgFile = installerIni.Read("BackgroundImage", "Main").ToLowerInvariant();
            if (bgFile != "")
            {
                if (File.Exists(bgFile))
                {
                    try
                    {
                        pictureBoxBG.Image = new Bitmap(bgFile);
                    }
                    catch
                    {
                        pictureBoxBG.Image = null;
                    }
                }
            }
            if (pictureBoxBG.Image == null)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string res = assembly.GetName().Name + ".Resources.me" + gameId + "_bg.jpg";
                pictureBoxBG.Image = new Bitmap(assembly.GetManifestResourceStream(res));
            }

            softShadowsModPath = installerIni.Read("SoftShadowsMod", "Main").ToLowerInvariant();
            if (softShadowsModPath != "")
            {
                if (!File.Exists(softShadowsModPath) || Path.GetExtension(softShadowsModPath).ToLowerInvariant() != ".zip")
                {
                    softShadowsModPath = "";
                }
            }

            string musicFile = installerIni.Read("MusicSource", "Main").ToLowerInvariant();
            if (musicFile != "" && File.Exists(musicFile))
            {
                try
                {
                    if (Path.GetExtension(musicFile).ToLowerInvariant() == ".mp3")
                    {
                        new System.Threading.Thread(delegate () {
                            try
                            {
                                byte[] srcBuffer = File.ReadAllBytes(musicFile);
                                byte[] wavBuffer = LibMadHelper.LibMad.Decompress(srcBuffer);
                                if (wavBuffer.Length != 0)
                                {
                                    MemoryStream wavStream = new MemoryStream(wavBuffer);
                                    musicPlayer = new System.Media.SoundPlayer(wavStream);
                                    musicPlayer.PlayLooping();
                                    Invoke(new Action(() => { buttonMute.Visible = true; }));
                                }
                            }
                            catch
                            {
                            }
                        }).Start();
                    }
                    else if (Path.GetExtension(musicFile).ToLowerInvariant() == ".wav")
                    {
                        musicPlayer = new System.Media.SoundPlayer(musicFile);
                        musicPlayer.PlayLooping();
                        buttonMute.Visible = true;
                    }
                }
                catch
                {
                }
            }

            return true;
        }

        bool detectMod(int gameId)
        {
            string path = "";
            if (gameId == (int)MeType.ME1_TYPE)
            {
                path = @"\BioGame\CookedPC\testVolumeLight_VFX.upk";
            }
            if (gameId == (int)MeType.ME2_TYPE)
            {
                path = @"\BioGame\CookedPC\BIOC_Materials.pcc";
            }
            if (gameId == (int)MeType.ME3_TYPE)
            {
                path = @"\BIOGame\CookedPCConsole\adv_combat_tutorial_xbox_D_Int.afc";
            }
            try
            {
                using (FileStream fs = new FileStream(GameData.GamePath + path, FileMode.Open, FileAccess.Read))
                {
                    fs.Seek(-16, SeekOrigin.End);
                    int prevMeuitmV = fs.ReadInt32();
                    int prevAlotV = fs.ReadInt32();
                    int prevProductV = fs.ReadInt32();
                    uint memiTag = fs.ReadUInt32();
                    if (memiTag == MEMI_TAG)
                    {
                        if (prevProductV < 10 || prevProductV == 4352 || prevProductV == 16777472) // default before MEM v178
                            prevProductV = prevAlotV = prevMeuitmV = 0;
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }

            return false;
        }

        static public bool applyModTag(int gameId, int MeuitmV, int AlotV)
        {
            string path = "";
            if (gameId == (int)MeType.ME1_TYPE)
            {
                path = @"\BioGame\CookedPC\testVolumeLight_VFX.upk";
            }
            if (gameId == (int)MeType.ME2_TYPE)
            {
                path = @"\BioGame\CookedPC\BIOC_Materials.pcc";
            }
            if (gameId == (int)MeType.ME3_TYPE)
            {
                path = @"\BIOGame\CookedPCConsole\adv_combat_tutorial_xbox_D_Int.afc";
            }
            try
            {
                using (FileStream fs = new FileStream(GameData.GamePath + path, FileMode.Open, FileAccess.ReadWrite))
                {
                    fs.Seek(-16, SeekOrigin.End);
                    int prevMeuitmV = fs.ReadInt32();
                    int prevAlotV = fs.ReadInt32();
                    int prevProductV = fs.ReadInt32();
                    uint memiTag = fs.ReadUInt32();
                    if (memiTag == MEMI_TAG)
                    {
                        if (prevProductV < 10 || prevProductV == 4352 || prevProductV == 16777472) // default before MEM v178
                            prevProductV = prevAlotV = prevMeuitmV = 0;
                    }
                    else
                        prevProductV = prevAlotV = prevMeuitmV = 0;
                    if (MeuitmV != 0)
                        prevMeuitmV = MeuitmV;
                    if (AlotV != 0)
                        prevAlotV = AlotV;
                    fs.WriteInt32(prevMeuitmV);
                    fs.WriteInt32(prevAlotV);
                    fs.WriteInt32((int)(prevProductV & 0xffff0000) | int.Parse(Application.ProductVersion));
                    fs.WriteUInt32(MEMI_TAG);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public void AddMarkers(MeType gameType)
        {
            for (int i = 0; i < pkgsToMarker.Count; i++)
            {
                updateProgressStatus("Adding markers " + ((i + 1) * 100 / pkgsToMarker.Count) + "%");
                try
                {
                    using (FileStream fs = new FileStream(GameData.GamePath + pkgsToMarker[i], FileMode.Open, FileAccess.ReadWrite))
                    {
                        fs.SeekEnd();
                        fs.Seek(-Package.MEMendFileMarker.Length, SeekOrigin.Current);
                        string marker = fs.ReadStringASCII(Package.MEMendFileMarker.Length);
                        if (marker != Package.MEMendFileMarker)
                        {
                            fs.SeekEnd();
                            fs.WriteStringASCII(Package.MEMendFileMarker);
                        }
                    }
                }
                catch
                {
                }
            }
        }

        private bool installSoftShadowsMod(GameData gameData, string path)
        {
            IntPtr handle = IntPtr.Zero;
            int result;
            ulong numEntries = 0;
            string fileName = "";
            ulong dstLen = 0;
            ZlibHelper.Zip zip = new ZlibHelper.Zip();
            try
            {
                handle = zip.Open(path, ref numEntries, 0);
                if (handle == IntPtr.Zero)
                    throw new Exception();
                for (uint i = 0; i < numEntries; i++)
                {
                    result = zip.GetCurrentFileInfo(handle, ref fileName, ref dstLen);
                    if (result != 0)
                        throw new Exception();

                    byte[] data = new byte[dstLen];
                    result = zip.ReadCurrentFile(handle, data, dstLen);
                    if (result != 0)
                    {
                        throw new Exception();
                    }

                    string filePath = GameData.GamePath + "\\Engine\\Shaders\\" + fileName;
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
                    {
                        fs.WriteFromBuffer(data);
                    }

                    zip.GoToNextFile(handle);
                }
            }
            catch
            {
                return false;
            }

            try
            {
                string cachePath = gameData.GameUserPath + "\\Published\\CookedPC\\LocalShaderCache-PC-D3D-SM3.upk";
                if (File.Exists(cachePath))
                    File.Delete(cachePath);
                cachePath = GameData.MainData + "\\LocalShaderCache-PC-D3D-SM3.upk";
                if (File.Exists(cachePath))
                    File.Delete(cachePath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool installBlackCrushFix(string path)
        {
            IntPtr handle = IntPtr.Zero;
            int result;
            ulong numEntries = 0;
            string fileName = "";
            ulong dstLen = 0;
            ZlibHelper.Zip zip = new ZlibHelper.Zip();
            try
            {
                handle = zip.Open(path, ref numEntries, 0);
                if (handle == IntPtr.Zero)
                    throw new Exception();
                for (uint i = 0; i < numEntries; i++)
                {
                    result = zip.GetCurrentFileInfo(handle, ref fileName, ref dstLen);
                    if (result != 0)
                        throw new Exception();
                    
                    byte[] data = new byte[dstLen];
                    result = zip.ReadCurrentFile(handle, data, dstLen);
                    if (result != 0)
                    {
                        throw new Exception();
                    }

                    string filePath = GameData.GamePath + "\\Engine\\Shaders\\" + fileName;
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
                    {
                        fs.WriteFromBuffer(data);
                    }

                    zip.GoToNextFile(handle);
                }
            }
            catch
            {
                return false;
            
            }

            return true;
        }

        private bool installSplashScreen(string path)
        {
            string filePath = GameData.bioGamePath + "\\Splash\\Splash.bmp";
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                File.Copy(path, filePath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool installSplashVideo(string path)
        {
            string filePath = GameData.MainData + "\\Movies\\db_standard.bik";
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                File.Copy(path, filePath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool installSplashVideo2(string path)
        {
            string filePath = GameData.bioGamePath + "\\Movies\\ME_EAsig_720p_v2_raw.bik";
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
                File.Copy(path, filePath);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool installIndirectSoundPath(string path)
        {
            IntPtr handle = IntPtr.Zero;
            int result;
            ulong numEntries = 0;
            string fileName = "";
            ulong dstLen = 0;
            ZlibHelper.Zip zip = new ZlibHelper.Zip();
            try
            {
                handle = zip.Open(path, ref numEntries, 0);
                if (handle == IntPtr.Zero)
                    throw new Exception();
                for (uint i = 0; i < numEntries; i++)
                {
                    result = zip.GetCurrentFileInfo(handle, ref fileName, ref dstLen);
                    if (result != 0)
                        throw new Exception();
                    if (fileName.ToLowerInvariant() != "dsound.dll" &&
                        fileName.ToLowerInvariant() != "dsound.ini")
                    {
                        continue;
                    }
                    byte[] data = new byte[dstLen];
                    result = zip.ReadCurrentFile(handle, data, dstLen);
                    if (result != 0)
                    {
                        throw new Exception();
                    }

                    string filePath = GameData.GamePath + "\\Binaries\\" + fileName;
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
                    {
                        fs.WriteFromBuffer(data);
                    }

                    zip.GoToNextFile(handle);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        private bool installReshadePath(string path)
        {
            IntPtr handle = IntPtr.Zero;
            int result;
            ulong numEntries = 0;
            string fileName = "";
            ulong dstLen = 0;
            ZlibHelper.Zip zip = new ZlibHelper.Zip();
            try
            {
                handle = zip.Open(path, ref numEntries, 0);
                if (handle == IntPtr.Zero)
                    throw new Exception();
                for (uint i = 0; i < numEntries; i++)
                {
                    result = zip.GetCurrentFileInfo(handle, ref fileName, ref dstLen);
                    if (result != 0)
                        throw new Exception();
                    fileName = fileName.Replace('/', '\\');
                    string filePath = GameData.GamePath + "\\Binaries\\" + fileName;
                    if (filePath.EndsWith("\\"))
                    {
                        if (!Directory.Exists(filePath))
                            Directory.CreateDirectory(filePath);
                        zip.GoToNextFile(handle);
                        continue;
                    }
                    if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    byte[] data = new byte[dstLen];
                    result = zip.ReadCurrentFile(handle, data, dstLen);
                    if (result != 0)
                    {
                        throw new Exception();
                    }

                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    using (FileStream fs = new FileStream(filePath, FileMode.CreateNew))
                    {
                        fs.WriteFromBuffer(data);
                    }

                    zip.GoToNextFile(handle);
                }
            }
            catch
            {
                return false;
            }

            if (File.Exists(GameData.GamePath + "\\Binaries\\d3d9.ini"))
            {
                try
                {
                    ConfIni shaderConf = new ConfIni(GameData.GamePath + "\\Binaries\\d3d9.ini");
                    shaderConf.Write("TextureSearchPaths", GameData.GamePath + "\\Binaries\\reshade-shaders\\Textures", "GENERAL");
                    shaderConf.Write("EffectSearchPaths", GameData.GamePath + "\\Binaries\\reshade-shaders\\Shaders", "GENERAL");
                    shaderConf.Write("PresetFiles", GameData.GamePath + "\\Binaries\\MassEffect.ini", "GENERAL");
                }
                catch
                {
                }
            }

            return true;
        }

        private bool PreInstallCheck()
        {
            customLabelFinalStatus.Text = "Checking game setup...";
            Application.DoEvents();

            string filename = "errors-precheck.txt";
            if (File.Exists(filename))
                File.Delete(filename);

            ulong memorySize = ((new ComputerInfo().TotalPhysicalMemory / 1024 / 1024) + 1023) / 1024;
            if (memorySize < 8)
            {
                MessageBox.Show("Detected small amount of physical RAM (8GB is recommended).\nInstallation may take a long time.", "Installer");
            }

            memFiles = Directory.GetFiles(".", "*.mem", SearchOption.AllDirectories).Where(item => item.EndsWith(".mem", StringComparison.OrdinalIgnoreCase)).ToList();
            memFiles.Sort(StringComparer.OrdinalIgnoreCase);
            if (memFiles.Count == 0)
            {
                customLabelFinalStatus.Text = "No MEM file mods found!, aborting...";
                customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);
                return false;
            }
            errors = "";
            log = "";
            for (int i = 0; i < memFiles.Count; i++)
            {
                using (FileStream fs = new FileStream(memFiles[i], FileMode.Open, FileAccess.Read))
                {
                    uint tag = fs.ReadUInt32();
                    uint version = fs.ReadUInt32();
                    if (tag != TreeScan.TextureModTag || version != TreeScan.TextureModVersion)
                    {
                        if (version != TreeScan.TextureModVersion)
                            errors += "File " + memFiles[i] + " was made with an older version of MEM, skipping..." + Environment.NewLine;
                        else
                            errors += "File " + memFiles[i] + " is not a valid MEM mod, skipping..." + Environment.NewLine;
                        continue;
                    }
                    else
                    {
                        uint gameType = 0;
                        fs.JumpTo(fs.ReadInt64());
                        gameType = fs.ReadUInt32();
                        if (gameType != gameId)
                        {
                            errors += "File " + memFiles[i] + " is not a MEM mod valid for ME" + gameId + ", skipping..." + Environment.NewLine;
                            continue;
                        }
                    }
                }
            }

            if (errors != "")
            {
                customLabelFinalStatus.Text = "There are some errors while detecting MEM mods, aborting...";
                customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);

                if (File.Exists(filename))
                    File.Delete(filename);
                using (FileStream fs = new FileStream(filename, FileMode.CreateNew))
                {
                    fs.WriteStringASCII(errors);
                }
                Process.Start(filename);
                return false;
            }


            gameData = new GameData((MeType)gameId, configIni);
            if (!Directory.Exists(GameData.GamePath))
            {
                customLabelFinalStatus.Text = "Game path is wrong, aborting...";
                customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);
                return false;
            }
            if (!gameData.getPackages(true, true))
            {
                customLabelFinalStatus.Text = "Missing game data, aborting...";
                customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);
                return false;
            }
            if (gameId == (int)MeType.ME1_TYPE)
            {
                if (!File.Exists(GameData.GamePath + "\\BioGame\\CookedPC\\Startup_int.upk"))
                {
                    customLabelFinalStatus.Text = "ME1 game not found, aborting...";
                    customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);
                    return false;
                }
            }
            if (gameId == (int)MeType.ME2_TYPE)
            {
                if (!File.Exists(GameData.GamePath + "\\BioGame\\CookedPC\\Textures.tfc"))
                {
                    customLabelFinalStatus.Text = "ME2 game not found, aborting...";
                    customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);
                    return false;
                }
            }
            if (gameId == (int)MeType.ME3_TYPE)
            {
                if (!File.Exists(GameData.GamePath + "\\BIOGame\\PCConsoleTOC.bin"))
                {
                    customLabelFinalStatus.Text = "ME3 game not found, aborting...";
                    customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);
                    return false;
                }
            }

            bool writeAccess = Misc.CheckAndCorrectAccessToGame((MeType)gameId);
            if (!writeAccess)
            {
                customLabelFinalStatus.Text = "Write access denied, aborting...";
                customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);
                return false;
            }


            long diskFreeSpace = Misc.getDiskFreeSpace(GameData.GamePath);
            long diskUsage = 0;

            for (int i = 0; i < memFiles.Count; i++)
            {
                diskUsage += new FileInfo(memFiles[i]).Length;
            }
            diskUsage = (long)(diskUsage * 2.5);

            unpackDLC = false;
            if (gameId == (int)MeType.ME3_TYPE)
            {
                if (Directory.Exists(GameData.DLCData))
                {
                    long diskUsageDLC = 0;
                    List<string> sfarFiles = Directory.GetFiles(GameData.DLCData, "Default.sfar", SearchOption.AllDirectories).ToList();
                    for (int i = 0; i < sfarFiles.Count; i++)
                    {
                        if (File.Exists(Path.Combine(Path.GetDirectoryName(sfarFiles[i]), "Mount.dlc")))
                            sfarFiles.RemoveAt(i--);
                    }
                    if (sfarFiles.Count != 0)
                        unpackDLC = true;
                    for (int i = 0; i < sfarFiles.Count; i++)
                    {
                        diskUsageDLC += new FileInfo(sfarFiles[i]).Length;
                    }
                    diskUsage = (long)(diskUsageDLC * 2.1);
                }
            }

            if (diskUsage > diskFreeSpace)
            {
                customLabelFinalStatus.Text = "You have not enough disk space remaining. You need about " + Misc.getBytesFormat(diskUsage) + " free.";
                customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);
                return false;
            }


            List<string> mods = Misc.detectMods((MeType)gameId);
            if (mods.Count != 0 && gameId == 1 && GameData.FullScanME1Game)
            {
                errors = Environment.NewLine + "------- Detected NOT supported mods with this version of game --------" + Environment.NewLine + Environment.NewLine;
                for (int l = 0; l < mods.Count; l++)
                {
                    errors += mods[l] + Environment.NewLine;
                }
                errors += "---------------------------------------------" + Environment.NewLine + Environment.NewLine;
                errors += Environment.NewLine + Environment.NewLine;

                if (File.Exists(filename))
                    File.Delete(filename);
                using (FileStream fs = new FileStream(filename, FileMode.CreateNew))
                {
                    fs.WriteStringASCII(errors);
                }
                Process.Start(filename);

                DialogResult resp = MessageBox.Show("Detected NOT compatible/supported mods with this version of game!" +
                    "\n\nPress Cancel to abort or press Ok button to continue.", "Warning !", MessageBoxButtons.OKCancel);
                if (resp == DialogResult.Cancel)
                {
                    customLabelFinalStatus.Text = "Detected NOT supported mod...";
                    customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);
                    return false;
                }
            }

            List<string> brokenMods = Misc.detectBrokenMod((MeType)gameId);
            if (brokenMods.Count != 0)
            {
                errors = Environment.NewLine + "------- Detected not compatible mods --------" + Environment.NewLine + Environment.NewLine;
                for (int l = 0; l < brokenMods.Count; l++)
                {
                    errors += brokenMods[l] + Environment.NewLine;
                }
                errors += "---------------------------------------------" + Environment.NewLine + Environment.NewLine;
                errors += Environment.NewLine + Environment.NewLine;

                if (File.Exists(filename))
                    File.Delete(filename);
                using (FileStream fs = new FileStream(filename, FileMode.CreateNew))
                {
                    fs.WriteStringASCII(errors);
                }
                Process.Start(filename);

                customLabelFinalStatus.Text = "Detected not compatible mod, aborting...";
                customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);
                return false;
            }

            errors = "";
            updateMode = detectMod(gameId);

            // unpack DLC
            if (gameId != 3 || !unpackDLC || (updateMode && gameId == 3))
                totalStages -= 1;

            // scan textures, remove empty mipmaps, adding markers
            if (updateMode)
                totalStages -= 3;

            // recompress game files
            if (!checkBoxOptionRepack.Checked)
                totalStages -= 1;

            if (updateMode)
            {
                string mapPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        Assembly.GetExecutingAssembly().GetName().Name);
                string mapFile = Path.Combine(mapPath, "me" + gameId + "map.bin");

                if (!File.Exists(mapFile))
                {
                    customLabelFinalStatus.Text = "Game was not scanned for textures, can not continue, aborting...";
                    customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);
                    return false;
                }

                if (!loadTexturesMap(mapFile))
                {
                    customLabelFinalStatus.Text = "Game inconsistent from previous scan! Reinstall ME" + gameId + " and restart.";
                    customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);
                    return false;
                }
            }

            string path = gameData.EngineConfigIniPath;
            bool exist = File.Exists(path);
            if (!exist && gameId == 1)
            {
                MessageBox.Show("Missing game configuration file.\nYou need atleast once launch the game first.");
                return false;
            }

            return true;
        }

        private bool loadTexturesMap(string mapPath)
        {
            textures = new List<FoundTexture>();

            if (!File.Exists(mapPath))
                return false;

            using (FileStream fs = new FileStream(mapPath, FileMode.Open, FileAccess.Read))
            {
                uint tag = fs.ReadUInt32();
                uint version = fs.ReadUInt32();
                if (tag != TreeScan.textureMapBinTag || version != TreeScan.textureMapBinVersion)
                {
                    errors += "Detected wrong or old version of textures scan file!" + Environment.NewLine;
                    log += "Detected wrong or old version of textures scan file!" + Environment.NewLine;
                    return false;
                }

                uint countTexture = fs.ReadUInt32();
                for (int i = 0; i < countTexture; i++)
                {
                    FoundTexture texture = new FoundTexture();
                    int len = fs.ReadInt32();
                    texture.name = fs.ReadStringASCII(len);
                    texture.crc = fs.ReadUInt32();
                    uint countPackages = fs.ReadUInt32();
                    texture.list = new List<MatchedTexture>();
                    for (int k = 0; k < countPackages; k++)
                    {
                        MatchedTexture matched = new MatchedTexture();
                        matched.exportID = fs.ReadInt32();
                        matched.linkToMaster = fs.ReadInt32();
                        len = fs.ReadInt32();
                        matched.path = fs.ReadStringASCII(len);
                        texture.list.Add(matched);
                    }
                    textures.Add(texture);
                }

                List<string> packages = new List<string>();
                int numPackages = fs.ReadInt32();
                for (int i = 0; i < numPackages; i++)
                {
                    int len = fs.ReadInt32();
                    string pkgPath = fs.ReadStringASCII(len);
                    packages.Add(pkgPath);
                }
                for (int i = 0; i < packages.Count; i++)
                {
                    if (GameData.packageFiles.Find(s => s.Equals(packages[i], StringComparison.OrdinalIgnoreCase)) == null)
                    {
                        errors += "Detected removal of game files since last game data scan." + Environment.NewLine + Environment.NewLine;
                        log += "Detected removal of game files since last game data scan." + Environment.NewLine + Environment.NewLine;
                        return false;
                    }
                }
                for (int i = 0; i < GameData.packageFiles.Count; i++)
                {
                    if (packages.Find(s => s.Equals(GameData.packageFiles[i], StringComparison.OrdinalIgnoreCase)) == null)
                    {
                        errors += "Detected additional game files not present in latest game data scan." + Environment.NewLine + Environment.NewLine;
                        log += "Detected additional game files not present in latest game data scan." + Environment.NewLine + Environment.NewLine;
                        return false;
                    }
                }
            }

            return true;
        }

        public void applyModules()
        {
            int totalNumberOfMods = 0;
            int currentNumberOfTotalMods = 1;

            updateProgressStatus("Preparing textures...");

            MipMaps.modsToReplace.Clear();
            for (int i = 0; i < memFiles.Count; i++)
            {
                if (memFiles[i].EndsWith(".mem", StringComparison.OrdinalIgnoreCase))
                {
                    using (FileStream fs = new FileStream(memFiles[i], FileMode.Open, FileAccess.Read))
                    {
                        uint tag = fs.ReadUInt32();
                        uint version = fs.ReadUInt32();
                        if (tag != TreeScan.TextureModTag || version != TreeScan.TextureModVersion)
                            continue;
                        fs.JumpTo(fs.ReadInt64());
                        fs.SkipInt32();
                        totalNumberOfMods += fs.ReadInt32();
                    }
                }
                else
                    throw new Exception();
            }

            for (int i = 0; i < memFiles.Count; i++)
            {
                log += "Mod: " + (i + 1) + " of " + memFiles.Count + " started: " + Path.GetFileName(memFiles[i]) + Environment.NewLine;
                using (FileStream fs = new FileStream(memFiles[i], FileMode.Open, FileAccess.Read))
                {
                    uint tag = fs.ReadUInt32();
                    uint version = fs.ReadUInt32();
                    if (tag != TreeScan.TextureModTag || version != TreeScan.TextureModVersion)
                    {
                        if (version != TreeScan.TextureModVersion)
                        {
                            errors += "File " + memFiles[i] + " was made with an older version of MEM, skipping..." + Environment.NewLine;
                            log += "File " + memFiles[i] + " was made with an older version of MEM, skipping..." + Environment.NewLine;
                        }
                        else
                        {
                            errors += "File " + memFiles[i] + " is not a valid MEM mod, skipping..." + Environment.NewLine;
                            log += "File " + memFiles[i] + " is not a valid MEM mod, skipping..." + Environment.NewLine;
                        }
                        continue;
                    }
                    else
                    {
                        uint gameType = 0;
                        fs.JumpTo(fs.ReadInt64());
                        gameType = fs.ReadUInt32();
                        if ((MeType)gameType != GameData.gameType)
                        {
                            errors += "File " + memFiles[i] + " is not a MEM mod valid for this game, skipping..." + Environment.NewLine;
                            log += "File " + memFiles[i] + " is not a MEM mod valid for this game, skipping..." + Environment.NewLine;
                            continue;
                        }
                    }
                    int numFiles = fs.ReadInt32();
                    List<MipMaps.FileMod> modFiles = new List<MipMaps.FileMod>();
                    for (int k = 0; k < numFiles; k++)
                    {
                        MipMaps.FileMod fileMod = new MipMaps.FileMod();
                        fileMod.tag = fs.ReadUInt32();
                        fileMod.name = fs.ReadStringASCIINull();
                        fileMod.offset = fs.ReadInt64();
                        fileMod.size = fs.ReadInt64();
                        modFiles.Add(fileMod);
                    }
                    numFiles = modFiles.Count;
                    for (int l = 0; l < numFiles; l++, currentNumberOfTotalMods++)
                    {
                        string name = "";
                        uint crc = 0;
                        long size = 0;
                        int exportId = -1;
                        string pkgPath = "";
                        byte[] dst = null;
                        fs.JumpTo(modFiles[l].offset);
                        size = modFiles[l].size;
                        if (modFiles[l].tag == MipMaps.FileTextureTag || modFiles[l].tag == MipMaps.FileTextureTag2)
                        {
                            name = fs.ReadStringASCIINull();
                            crc = fs.ReadUInt32();
                        }
                        else if (modFiles[l].tag == MipMaps.FileBinaryTag)
                        {
                            name = modFiles[l].name;
                            exportId = fs.ReadInt32();
                            pkgPath = fs.ReadStringASCIINull();
                        }
                        else if (modFiles[l].tag == MipMaps.FileXdeltaTag)
                        {
                            name = modFiles[l].name;
                            exportId = fs.ReadInt32();
                            pkgPath = fs.ReadStringASCIINull();
                        }

                        if (modFiles[l].tag == MipMaps.FileBinaryTag || modFiles[l].tag == MipMaps.FileXdeltaTag)
                        {
                            dst = MipMaps.decompressData(fs, size);
                        }

                        if (modFiles[l].tag == MipMaps.FileTextureTag || modFiles[l].tag == MipMaps.FileTextureTag2)
                        {
                            FoundTexture foundTexture;
                            foundTexture = textures.Find(s => s.crc == crc);
                            if (foundTexture.crc != 0)
                            {
                                ModEntry entry = new ModEntry();
                                entry.textureCrc = foundTexture.crc;
                                entry.textureName = foundTexture.name;
                                if (modFiles[l].tag == MipMaps.FileTextureTag2)
                                    entry.markConvert = true;
                                entry.memPath = memFiles[i];
                                entry.memEntryOffset = fs.Position;
                                entry.memEntrySize = size;
                                MipMaps.modsToReplace.Add(entry);
                            }
                            else
                            {
                                log += "Texture skipped. Texture " + name + string.Format("_0x{0:X8}", crc) + " is not present in your game setup" + Environment.NewLine;
                            }
                        }
                        else if (modFiles[l].tag == MipMaps.FileBinaryTag)
                        {
                            string path = GameData.GamePath + pkgPath;
                            if (!File.Exists(path))
                            {
                                log += "Warning: File " + path + " not exists in your game setup." + Environment.NewLine;
                                continue;
                            }
                            ModEntry entry = new ModEntry();
                            entry.binaryModType = true;
                            entry.packagePath = pkgPath;
                            entry.exportId = exportId;
                            entry.binaryModData = dst;
                            MipMaps.modsToReplace.Add(entry);
                        }
                        else if (modFiles[l].tag == MipMaps.FileXdeltaTag)
                        {
                            string path = GameData.GamePath + pkgPath;
                            if (!File.Exists(path))
                            {
                                log += "Warning: File " + path + " not exists in your game setup." + Environment.NewLine;
                                continue;
                            }
                            ModEntry entry = new ModEntry();
                            Package pkg = new Package(path);
                            byte[] buffer = new Xdelta3Helper.Xdelta3().Decompress(pkg.getExportData(exportId), dst);
                            if (buffer.Length == 0)
                            {
                                errors += "Warning: Xdelta patch for " + path + " failed to apply." + Environment.NewLine;
                                log += "Warning: Xdelta patch for " + path + " failed to apply." + Environment.NewLine;
                                continue;
                            }
                            entry.binaryModType = true;
                            entry.packagePath = pkgPath;
                            entry.exportId = exportId;
                            entry.binaryModData = buffer;
                            MipMaps.modsToReplace.Add(entry);
                            pkg.Dispose();
                        }
                        else
                        {
                            errors += "Unknown tag for file: " + name + Environment.NewLine;
                            log += "Unknown tag for file: " + name + Environment.NewLine;
                        }
                    }
                }
            }

            errors += mipMaps.replaceModsFromList(textures, null, this, checkBoxOptionRepack.Checked,
                !updateMode, false, !updateMode, false);

            MipMaps.modsToReplace.Clear();
        }

        private void buttonSTART_Click(object sender, EventArgs e)
        {
            List<string> selectedFileMods = new List<string>();
            for (int i = 1; i <= modsSelection.Count; i++)
            {
                ModSelection modSelect = modsSelection[i - 1];
                string file = "";
                switch (i)
                {
                    case 1:
                        file = modSelect.files[comboBoxMod0.SelectedIndex];
                        break;
                    case 2:
                        file = modSelect.files[comboBoxMod1.SelectedIndex];
                        break;
                    case 3:
                        file = modSelect.files[comboBoxMod2.SelectedIndex];
                        break;
                    case 4:
                        file = modSelect.files[comboBoxMod3.SelectedIndex];
                        break;
                    case 5:
                        file = modSelect.files[comboBoxMod4.SelectedIndex];
                        break;
                    case 6:
                        file = modSelect.files[comboBoxMod5.SelectedIndex];
                        break;
                    case 7:
                        file = modSelect.files[comboBoxMod6.SelectedIndex];
                        break;
                    case 8:
                        file = modSelect.files[comboBoxMod7.SelectedIndex];
                        break;
                    case 9:
                        file = modSelect.files[comboBoxMod8.SelectedIndex];
                        break;
                    case 10:
                        file = modSelect.files[comboBoxMod9.SelectedIndex];
                        break;                   
                    case 11:
                        file = modSelect.files[comboBoxMod10.SelectedIndex];
                        break;
                    case 12:
                        file = modSelect.files[comboBoxMod11.SelectedIndex];
                        break;
                    case 13:
                        file = modSelect.files[comboBoxMod12.SelectedIndex];
                        break;
                    case 14:
                        file = modSelect.files[comboBoxMod13.SelectedIndex];
                        break;
                    case 15:
                        file = modSelect.files[comboBoxMod14.SelectedIndex];
                        break;
                    case 16:
                        file = modSelect.files[comboBoxMod15.SelectedIndex];
                        break;
                    case 17:
                        file = modSelect.files[comboBoxMod16.SelectedIndex];
                        break;
                    case 18:
                        file = modSelect.files[comboBoxMod17.SelectedIndex];
                        break;
                    case 19:
                        file = modSelect.files[comboBoxMod18.SelectedIndex];
                        break;
                    case 20:
                        file = modSelect.files[comboBoxMod19.SelectedIndex];
                        break;
                    case 21:
                        file = modSelect.files[comboBoxMod20.SelectedIndex];
                        break;
                    case 22:
                        file = modSelect.files[comboBoxMod21.SelectedIndex];
                        break;
                    case 23:
                        file = modSelect.files[comboBoxMod22.SelectedIndex];
                        break;
                    case 24:
                        file = modSelect.files[comboBoxMod23.SelectedIndex];
                        break;
                    case 25:
                        file = modSelect.files[comboBoxMod24.SelectedIndex];
                        break;
                    case 26:
                        file = modSelect.files[comboBoxMod25.SelectedIndex];
                        break;
                    case 27:
                        file = modSelect.files[comboBoxMod26.SelectedIndex];
                        break;
                    case 28:
                        file = modSelect.files[comboBoxMod27.SelectedIndex];
                        break;
                    case 29:
                        file = modSelect.files[comboBoxMod28.SelectedIndex];
                        break;
                    case 30:
                        file = modSelect.files[comboBoxMod29.SelectedIndex];
                        break;
                    case 31:
                        file = modSelect.files[comboBoxMod30.SelectedIndex];
                        break;
                    case 32:
                        file = modSelect.files[comboBoxMod31.SelectedIndex];
                        break;
                    case 33:
                        file = modSelect.files[comboBoxMod32.SelectedIndex];
                        break;
                    case 34:
                        file = modSelect.files[comboBoxMod33.SelectedIndex];
                        break;
                    case 35:
                        file = modSelect.files[comboBoxMod34.SelectedIndex];
                        break;
                    case 36:
                        file = modSelect.files[comboBoxMod35.SelectedIndex];
                        break;
                    case 37:
                        file = modSelect.files[comboBoxMod36.SelectedIndex];
                        break;
                    case 38:
                        file = modSelect.files[comboBoxMod37.SelectedIndex];
                        break;
                    case 39:
                        file = modSelect.files[comboBoxMod38.SelectedIndex];
                        break;
                    case 40:
                        file = modSelect.files[comboBoxMod39.SelectedIndex];
                        break;
                    case 41:
                        file = modSelect.files[comboBoxMod40.SelectedIndex];
                        break;
                    case 42:
                        file = modSelect.files[comboBoxMod41.SelectedIndex];
                        break;
                    case 43:
                        file = modSelect.files[comboBoxMod42.SelectedIndex];
                        break;
                    case 44:
                        file = modSelect.files[comboBoxMod43.SelectedIndex];
                        break;
                    case 45:
                        file = modSelect.files[comboBoxMod44.SelectedIndex];
                        break;
                    case 46:
                        file = modSelect.files[comboBoxMod45.SelectedIndex];
                        break;
                    case 47:
                        file = modSelect.files[comboBoxMod46.SelectedIndex];
                        break;
                    case 48:
                        file = modSelect.files[comboBoxMod47.SelectedIndex];
                        break;
                    case 49:
                        file = modSelect.files[comboBoxMod48.SelectedIndex];
                        break;
                    case 50:
                        file = modSelect.files[comboBoxMod49.SelectedIndex];
                        break;
                    case 51:
                        file = modSelect.files[comboBoxMod50.SelectedIndex];
                        break;
                    case 52:
                        file = modSelect.files[comboBoxMod51.SelectedIndex];
                        break;
                    case 53:
                        file = modSelect.files[comboBoxMod52.SelectedIndex];
                        break;
                    case 54:
                        file = modSelect.files[comboBoxMod53.SelectedIndex];
                        break;
                    case 55:
                        file = modSelect.files[comboBoxMod54.SelectedIndex];
                        break;
                    case 56:
                        file = modSelect.files[comboBoxMod55.SelectedIndex];
                        break;
                    case 57:
                        file = modSelect.files[comboBoxMod56.SelectedIndex];
                        break;
                    case 58:
                        file = modSelect.files[comboBoxMod57.SelectedIndex];
                        break;
                    case 59:
                        file = modSelect.files[comboBoxMod58.SelectedIndex];
                        break;
                    case 60:
                        file = modSelect.files[comboBoxMod59.SelectedIndex];
                        break;

                }
                selectedFileMods.Add(file);
            }

            buttonNormal.Visible = false;
            buttonSTART.Visible = false;
            checkBoxOptionRepack.Visible = labelOptionRepack.Visible = false;
            checkBoxOption2kLimit.Visible = labelOption2kLimit.Visible = false;
            checkBoxOptionReshade.Visible = labelOptionReshade.Visible = false;
            checkBoxOptionBikInst.Visible = labelOptionBikinst.Visible = false;
            checkBoxOptionBlackCrush.Visible = labelOptionBlackCrush.Visible = false;
            labelOptions.Visible = false;
            if (meuitmMode)
                customLabelDesc.Text = "Installing MEUITM";
            else
                customLabelDesc.Text = "Installing for Mass Effect" + gameId;
            comboBoxMod0.Visible = comboBoxMod1.Visible = comboBoxMod2.Visible = comboBoxMod3.Visible = false;
            comboBoxMod4.Visible = comboBoxMod5.Visible = comboBoxMod6.Visible = comboBoxMod7.Visible = false;
            comboBoxMod8.Visible = comboBoxMod9.Visible = comboBoxMod10.Visible = comboBoxMod11.Visible = false;
            comboBoxMod12.Visible = comboBoxMod13.Visible = comboBoxMod14.Visible = comboBoxMod15.Visible = false;
            comboBoxMod16.Visible = comboBoxMod17.Visible = comboBoxMod18.Visible = comboBoxMod19.Visible = false;
            comboBoxMod20.Visible = comboBoxMod21.Visible = comboBoxMod22.Visible = comboBoxMod23.Visible = false;
            comboBoxMod24.Visible = comboBoxMod25.Visible = comboBoxMod26.Visible = comboBoxMod27.Visible = false;
            comboBoxMod28.Visible = comboBoxMod29.Visible = comboBoxMod30.Visible = comboBoxMod31.Visible = false;
            comboBoxMod32.Visible = comboBoxMod33.Visible = comboBoxMod34.Visible = comboBoxMod35.Visible = false;
            comboBoxMod36.Visible = comboBoxMod37.Visible = comboBoxMod38.Visible = comboBoxMod39.Visible = false;
            comboBoxMod40.Visible = comboBoxMod41.Visible = comboBoxMod42.Visible = comboBoxMod43.Visible = false;
            comboBoxMod44.Visible = comboBoxMod45.Visible = comboBoxMod46.Visible = comboBoxMod47.Visible = false;
            comboBoxMod48.Visible = comboBoxMod49.Visible = comboBoxMod50.Visible = comboBoxMod51.Visible = false;
            comboBoxMod52.Visible = comboBoxMod53.Visible = comboBoxMod54.Visible = comboBoxMod55.Visible = false;
            comboBoxMod56.Visible = comboBoxMod57.Visible = comboBoxMod58.Visible = comboBoxMod59.Visible = false;
            labelModsSelection.Visible = false;
            customLabelDesc.Visible = true;
            customLabelFinalStatus.Visible = true;
            customLabelCurrentStatus.Visible = true;

            if (!PreInstallCheck())
            {
                customLabelDesc.Visible = false;
                customLabelFinalStatus.Visible = false;
                customLabelCurrentStatus.Visible = false;

                return;
            }
            for (int i = 0; i < selectedFileMods.Count; i++)
            {
                allMemMods.Remove(selectedFileMods[i]);
            }
            for (int i = 0; i < memFiles.Count; i++)
            {
                string file = Path.GetFileName(memFiles[i]).ToLowerInvariant();
                if (allMemMods.Contains(file))
                    memFiles.RemoveAt(i--);
            }

            customLabelFinalStatus.Text = "";
            customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.White);

            errors = "";
            log = "";
            Misc.startTimer();

            if (!updateMode && gameId == 3 && unpackDLC)
            {
                log += "Unpacking DLCs started..." + Environment.NewLine;
                customLabelFinalStatus.Text = "Stage " + stage++ + " of " + totalStages;
                ME3DLC.unpackAllDLC(null, this, false);
                gameData.getPackages(true, true);
                log += "Unpacking DLCs finished" + Environment.NewLine + Environment.NewLine;
            }

            if (checkBoxOptionRepack.Checked)
            {
                pkgsToRepack = new List<string>();
                for (int i = 0; i < GameData.packageFiles.Count; i++)
                {
                    pkgsToRepack.Add(GameData.packageFiles[i]);
                }
                if (GameData.gameType == MeType.ME1_TYPE)
                    pkgsToRepack.Remove(@"\BioGame\CookedPC\testVolumeLight_VFX.upk");
                if (GameData.gameType == MeType.ME2_TYPE)
                    pkgsToRepack.Remove(@"\BioGame\CookedPC\BIOC_Materials.pcc");
            }

            if (GameData.gameType != MeType.ME1_TYPE)
                gameData.getTfcTextures();

            if (!updateMode)
            {
                if (Directory.Exists(GameData.DLCData))
                {
                    List<string> dirs = Directory.EnumerateDirectories(GameData.DLCData).ToList();
                    log += "Detected folowing folders in DLC path:" + Environment.NewLine;
                    for (int dl = 0; dl < dirs.Count; dl++)
                    {
                        log += Path.GetFileName(dirs[dl]) + Environment.NewLine;
                    }
                }
                else
                {
                    log += "Not detected folders in DLC path" + Environment.NewLine;
                }
                log += Environment.NewLine;

                pkgsToMarker = new List<string>();
                for (int i = 0; i < GameData.packageFiles.Count; i++)
                {
                    pkgsToMarker.Add(GameData.packageFiles[i]);
                }
                if (GameData.gameType == MeType.ME1_TYPE)
                    pkgsToMarker.Remove(@"\BioGame\CookedPC\testVolumeLight_VFX.upk");
                if (GameData.gameType == MeType.ME2_TYPE)
                    pkgsToMarker.Remove(@"\BioGame\CookedPC\BIOC_Materials.pcc");

                customLabelFinalStatus.Text = "Stage " + stage++ + " of " + totalStages;

                log += "Scan textures started..." + Environment.NewLine;
                errors += treeScan.PrepareListOfTextures(GameData.gameType, null, null, this, ref log, false);
                textures = treeScan.treeScan;
                log += "Scan textures finished" + Environment.NewLine + Environment.NewLine;
            }

            customLabelFinalStatus.Text = "Stage " + stage++ + " of " + totalStages;
            log += "Process textures started..." + Environment.NewLine;
            applyModules();
            log += "Process textures finished" + Environment.NewLine + Environment.NewLine;


            if (!updateMode)
            {
                customLabelFinalStatus.Text = "Stage " + stage++ + " of " + totalStages;
                log += "Remove mipmaps started..." + Environment.NewLine;
                if (gameId == 1)
                {
                    errors += mipMaps.removeMipMapsME1(1, textures, null, this, false);
                    errors += mipMaps.removeMipMapsME1(2, textures, null, this, false);
                }
                else
                {
                    errors += mipMaps.removeMipMapsME2ME3(textures, null, this, false, checkBoxOptionRepack.Checked);
                }
                log += "Remove mipmaps finished" + Environment.NewLine + Environment.NewLine;
            }

            if (checkBoxOptionRepack.Checked)
            {
                customLabelFinalStatus.Text = "Stage " + stage++ + " of " + totalStages;
                log += "Repack started..." + Environment.NewLine;
                if (GameData.gameType == MeType.ME2_TYPE)
                    pkgsToRepack.Remove(@"\BioGame\CookedPC\BIOC_Materials.pcc");
                for (int i = 0; i < pkgsToRepack.Count; i++)
                {
                    updateProgressStatus("Repack game files " + ((i + 1) * 100 / pkgsToRepack.Count) + "%");
                    try
                    {
                        Package package = new Package(GameData.GamePath + pkgsToRepack[i], true);
                        if (!package.compressed || package.compressed && package.compressionType != Package.CompressionType.Zlib)
                        {
                            package.Dispose();
                            package = new Package(GameData.GamePath + pkgsToRepack[i]);
                            if (package.SaveToFile(true, false, !updateMode))
                            {
                                if (pkgsToMarker != null)
                                    pkgsToMarker.Remove(GameData.RelativeGameData(package.packagePath));
                            }
                        }
                        package.Dispose();
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.Contains("Problem with PCC file header:"))
                            continue;
                    }

                }
                log += "Repack finished" + Environment.NewLine + Environment.NewLine;
            }

            if (!updateMode)
            {
                customLabelFinalStatus.Text = "Stage " + stage++ + " of " + totalStages;
                log += "Adding markers started..." + Environment.NewLine;
                AddMarkers((MeType)gameId);
                log += "Repack finished" + Environment.NewLine + Environment.NewLine;
            }

            if (!applyModTag(gameId, MeuitmVer, 0))
                errors += "Failed applying stamp for installation!\n";

            if (GameData.gameType == MeType.ME3_TYPE)
                TOCBinFile.UpdateAllTOCBinFiles();

            log += "Updating GFX settings started..." + Environment.NewLine;
            string path = gameData.EngineConfigIniPath;
            bool exist = File.Exists(path);
            if (!exist)
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            ConfIni engineConf = new ConfIni(path);
            LODSettings.updateLOD((MeType)gameId, engineConf, checkBoxOption2kLimit.Checked);
            LODSettings.updateGFXSettings((MeType)gameId, engineConf, softShadowsModPath != "", meuitmMode);
            log += "Updating GFX settings finished" + Environment.NewLine + Environment.NewLine;


            if (gameId == 1 && softShadowsModPath != "")
            {
                if (installSoftShadowsMod(gameData, softShadowsModPath))
                    log += "Soft Shadows mod installed." + Environment.NewLine + Environment.NewLine;
                else
                {
                    log += "Soft Shadows mod failed to install!" + Environment.NewLine + Environment.NewLine;
                    errors += "Soft Shadows mod failed to install!\n";
                }
            }

            if (gameId == 1 && splashBitmapPath != "")
            {
                if (installSplashScreen(splashBitmapPath))
                    log += "Splash screen mod installed." + Environment.NewLine + Environment.NewLine;
                else
                {
                    log += "Splash mod failed to install!" + Environment.NewLine + Environment.NewLine;
                    errors += "Splash mod failed to install!\n";
                }
            }

            if (gameId == 1 && splashDemiurge != "" && checkBoxOptionBikInst.Checked)
            {
                if (installSplashVideo(splashDemiurge))
                    log += "Splash video mod installed." + Environment.NewLine + Environment.NewLine;
                else
                {
                    log += "Splash video mod failed to install!" + Environment.NewLine + Environment.NewLine;
                    errors += "Splash video mod failed to install!\n";
                }
            }

            if (gameId == 2 && splashEA != "" && checkBoxOptionBikInst.Checked)
            {
                if (installSplashVideo2(splashEA))
                    log += "Splash video mod installed." + Environment.NewLine + Environment.NewLine;
                else
                {
                    log += "Splash video mod failed to install!" + Environment.NewLine + Environment.NewLine;
                    errors += "Splash video mod failed to install!\n";
                }
            }

            if (gameId == 2 && BlackCrushRemoval != "" && checkBoxOptionBlackCrush.Checked)
            {
                if (installBlackCrushFix(BlackCrushRemoval))
                    log += "Black Crush Removal mod installed." + Environment.NewLine + Environment.NewLine;
                else
                {
                    log += "Black crush Removal mod failed to install!" + Environment.NewLine + Environment.NewLine;
                    errors += "Black crush Removal mod failed to install!\n";
                }
            }

            if (gameId == 1 && reshadePath != "" && checkBoxOptionReshade.Checked)
            {
                if (installReshadePath(reshadePath))
                    log += "ReShade installed." + Environment.NewLine + Environment.NewLine;
                else
                {
                    log += "ReShade failed to install!" + Environment.NewLine + Environment.NewLine;
                    errors += "ReShade failed to install!\n";
                }
            }

            if (gameId == 2 && reshadePath != "" && checkBoxOptionReshade.Checked)
            {
                if (installReshadePath(reshadePath))
                    log += "ReShade installed." + Environment.NewLine + Environment.NewLine;
                else
                {
                    log += "ReShade failed to install!" + Environment.NewLine + Environment.NewLine;
                    errors += "ReShade failed to install!\n";
                }
            }

            var time = Misc.stopTimer();
            log += "Installation finished. Process total time: " + Misc.getTimerFormat(time) + Environment.NewLine;
            customLabelFinalStatus.Text = "Installation finished.";
            customLabelCurrentStatus.Text = "";
            customLabelCurrentStatus.ForeColor = Color.FromKnownColor(KnownColor.White);
            customLabelDesc.Text = "";
            buttonNormal.Visible = true;

            log += "==========================================" + Environment.NewLine;
            log += "LOD settings:" + Environment.NewLine;
            LODSettings.readLOD((MeType)gameId, engineConf, ref log);
            log += "==========================================" + Environment.NewLine;

            string filename = "install-log.txt";
            if (File.Exists(filename))
                File.Delete(filename);
            using (FileStream fs = new FileStream(filename, FileMode.CreateNew))
            {
                fs.WriteStringASCII(log);
            }

            filename = "errors-install.txt";
            if (File.Exists(filename))
                File.Delete(filename);
            if (errors != "")
            {
                using (FileStream fs = new FileStream(filename, FileMode.CreateNew))
                {
                    fs.WriteStringASCII(errors);
                }
                customLabelFinalStatus.Text = "WARNING: Some errors have occured!";
                customLabelFinalStatus.ForeColor = Color.FromKnownColor(KnownColor.Yellow);
                Process.Start(filename);
            }
        }

        public void updateStatusPrepare(string text)
        {
            customLabelCurrentStatus.Text = text;
            Application.DoEvents();
        }

        public void updateProgressStatus(string text)
        {
            customLabelCurrentStatus.Text = text;
            Application.DoEvents();
        }

        private void buttonNormal_Click(object sender, EventArgs e)
        {
            exitToModder = true;
            Close();
        }

        private void buttonMute_Click(object sender, EventArgs e)
        {
            if (musicPlayer != null)
            {
                buttonMute.Visible = true;
                if (mute)
                {
                    buttonMute.ImageIndex = 0;
                    mute = false;
                    musicPlayer.PlayLooping();
                }
                else
                {
                    buttonMute.ImageIndex = 1;
                    mute = true;
                    musicPlayer.Stop();
                }
            }
        }

        private void Installer_Load(object sender, EventArgs e)
        {

        }
    }
}
