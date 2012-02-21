/*
Copyright (C) 2004 SensePost Research

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using SensePost.Wikto.com.google.api;
using System.Xml;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Org.Mentalis.Security.Ssl;
using Org.Mentalis.Security.Certificates;

namespace SensePost.Wikto
{
    #region Thread Delegates
    public delegate String DlgControlTextGet(System.Windows.Forms.Control myctl);
    public delegate void DlgControlTextSet(System.Windows.Forms.Control myctl, String s);
    public delegate void DlgControlTextApp(System.Windows.Forms.TextBox myctl, String s);
    public delegate void DlgControlDisable(System.Windows.Forms.Control myctl, bool b);
    public delegate void DlgControlListCls(System.Windows.Forms.ListBox myctl);
    public delegate void DlgControlListAdd(System.Windows.Forms.ListBox myctl, String s);
    public delegate void DlgControlListUnq(System.Windows.Forms.ListBox myctl, String s);
    public delegate void DlgControlListSel(System.Windows.Forms.ListBox myctl, int i);
    public delegate void DlgControlViewSel(System.Windows.Forms.ListView myctl, int i);
    public delegate void DlgControlViewAdd(System.Windows.Forms.ListView myctl, ListViewItem s);
    public delegate void DlgControlViewCls(System.Windows.Forms.ListView myctl);
    public delegate void DlgControlViewUnq(System.Windows.Forms.ListView myctl, ListViewItem s);
    public delegate String DlgControlListArr(System.Windows.Forms.ListBox myctl);
    public delegate String DlgControlViewArr(System.Windows.Forms.ListView myctl, int ImageIndex);
    public delegate void DlgControlProgMax(System.Windows.Forms.ProgressBar myctl, int i);
    public delegate void DlgControlProgVal(System.Windows.Forms.ProgressBar myctl, int i);
    public delegate void DlgControlProgInc(System.Windows.Forms.ProgressBar myctl, int i);
    public delegate bool DlgControlChckGet(System.Windows.Forms.CheckBox myctl);
    public delegate void DlgControlLViewSetActive(System.Windows.Forms.ListView myctl, int i);
    public delegate void DlgControlSetReadonly(System.Windows.Forms.TextBox myctl, bool b);
    public delegate String DlgControlNupdownGet(System.Windows.Forms.NumericUpDown myctl);
    public delegate int DlgControlProgMaxGet(System.Windows.Forms.ProgressBar myctl);
    #endregion

    #region frm_Wikto
    public class frm_Wikto : System.Windows.Forms.Form
    {

        #region Delegate Declarations
        public DlgControlTextGet dlgControlTextGet;
        public DlgControlTextSet dlgControlTextSet;
        public DlgControlTextApp dlgControlTextApp;
        public DlgControlDisable dlgControlDisable;
        public DlgControlListCls dlgControlListCls;
        public DlgControlListAdd dlgControlListAdd;
        public DlgControlListUnq dlgControlListUnq;
        public DlgControlListArr dlgControlListArr;
        public DlgControlListSel dlgControlListSel;
        public DlgControlViewSel dlgControlViewSel;
        public DlgControlViewAdd dlgControlViewAdd;
        public DlgControlViewCls dlgControlViewCls;
        public DlgControlViewUnq dlgControlViewUnq;
        public DlgControlViewArr dlgControlViewArr;
        public DlgControlProgMax dlgControlProgMax;
        public DlgControlProgVal dlgControlProgVal;
        public DlgControlProgInc dlgControlProgInc;
        public DlgControlChckGet dlgControlChckGet;
        public DlgControlLViewSetActive dlgControlListView;
        public DlgControlSetReadonly dlgControlerSetReadonly;
        public DlgControlNupdownGet dlgControllNupGet;
        public DlgControlProgMaxGet dlgControlProgMaxGet;
        public delegate void DelegateToSpider();
        public delegate void DelegateToLongTask();
        public delegate void DelegateToGoogleTask();
        public delegate void DelegateToNiktoTask();
        public delegate void DelegateToGoogleHackTask();
        #endregion

        #region Public Class Variables
        public static bool stoppit=false;
		public static bool stopdir=false;
		public static bool stopnikto=false;
		public static bool skipDirectories=false;
		public static bool stoppitgoogle=false;
		public static bool stopGH=false;
		public static bool stopscroll=false;
		public static bool GHstopscroll=false;
        public static bool pauseBackEnd = false;
        public static bool pauseWikto = false;
        public bool bl_ShowStart = true;
        public bool bl_ShowStartWiz = true;
        public Hashtable IWWorking = new Hashtable();
        public Hashtable IWDiscovered = new Hashtable();
        public Hashtable NiktoOptimised = new Hashtable();
        public ArrayList IWResults = new ArrayList();
        public ArrayList TheGoogleHackDatabase = new ArrayList();
        #endregion

        #region Public Class Structures
        public struct niktoRequests
        {
            public string type;
            public string request;
            public string description;
            public string trigger;
            public string method;
            public string sensepostreq;
        }

        public struct niktoFP
        {
            public string URLlocation;
            public string HTTPblob;
            public string filetype;
            public string method;
        }

        public struct BackEndMining
        {
            public string location;
            public bool indexable;
            public string responsecode;
            public string ai;
        }

        public struct niktoRes
        {
            public niktoRequests theNiktoRequest;
            public double fuzzValue;
            public string rawrequest;
            public string rawresult;
            public string theoriginalrequest;
        }

        public struct GoogleHackDB_type
        {
            public string signatureReferenceNumber;
            public string categoryref;
            public string category;
            public string querytype;
            public string querystring;
            public string shortDescription;
            public string textualDescription;
            public string cveNumber;
            public string cveLocation;
        }
        #endregion

        #region Private Class Variables
        private String BFDirErr = "";
        private String BFFilErr = "";
        private decimal BFNupVal;
        private int previousTime;
        private bool WeirdDummy = false;
        #endregion

        #region Class Variables
        string prgVersion = "2.1";
        int numberofNiktorequests = 0;
        niktoRequests[] niktoRequest = new niktoRequests[20000];
        niktoFP[] nikto_FP = new niktoFP[200000];
        niktoFP[] backend_FP = new niktoFP[200000];
        BackEndMining[] backend_dirresults = new BackEndMining[200000];
        BackEndMining[] backend_filresults = new BackEndMining[200000];
        int globalFP = 0;
        int globalFPb = 0;
        niktoRes[] nikto_result = new niktoRes[200000];
        int niktoResultCounter = 0;
        decimal NiktoFuzzNValueNow = 0;
        GoogleHackDB_type[] GoogleHack = new GoogleHackDB_type[5000];
        int numberofGoogleHacks = 0;
        Spider spd;
        double maxBackEndAI = 0;
        double minBackEndAI = 100;
        bool isitIndexable = false;
        bool isFirstTimeRun = true;
        #endregion

        #region Widgets
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.OpenFileDialog fdlLoadBackEndDirs;
		private System.Windows.Forms.OpenFileDialog fdlLoadBackEndFiles;
		private System.Windows.Forms.OpenFileDialog fdlLoadBackEndExt;
		private System.Windows.Forms.SaveFileDialog fdlExportBackEnd;
        private System.Windows.Forms.OpenFileDialog fdlLoadNiktoDB;
        private System.Windows.Forms.SaveFileDialog fdlExportWikto;
		private System.Windows.Forms.OpenFileDialog fdlGoogleHacksOpenDB;
        private System.Windows.Forms.ContextMenu cntxtGoogleHacks;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private TabPage pnl_configleft;
        private TabPage GoogleHacks;
        private Panel pnl_GHMain;
        private TableLayoutPanel tpnl_HMain;
        private TableLayoutPanel tpnl_GHManQuery;
        private Panel pnl_GHMQ1;
        private TextBox txtGoogleHackOnceOff;
        private Panel pnl_GHMQ2;
        private DotNetSkin.SkinControls.SkinButton btn_GHManualQuery;
        private Panel pbl_GoogleHackDb;
        private ListBox lstGoogleHack;
        private Panel pnl_GHDesc;
        private TextBox txtGoogleHackDesc;
        private TableLayoutPanel tpnl_GHResults;
        private TableLayoutPanel tpnl_GHResultsTop;
        private Panel pnl_GHRes1;
        private Label label4;
        private Panel pnl_GHRes2;
        private DotNetSkin.SkinControls.SkinButtonRed btn_GHClearResults;
        private Panel pnl_GHRes3;
        private ListBox lstGoogleHackResults;
        private Panel panel6;
        private Label lblGoogleHackEst;
        private Label lblGoogleHackPage;
        private DotNetSkin.SkinControls.SkinButton btn_GHLoadDatabase;
        private Label label1;
        private Label lblGoogleHackStatus;
        private ProgressBar prgGHQuick;
        private ProgressBar prgsGoogleHackAll;
        private TextBox txtGoogleHackTarget;
        private PictureBox pictureBox7;
        private DotNetSkin.SkinControls.SkinButtonRed btn_GHQuit;
        private DotNetSkin.SkinControls.SkinButtonYellow btn_GHStop;
        private DotNetSkin.SkinControls.SkinButtonGreen btn_GHStart;
        private Label label21;
        private Label label24;
        private TabPage BackEndMiner;
        private Panel pnl_BackEndMain;
        private TableLayoutPanel tpnl_BackendMain;
        private Panel pnl_BackEndTop;
        private TableLayoutPanel tpnl_BackEndTop;
        private Panel pnl_BETopLeft1;
        private Label label13;
        private DotNetSkin.SkinControls.SkinButton btn_BEImportInDirG;
        private DotNetSkin.SkinControls.SkinButton btn_BEInDirImportM;
        private DotNetSkin.SkinControls.SkinButtonRed btn_BEInDirClear;
        private Panel pbl_TopLeft3;
        private RichTextBox txtInDirs;
        private Panel pnl_BETopMid1;
        private Label label15;
        private DotNetSkin.SkinControls.SkinButtonRed btn_BEInFileClear;
        private Panel pnl_BETopMid3;
        private RichTextBox txtInFiles;
        private Panel pnl_BETopRight1;
        private Label label38;
        private DotNetSkin.SkinControls.SkinButtonRed btn_BEInExtClear;
        private Panel pnl_BETopRight3;
        private RichTextBox txtInFileTypes;
        private Panel pnl_BackEndBottom;
        private TableLayoutPanel tpnl_BEBottom1;
        private TableLayoutPanel tpnl_BEBottom2;
        private Panel pnl_BEBottomLeft1;
        private Label label50;
        private DotNetSkin.SkinControls.SkinButtonRed btn_BEOutDirClear;
        private Panel pnl_BEBottomLeft3;
        private TableLayoutPanel tpnl_BEBottomRight;
        private Panel pnl_BEBottomRight1;
        private Label label59;
        private DotNetSkin.SkinControls.SkinButtonRed btn_BEOutIndexClear;
        private Panel pnl_BEBottomRight3;
        private TableLayoutPanel tpnl_BEFiles;
        private Panel pnl_BEFile1;
        private Label label52;
        private DotNetSkin.SkinControls.SkinButtonRed btn_BEOutFileClear;
        private Panel pnl_BEFile3;
        private Panel panel4;
        private DotNetSkin.SkinControls.SkinCheckBox chkPreserve;
        private DotNetSkin.SkinControls.SkinButtonYellow btn_BESkiptoDirs;
        private DotNetSkin.SkinControls.SkinButtonRed btn_BEQuit;
        private DotNetSkin.SkinControls.SkinButtonYellow btn_BEStop;
        private DotNetSkin.SkinControls.SkinButtonGreen btn_BEStart;
        private DotNetSkin.SkinControls.SkinButtonYellow btn_BESkiptoFiles;
        private DotNetSkin.SkinControls.SkinButtonGreen btn_BackEndExport;
        private GroupBox groupBox10;
        private DotNetSkin.SkinControls.SkinButtonRed btn_BEClearDB;
        private DotNetSkin.SkinControls.SkinCheckBox chkBackEndAI;
        private NumericUpDown NUPDOWNBackEnd;
        private TextBox txtErrorCodeFile;
        private TextBox txtErrorCodeDir;
        private Panel panel2;
        private Label label11;
        private Label label14;
        private GroupBox groupBox11;
        private DotNetSkin.SkinControls.SkinButton btn_BEUpdateFromSP;
        private ComboBox cmbBackEndUpdate;
        private DotNetSkin.SkinControls.SkinButton btn_BELoadExts;
        private DotNetSkin.SkinControls.SkinButton btn_BELoadFiles;
        private GroupBox groupBox12;
        private Label label9;
        private Label label8;
        private TextBox txtIPPort;
        private DotNetSkin.SkinControls.SkinCheckBox chkBackEnduseSSLport;
        private TextBox txtIPNumber;
        private Label label56;
        private PictureBox pictureBox3;
        private Label label58;
        private ProgressBar prgsFiles;
        private ProgressBar prgsDirs;
        private Label lblStatus;
        private TabPage NiktoIsh;
        private Panel pnl_WiktoMain;
        private TableLayoutPanel tpnl_WiktoMain;
        private TableLayoutPanel tpnl_WiktoT1;
        private Panel pnl_WiktoTL1;
        private Label label5;
        private DotNetSkin.SkinControls.SkinButton btn_WiktoImportBackEnd;
        private DotNetSkin.SkinControls.SkinButton btn_WiktoImportGoogle;
        private DotNetSkin.SkinControls.SkinButton btn_WiktoImportMirror;
        private DotNetSkin.SkinControls.SkinButtonRed btn_WiktoClearCGI;
        private Panel pnl_WiktoTL3;
        private ListBox lst_NiktoCGI;
        private TableLayoutPanel tpnl_WiktoT2;
        private Panel pnl_WiktoTM1;
        private Label label12;
        private Panel pnl_WiktoTM2;
        private ListView lvw_NiktoDb;
        private ColumnHeader col_desc;
        private ColumnHeader col_target;
        private TableLayoutPanel tpnl_WiktoT3;
        private Panel pnl_WiktoTR1;
        private Label label26;
        private Panel pnl_WiktoTR2;
        private ListView lvw_NiktoDesc;
        private ColumnHeader col_ndbd;
        private ColumnHeader col_ndv;
        private TableLayoutPanel tpnl_WiktoB1;
        private Panel panpnl_WiktoBL1;
        private Label label60;
        private Panel panpnl_WiktoBL2;
        private ListView lvw_NiktoResults;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private TableLayoutPanel tpnl_WiktoB2;
        private Panel panpnl_WiktoBM1;
        private Label label61;
        private Panel panpnl_WiktoBM2;
        private TextBox txtNiktoReq;
        private TableLayoutPanel tpnl_WiktoB3;
        private Panel panpnl_WiktoBR1;
        private Label label62;
        private Panel panpnl_WiktoBR2;
        private TextBox txtNiktoRes;
        private Panel panel5;
        private Label label2;
        private ProgressBar prgNik;
        private ProgressBar prgNiktoWork;
        private DotNetSkin.SkinControls.SkinButton btn_NiktoLoad;
        private DotNetSkin.SkinControls.SkinButtonGreen skinButtonGreen2;
        private Label lblNiktoAI;
        private GroupBox groupBox13;
        private DotNetSkin.SkinControls.SkinButtonRed btnClearNiktoAI;
        private DotNetSkin.SkinControls.SkinButton btnNiktoRestFuzz;
        private DotNetSkin.SkinControls.SkinButton btnNiktoShowAll;
        private DotNetSkin.SkinControls.SkinButton btnNiktoFuzzUpdate;
        private NumericUpDown NUPDOWNfuzz;
        private GroupBox groupBox14;
        private DotNetSkin.SkinControls.SkinCheckBox chkuseSSLWikto;
        private TextBox txtNiktoPort;
        private TextBox txtNiktoTarget;
        private Label label28;
        private Label label29;
        private PictureBox pictureBox2;
        private DotNetSkin.SkinControls.SkinButtonRed skinButtonRed1;
        private DotNetSkin.SkinControls.SkinButtonYellow btn_WiktoStop;
        private DotNetSkin.SkinControls.SkinButtonGreen btn_WiktoStart;
        private Label label3;
        private TabPage Googler;
        private TableLayoutPanel tpnlGoogleMain;
        private Panel panel3;
        private Label label57;
        private Label label53;
        private TextBox txtGoogleQuery;
        private Label lblEstimate;
        private Label lblPageNumber;
        private TableLayoutPanel tpnlGoogleDir;
        private TableLayoutPanel tpnlGoogleDirTop;
        private Panel pnlGoogleDirLeft;
        private Label label20;
        private Panel pnlGoogleDirRight;
        private DotNetSkin.SkinControls.SkinButtonRed btnGoogleClearDir;
        private Panel pnlGoogleDirMain;
        private ListBox lstGoogleDir;
        private TableLayoutPanel tpnlGoogleLink;
        private TableLayoutPanel tpnlGoogleLinkTop;
        private Panel pnlGoogleLinkLeft;
        private Label label7;
        private Panel pnlGoogleLinkRight;
        private DotNetSkin.SkinControls.SkinButtonRed btnGoogleClearLink;
        private Panel pnlGoogleLinkMain;
        private ListBox lstGoogleLink;
        private Panel pnlGoogleLeft;
        private Label lblGoogleStatus;
        private PictureBox pictureBox4;
        private DotNetSkin.SkinControls.SkinButtonRed btnGoogleQuit;
        private DotNetSkin.SkinControls.SkinButtonYellow btnStopGoole;
        private DotNetSkin.SkinControls.SkinButtonGreen btnGoogleStart;
        private Label label54;
        private ProgressBar prgGoogle;
        private Label label55;
        private Label label23;
        private TextBox txtWords;
        private TextBox txtGoogleKeyword;
        private Label label22;
        private TextBox txtGoogleTarget;
        private Label lblQuery;
        private TabPage Mirror;
        private TableLayoutPanel tpnlMirror;
        private TableLayoutPanel tpnlMirrorLinks;
        private TableLayoutPanel tpnlMirrorLinkTop;
        private Panel pnlMirrorLinkLeft;
        private Label label34;
        private Panel pnlMrrorLinkRight;
        private DotNetSkin.SkinControls.SkinButtonRed btn_MirrorClearLinks;
        private ListBox lstMirrorLinks;
        private TableLayoutPanel tpnlMirrorDir;
        private ListBox lstMirrorDirs;
        private TableLayoutPanel tpnlMirrorDirTop;
        private Panel pnlMirrorDirLeft;
        private Label label31;
        private Panel pnlMirrorDirRight;
        private DotNetSkin.SkinControls.SkinButtonRed btn_MirrorClearDirs;
        private Panel pnlMirrorLeft;
        private Label lblMirrorStatus;
        private PictureBox pictureBox6;
        private TextBox txtHTTarget;
        private DotNetSkin.SkinControls.SkinButtonRed btn_MirrorQuit;
        private DotNetSkin.SkinControls.SkinButtonYellow btnHTStop;
        private DotNetSkin.SkinControls.SkinButtonGreen btnHTStart;
        private Label label32;
        private ProgressBar prgHT;
        private TabControl tabControl1;
        private DotNetSkin.SkinControls.SkinRadioButton radioHEAD;
        private DotNetSkin.SkinControls.SkinRadioButton radioGET;
        private DotNetSkin.SkinControls.SkinButton btn_BELoadDirs;
        private Panel pnl_ConfigMain;
        private TabControl tab_configMain;
        private TabPage cfg_Proxy;
        private Panel panel1;
        private PictureBox pictureBox5;
        private DotNetSkin.SkinControls.SkinCheckBox chkProxyPresent;
        private TextBox txtProxySettings;
        private Label label17;
        private TabPage cfg_Google;
        private TabPage cfg_Timing;
        private NumericUpDown updownRetryTCP;
        private NumericUpDown updownTimeOutTCP;
        private Label label6;
        private Label label16;
        private TabPage cfg_DB;
        private TextBox txtDBLocationGH;
        private TextBox txtDBlocationNikto;
        private TabPage cfg_Header;
        private TabPage cfg_Update;
        private Label label49;
        private Label label48;
        private Label label46;
        private TextBox txtURLUpdateGHDB;
        private TextBox txtURLUpdateNiktoDB;
        private TextBox txtURLUpdate;
        private Label label25;
        private DotNetSkin.SkinControls.SkinButton btn_CfgLocateNDb;
        private DotNetSkin.SkinControls.SkinButton btn_CfgLocateGDb;
        private Label label27;
        private TextBox txtHeader;
        private DotNetSkin.SkinControls.SkinButtonRed btn_CnfReset;
        private DotNetSkin.SkinControls.SkinButtonYellow btn_CnfSave;
        private DotNetSkin.SkinControls.SkinButtonGreen btn_CnfLoad;
        private DotNetSkin.SkinControls.SkinButtonRed btn_CnfQuit;
        private Label label37;
        private Label lblConfigFileLocation;
        private TabPage cfg_Help;
        private LinkLabel lbl_ResearchMail;
        private Label label41;
        private LinkLabel lbl_WiktoHome;
        private Label label39;
        private Label label47;
        private Label lbl_About;
        private DotNetSkin.SkinControls.SkinButton skinButton4;
        private DotNetSkin.SkinControls.SkinButton skinButton5;
        private DotNetSkin.SkinControls.SkinButton skinButton6;
        private TabPage cfg_Spider;
        private Label label63;
        private NumericUpDown NUPDOWNspider;
        private Label label64;
        private Label label67;
        public TextBox txt_ConfigSpiderExclude;
        private TextBox txt_ConfigSpiderExtension;
        private DotNetSkin.SkinControls.SkinCheckBox chk_SpiderSSL;
        private Label label10;
        private TextBox txt_SpiderPort;
        private DotNetSkin.SkinControls.SkinCheckBox chkOptimizedNikto;
        private DotNetSkin.SkinControls.SkinButton btnBackEndPause;
        private DotNetSkin.SkinControls.SkinButton btnPauseWikto;
        private TabPage cfg_Startup;
        private DotNetSkin.SkinControls.SkinButton btn_ShowNews;
        private DotNetSkin.SkinControls.SkinCheckBox chk_ShowStart;
        private TabPage tab_wizard;
        private DotNetSkin.SkinControls.SkinCheckBox chk_StartWiz;
        private DotNetSkin.SkinControls.SkinButton btn_browseghdb;
        private DotNetSkin.SkinControls.SkinButton btn_browsenikto;
        private Label label35;
        private TextBox txt_content;
        private Label label33;
        private Label label36;
        private NumericUpDown nud_contentsize;
        private TextBox txt_excdirs;
        private Label label40;
        private TextBox txt_idxflags;
        private Label label42;
        private DotNetSkin.SkinControls.SkinCheckBox chk_ignoreidx;
        private GroupBox groupBox1;
        private DotNetSkin.SkinControls.SkinCheckBox chkTimeAnomalies;
        private NumericUpDown nudTimeAnomaly;
        private ListView lstViewDirs;
        private ImageList imageList1;
        private ColumnHeader columnHeader4;
        private ListView lstViewIndexDirs;
        private ColumnHeader columnHeader5;
        private ListView lstViewFiles;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader7;
        private ColumnHeader columnHeader8;
        private ColumnHeader columnHeader9;
        private ListViewColumnSorter lvwColumnSorter;
        private DotNetSkin.SkinControls.SkinButton btn_StartAura;
        private NumericUpDown updownGoogleDepth;
        private Label label18;
        private ColumnHeader columnHeader10;
        private DotNetSkin.SkinControls.SkinButton btn_EditDirs;
        private Label label19;
        private TextBox txtSpudDirectory;
        private DotNetSkin.SkinControls.SkinButton btnSpudLocate;
        private PictureBox pictureBox1;
		private System.ComponentModel.IContainer components;
		#endregion

        #region Class Instantiation
        public frm_Wikto() {
			InitializeComponent();
            #region Delegate Instantiation
            dlgControlTextGet = new DlgControlTextGet(this.DelgateDeclarationTextGet);
            dlgControlTextSet = new DlgControlTextSet(this.DelgateDeclarationTextSet);
            dlgControlTextApp = new DlgControlTextApp(this.DelgateDeclarationTextApp);
            dlgControlDisable = new DlgControlDisable(this.DelgateDeclarationDisable);
            dlgControlListCls = new DlgControlListCls(this.DelgateDeclarationListCls);
            dlgControlListAdd = new DlgControlListAdd(this.DelgateDeclarationListAdd);
            dlgControlListUnq = new DlgControlListUnq(this.DelgateDeclarationListUnq);
            dlgControlListArr = new DlgControlListArr(this.DelgateDeclarationListArr);
            dlgControlListSel = new DlgControlListSel(this.DelgateDeclarationListSel);
            dlgControlViewSel = new DlgControlViewSel(this.DelgateDeclarationViewSel);
            dlgControlViewAdd = new DlgControlViewAdd(this.DelgateDeclarationViewAdd);
            dlgControlViewCls = new DlgControlViewCls(this.DelgateDeclarationViewCls);
            dlgControlViewUnq = new DlgControlViewUnq(this.DelgateDeclarationViewUnq);
            dlgControlViewArr = new DlgControlViewArr(this.DelgateDeclarationViewArr);
            dlgControlProgMax = new DlgControlProgMax(this.DelgateDeclarationProgMax);
            dlgControlProgVal = new DlgControlProgVal(this.DelgateDeclarationProgVal);
            dlgControlProgInc = new DlgControlProgInc(this.DelgateDeclarationProgInc);
            dlgControlChckGet = new DlgControlChckGet(this.DelgateDeclarationChckGet);
            dlgControlListView = new DlgControlLViewSetActive(this.DelgateDeclarationLviewSet);
            dlgControlerSetReadonly = new DlgControlSetReadonly(this.DelgateDeclarationReadOnlySet);
            dlgControllNupGet = new DlgControlNupdownGet(this.DelgateDeclarationNupGet);
            dlgControlProgMaxGet = new DlgControlProgMaxGet(this.DelgateDeclarationProgMaxGet);
            #endregion
            string label		= string.Format("{0} {1} v{2}", Application.CompanyName, Application.ProductName, Application.ProductVersion);
			this.Text			= label;
            BFDirErr = txtErrorCodeDir.Text;
            BFFilErr = txtErrorCodeFile.Text;
            BFNupVal = NUPDOWNBackEnd.Value;
            lvwColumnSorter = new ListViewColumnSorter();
            lstViewDirs.ListViewItemSorter = lvwColumnSorter;
            lstViewIndexDirs.ListViewItemSorter = lvwColumnSorter;
            lstViewFiles.ListViewItemSorter = lvwColumnSorter;
            lvw_NiktoResults.ListViewItemSorter = lvwColumnSorter;
		}
        #endregion


        #region Exit and Dispose Object to Clear Up resources...
        protected override void Dispose( bool disposing )
        {
			if( disposing )
            {
				if (components != null)
                {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
        }

        public void Quitter()
        {
            DialogResult dr = MessageBox.Show("Wikto version" + prgVersion + " by SensePost\nwww.sensepost.com\nresearch@sensepost.com\n\n\nAre you sure you want to quit?", "Keep this tool free - support us!",MessageBoxButtons.YesNo);
            if(dr.Equals(DialogResult.Yes))
                Application.Exit();
        }

        #endregion

        #region Windows Form Designer generated code
		private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_Wikto));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.cntxtGoogleHacks = new System.Windows.Forms.ContextMenu();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btn_CfgLocateNDb = new DotNetSkin.SkinControls.SkinButton();
            this.btn_CfgLocateGDb = new DotNetSkin.SkinControls.SkinButton();
            this.txtDBLocationGH = new System.Windows.Forms.TextBox();
            this.txtDBlocationNikto = new System.Windows.Forms.TextBox();
            this.txtHeader = new System.Windows.Forms.TextBox();
            this.chkProxyPresent = new DotNetSkin.SkinControls.SkinCheckBox();
            this.txtProxySettings = new System.Windows.Forms.TextBox();
            this.NUPDOWNspider = new System.Windows.Forms.NumericUpDown();
            this.txt_ConfigSpiderExclude = new System.Windows.Forms.TextBox();
            this.txt_ConfigSpiderExtension = new System.Windows.Forms.TextBox();
            this.updownRetryTCP = new System.Windows.Forms.NumericUpDown();
            this.updownTimeOutTCP = new System.Windows.Forms.NumericUpDown();
            this.txtURLUpdateGHDB = new System.Windows.Forms.TextBox();
            this.txtURLUpdateNiktoDB = new System.Windows.Forms.TextBox();
            this.txtURLUpdate = new System.Windows.Forms.TextBox();
            this.btn_ShowNews = new DotNetSkin.SkinControls.SkinButton();
            this.chk_ShowStart = new DotNetSkin.SkinControls.SkinCheckBox();
            this.btn_CnfQuit = new DotNetSkin.SkinControls.SkinButtonRed();
            this.btn_CnfReset = new DotNetSkin.SkinControls.SkinButtonRed();
            this.btn_CnfSave = new DotNetSkin.SkinControls.SkinButtonYellow();
            this.btn_CnfLoad = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.btn_GHManualQuery = new DotNetSkin.SkinControls.SkinButton();
            this.btn_GHLoadDatabase = new DotNetSkin.SkinControls.SkinButton();
            this.txtGoogleHackTarget = new System.Windows.Forms.TextBox();
            this.btn_GHQuit = new DotNetSkin.SkinControls.SkinButtonRed();
            this.btn_GHStop = new DotNetSkin.SkinControls.SkinButtonYellow();
            this.btn_GHStart = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.btn_BEInDirImportM = new DotNetSkin.SkinControls.SkinButton();
            this.btn_BEImportInDirG = new DotNetSkin.SkinControls.SkinButton();
            this.btnBackEndPause = new DotNetSkin.SkinControls.SkinButton();
            this.chkPreserve = new DotNetSkin.SkinControls.SkinCheckBox();
            this.btn_BESkiptoDirs = new DotNetSkin.SkinControls.SkinButtonYellow();
            this.btn_BEQuit = new DotNetSkin.SkinControls.SkinButtonRed();
            this.btn_BEStop = new DotNetSkin.SkinControls.SkinButtonYellow();
            this.btn_BEStart = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.btn_BESkiptoFiles = new DotNetSkin.SkinControls.SkinButtonYellow();
            this.btn_BackEndExport = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.skinButton4 = new DotNetSkin.SkinControls.SkinButton();
            this.skinButton5 = new DotNetSkin.SkinControls.SkinButton();
            this.skinButton6 = new DotNetSkin.SkinControls.SkinButton();
            this.radioHEAD = new DotNetSkin.SkinControls.SkinRadioButton();
            this.radioGET = new DotNetSkin.SkinControls.SkinRadioButton();
            this.btn_BEClearDB = new DotNetSkin.SkinControls.SkinButtonRed();
            this.chkBackEndAI = new DotNetSkin.SkinControls.SkinCheckBox();
            this.txtErrorCodeDir = new System.Windows.Forms.TextBox();
            this.txtErrorCodeFile = new System.Windows.Forms.TextBox();
            this.btn_BELoadDirs = new DotNetSkin.SkinControls.SkinButton();
            this.btn_BEUpdateFromSP = new DotNetSkin.SkinControls.SkinButton();
            this.cmbBackEndUpdate = new System.Windows.Forms.ComboBox();
            this.btn_BELoadExts = new DotNetSkin.SkinControls.SkinButton();
            this.btn_BELoadFiles = new DotNetSkin.SkinControls.SkinButton();
            this.txtIPPort = new System.Windows.Forms.TextBox();
            this.chkBackEnduseSSLport = new DotNetSkin.SkinControls.SkinCheckBox();
            this.txtIPNumber = new System.Windows.Forms.TextBox();
            this.btn_WiktoImportBackEnd = new DotNetSkin.SkinControls.SkinButton();
            this.btn_WiktoImportMirror = new DotNetSkin.SkinControls.SkinButton();
            this.btn_WiktoImportGoogle = new DotNetSkin.SkinControls.SkinButton();
            this.btn_NiktoLoad = new DotNetSkin.SkinControls.SkinButton();
            this.skinButtonGreen2 = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.btnClearNiktoAI = new DotNetSkin.SkinControls.SkinButtonRed();
            this.btnNiktoRestFuzz = new DotNetSkin.SkinControls.SkinButton();
            this.btnNiktoShowAll = new DotNetSkin.SkinControls.SkinButton();
            this.chkOptimizedNikto = new DotNetSkin.SkinControls.SkinCheckBox();
            this.btnNiktoFuzzUpdate = new DotNetSkin.SkinControls.SkinButton();
            this.NUPDOWNfuzz = new System.Windows.Forms.NumericUpDown();
            this.chkuseSSLWikto = new DotNetSkin.SkinControls.SkinCheckBox();
            this.txtNiktoPort = new System.Windows.Forms.TextBox();
            this.txtNiktoTarget = new System.Windows.Forms.TextBox();
            this.skinButtonRed1 = new DotNetSkin.SkinControls.SkinButtonRed();
            this.btn_WiktoStop = new DotNetSkin.SkinControls.SkinButtonYellow();
            this.btn_WiktoStart = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.btnGoogleQuit = new DotNetSkin.SkinControls.SkinButtonRed();
            this.btnStopGoole = new DotNetSkin.SkinControls.SkinButtonYellow();
            this.btnGoogleStart = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.txtWords = new System.Windows.Forms.TextBox();
            this.txtGoogleKeyword = new System.Windows.Forms.TextBox();
            this.txtGoogleTarget = new System.Windows.Forms.TextBox();
            this.chk_SpiderSSL = new DotNetSkin.SkinControls.SkinCheckBox();
            this.txt_SpiderPort = new System.Windows.Forms.TextBox();
            this.txtHTTarget = new System.Windows.Forms.TextBox();
            this.btn_MirrorQuit = new DotNetSkin.SkinControls.SkinButtonRed();
            this.btnHTStop = new DotNetSkin.SkinControls.SkinButtonYellow();
            this.btnHTStart = new DotNetSkin.SkinControls.SkinButtonGreen();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Mirror = new System.Windows.Forms.TabPage();
            this.tpnlMirror = new System.Windows.Forms.TableLayoutPanel();
            this.tpnlMirrorLinks = new System.Windows.Forms.TableLayoutPanel();
            this.tpnlMirrorLinkTop = new System.Windows.Forms.TableLayoutPanel();
            this.pnlMirrorLinkLeft = new System.Windows.Forms.Panel();
            this.label34 = new System.Windows.Forms.Label();
            this.pnlMrrorLinkRight = new System.Windows.Forms.Panel();
            this.btn_MirrorClearLinks = new DotNetSkin.SkinControls.SkinButtonRed();
            this.lstMirrorLinks = new System.Windows.Forms.ListBox();
            this.tpnlMirrorDir = new System.Windows.Forms.TableLayoutPanel();
            this.lstMirrorDirs = new System.Windows.Forms.ListBox();
            this.tpnlMirrorDirTop = new System.Windows.Forms.TableLayoutPanel();
            this.pnlMirrorDirLeft = new System.Windows.Forms.Panel();
            this.label31 = new System.Windows.Forms.Label();
            this.pnlMirrorDirRight = new System.Windows.Forms.Panel();
            this.btn_MirrorClearDirs = new DotNetSkin.SkinControls.SkinButtonRed();
            this.pnlMirrorLeft = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.lblMirrorStatus = new System.Windows.Forms.Label();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.label32 = new System.Windows.Forms.Label();
            this.prgHT = new System.Windows.Forms.ProgressBar();
            this.Googler = new System.Windows.Forms.TabPage();
            this.tpnlGoogleMain = new System.Windows.Forms.TableLayoutPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label57 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.txtGoogleQuery = new System.Windows.Forms.TextBox();
            this.lblEstimate = new System.Windows.Forms.Label();
            this.lblPageNumber = new System.Windows.Forms.Label();
            this.tpnlGoogleDir = new System.Windows.Forms.TableLayoutPanel();
            this.tpnlGoogleDirTop = new System.Windows.Forms.TableLayoutPanel();
            this.pnlGoogleDirLeft = new System.Windows.Forms.Panel();
            this.label20 = new System.Windows.Forms.Label();
            this.pnlGoogleDirRight = new System.Windows.Forms.Panel();
            this.btnGoogleClearDir = new DotNetSkin.SkinControls.SkinButtonRed();
            this.pnlGoogleDirMain = new System.Windows.Forms.Panel();
            this.lstGoogleDir = new System.Windows.Forms.ListBox();
            this.tpnlGoogleLink = new System.Windows.Forms.TableLayoutPanel();
            this.tpnlGoogleLinkTop = new System.Windows.Forms.TableLayoutPanel();
            this.pnlGoogleLinkLeft = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.pnlGoogleLinkRight = new System.Windows.Forms.Panel();
            this.btnGoogleClearLink = new DotNetSkin.SkinControls.SkinButtonRed();
            this.pnlGoogleLinkMain = new System.Windows.Forms.Panel();
            this.lstGoogleLink = new System.Windows.Forms.ListBox();
            this.pnlGoogleLeft = new System.Windows.Forms.Panel();
            this.lblGoogleStatus = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.label54 = new System.Windows.Forms.Label();
            this.prgGoogle = new System.Windows.Forms.ProgressBar();
            this.label55 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.lblQuery = new System.Windows.Forms.Label();
            this.BackEndMiner = new System.Windows.Forms.TabPage();
            this.pnl_BackEndMain = new System.Windows.Forms.Panel();
            this.tpnl_BackendMain = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_BackEndTop = new System.Windows.Forms.Panel();
            this.tpnl_BackEndTop = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_BETopLeft1 = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.btn_BEInDirClear = new DotNetSkin.SkinControls.SkinButtonRed();
            this.pbl_TopLeft3 = new System.Windows.Forms.Panel();
            this.txtInDirs = new System.Windows.Forms.RichTextBox();
            this.pnl_BETopMid1 = new System.Windows.Forms.Panel();
            this.btn_BEInFileClear = new DotNetSkin.SkinControls.SkinButtonRed();
            this.label15 = new System.Windows.Forms.Label();
            this.pnl_BETopMid3 = new System.Windows.Forms.Panel();
            this.txtInFiles = new System.Windows.Forms.RichTextBox();
            this.pnl_BETopRight1 = new System.Windows.Forms.Panel();
            this.btn_BEInExtClear = new DotNetSkin.SkinControls.SkinButtonRed();
            this.label38 = new System.Windows.Forms.Label();
            this.pnl_BETopRight3 = new System.Windows.Forms.Panel();
            this.txtInFileTypes = new System.Windows.Forms.RichTextBox();
            this.pnl_BackEndBottom = new System.Windows.Forms.Panel();
            this.tpnl_BEBottom1 = new System.Windows.Forms.TableLayoutPanel();
            this.tpnl_BEBottom2 = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_BEBottomLeft1 = new System.Windows.Forms.Panel();
            this.btn_BEOutDirClear = new DotNetSkin.SkinControls.SkinButtonRed();
            this.label50 = new System.Windows.Forms.Label();
            this.pnl_BEBottomLeft3 = new System.Windows.Forms.Panel();
            this.lstViewDirs = new System.Windows.Forms.ListView();
            this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tpnl_BEBottomRight = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_BEBottomRight1 = new System.Windows.Forms.Panel();
            this.btn_BEOutIndexClear = new DotNetSkin.SkinControls.SkinButtonRed();
            this.label59 = new System.Windows.Forms.Label();
            this.pnl_BEBottomRight3 = new System.Windows.Forms.Panel();
            this.lstViewIndexDirs = new System.Windows.Forms.ListView();
            this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.tpnl_BEFiles = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_BEFile1 = new System.Windows.Forms.Panel();
            this.btn_BEOutFileClear = new DotNetSkin.SkinControls.SkinButtonRed();
            this.label52 = new System.Windows.Forms.Label();
            this.pnl_BEFile3 = new System.Windows.Forms.Panel();
            this.lstViewFiles = new System.Windows.Forms.ListView();
            this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.panel4 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudTimeAnomaly = new System.Windows.Forms.NumericUpDown();
            this.chkTimeAnomalies = new DotNetSkin.SkinControls.SkinCheckBox();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.NUPDOWNBackEnd = new System.Windows.Forms.NumericUpDown();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.groupBox11 = new System.Windows.Forms.GroupBox();
            this.groupBox12 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label58 = new System.Windows.Forms.Label();
            this.prgsFiles = new System.Windows.Forms.ProgressBar();
            this.prgsDirs = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.NiktoIsh = new System.Windows.Forms.TabPage();
            this.pnl_WiktoMain = new System.Windows.Forms.Panel();
            this.tpnl_WiktoMain = new System.Windows.Forms.TableLayoutPanel();
            this.tpnl_WiktoB1 = new System.Windows.Forms.TableLayoutPanel();
            this.panpnl_WiktoBL1 = new System.Windows.Forms.Panel();
            this.label60 = new System.Windows.Forms.Label();
            this.panpnl_WiktoBL2 = new System.Windows.Forms.Panel();
            this.lvw_NiktoResults = new System.Windows.Forms.ListView();
            this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.tpnl_WiktoB2 = new System.Windows.Forms.TableLayoutPanel();
            this.panpnl_WiktoBM1 = new System.Windows.Forms.Panel();
            this.label61 = new System.Windows.Forms.Label();
            this.panpnl_WiktoBM2 = new System.Windows.Forms.Panel();
            this.txtNiktoReq = new System.Windows.Forms.TextBox();
            this.tpnl_WiktoB3 = new System.Windows.Forms.TableLayoutPanel();
            this.panpnl_WiktoBR1 = new System.Windows.Forms.Panel();
            this.label62 = new System.Windows.Forms.Label();
            this.panpnl_WiktoBR2 = new System.Windows.Forms.Panel();
            this.txtNiktoRes = new System.Windows.Forms.TextBox();
            this.tpnl_WiktoT3 = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_WiktoTR1 = new System.Windows.Forms.Panel();
            this.label26 = new System.Windows.Forms.Label();
            this.pnl_WiktoTR2 = new System.Windows.Forms.Panel();
            this.lvw_NiktoDesc = new System.Windows.Forms.ListView();
            this.col_ndbd = new System.Windows.Forms.ColumnHeader();
            this.col_ndv = new System.Windows.Forms.ColumnHeader();
            this.tpnl_WiktoT2 = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_WiktoTM1 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.pnl_WiktoTM2 = new System.Windows.Forms.Panel();
            this.lvw_NiktoDb = new System.Windows.Forms.ListView();
            this.col_desc = new System.Windows.Forms.ColumnHeader();
            this.col_target = new System.Windows.Forms.ColumnHeader();
            this.tpnl_WiktoT1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_WiktoTL1 = new System.Windows.Forms.Panel();
            this.btn_EditDirs = new DotNetSkin.SkinControls.SkinButton();
            this.btn_WiktoClearCGI = new DotNetSkin.SkinControls.SkinButtonRed();
            this.label5 = new System.Windows.Forms.Label();
            this.pnl_WiktoTL3 = new System.Windows.Forms.Panel();
            this.lst_NiktoCGI = new System.Windows.Forms.ListBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnPauseWikto = new DotNetSkin.SkinControls.SkinButton();
            this.label2 = new System.Windows.Forms.Label();
            this.prgNik = new System.Windows.Forms.ProgressBar();
            this.prgNiktoWork = new System.Windows.Forms.ProgressBar();
            this.lblNiktoAI = new System.Windows.Forms.Label();
            this.groupBox13 = new System.Windows.Forms.GroupBox();
            this.groupBox14 = new System.Windows.Forms.GroupBox();
            this.label28 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.GoogleHacks = new System.Windows.Forms.TabPage();
            this.pnl_GHMain = new System.Windows.Forms.Panel();
            this.tpnl_HMain = new System.Windows.Forms.TableLayoutPanel();
            this.tpnl_GHManQuery = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_GHMQ1 = new System.Windows.Forms.Panel();
            this.txtGoogleHackOnceOff = new System.Windows.Forms.TextBox();
            this.pnl_GHMQ2 = new System.Windows.Forms.Panel();
            this.pbl_GoogleHackDb = new System.Windows.Forms.Panel();
            this.lstGoogleHack = new System.Windows.Forms.ListBox();
            this.pnl_GHDesc = new System.Windows.Forms.Panel();
            this.txtGoogleHackDesc = new System.Windows.Forms.TextBox();
            this.tpnl_GHResults = new System.Windows.Forms.TableLayoutPanel();
            this.tpnl_GHResultsTop = new System.Windows.Forms.TableLayoutPanel();
            this.pnl_GHRes1 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.pnl_GHRes2 = new System.Windows.Forms.Panel();
            this.btn_GHClearResults = new DotNetSkin.SkinControls.SkinButtonRed();
            this.pnl_GHRes3 = new System.Windows.Forms.Panel();
            this.lstGoogleHackResults = new System.Windows.Forms.ListBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.lblGoogleHackEst = new System.Windows.Forms.Label();
            this.lblGoogleHackPage = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblGoogleHackStatus = new System.Windows.Forms.Label();
            this.prgGHQuick = new System.Windows.Forms.ProgressBar();
            this.prgsGoogleHackAll = new System.Windows.Forms.ProgressBar();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.pnl_configleft = new System.Windows.Forms.TabPage();
            this.pnl_ConfigMain = new System.Windows.Forms.Panel();
            this.tab_configMain = new System.Windows.Forms.TabControl();
            this.cfg_DB = new System.Windows.Forms.TabPage();
            this.btn_browseghdb = new DotNetSkin.SkinControls.SkinButton();
            this.btn_browsenikto = new DotNetSkin.SkinControls.SkinButton();
            this.label27 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.cfg_Header = new System.Windows.Forms.TabPage();
            this.cfg_Google = new System.Windows.Forms.TabPage();
            this.btnSpudLocate = new DotNetSkin.SkinControls.SkinButton();
            this.txtSpudDirectory = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.btn_StartAura = new DotNetSkin.SkinControls.SkinButton();
            this.updownGoogleDepth = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.cfg_Proxy = new System.Windows.Forms.TabPage();
            this.label17 = new System.Windows.Forms.Label();
            this.cfg_Spider = new System.Windows.Forms.TabPage();
            this.chk_ignoreidx = new DotNetSkin.SkinControls.SkinCheckBox();
            this.txt_idxflags = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.txt_excdirs = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.nud_contentsize = new System.Windows.Forms.NumericUpDown();
            this.label35 = new System.Windows.Forms.Label();
            this.txt_content = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.label63 = new System.Windows.Forms.Label();
            this.label64 = new System.Windows.Forms.Label();
            this.label67 = new System.Windows.Forms.Label();
            this.cfg_Timing = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.cfg_Update = new System.Windows.Forms.TabPage();
            this.label49 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.cfg_Startup = new System.Windows.Forms.TabPage();
            this.chk_StartWiz = new DotNetSkin.SkinControls.SkinCheckBox();
            this.cfg_Help = new System.Windows.Forms.TabPage();
            this.lbl_About = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.lbl_ResearchMail = new System.Windows.Forms.LinkLabel();
            this.label41 = new System.Windows.Forms.Label();
            this.lbl_WiktoHome = new System.Windows.Forms.LinkLabel();
            this.label39 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label37 = new System.Windows.Forms.Label();
            this.lblConfigFileLocation = new System.Windows.Forms.Label();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.tab_wizard = new System.Windows.Forms.TabPage();
            this.fdlLoadBackEndDirs = new System.Windows.Forms.OpenFileDialog();
            this.fdlLoadBackEndFiles = new System.Windows.Forms.OpenFileDialog();
            this.fdlLoadBackEndExt = new System.Windows.Forms.OpenFileDialog();
            this.fdlExportBackEnd = new System.Windows.Forms.SaveFileDialog();
            this.fdlLoadNiktoDB = new System.Windows.Forms.OpenFileDialog();
            this.fdlExportWikto = new System.Windows.Forms.SaveFileDialog();
            this.fdlGoogleHacksOpenDB = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.NUPDOWNspider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownRetryTCP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownTimeOutTCP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUPDOWNfuzz)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.Mirror.SuspendLayout();
            this.tpnlMirror.SuspendLayout();
            this.tpnlMirrorLinks.SuspendLayout();
            this.tpnlMirrorLinkTop.SuspendLayout();
            this.pnlMirrorLinkLeft.SuspendLayout();
            this.pnlMrrorLinkRight.SuspendLayout();
            this.tpnlMirrorDir.SuspendLayout();
            this.tpnlMirrorDirTop.SuspendLayout();
            this.pnlMirrorDirLeft.SuspendLayout();
            this.pnlMirrorDirRight.SuspendLayout();
            this.pnlMirrorLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            this.Googler.SuspendLayout();
            this.tpnlGoogleMain.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tpnlGoogleDir.SuspendLayout();
            this.tpnlGoogleDirTop.SuspendLayout();
            this.pnlGoogleDirLeft.SuspendLayout();
            this.pnlGoogleDirRight.SuspendLayout();
            this.pnlGoogleDirMain.SuspendLayout();
            this.tpnlGoogleLink.SuspendLayout();
            this.tpnlGoogleLinkTop.SuspendLayout();
            this.pnlGoogleLinkLeft.SuspendLayout();
            this.pnlGoogleLinkRight.SuspendLayout();
            this.pnlGoogleLinkMain.SuspendLayout();
            this.pnlGoogleLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.BackEndMiner.SuspendLayout();
            this.pnl_BackEndMain.SuspendLayout();
            this.tpnl_BackendMain.SuspendLayout();
            this.pnl_BackEndTop.SuspendLayout();
            this.tpnl_BackEndTop.SuspendLayout();
            this.pnl_BETopLeft1.SuspendLayout();
            this.pbl_TopLeft3.SuspendLayout();
            this.pnl_BETopMid1.SuspendLayout();
            this.pnl_BETopMid3.SuspendLayout();
            this.pnl_BETopRight1.SuspendLayout();
            this.pnl_BETopRight3.SuspendLayout();
            this.pnl_BackEndBottom.SuspendLayout();
            this.tpnl_BEBottom1.SuspendLayout();
            this.tpnl_BEBottom2.SuspendLayout();
            this.pnl_BEBottomLeft1.SuspendLayout();
            this.pnl_BEBottomLeft3.SuspendLayout();
            this.tpnl_BEBottomRight.SuspendLayout();
            this.pnl_BEBottomRight1.SuspendLayout();
            this.pnl_BEBottomRight3.SuspendLayout();
            this.tpnl_BEFiles.SuspendLayout();
            this.pnl_BEFile1.SuspendLayout();
            this.pnl_BEFile3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeAnomaly)).BeginInit();
            this.groupBox10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUPDOWNBackEnd)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBox11.SuspendLayout();
            this.groupBox12.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.NiktoIsh.SuspendLayout();
            this.pnl_WiktoMain.SuspendLayout();
            this.tpnl_WiktoMain.SuspendLayout();
            this.tpnl_WiktoB1.SuspendLayout();
            this.panpnl_WiktoBL1.SuspendLayout();
            this.panpnl_WiktoBL2.SuspendLayout();
            this.tpnl_WiktoB2.SuspendLayout();
            this.panpnl_WiktoBM1.SuspendLayout();
            this.panpnl_WiktoBM2.SuspendLayout();
            this.tpnl_WiktoB3.SuspendLayout();
            this.panpnl_WiktoBR1.SuspendLayout();
            this.panpnl_WiktoBR2.SuspendLayout();
            this.tpnl_WiktoT3.SuspendLayout();
            this.pnl_WiktoTR1.SuspendLayout();
            this.pnl_WiktoTR2.SuspendLayout();
            this.tpnl_WiktoT2.SuspendLayout();
            this.pnl_WiktoTM1.SuspendLayout();
            this.pnl_WiktoTM2.SuspendLayout();
            this.tpnl_WiktoT1.SuspendLayout();
            this.pnl_WiktoTL1.SuspendLayout();
            this.pnl_WiktoTL3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.groupBox13.SuspendLayout();
            this.groupBox14.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.GoogleHacks.SuspendLayout();
            this.pnl_GHMain.SuspendLayout();
            this.tpnl_HMain.SuspendLayout();
            this.tpnl_GHManQuery.SuspendLayout();
            this.pnl_GHMQ1.SuspendLayout();
            this.pnl_GHMQ2.SuspendLayout();
            this.pbl_GoogleHackDb.SuspendLayout();
            this.pnl_GHDesc.SuspendLayout();
            this.tpnl_GHResults.SuspendLayout();
            this.tpnl_GHResultsTop.SuspendLayout();
            this.pnl_GHRes1.SuspendLayout();
            this.pnl_GHRes2.SuspendLayout();
            this.pnl_GHRes3.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            this.pnl_configleft.SuspendLayout();
            this.pnl_ConfigMain.SuspendLayout();
            this.tab_configMain.SuspendLayout();
            this.cfg_DB.SuspendLayout();
            this.cfg_Header.SuspendLayout();
            this.cfg_Google.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updownGoogleDepth)).BeginInit();
            this.cfg_Proxy.SuspendLayout();
            this.cfg_Spider.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_contentsize)).BeginInit();
            this.cfg_Timing.SuspendLayout();
            this.cfg_Update.SuspendLayout();
            this.cfg_Startup.SuspendLayout();
            this.cfg_Help.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 741);
            this.splitter1.TabIndex = 57;
            this.splitter1.TabStop = false;
            // 
            // cntxtGoogleHacks
            // 
            this.cntxtGoogleHacks.Popup += new System.EventHandler(this.cntxtGoogleHacks_Popup);
            // 
            // btn_CfgLocateNDb
            // 
            this.btn_CfgLocateNDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_CfgLocateNDb.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_CfgLocateNDb.Location = new System.Drawing.Point(663, 7);
            this.btn_CfgLocateNDb.Name = "btn_CfgLocateNDb";
            this.btn_CfgLocateNDb.Size = new System.Drawing.Size(106, 22);
            this.btn_CfgLocateNDb.TabIndex = 10;
            this.btn_CfgLocateNDb.Text = "Update Now";
            this.toolTip1.SetToolTip(this.btn_CfgLocateNDb, "Update this database now...");
            this.btn_CfgLocateNDb.UseVisualStyleBackColor = true;
            this.btn_CfgLocateNDb.Click += new System.EventHandler(this.btnDBUpdateNikto_Click);
            // 
            // btn_CfgLocateGDb
            // 
            this.btn_CfgLocateGDb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_CfgLocateGDb.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_CfgLocateGDb.Location = new System.Drawing.Point(663, 31);
            this.btn_CfgLocateGDb.Name = "btn_CfgLocateGDb";
            this.btn_CfgLocateGDb.Size = new System.Drawing.Size(106, 22);
            this.btn_CfgLocateGDb.TabIndex = 12;
            this.btn_CfgLocateGDb.Text = "Update Now";
            this.toolTip1.SetToolTip(this.btn_CfgLocateGDb, "Update this database now...");
            this.btn_CfgLocateGDb.UseVisualStyleBackColor = true;
            this.btn_CfgLocateGDb.Click += new System.EventHandler(this.btnDBUpdateGH_Click);
            // 
            // txtDBLocationGH
            // 
            this.txtDBLocationGH.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDBLocationGH.BackColor = System.Drawing.Color.Snow;
            this.txtDBLocationGH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDBLocationGH.Location = new System.Drawing.Point(149, 34);
            this.txtDBLocationGH.Name = "txtDBLocationGH";
            this.txtDBLocationGH.Size = new System.Drawing.Size(430, 18);
            this.txtDBLocationGH.TabIndex = 11;
            this.txtDBLocationGH.Text = "databases\\GHDB.xml";
            this.txtDBLocationGH.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtDBLocationGH, "The Google Hack Database Location");
            // 
            // txtDBlocationNikto
            // 
            this.txtDBlocationNikto.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDBlocationNikto.BackColor = System.Drawing.Color.Snow;
            this.txtDBlocationNikto.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDBlocationNikto.Location = new System.Drawing.Point(149, 10);
            this.txtDBlocationNikto.Name = "txtDBlocationNikto";
            this.txtDBlocationNikto.Size = new System.Drawing.Size(430, 18);
            this.txtDBlocationNikto.TabIndex = 9;
            this.txtDBlocationNikto.Text = "databases\\nikto-scan_database.db";
            this.txtDBlocationNikto.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtDBlocationNikto, "The Nikto Database location");
            // 
            // txtHeader
            // 
            this.txtHeader.AcceptsReturn = true;
            this.txtHeader.BackColor = System.Drawing.Color.Snow;
            this.txtHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtHeader.Location = new System.Drawing.Point(3, 3);
            this.txtHeader.Multiline = true;
            this.txtHeader.Name = "txtHeader";
            this.txtHeader.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtHeader.Size = new System.Drawing.Size(763, 676);
            this.txtHeader.TabIndex = 13;
            this.toolTip1.SetToolTip(this.txtHeader, "The HTTP Header to use during your scan");
            this.txtHeader.WordWrap = false;
            // 
            // chkProxyPresent
            // 
            this.chkProxyPresent.AutoSize = true;
            this.chkProxyPresent.BackColor = System.Drawing.Color.Transparent;
            this.chkProxyPresent.Location = new System.Drawing.Point(6, 36);
            this.chkProxyPresent.Name = "chkProxyPresent";
            this.chkProxyPresent.Size = new System.Drawing.Size(78, 16);
            this.chkProxyPresent.TabIndex = 28;
            this.chkProxyPresent.Text = "Enable Proxy";
            this.toolTip1.SetToolTip(this.chkProxyPresent, "If you want to use a proxy during your scan, please make sure that this is checke" +
                    "d.");
            this.chkProxyPresent.UseVisualStyleBackColor = true;
            // 
            // txtProxySettings
            // 
            this.txtProxySettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProxySettings.BackColor = System.Drawing.Color.Snow;
            this.txtProxySettings.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProxySettings.Location = new System.Drawing.Point(134, 10);
            this.txtProxySettings.Name = "txtProxySettings";
            this.txtProxySettings.Size = new System.Drawing.Size(508, 18);
            this.txtProxySettings.TabIndex = 27;
            this.txtProxySettings.Text = "ProxyIP/DNS:Port";
            this.txtProxySettings.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtProxySettings, "Proxy Server and Port (ie: localhost:3128)");
            // 
            // NUPDOWNspider
            // 
            this.NUPDOWNspider.BackColor = System.Drawing.Color.Snow;
            this.NUPDOWNspider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NUPDOWNspider.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUPDOWNspider.Location = new System.Drawing.Point(134, 154);
            this.NUPDOWNspider.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NUPDOWNspider.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NUPDOWNspider.Name = "NUPDOWNspider";
            this.NUPDOWNspider.Size = new System.Drawing.Size(120, 18);
            this.NUPDOWNspider.TabIndex = 149;
            this.toolTip1.SetToolTip(this.NUPDOWNspider, "The number of worker threads to use during the spidering process");
            this.NUPDOWNspider.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // txt_ConfigSpiderExclude
            // 
            this.txt_ConfigSpiderExclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_ConfigSpiderExclude.BackColor = System.Drawing.Color.Snow;
            this.txt_ConfigSpiderExclude.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_ConfigSpiderExclude.Location = new System.Drawing.Point(134, 10);
            this.txt_ConfigSpiderExclude.Name = "txt_ConfigSpiderExclude";
            this.txt_ConfigSpiderExclude.Size = new System.Drawing.Size(508, 18);
            this.txt_ConfigSpiderExclude.TabIndex = 147;
            this.txt_ConfigSpiderExclude.Text = "click.asp,mailto,javascript";
            this.txt_ConfigSpiderExclude.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txt_ConfigSpiderExclude, "Exclude hrefs that match these.");
            // 
            // txt_ConfigSpiderExtension
            // 
            this.txt_ConfigSpiderExtension.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_ConfigSpiderExtension.BackColor = System.Drawing.Color.Snow;
            this.txt_ConfigSpiderExtension.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_ConfigSpiderExtension.Location = new System.Drawing.Point(134, 34);
            this.txt_ConfigSpiderExtension.Name = "txt_ConfigSpiderExtension";
            this.txt_ConfigSpiderExtension.Size = new System.Drawing.Size(508, 18);
            this.txt_ConfigSpiderExtension.TabIndex = 148;
            this.txt_ConfigSpiderExtension.Text = "xml,mso,swf,tgz,tar,xls,doc,ppt,msi,jpg,gif,png,css,js,ico,avi,wmv,mp3,rm,exe,zip" +
                ",pdf,h,c,cpp,sh,bin,gz,bz2,mpg";
            this.txt_ConfigSpiderExtension.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txt_ConfigSpiderExtension, "Do not try and spider files with these extensions");
            // 
            // updownRetryTCP
            // 
            this.updownRetryTCP.BackColor = System.Drawing.Color.Snow;
            this.updownRetryTCP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.updownRetryTCP.Location = new System.Drawing.Point(134, 34);
            this.updownRetryTCP.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.updownRetryTCP.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownRetryTCP.Name = "updownRetryTCP";
            this.updownRetryTCP.Size = new System.Drawing.Size(44, 18);
            this.updownRetryTCP.TabIndex = 30;
            this.updownRetryTCP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.updownRetryTCP, "Number of attempts to make should a page timeout");
            this.updownRetryTCP.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // updownTimeOutTCP
            // 
            this.updownTimeOutTCP.BackColor = System.Drawing.Color.Snow;
            this.updownTimeOutTCP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.updownTimeOutTCP.Increment = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.updownTimeOutTCP.Location = new System.Drawing.Point(134, 10);
            this.updownTimeOutTCP.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.updownTimeOutTCP.Name = "updownTimeOutTCP";
            this.updownTimeOutTCP.Size = new System.Drawing.Size(68, 18);
            this.updownTimeOutTCP.TabIndex = 29;
            this.updownTimeOutTCP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.updownTimeOutTCP, "Timeout value in ms");
            this.updownTimeOutTCP.Value = new decimal(new int[] {
            6000,
            0,
            0,
            0});
            // 
            // txtURLUpdateGHDB
            // 
            this.txtURLUpdateGHDB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtURLUpdateGHDB.BackColor = System.Drawing.Color.Snow;
            this.txtURLUpdateGHDB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtURLUpdateGHDB.Location = new System.Drawing.Point(134, 58);
            this.txtURLUpdateGHDB.Name = "txtURLUpdateGHDB";
            this.txtURLUpdateGHDB.Size = new System.Drawing.Size(508, 18);
            this.txtURLUpdateGHDB.TabIndex = 33;
            this.txtURLUpdateGHDB.Text = "http://johnny.ihackstuff.com/xml/ghdb.php";
            this.txtURLUpdateGHDB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtURLUpdateGHDB, "The Google Hack database update location");
            // 
            // txtURLUpdateNiktoDB
            // 
            this.txtURLUpdateNiktoDB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtURLUpdateNiktoDB.BackColor = System.Drawing.Color.Snow;
            this.txtURLUpdateNiktoDB.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtURLUpdateNiktoDB.Location = new System.Drawing.Point(134, 34);
            this.txtURLUpdateNiktoDB.Name = "txtURLUpdateNiktoDB";
            this.txtURLUpdateNiktoDB.Size = new System.Drawing.Size(508, 18);
            this.txtURLUpdateNiktoDB.TabIndex = 32;
            this.txtURLUpdateNiktoDB.Text = "http://www.sensepost.com/research/wikto/DB/nikto.db";
            this.txtURLUpdateNiktoDB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtURLUpdateNiktoDB, "The Nikto database update location");
            // 
            // txtURLUpdate
            // 
            this.txtURLUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtURLUpdate.BackColor = System.Drawing.Color.Snow;
            this.txtURLUpdate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtURLUpdate.Location = new System.Drawing.Point(134, 10);
            this.txtURLUpdate.Name = "txtURLUpdate";
            this.txtURLUpdate.Size = new System.Drawing.Size(508, 18);
            this.txtURLUpdate.TabIndex = 31;
            this.txtURLUpdate.Text = "http://www.sensepost.com/research/wikto/DB/BackEnd/";
            this.txtURLUpdate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtURLUpdate, "The backend database update location");
            // 
            // btn_ShowNews
            // 
            this.btn_ShowNews.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_ShowNews.Location = new System.Drawing.Point(6, 60);
            this.btn_ShowNews.Name = "btn_ShowNews";
            this.btn_ShowNews.Size = new System.Drawing.Size(106, 22);
            this.btn_ShowNews.TabIndex = 30;
            this.btn_ShowNews.Text = "Show News Now";
            this.toolTip1.SetToolTip(this.btn_ShowNews, "Show the news now...");
            this.btn_ShowNews.UseVisualStyleBackColor = true;
            this.btn_ShowNews.Click += new System.EventHandler(this.skinButton3_Click);
            // 
            // chk_ShowStart
            // 
            this.chk_ShowStart.AutoSize = true;
            this.chk_ShowStart.BackColor = System.Drawing.Color.Transparent;
            this.chk_ShowStart.Location = new System.Drawing.Point(6, 12);
            this.chk_ShowStart.Name = "chk_ShowStart";
            this.chk_ShowStart.Size = new System.Drawing.Size(119, 16);
            this.chk_ShowStart.TabIndex = 29;
            this.chk_ShowStart.Text = "Show News on Startup";
            this.toolTip1.SetToolTip(this.chk_ShowStart, "If this is checked, the news page will show when Wikto Starts");
            this.chk_ShowStart.UseVisualStyleBackColor = true;
            this.chk_ShowStart.CheckedChanged += new System.EventHandler(this.chk_ShowStart_CheckedChanged);
            // 
            // btn_CnfQuit
            // 
            this.btn_CnfQuit.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_CnfQuit.ForeColor = System.Drawing.Color.Brown;
            this.btn_CnfQuit.Location = new System.Drawing.Point(121, 86);
            this.btn_CnfQuit.Name = "btn_CnfQuit";
            this.btn_CnfQuit.Size = new System.Drawing.Size(86, 28);
            this.btn_CnfQuit.TabIndex = 5;
            this.btn_CnfQuit.Text = "Quit";
            this.toolTip1.SetToolTip(this.btn_CnfQuit, "Quit Wikto");
            this.btn_CnfQuit.UseVisualStyleBackColor = true;
            this.btn_CnfQuit.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btn_CnfReset
            // 
            this.btn_CnfReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_CnfReset.ForeColor = System.Drawing.Color.Brown;
            this.btn_CnfReset.Location = new System.Drawing.Point(121, 59);
            this.btn_CnfReset.Name = "btn_CnfReset";
            this.btn_CnfReset.Size = new System.Drawing.Size(86, 28);
            this.btn_CnfReset.TabIndex = 4;
            this.btn_CnfReset.Text = "Factory Reset";
            this.toolTip1.SetToolTip(this.btn_CnfReset, "Perform a factory reset");
            this.btn_CnfReset.UseVisualStyleBackColor = true;
            this.btn_CnfReset.Click += new System.EventHandler(this.button2_Click_2);
            // 
            // btn_CnfSave
            // 
            this.btn_CnfSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_CnfSave.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_CnfSave.Location = new System.Drawing.Point(121, 32);
            this.btn_CnfSave.Name = "btn_CnfSave";
            this.btn_CnfSave.Size = new System.Drawing.Size(86, 28);
            this.btn_CnfSave.TabIndex = 3;
            this.btn_CnfSave.Text = "Save";
            this.toolTip1.SetToolTip(this.btn_CnfSave, "Save the current settings");
            this.btn_CnfSave.UseVisualStyleBackColor = true;
            this.btn_CnfSave.Click += new System.EventHandler(this.btnConfigSave_Click);
            // 
            // btn_CnfLoad
            // 
            this.btn_CnfLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_CnfLoad.ForeColor = System.Drawing.Color.ForestGreen;
            this.btn_CnfLoad.Location = new System.Drawing.Point(121, 5);
            this.btn_CnfLoad.Name = "btn_CnfLoad";
            this.btn_CnfLoad.Size = new System.Drawing.Size(86, 28);
            this.btn_CnfLoad.TabIndex = 2;
            this.btn_CnfLoad.Text = "Load";
            this.toolTip1.SetToolTip(this.btn_CnfLoad, "Load a configuration file");
            this.btn_CnfLoad.UseVisualStyleBackColor = true;
            this.btn_CnfLoad.Click += new System.EventHandler(this.btnLoadConfig_Click);
            // 
            // btn_GHManualQuery
            // 
            this.btn_GHManualQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_GHManualQuery.Location = new System.Drawing.Point(0, 0);
            this.btn_GHManualQuery.Name = "btn_GHManualQuery";
            this.btn_GHManualQuery.Size = new System.Drawing.Size(94, 24);
            this.btn_GHManualQuery.TabIndex = 12;
            this.btn_GHManualQuery.Text = "Manual Query";
            this.toolTip1.SetToolTip(this.btn_GHManualQuery, "Perform a Manual Query");
            this.btn_GHManualQuery.UseVisualStyleBackColor = true;
            this.btn_GHManualQuery.Click += new System.EventHandler(this.btnGoogleHackOnceOff_Click);
            // 
            // btn_GHLoadDatabase
            // 
            this.btn_GHLoadDatabase.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_GHLoadDatabase.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_GHLoadDatabase.Location = new System.Drawing.Point(25, 143);
            this.btn_GHLoadDatabase.Name = "btn_GHLoadDatabase";
            this.btn_GHLoadDatabase.Size = new System.Drawing.Size(161, 24);
            this.btn_GHLoadDatabase.TabIndex = 6;
            this.btn_GHLoadDatabase.Text = "Load Google Hack Database";
            this.toolTip1.SetToolTip(this.btn_GHLoadDatabase, "Load a Google Hack Database");
            this.btn_GHLoadDatabase.UseVisualStyleBackColor = true;
            this.btn_GHLoadDatabase.Click += new System.EventHandler(this.btnLoadGoogleHack_Click);
            // 
            // txtGoogleHackTarget
            // 
            this.txtGoogleHackTarget.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.txtGoogleHackTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGoogleHackTarget.Location = new System.Drawing.Point(71, 98);
            this.txtGoogleHackTarget.Name = "txtGoogleHackTarget";
            this.txtGoogleHackTarget.Size = new System.Drawing.Size(136, 18);
            this.txtGoogleHackTarget.TabIndex = 5;
            this.txtGoogleHackTarget.Text = "localhost";
            this.txtGoogleHackTarget.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtGoogleHackTarget, "Site to search");
            // 
            // btn_GHQuit
            // 
            this.btn_GHQuit.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_GHQuit.ForeColor = System.Drawing.Color.Brown;
            this.btn_GHQuit.Location = new System.Drawing.Point(121, 59);
            this.btn_GHQuit.Name = "btn_GHQuit";
            this.btn_GHQuit.Size = new System.Drawing.Size(86, 28);
            this.btn_GHQuit.TabIndex = 4;
            this.btn_GHQuit.Text = "Quit";
            this.toolTip1.SetToolTip(this.btn_GHQuit, "Quit Wikto");
            this.btn_GHQuit.UseVisualStyleBackColor = true;
            this.btn_GHQuit.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btn_GHStop
            // 
            this.btn_GHStop.Enabled = false;
            this.btn_GHStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_GHStop.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_GHStop.Location = new System.Drawing.Point(121, 32);
            this.btn_GHStop.Name = "btn_GHStop";
            this.btn_GHStop.Size = new System.Drawing.Size(86, 28);
            this.btn_GHStop.TabIndex = 3;
            this.btn_GHStop.Text = "Stop";
            this.toolTip1.SetToolTip(this.btn_GHStop, "Stop the Scan");
            this.btn_GHStop.UseVisualStyleBackColor = true;
            this.btn_GHStop.Click += new System.EventHandler(this.btnStopGoogleHack_Click);
            // 
            // btn_GHStart
            // 
            this.btn_GHStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_GHStart.ForeColor = System.Drawing.Color.ForestGreen;
            this.btn_GHStart.Location = new System.Drawing.Point(121, 5);
            this.btn_GHStart.Name = "btn_GHStart";
            this.btn_GHStart.Size = new System.Drawing.Size(86, 28);
            this.btn_GHStart.TabIndex = 2;
            this.btn_GHStart.Text = "Start";
            this.toolTip1.SetToolTip(this.btn_GHStart, "Start the Scan");
            this.btn_GHStart.UseVisualStyleBackColor = true;
            this.btn_GHStart.Click += new System.EventHandler(this.btnStartGoogleHack_Click);
            // 
            // btn_BEInDirImportM
            // 
            this.btn_BEInDirImportM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_BEInDirImportM.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_BEInDirImportM.Location = new System.Drawing.Point(87, 0);
            this.btn_BEInDirImportM.Name = "btn_BEInDirImportM";
            this.btn_BEInDirImportM.Size = new System.Drawing.Size(48, 21);
            this.btn_BEInDirImportM.TabIndex = 30;
            this.btn_BEInDirImportM.Text = "Spider";
            this.toolTip1.SetToolTip(this.btn_BEInDirImportM, "Import Directories from the Spider");
            this.btn_BEInDirImportM.UseVisualStyleBackColor = true;
            this.btn_BEInDirImportM.Click += new System.EventHandler(this.btn_BEInDirImportM_Click);
            // 
            // btn_BEImportInDirG
            // 
            this.btn_BEImportInDirG.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_BEImportInDirG.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_BEImportInDirG.Location = new System.Drawing.Point(137, 0);
            this.btn_BEImportInDirG.Name = "btn_BEImportInDirG";
            this.btn_BEImportInDirG.Size = new System.Drawing.Size(48, 21);
            this.btn_BEImportInDirG.TabIndex = 31;
            this.btn_BEImportInDirG.Text = "Google";
            this.toolTip1.SetToolTip(this.btn_BEImportInDirG, "Import Directories from the Google Miner");
            this.btn_BEImportInDirG.UseVisualStyleBackColor = true;
            this.btn_BEImportInDirG.Click += new System.EventHandler(this.btn_BEImportInDirG_Click);
            // 
            // btnBackEndPause
            // 
            this.btnBackEndPause.Enabled = false;
            this.btnBackEndPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btnBackEndPause.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnBackEndPause.Location = new System.Drawing.Point(121, 86);
            this.btnBackEndPause.Name = "btnBackEndPause";
            this.btnBackEndPause.Size = new System.Drawing.Size(86, 28);
            this.btnBackEndPause.TabIndex = 256;
            this.btnBackEndPause.Text = "Pause";
            this.toolTip1.SetToolTip(this.btnBackEndPause, "Pause / Resume a Scan");
            this.btnBackEndPause.UseVisualStyleBackColor = true;
            this.btnBackEndPause.Click += new System.EventHandler(this.btnBackEndPause_Click);
            // 
            // chkPreserve
            // 
            this.chkPreserve.AutoSize = true;
            this.chkPreserve.BackColor = System.Drawing.Color.Transparent;
            this.chkPreserve.Location = new System.Drawing.Point(7, 135);
            this.chkPreserve.Name = "chkPreserve";
            this.chkPreserve.Size = new System.Drawing.Size(96, 16);
            this.chkPreserve.TabIndex = 7;
            this.chkPreserve.Text = "Preserve Results";
            this.toolTip1.SetToolTip(this.chkPreserve, "Preserve Existing Results");
            this.chkPreserve.UseVisualStyleBackColor = true;
            // 
            // btn_BESkiptoDirs
            // 
            this.btn_BESkiptoDirs.Enabled = false;
            this.btn_BESkiptoDirs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.btn_BESkiptoDirs.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_BESkiptoDirs.Location = new System.Drawing.Point(121, 155);
            this.btn_BESkiptoDirs.Name = "btn_BESkiptoDirs";
            this.btn_BESkiptoDirs.Size = new System.Drawing.Size(86, 16);
            this.btn_BESkiptoDirs.TabIndex = 6;
            this.btn_BESkiptoDirs.Text = "Skip to Dirs.";
            this.toolTip1.SetToolTip(this.btn_BESkiptoDirs, "Skip to scanning directories");
            this.btn_BESkiptoDirs.UseVisualStyleBackColor = true;
            this.btn_BESkiptoDirs.Click += new System.EventHandler(this.btnSkipFile_Click);
            // 
            // btn_BEQuit
            // 
            this.btn_BEQuit.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_BEQuit.ForeColor = System.Drawing.Color.Brown;
            this.btn_BEQuit.Location = new System.Drawing.Point(121, 59);
            this.btn_BEQuit.Name = "btn_BEQuit";
            this.btn_BEQuit.Size = new System.Drawing.Size(86, 28);
            this.btn_BEQuit.TabIndex = 4;
            this.btn_BEQuit.Text = "Quit";
            this.toolTip1.SetToolTip(this.btn_BEQuit, "Quit Wikto");
            this.btn_BEQuit.UseVisualStyleBackColor = true;
            this.btn_BEQuit.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btn_BEStop
            // 
            this.btn_BEStop.Enabled = false;
            this.btn_BEStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_BEStop.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_BEStop.Location = new System.Drawing.Point(121, 32);
            this.btn_BEStop.Name = "btn_BEStop";
            this.btn_BEStop.Size = new System.Drawing.Size(86, 28);
            this.btn_BEStop.TabIndex = 3;
            this.btn_BEStop.Text = "Stop";
            this.toolTip1.SetToolTip(this.btn_BEStop, "Stop the Scan");
            this.btn_BEStop.UseVisualStyleBackColor = true;
            this.btn_BEStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btn_BEStart
            // 
            this.btn_BEStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_BEStart.ForeColor = System.Drawing.Color.ForestGreen;
            this.btn_BEStart.Location = new System.Drawing.Point(121, 5);
            this.btn_BEStart.Name = "btn_BEStart";
            this.btn_BEStart.Size = new System.Drawing.Size(86, 28);
            this.btn_BEStart.TabIndex = 2;
            this.btn_BEStart.Text = "Start";
            this.toolTip1.SetToolTip(this.btn_BEStart, "Start the Scan");
            this.btn_BEStart.UseVisualStyleBackColor = true;
            this.btn_BEStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btn_BESkiptoFiles
            // 
            this.btn_BESkiptoFiles.Enabled = false;
            this.btn_BESkiptoFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.btn_BESkiptoFiles.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_BESkiptoFiles.Location = new System.Drawing.Point(121, 135);
            this.btn_BESkiptoFiles.Name = "btn_BESkiptoFiles";
            this.btn_BESkiptoFiles.Size = new System.Drawing.Size(86, 16);
            this.btn_BESkiptoFiles.TabIndex = 5;
            this.btn_BESkiptoFiles.Text = "Skip to Files";
            this.toolTip1.SetToolTip(this.btn_BESkiptoFiles, "Skip to scanning files");
            this.btn_BESkiptoFiles.UseVisualStyleBackColor = true;
            this.btn_BESkiptoFiles.Click += new System.EventHandler(this.btnSkipDirs_Click);
            // 
            // btn_BackEndExport
            // 
            this.btn_BackEndExport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_BackEndExport.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_BackEndExport.ForeColor = System.Drawing.Color.ForestGreen;
            this.btn_BackEndExport.Location = new System.Drawing.Point(3, 588);
            this.btn_BackEndExport.Name = "btn_BackEndExport";
            this.btn_BackEndExport.Size = new System.Drawing.Size(208, 24);
            this.btn_BackEndExport.TabIndex = 28;
            this.btn_BackEndExport.Text = "Export Results";
            this.toolTip1.SetToolTip(this.btn_BackEndExport, "Export Results to file");
            this.btn_BackEndExport.UseVisualStyleBackColor = true;
            this.btn_BackEndExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // skinButton4
            // 
            this.skinButton4.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.skinButton4.Location = new System.Drawing.Point(114, 65);
            this.skinButton4.Name = "skinButton4";
            this.skinButton4.Size = new System.Drawing.Size(86, 21);
            this.skinButton4.TabIndex = 151;
            this.skinButton4.Text = "Reset";
            this.toolTip1.SetToolTip(this.skinButton4, "Reset the result counters");
            this.skinButton4.UseVisualStyleBackColor = true;
            this.skinButton4.Click += new System.EventHandler(this.skinButton4_Click);
            // 
            // skinButton5
            // 
            this.skinButton5.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.skinButton5.Location = new System.Drawing.Point(114, 38);
            this.skinButton5.Name = "skinButton5";
            this.skinButton5.Size = new System.Drawing.Size(86, 21);
            this.skinButton5.TabIndex = 150;
            this.skinButton5.Text = "Show All";
            this.toolTip1.SetToolTip(this.skinButton5, "Show all results and trigger values");
            this.skinButton5.UseVisualStyleBackColor = true;
            this.skinButton5.Click += new System.EventHandler(this.skinButton5_Click);
            // 
            // skinButton6
            // 
            this.skinButton6.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.skinButton6.Location = new System.Drawing.Point(114, 11);
            this.skinButton6.Name = "skinButton6";
            this.skinButton6.Size = new System.Drawing.Size(86, 21);
            this.skinButton6.TabIndex = 149;
            this.skinButton6.Text = "Update";
            this.toolTip1.SetToolTip(this.skinButton6, "Update result lists with the current triggers");
            this.skinButton6.UseVisualStyleBackColor = true;
            this.skinButton6.Click += new System.EventHandler(this.skinButton6_Click);
            // 
            // radioHEAD
            // 
            this.radioHEAD.AutoSize = true;
            this.radioHEAD.BackColor = System.Drawing.Color.Transparent;
            this.radioHEAD.Location = new System.Drawing.Point(8, 149);
            this.radioHEAD.Name = "radioHEAD";
            this.radioHEAD.Size = new System.Drawing.Size(50, 16);
            this.radioHEAD.TabIndex = 26;
            this.radioHEAD.Text = "HEAD";
            this.toolTip1.SetToolTip(this.radioHEAD, "Use the HEAD HTTP method");
            this.radioHEAD.UseVisualStyleBackColor = true;
            // 
            // radioGET
            // 
            this.radioGET.AutoSize = true;
            this.radioGET.BackColor = System.Drawing.Color.Transparent;
            this.radioGET.Checked = true;
            this.radioGET.Location = new System.Drawing.Point(122, 149);
            this.radioGET.Name = "radioGET";
            this.radioGET.Size = new System.Drawing.Size(41, 16);
            this.radioGET.TabIndex = 27;
            this.radioGET.TabStop = true;
            this.radioGET.Text = "GET";
            this.toolTip1.SetToolTip(this.radioGET, "Use the GET HTTP method (recommended)");
            this.radioGET.UseVisualStyleBackColor = true;
            // 
            // btn_BEClearDB
            // 
            this.btn_BEClearDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_BEClearDB.ForeColor = System.Drawing.Color.Brown;
            this.btn_BEClearDB.Location = new System.Drawing.Point(8, 65);
            this.btn_BEClearDB.Name = "btn_BEClearDB";
            this.btn_BEClearDB.Size = new System.Drawing.Size(100, 20);
            this.btn_BEClearDB.TabIndex = 21;
            this.btn_BEClearDB.Text = "Clear DB";
            this.toolTip1.SetToolTip(this.btn_BEClearDB, "Clear the result cache");
            this.btn_BEClearDB.UseVisualStyleBackColor = true;
            this.btn_BEClearDB.Click += new System.EventHandler(this.btnClearBackEndDB_Click);
            // 
            // chkBackEndAI
            // 
            this.chkBackEndAI.AutoSize = true;
            this.chkBackEndAI.BackColor = System.Drawing.Color.Transparent;
            this.chkBackEndAI.Location = new System.Drawing.Point(8, 14);
            this.chkBackEndAI.Name = "chkBackEndAI";
            this.chkBackEndAI.Size = new System.Drawing.Size(53, 16);
            this.chkBackEndAI.TabIndex = 19;
            this.chkBackEndAI.Text = "Use AI";
            this.toolTip1.SetToolTip(this.chkBackEndAI, "Use SensePost\'s AI triggers t reduce false positives (we recommend a value of aro" +
                    "und 0.8)");
            this.chkBackEndAI.UseVisualStyleBackColor = true;
            this.chkBackEndAI.CheckedChanged += new System.EventHandler(this.ToggleBackendAI);
            // 
            // txtErrorCodeDir
            // 
            this.txtErrorCodeDir.BackColor = System.Drawing.Color.Snow;
            this.txtErrorCodeDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtErrorCodeDir.Location = new System.Drawing.Point(3, 3);
            this.txtErrorCodeDir.Name = "txtErrorCodeDir";
            this.txtErrorCodeDir.Size = new System.Drawing.Size(100, 18);
            this.txtErrorCodeDir.TabIndex = 24;
            this.txtErrorCodeDir.Text = "403,401,200,302,301";
            this.txtErrorCodeDir.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.txtErrorCodeDir, "HTTP response codes indicating thet directories exist");
            // 
            // txtErrorCodeFile
            // 
            this.txtErrorCodeFile.BackColor = System.Drawing.Color.Snow;
            this.txtErrorCodeFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtErrorCodeFile.Location = new System.Drawing.Point(3, 27);
            this.txtErrorCodeFile.Name = "txtErrorCodeFile";
            this.txtErrorCodeFile.Size = new System.Drawing.Size(100, 18);
            this.txtErrorCodeFile.TabIndex = 25;
            this.txtErrorCodeFile.Text = "200,302,301,403";
            this.txtErrorCodeFile.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.txtErrorCodeFile, "HTTP response codes indicating thet files exist");
            // 
            // btn_BELoadDirs
            // 
            this.btn_BELoadDirs.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_BELoadDirs.Location = new System.Drawing.Point(8, 20);
            this.btn_BELoadDirs.Name = "btn_BELoadDirs";
            this.btn_BELoadDirs.Size = new System.Drawing.Size(100, 20);
            this.btn_BELoadDirs.TabIndex = 13;
            this.btn_BELoadDirs.Text = "Load Directories";
            this.toolTip1.SetToolTip(this.btn_BELoadDirs, "Load a list of directory names from file");
            this.btn_BELoadDirs.UseVisualStyleBackColor = true;
            this.btn_BELoadDirs.Click += new System.EventHandler(this.btnLoadDirs_Click);
            // 
            // btn_BEUpdateFromSP
            // 
            this.btn_BEUpdateFromSP.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_BEUpdateFromSP.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_BEUpdateFromSP.Location = new System.Drawing.Point(108, 20);
            this.btn_BEUpdateFromSP.Name = "btn_BEUpdateFromSP";
            this.btn_BEUpdateFromSP.Size = new System.Drawing.Size(100, 36);
            this.btn_BEUpdateFromSP.TabIndex = 16;
            this.btn_BEUpdateFromSP.Text = "Update from SensePost";
            this.toolTip1.SetToolTip(this.btn_BEUpdateFromSP, "Update the directory, file and extension lists from SensePost");
            this.btn_BEUpdateFromSP.UseVisualStyleBackColor = true;
            this.btn_BEUpdateFromSP.Click += new System.EventHandler(this.btnBackEndUpdateDirs_Click);
            // 
            // cmbBackEndUpdate
            // 
            this.cmbBackEndUpdate.BackColor = System.Drawing.Color.Snow;
            this.cmbBackEndUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbBackEndUpdate.Location = new System.Drawing.Point(116, 64);
            this.cmbBackEndUpdate.Name = "cmbBackEndUpdate";
            this.cmbBackEndUpdate.Size = new System.Drawing.Size(84, 20);
            this.cmbBackEndUpdate.TabIndex = 17;
            this.cmbBackEndUpdate.Text = "Refresh...";
            this.toolTip1.SetToolTip(this.cmbBackEndUpdate, "Select the update option");
            this.cmbBackEndUpdate.Click += new System.EventHandler(this.cmbBackEndUpdate_SelectedIndexChanged);
            // 
            // btn_BELoadExts
            // 
            this.btn_BELoadExts.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_BELoadExts.Location = new System.Drawing.Point(8, 63);
            this.btn_BELoadExts.Name = "btn_BELoadExts";
            this.btn_BELoadExts.Size = new System.Drawing.Size(100, 20);
            this.btn_BELoadExts.TabIndex = 15;
            this.btn_BELoadExts.Text = "Load File Types";
            this.toolTip1.SetToolTip(this.btn_BELoadExts, "Load a list of filetypes from file");
            this.btn_BELoadExts.UseVisualStyleBackColor = true;
            this.btn_BELoadExts.Click += new System.EventHandler(this.btnLoadFileformat_Click);
            // 
            // btn_BELoadFiles
            // 
            this.btn_BELoadFiles.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_BELoadFiles.Location = new System.Drawing.Point(8, 42);
            this.btn_BELoadFiles.Name = "btn_BELoadFiles";
            this.btn_BELoadFiles.Size = new System.Drawing.Size(100, 20);
            this.btn_BELoadFiles.TabIndex = 14;
            this.btn_BELoadFiles.Text = "Load File Names";
            this.toolTip1.SetToolTip(this.btn_BELoadFiles, "Load a list of files from file");
            this.btn_BELoadFiles.UseVisualStyleBackColor = true;
            this.btn_BELoadFiles.Click += new System.EventHandler(this.btnLoadFiles_Click);
            // 
            // txtIPPort
            // 
            this.txtIPPort.BackColor = System.Drawing.Color.Snow;
            this.txtIPPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIPPort.Location = new System.Drawing.Point(8, 44);
            this.txtIPPort.Name = "txtIPPort";
            this.txtIPPort.Size = new System.Drawing.Size(112, 18);
            this.txtIPPort.TabIndex = 10;
            this.txtIPPort.Text = "80";
            this.txtIPPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.txtIPPort, "The target port (ie: 80 for http, 443 for https)");
            // 
            // chkBackEnduseSSLport
            // 
            this.chkBackEnduseSSLport.AutoSize = true;
            this.chkBackEnduseSSLport.BackColor = System.Drawing.Color.Transparent;
            this.chkBackEnduseSSLport.Location = new System.Drawing.Point(154, 47);
            this.chkBackEnduseSSLport.Name = "chkBackEnduseSSLport";
            this.chkBackEnduseSSLport.Size = new System.Drawing.Size(41, 16);
            this.chkBackEnduseSSLport.TabIndex = 11;
            this.chkBackEnduseSSLport.Text = "SSL";
            this.toolTip1.SetToolTip(this.chkBackEnduseSSLport, "If you are scanning a secure web server(https), ensure that this is checked.");
            this.chkBackEnduseSSLport.UseVisualStyleBackColor = true;
            this.chkBackEnduseSSLport.CheckedChanged += new System.EventHandler(this.chkBackEnduseSSLport_CheckedChanged);
            // 
            // txtIPNumber
            // 
            this.txtIPNumber.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.txtIPNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIPNumber.Location = new System.Drawing.Point(8, 20);
            this.txtIPNumber.Name = "txtIPNumber";
            this.txtIPNumber.Size = new System.Drawing.Size(112, 18);
            this.txtIPNumber.TabIndex = 9;
            this.txtIPNumber.Text = "localhost";
            this.txtIPNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtIPNumber, "The target host (ie: localhost)");
            this.txtIPNumber.TextChanged += new System.EventHandler(this.populateHeader);
            // 
            // btn_WiktoImportBackEnd
            // 
            this.btn_WiktoImportBackEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_WiktoImportBackEnd.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_WiktoImportBackEnd.Location = new System.Drawing.Point(227, 0);
            this.btn_WiktoImportBackEnd.Name = "btn_WiktoImportBackEnd";
            this.btn_WiktoImportBackEnd.Size = new System.Drawing.Size(48, 21);
            this.btn_WiktoImportBackEnd.TabIndex = 21;
            this.btn_WiktoImportBackEnd.Text = "Backend";
            this.toolTip1.SetToolTip(this.btn_WiktoImportBackEnd, "Import Directories from Backend Miner");
            this.btn_WiktoImportBackEnd.UseVisualStyleBackColor = true;
            this.btn_WiktoImportBackEnd.Click += new System.EventHandler(this.btn_WiktoImportBackEnd_Click);
            // 
            // btn_WiktoImportMirror
            // 
            this.btn_WiktoImportMirror.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_WiktoImportMirror.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_WiktoImportMirror.Location = new System.Drawing.Point(129, 0);
            this.btn_WiktoImportMirror.Name = "btn_WiktoImportMirror";
            this.btn_WiktoImportMirror.Size = new System.Drawing.Size(44, 21);
            this.btn_WiktoImportMirror.TabIndex = 22;
            this.btn_WiktoImportMirror.Text = "Spider";
            this.toolTip1.SetToolTip(this.btn_WiktoImportMirror, "Import Directories from Spider");
            this.btn_WiktoImportMirror.UseVisualStyleBackColor = true;
            this.btn_WiktoImportMirror.Click += new System.EventHandler(this.btn_WiktoImportMirror_Click);
            // 
            // btn_WiktoImportGoogle
            // 
            this.btn_WiktoImportGoogle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_WiktoImportGoogle.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_WiktoImportGoogle.Location = new System.Drawing.Point(179, 0);
            this.btn_WiktoImportGoogle.Name = "btn_WiktoImportGoogle";
            this.btn_WiktoImportGoogle.Size = new System.Drawing.Size(42, 21);
            this.btn_WiktoImportGoogle.TabIndex = 23;
            this.btn_WiktoImportGoogle.Text = "Google";
            this.toolTip1.SetToolTip(this.btn_WiktoImportGoogle, "Import Directories from Google Miner");
            this.btn_WiktoImportGoogle.UseVisualStyleBackColor = true;
            this.btn_WiktoImportGoogle.Click += new System.EventHandler(this.btn_WiktoImportGoogle_Click);
            // 
            // btn_NiktoLoad
            // 
            this.btn_NiktoLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_NiktoLoad.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_NiktoLoad.Location = new System.Drawing.Point(7, 360);
            this.btn_NiktoLoad.Name = "btn_NiktoLoad";
            this.btn_NiktoLoad.Size = new System.Drawing.Size(208, 24);
            this.btn_NiktoLoad.TabIndex = 18;
            this.btn_NiktoLoad.Text = "Load Nikto Database";
            this.toolTip1.SetToolTip(this.btn_NiktoLoad, "Load a Nikto Database");
            this.btn_NiktoLoad.UseVisualStyleBackColor = true;
            this.btn_NiktoLoad.Click += new System.EventHandler(this.btnNiktoLoad_Click_1);
            // 
            // skinButtonGreen2
            // 
            this.skinButtonGreen2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.skinButtonGreen2.ForeColor = System.Drawing.Color.ForestGreen;
            this.skinButtonGreen2.Location = new System.Drawing.Point(7, 330);
            this.skinButtonGreen2.Name = "skinButtonGreen2";
            this.skinButtonGreen2.Size = new System.Drawing.Size(208, 24);
            this.skinButtonGreen2.TabIndex = 17;
            this.skinButtonGreen2.Text = "Export Results";
            this.toolTip1.SetToolTip(this.skinButtonGreen2, "Export results to file");
            this.skinButtonGreen2.UseVisualStyleBackColor = true;
            this.skinButtonGreen2.Click += new System.EventHandler(this.btnNiktoExport_Click);
            // 
            // btnClearNiktoAI
            // 
            this.btnClearNiktoAI.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btnClearNiktoAI.ForeColor = System.Drawing.Color.Brown;
            this.btnClearNiktoAI.Location = new System.Drawing.Point(6, 61);
            this.btnClearNiktoAI.Name = "btnClearNiktoAI";
            this.btnClearNiktoAI.Size = new System.Drawing.Size(86, 21);
            this.btnClearNiktoAI.TabIndex = 14;
            this.btnClearNiktoAI.Text = "Clear DB";
            this.toolTip1.SetToolTip(this.btnClearNiktoAI, "Clear the results dataase");
            this.btnClearNiktoAI.UseVisualStyleBackColor = true;
            this.btnClearNiktoAI.Click += new System.EventHandler(this.btnClearNiktoAI_Click);
            // 
            // btnNiktoRestFuzz
            // 
            this.btnNiktoRestFuzz.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnNiktoRestFuzz.Location = new System.Drawing.Point(6, 41);
            this.btnNiktoRestFuzz.Name = "btnNiktoRestFuzz";
            this.btnNiktoRestFuzz.Size = new System.Drawing.Size(86, 21);
            this.btnNiktoRestFuzz.TabIndex = 15;
            this.btnNiktoRestFuzz.Text = "Reset";
            this.toolTip1.SetToolTip(this.btnNiktoRestFuzz, "Reset the results database");
            this.btnNiktoRestFuzz.UseVisualStyleBackColor = true;
            this.btnNiktoRestFuzz.Click += new System.EventHandler(this.btnNiktoRestFuzz_Click);
            // 
            // btnNiktoShowAll
            // 
            this.btnNiktoShowAll.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnNiktoShowAll.Location = new System.Drawing.Point(114, 61);
            this.btnNiktoShowAll.Name = "btnNiktoShowAll";
            this.btnNiktoShowAll.Size = new System.Drawing.Size(86, 21);
            this.btnNiktoShowAll.TabIndex = 13;
            this.btnNiktoShowAll.Text = "Show All";
            this.toolTip1.SetToolTip(this.btnNiktoShowAll, "Show all results");
            this.btnNiktoShowAll.UseVisualStyleBackColor = true;
            this.btnNiktoShowAll.Click += new System.EventHandler(this.btnNiktoShowAll_Click);
            // 
            // chkOptimizedNikto
            // 
            this.chkOptimizedNikto.AutoSize = true;
            this.chkOptimizedNikto.BackColor = System.Drawing.Color.Transparent;
            this.chkOptimizedNikto.Checked = true;
            this.chkOptimizedNikto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOptimizedNikto.Location = new System.Drawing.Point(116, 18);
            this.chkOptimizedNikto.Name = "chkOptimizedNikto";
            this.chkOptimizedNikto.Size = new System.Drawing.Size(66, 16);
            this.chkOptimizedNikto.TabIndex = 16;
            this.chkOptimizedNikto.Text = "Optimised";
            this.toolTip1.SetToolTip(this.chkOptimizedNikto, "Cache AI levels for known file types");
            this.chkOptimizedNikto.UseVisualStyleBackColor = true;
            // 
            // btnNiktoFuzzUpdate
            // 
            this.btnNiktoFuzzUpdate.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnNiktoFuzzUpdate.Location = new System.Drawing.Point(114, 41);
            this.btnNiktoFuzzUpdate.Name = "btnNiktoFuzzUpdate";
            this.btnNiktoFuzzUpdate.Size = new System.Drawing.Size(86, 21);
            this.btnNiktoFuzzUpdate.TabIndex = 12;
            this.btnNiktoFuzzUpdate.Text = "Update";
            this.toolTip1.SetToolTip(this.btnNiktoFuzzUpdate, "Update results lists using the current trigger");
            this.btnNiktoFuzzUpdate.UseVisualStyleBackColor = true;
            this.btnNiktoFuzzUpdate.Click += new System.EventHandler(this.btnNiktoFuzzUpdate_Click);
            // 
            // NUPDOWNfuzz
            // 
            this.NUPDOWNfuzz.BackColor = System.Drawing.Color.Snow;
            this.NUPDOWNfuzz.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NUPDOWNfuzz.DecimalPlaces = 3;
            this.NUPDOWNfuzz.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.NUPDOWNfuzz.Location = new System.Drawing.Point(6, 17);
            this.NUPDOWNfuzz.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NUPDOWNfuzz.Name = "NUPDOWNfuzz";
            this.NUPDOWNfuzz.Size = new System.Drawing.Size(86, 18);
            this.NUPDOWNfuzz.TabIndex = 11;
            this.toolTip1.SetToolTip(this.NUPDOWNfuzz, "The AI trigger level (we recommend a value of aroun 0.8)");
            this.NUPDOWNfuzz.Value = new decimal(new int[] {
            9,
            0,
            0,
            65536});
            // 
            // chkuseSSLWikto
            // 
            this.chkuseSSLWikto.AutoSize = true;
            this.chkuseSSLWikto.BackColor = System.Drawing.Color.Transparent;
            this.chkuseSSLWikto.Location = new System.Drawing.Point(156, 42);
            this.chkuseSSLWikto.Name = "chkuseSSLWikto";
            this.chkuseSSLWikto.Size = new System.Drawing.Size(41, 16);
            this.chkuseSSLWikto.TabIndex = 8;
            this.chkuseSSLWikto.Text = "SSL";
            this.toolTip1.SetToolTip(this.chkuseSSLWikto, "If you are scanning a secure web server(https), ensure that this is checked.");
            this.chkuseSSLWikto.UseVisualStyleBackColor = true;
            this.chkuseSSLWikto.CheckedChanged += new System.EventHandler(this.chkuseSSLWikto_CheckedChanged);
            // 
            // txtNiktoPort
            // 
            this.txtNiktoPort.BackColor = System.Drawing.Color.Snow;
            this.txtNiktoPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNiktoPort.Location = new System.Drawing.Point(6, 41);
            this.txtNiktoPort.Name = "txtNiktoPort";
            this.txtNiktoPort.Size = new System.Drawing.Size(112, 18);
            this.txtNiktoPort.TabIndex = 7;
            this.txtNiktoPort.Text = "80";
            this.txtNiktoPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.txtNiktoPort, "The target port (ie: 80 for http, 443 for https)");
            // 
            // txtNiktoTarget
            // 
            this.txtNiktoTarget.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.txtNiktoTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNiktoTarget.Location = new System.Drawing.Point(6, 17);
            this.txtNiktoTarget.Name = "txtNiktoTarget";
            this.txtNiktoTarget.Size = new System.Drawing.Size(112, 18);
            this.txtNiktoTarget.TabIndex = 6;
            this.txtNiktoTarget.Text = "localhost";
            this.txtNiktoTarget.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtNiktoTarget, "The target host (ie: localhost)");
            // 
            // skinButtonRed1
            // 
            this.skinButtonRed1.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.skinButtonRed1.ForeColor = System.Drawing.Color.Brown;
            this.skinButtonRed1.Location = new System.Drawing.Point(121, 59);
            this.skinButtonRed1.Name = "skinButtonRed1";
            this.skinButtonRed1.Size = new System.Drawing.Size(86, 28);
            this.skinButtonRed1.TabIndex = 4;
            this.skinButtonRed1.Text = "Quit";
            this.toolTip1.SetToolTip(this.skinButtonRed1, "Quit Wikto");
            this.skinButtonRed1.UseVisualStyleBackColor = true;
            this.skinButtonRed1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btn_WiktoStop
            // 
            this.btn_WiktoStop.Enabled = false;
            this.btn_WiktoStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_WiktoStop.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btn_WiktoStop.Location = new System.Drawing.Point(121, 32);
            this.btn_WiktoStop.Name = "btn_WiktoStop";
            this.btn_WiktoStop.Size = new System.Drawing.Size(86, 28);
            this.btn_WiktoStop.TabIndex = 3;
            this.btn_WiktoStop.Text = "Stop";
            this.toolTip1.SetToolTip(this.btn_WiktoStop, "Stop the Scan");
            this.btn_WiktoStop.UseVisualStyleBackColor = true;
            this.btn_WiktoStop.Click += new System.EventHandler(this.btnStopNikto_Click);
            // 
            // btn_WiktoStart
            // 
            this.btn_WiktoStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_WiktoStart.ForeColor = System.Drawing.Color.ForestGreen;
            this.btn_WiktoStart.Location = new System.Drawing.Point(121, 5);
            this.btn_WiktoStart.Name = "btn_WiktoStart";
            this.btn_WiktoStart.Size = new System.Drawing.Size(86, 28);
            this.btn_WiktoStart.TabIndex = 2;
            this.btn_WiktoStart.Text = "Start";
            this.toolTip1.SetToolTip(this.btn_WiktoStart, "Start the Scan");
            this.btn_WiktoStart.UseVisualStyleBackColor = true;
            this.btn_WiktoStart.Click += new System.EventHandler(this.btnStartNikto_Click);
            // 
            // btnGoogleQuit
            // 
            this.btnGoogleQuit.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btnGoogleQuit.ForeColor = System.Drawing.Color.Brown;
            this.btnGoogleQuit.Location = new System.Drawing.Point(121, 59);
            this.btnGoogleQuit.Name = "btnGoogleQuit";
            this.btnGoogleQuit.Size = new System.Drawing.Size(86, 28);
            this.btnGoogleQuit.TabIndex = 4;
            this.btnGoogleQuit.Text = "Quit";
            this.toolTip1.SetToolTip(this.btnGoogleQuit, "Quit Wikto");
            this.btnGoogleQuit.UseVisualStyleBackColor = true;
            this.btnGoogleQuit.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btnStopGoole
            // 
            this.btnStopGoole.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btnStopGoole.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btnStopGoole.Location = new System.Drawing.Point(121, 32);
            this.btnStopGoole.Name = "btnStopGoole";
            this.btnStopGoole.Size = new System.Drawing.Size(86, 28);
            this.btnStopGoole.TabIndex = 3;
            this.btnStopGoole.Text = "Stop";
            this.toolTip1.SetToolTip(this.btnStopGoole, "Stop the Scan");
            this.btnStopGoole.UseVisualStyleBackColor = true;
            this.btnStopGoole.Click += new System.EventHandler(this.btnStopGoole_Click);
            // 
            // btnGoogleStart
            // 
            this.btnGoogleStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btnGoogleStart.ForeColor = System.Drawing.Color.ForestGreen;
            this.btnGoogleStart.Location = new System.Drawing.Point(121, 5);
            this.btnGoogleStart.Name = "btnGoogleStart";
            this.btnGoogleStart.Size = new System.Drawing.Size(86, 28);
            this.btnGoogleStart.TabIndex = 2;
            this.btnGoogleStart.Text = "Start";
            this.toolTip1.SetToolTip(this.btnGoogleStart, "Start the Scan");
            this.btnGoogleStart.UseVisualStyleBackColor = true;
            this.btnGoogleStart.Click += new System.EventHandler(this.btnGoogleStart_Click);
            // 
            // txtWords
            // 
            this.txtWords.BackColor = System.Drawing.Color.Snow;
            this.txtWords.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWords.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.txtWords.Location = new System.Drawing.Point(71, 122);
            this.txtWords.Name = "txtWords";
            this.txtWords.Size = new System.Drawing.Size(136, 18);
            this.txtWords.TabIndex = 6;
            this.txtWords.Text = "htm,html,asp,pl,php,cgi,aspx,wsdl,xml,xls,sh,csv,txt,doc,pdf,mdb,zip";
            this.txtWords.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtWords, "Filetypes to scan for in order to obtain directory information from Google");
            // 
            // txtGoogleKeyword
            // 
            this.txtGoogleKeyword.BackColor = System.Drawing.Color.Snow;
            this.txtGoogleKeyword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGoogleKeyword.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.txtGoogleKeyword.Location = new System.Drawing.Point(71, 146);
            this.txtGoogleKeyword.Name = "txtGoogleKeyword";
            this.txtGoogleKeyword.Size = new System.Drawing.Size(136, 18);
            this.txtGoogleKeyword.TabIndex = 7;
            this.txtGoogleKeyword.Text = "localhost";
            this.txtGoogleKeyword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtGoogleKeyword, "A keyword to use in your Google Search");
            // 
            // txtGoogleTarget
            // 
            this.txtGoogleTarget.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.txtGoogleTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGoogleTarget.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.txtGoogleTarget.Location = new System.Drawing.Point(71, 98);
            this.txtGoogleTarget.Name = "txtGoogleTarget";
            this.txtGoogleTarget.Size = new System.Drawing.Size(136, 18);
            this.txtGoogleTarget.TabIndex = 5;
            this.txtGoogleTarget.Text = "localhost";
            this.txtGoogleTarget.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtGoogleTarget, "The hostname to scan for (ie: localhost).  This will become part of the site: dir" +
                    "ective for Google");
            this.txtGoogleTarget.Enter += new System.EventHandler(this.btnStopGoole_Click);
            this.txtGoogleTarget.Leave += new System.EventHandler(this.populatekeyword);
            // 
            // chk_SpiderSSL
            // 
            this.chk_SpiderSSL.AutoSize = true;
            this.chk_SpiderSSL.BackColor = System.Drawing.Color.Transparent;
            this.chk_SpiderSSL.Location = new System.Drawing.Point(51, 165);
            this.chk_SpiderSSL.Name = "chk_SpiderSSL";
            this.chk_SpiderSSL.Size = new System.Drawing.Size(41, 16);
            this.chk_SpiderSSL.TabIndex = 258;
            this.chk_SpiderSSL.Text = "SSL";
            this.toolTip1.SetToolTip(this.chk_SpiderSSL, "If you are scanning a secure web server(https), ensure that this is checked.");
            this.chk_SpiderSSL.UseVisualStyleBackColor = true;
            this.chk_SpiderSSL.CheckedChanged += new System.EventHandler(this.chk_SpiderSSL_CheckedChanged);
            // 
            // txt_SpiderPort
            // 
            this.txt_SpiderPort.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txt_SpiderPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_SpiderPort.Location = new System.Drawing.Point(51, 130);
            this.txt_SpiderPort.Name = "txt_SpiderPort";
            this.txt_SpiderPort.Size = new System.Drawing.Size(156, 18);
            this.txt_SpiderPort.TabIndex = 256;
            this.txt_SpiderPort.Text = "80";
            this.txt_SpiderPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.toolTip1.SetToolTip(this.txt_SpiderPort, "The target port (ie: 80 for http, 443 for https)");
            // 
            // txtHTTarget
            // 
            this.txtHTTarget.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.txtHTTarget.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHTTarget.Location = new System.Drawing.Point(51, 99);
            this.txtHTTarget.Name = "txtHTTarget";
            this.txtHTTarget.Size = new System.Drawing.Size(156, 18);
            this.txtHTTarget.TabIndex = 5;
            this.txtHTTarget.Text = "localhost";
            this.txtHTTarget.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txtHTTarget, "The target host (ie: localhost)");
            // 
            // btn_MirrorQuit
            // 
            this.btn_MirrorQuit.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_MirrorQuit.ForeColor = System.Drawing.Color.Brown;
            this.btn_MirrorQuit.Location = new System.Drawing.Point(121, 59);
            this.btn_MirrorQuit.Name = "btn_MirrorQuit";
            this.btn_MirrorQuit.Size = new System.Drawing.Size(86, 28);
            this.btn_MirrorQuit.TabIndex = 4;
            this.btn_MirrorQuit.Text = "Quit";
            this.toolTip1.SetToolTip(this.btn_MirrorQuit, "Quit Wikto");
            this.btn_MirrorQuit.UseVisualStyleBackColor = true;
            this.btn_MirrorQuit.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // btnHTStop
            // 
            this.btnHTStop.Enabled = false;
            this.btnHTStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btnHTStop.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.btnHTStop.Location = new System.Drawing.Point(121, 32);
            this.btnHTStop.Name = "btnHTStop";
            this.btnHTStop.Size = new System.Drawing.Size(86, 28);
            this.btnHTStop.TabIndex = 3;
            this.btnHTStop.Text = "Stop";
            this.toolTip1.SetToolTip(this.btnHTStop, "Stop the Scan");
            this.btnHTStop.UseVisualStyleBackColor = true;
            this.btnHTStop.Click += new System.EventHandler(this.btnHTStop_Click);
            // 
            // btnHTStart
            // 
            this.btnHTStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btnHTStart.ForeColor = System.Drawing.Color.ForestGreen;
            this.btnHTStart.Location = new System.Drawing.Point(121, 5);
            this.btnHTStart.Name = "btnHTStart";
            this.btnHTStart.Size = new System.Drawing.Size(86, 28);
            this.btnHTStart.TabIndex = 2;
            this.btnHTStart.Text = "Start";
            this.toolTip1.SetToolTip(this.btnHTStart, "Start the Scan");
            this.btnHTStart.UseVisualStyleBackColor = true;
            this.btnHTStart.Click += new System.EventHandler(this.btn_InsieWinsieSpider);
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.Mirror);
            this.tabControl1.Controls.Add(this.Googler);
            this.tabControl1.Controls.Add(this.BackEndMiner);
            this.tabControl1.Controls.Add(this.NiktoIsh);
            this.tabControl1.Controls.Add(this.GoogleHacks);
            this.tabControl1.Controls.Add(this.pnl_configleft);
            this.tabControl1.Controls.Add(this.tab_wizard);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1013, 741);
            this.tabControl1.TabIndex = 0;
            this.toolTip1.SetToolTip(this.tabControl1, "Pause / Resume a Scan");
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.CheckIfIShouldStartTheWizard);
            // 
            // Mirror
            // 
            this.Mirror.BackColor = System.Drawing.Color.DarkGray;
            this.Mirror.Controls.Add(this.tpnlMirror);
            this.Mirror.Controls.Add(this.pnlMirrorLeft);
            this.Mirror.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.Mirror.Location = new System.Drawing.Point(4, 24);
            this.Mirror.Name = "Mirror";
            this.Mirror.Size = new System.Drawing.Size(1005, 713);
            this.Mirror.TabIndex = 5;
            this.Mirror.Text = "Spider";
            this.Mirror.UseVisualStyleBackColor = true;
            // 
            // tpnlMirror
            // 
            this.tpnlMirror.ColumnCount = 1;
            this.tpnlMirror.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnlMirror.Controls.Add(this.tpnlMirrorLinks, 0, 1);
            this.tpnlMirror.Controls.Add(this.tpnlMirrorDir, 0, 0);
            this.tpnlMirror.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnlMirror.Location = new System.Drawing.Point(224, 0);
            this.tpnlMirror.Margin = new System.Windows.Forms.Padding(0);
            this.tpnlMirror.Name = "tpnlMirror";
            this.tpnlMirror.RowCount = 2;
            this.tpnlMirror.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlMirror.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlMirror.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpnlMirror.Size = new System.Drawing.Size(781, 713);
            this.tpnlMirror.TabIndex = 147;
            // 
            // tpnlMirrorLinks
            // 
            this.tpnlMirrorLinks.ColumnCount = 1;
            this.tpnlMirrorLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnlMirrorLinks.Controls.Add(this.tpnlMirrorLinkTop, 0, 0);
            this.tpnlMirrorLinks.Controls.Add(this.lstMirrorLinks, 0, 1);
            this.tpnlMirrorLinks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnlMirrorLinks.Location = new System.Drawing.Point(0, 356);
            this.tpnlMirrorLinks.Margin = new System.Windows.Forms.Padding(0);
            this.tpnlMirrorLinks.Name = "tpnlMirrorLinks";
            this.tpnlMirrorLinks.RowCount = 2;
            this.tpnlMirrorLinks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnlMirrorLinks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnlMirrorLinks.Size = new System.Drawing.Size(781, 357);
            this.tpnlMirrorLinks.TabIndex = 16;
            // 
            // tpnlMirrorLinkTop
            // 
            this.tpnlMirrorLinkTop.ColumnCount = 2;
            this.tpnlMirrorLinkTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlMirrorLinkTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlMirrorLinkTop.Controls.Add(this.pnlMirrorLinkLeft, 0, 0);
            this.tpnlMirrorLinkTop.Controls.Add(this.pnlMrrorLinkRight, 1, 0);
            this.tpnlMirrorLinkTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnlMirrorLinkTop.Location = new System.Drawing.Point(0, 0);
            this.tpnlMirrorLinkTop.Margin = new System.Windows.Forms.Padding(0);
            this.tpnlMirrorLinkTop.Name = "tpnlMirrorLinkTop";
            this.tpnlMirrorLinkTop.RowCount = 1;
            this.tpnlMirrorLinkTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlMirrorLinkTop.Size = new System.Drawing.Size(781, 30);
            this.tpnlMirrorLinkTop.TabIndex = 0;
            // 
            // pnlMirrorLinkLeft
            // 
            this.pnlMirrorLinkLeft.Controls.Add(this.label34);
            this.pnlMirrorLinkLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMirrorLinkLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlMirrorLinkLeft.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMirrorLinkLeft.Name = "pnlMirrorLinkLeft";
            this.pnlMirrorLinkLeft.Size = new System.Drawing.Size(390, 30);
            this.pnlMirrorLinkLeft.TabIndex = 13;
            // 
            // label34
            // 
            this.label34.Location = new System.Drawing.Point(1, 10);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(104, 16);
            this.label34.TabIndex = 91;
            this.label34.Text = "External Links";
            // 
            // pnlMrrorLinkRight
            // 
            this.pnlMrrorLinkRight.Controls.Add(this.btn_MirrorClearLinks);
            this.pnlMrrorLinkRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMrrorLinkRight.Location = new System.Drawing.Point(390, 0);
            this.pnlMrrorLinkRight.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMrrorLinkRight.Name = "pnlMrrorLinkRight";
            this.pnlMrrorLinkRight.Size = new System.Drawing.Size(391, 30);
            this.pnlMrrorLinkRight.TabIndex = 14;
            // 
            // btn_MirrorClearLinks
            // 
            this.btn_MirrorClearLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_MirrorClearLinks.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_MirrorClearLinks.ForeColor = System.Drawing.Color.Brown;
            this.btn_MirrorClearLinks.Location = new System.Drawing.Point(319, 2);
            this.btn_MirrorClearLinks.Name = "btn_MirrorClearLinks";
            this.btn_MirrorClearLinks.Size = new System.Drawing.Size(69, 28);
            this.btn_MirrorClearLinks.TabIndex = 15;
            this.btn_MirrorClearLinks.Text = "Clear List";
            this.btn_MirrorClearLinks.UseVisualStyleBackColor = true;
            this.btn_MirrorClearLinks.Click += new System.EventHandler(this.btnHTCleanLinks_Click);
            // 
            // lstMirrorLinks
            // 
            this.lstMirrorLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstMirrorLinks.BackColor = System.Drawing.Color.Snow;
            this.lstMirrorLinks.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstMirrorLinks.FormattingEnabled = true;
            this.lstMirrorLinks.ItemHeight = 12;
            this.lstMirrorLinks.Location = new System.Drawing.Point(3, 33);
            this.lstMirrorLinks.Name = "lstMirrorLinks";
            this.lstMirrorLinks.Size = new System.Drawing.Size(775, 314);
            this.lstMirrorLinks.TabIndex = 17;
            // 
            // tpnlMirrorDir
            // 
            this.tpnlMirrorDir.ColumnCount = 1;
            this.tpnlMirrorDir.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnlMirrorDir.Controls.Add(this.lstMirrorDirs, 0, 1);
            this.tpnlMirrorDir.Controls.Add(this.tpnlMirrorDirTop, 0, 0);
            this.tpnlMirrorDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnlMirrorDir.Location = new System.Drawing.Point(0, 0);
            this.tpnlMirrorDir.Margin = new System.Windows.Forms.Padding(0);
            this.tpnlMirrorDir.Name = "tpnlMirrorDir";
            this.tpnlMirrorDir.RowCount = 2;
            this.tpnlMirrorDir.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnlMirrorDir.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnlMirrorDir.Size = new System.Drawing.Size(781, 356);
            this.tpnlMirrorDir.TabIndex = 11;
            // 
            // lstMirrorDirs
            // 
            this.lstMirrorDirs.BackColor = System.Drawing.Color.Snow;
            this.lstMirrorDirs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstMirrorDirs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMirrorDirs.FormattingEnabled = true;
            this.lstMirrorDirs.ItemHeight = 12;
            this.lstMirrorDirs.Location = new System.Drawing.Point(3, 33);
            this.lstMirrorDirs.Name = "lstMirrorDirs";
            this.lstMirrorDirs.Size = new System.Drawing.Size(775, 314);
            this.lstMirrorDirs.TabIndex = 12;
            // 
            // tpnlMirrorDirTop
            // 
            this.tpnlMirrorDirTop.ColumnCount = 2;
            this.tpnlMirrorDirTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlMirrorDirTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlMirrorDirTop.Controls.Add(this.pnlMirrorDirLeft, 0, 0);
            this.tpnlMirrorDirTop.Controls.Add(this.pnlMirrorDirRight, 1, 0);
            this.tpnlMirrorDirTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnlMirrorDirTop.Location = new System.Drawing.Point(0, 0);
            this.tpnlMirrorDirTop.Margin = new System.Windows.Forms.Padding(0);
            this.tpnlMirrorDirTop.Name = "tpnlMirrorDirTop";
            this.tpnlMirrorDirTop.RowCount = 1;
            this.tpnlMirrorDirTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlMirrorDirTop.Size = new System.Drawing.Size(781, 30);
            this.tpnlMirrorDirTop.TabIndex = 0;
            // 
            // pnlMirrorDirLeft
            // 
            this.pnlMirrorDirLeft.Controls.Add(this.label31);
            this.pnlMirrorDirLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMirrorDirLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlMirrorDirLeft.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMirrorDirLeft.Name = "pnlMirrorDirLeft";
            this.pnlMirrorDirLeft.Size = new System.Drawing.Size(390, 30);
            this.pnlMirrorDirLeft.TabIndex = 8;
            // 
            // label31
            // 
            this.label31.Location = new System.Drawing.Point(1, 8);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(104, 16);
            this.label31.TabIndex = 83;
            this.label31.Text = "Directories mined";
            // 
            // pnlMirrorDirRight
            // 
            this.pnlMirrorDirRight.Controls.Add(this.btn_MirrorClearDirs);
            this.pnlMirrorDirRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMirrorDirRight.Location = new System.Drawing.Point(390, 0);
            this.pnlMirrorDirRight.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMirrorDirRight.Name = "pnlMirrorDirRight";
            this.pnlMirrorDirRight.Size = new System.Drawing.Size(391, 30);
            this.pnlMirrorDirRight.TabIndex = 9;
            // 
            // btn_MirrorClearDirs
            // 
            this.btn_MirrorClearDirs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_MirrorClearDirs.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_MirrorClearDirs.ForeColor = System.Drawing.Color.Brown;
            this.btn_MirrorClearDirs.Location = new System.Drawing.Point(319, 0);
            this.btn_MirrorClearDirs.Name = "btn_MirrorClearDirs";
            this.btn_MirrorClearDirs.Size = new System.Drawing.Size(69, 28);
            this.btn_MirrorClearDirs.TabIndex = 10;
            this.btn_MirrorClearDirs.Text = "Clear List";
            this.btn_MirrorClearDirs.UseVisualStyleBackColor = true;
            this.btn_MirrorClearDirs.Click += new System.EventHandler(this.skinButtonRed3_Click);
            // 
            // pnlMirrorLeft
            // 
            this.pnlMirrorLeft.AutoScroll = true;
            this.pnlMirrorLeft.AutoScrollMinSize = new System.Drawing.Size(220, 670);
            this.pnlMirrorLeft.BackColor = System.Drawing.Color.Gray;
            this.pnlMirrorLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMirrorLeft.Controls.Add(this.pictureBox1);
            this.pnlMirrorLeft.Controls.Add(this.chk_SpiderSSL);
            this.pnlMirrorLeft.Controls.Add(this.label10);
            this.pnlMirrorLeft.Controls.Add(this.txt_SpiderPort);
            this.pnlMirrorLeft.Controls.Add(this.lblMirrorStatus);
            this.pnlMirrorLeft.Controls.Add(this.pictureBox6);
            this.pnlMirrorLeft.Controls.Add(this.txtHTTarget);
            this.pnlMirrorLeft.Controls.Add(this.btn_MirrorQuit);
            this.pnlMirrorLeft.Controls.Add(this.btnHTStop);
            this.pnlMirrorLeft.Controls.Add(this.btnHTStart);
            this.pnlMirrorLeft.Controls.Add(this.label32);
            this.pnlMirrorLeft.Controls.Add(this.prgHT);
            this.pnlMirrorLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMirrorLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlMirrorLeft.Name = "pnlMirrorLeft";
            this.pnlMirrorLeft.Size = new System.Drawing.Size(224, 713);
            this.pnlMirrorLeft.TabIndex = 1;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(71, 220);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(76, 69);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 259;
            this.pictureBox1.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(5, 132);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(22, 12);
            this.label10.TabIndex = 257;
            this.label10.Text = "Port";
            // 
            // lblMirrorStatus
            // 
            this.lblMirrorStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMirrorStatus.Location = new System.Drawing.Point(5, 674);
            this.lblMirrorStatus.Name = "lblMirrorStatus";
            this.lblMirrorStatus.Size = new System.Drawing.Size(196, 13);
            this.lblMirrorStatus.TabIndex = 255;
            this.lblMirrorStatus.Text = "Not Running";
            this.lblMirrorStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox6
            // 
            this.pictureBox6.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox6.BackgroundImage")));
            this.pictureBox6.Location = new System.Drawing.Point(7, 5);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(108, 84);
            this.pictureBox6.TabIndex = 119;
            this.pictureBox6.TabStop = false;
            // 
            // label32
            // 
            this.label32.Location = new System.Drawing.Point(5, 101);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(40, 16);
            this.label32.TabIndex = 121;
            this.label32.Text = "Target";
            // 
            // prgHT
            // 
            this.prgHT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.prgHT.BackColor = System.Drawing.Color.DarkGray;
            this.prgHT.Location = new System.Drawing.Point(7, 690);
            this.prgHT.Name = "prgHT";
            this.prgHT.Size = new System.Drawing.Size(194, 13);
            this.prgHT.TabIndex = 255;
            // 
            // Googler
            // 
            this.Googler.BackColor = System.Drawing.Color.Gray;
            this.Googler.Controls.Add(this.tpnlGoogleMain);
            this.Googler.Controls.Add(this.pnlGoogleLeft);
            this.Googler.Controls.Add(this.lblQuery);
            this.Googler.Location = new System.Drawing.Point(4, 25);
            this.Googler.Name = "Googler";
            this.Googler.Size = new System.Drawing.Size(1005, 712);
            this.Googler.TabIndex = 1;
            this.Googler.Text = "Googler";
            this.Googler.UseVisualStyleBackColor = true;
            // 
            // tpnlGoogleMain
            // 
            this.tpnlGoogleMain.BackColor = System.Drawing.Color.DarkGray;
            this.tpnlGoogleMain.ColumnCount = 1;
            this.tpnlGoogleMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnlGoogleMain.Controls.Add(this.panel3, 0, 2);
            this.tpnlGoogleMain.Controls.Add(this.tpnlGoogleDir, 0, 0);
            this.tpnlGoogleMain.Controls.Add(this.tpnlGoogleLink, 0, 1);
            this.tpnlGoogleMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnlGoogleMain.Location = new System.Drawing.Point(224, 0);
            this.tpnlGoogleMain.Margin = new System.Windows.Forms.Padding(0);
            this.tpnlGoogleMain.Name = "tpnlGoogleMain";
            this.tpnlGoogleMain.RowCount = 3;
            this.tpnlGoogleMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlGoogleMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlGoogleMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tpnlGoogleMain.Size = new System.Drawing.Size(781, 712);
            this.tpnlGoogleMain.TabIndex = 148;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label57);
            this.panel3.Controls.Add(this.label53);
            this.panel3.Controls.Add(this.txtGoogleQuery);
            this.panel3.Controls.Add(this.lblEstimate);
            this.panel3.Controls.Add(this.lblPageNumber);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 632);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(781, 80);
            this.panel3.TabIndex = 18;
            // 
            // label57
            // 
            this.label57.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label57.Location = new System.Drawing.Point(3, 47);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(775, 13);
            this.label57.TabIndex = 74;
            this.label57.Text = "Estimated Progress:";
            this.label57.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label53
            // 
            this.label53.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label53.Location = new System.Drawing.Point(0, 11);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(781, 12);
            this.label53.TabIndex = 72;
            this.label53.Text = "Current Google Query:";
            this.label53.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtGoogleQuery
            // 
            this.txtGoogleQuery.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGoogleQuery.BackColor = System.Drawing.Color.Snow;
            this.txtGoogleQuery.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGoogleQuery.Location = new System.Drawing.Point(3, 26);
            this.txtGoogleQuery.Name = "txtGoogleQuery";
            this.txtGoogleQuery.ReadOnly = true;
            this.txtGoogleQuery.Size = new System.Drawing.Size(775, 18);
            this.txtGoogleQuery.TabIndex = 29;
            this.txtGoogleQuery.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblEstimate
            // 
            this.lblEstimate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEstimate.BackColor = System.Drawing.Color.Snow;
            this.lblEstimate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblEstimate.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.lblEstimate.Location = new System.Drawing.Point(742, 64);
            this.lblEstimate.Name = "lblEstimate";
            this.lblEstimate.Size = new System.Drawing.Size(36, 16);
            this.lblEstimate.TabIndex = 21;
            this.lblEstimate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPageNumber
            // 
            this.lblPageNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPageNumber.BackColor = System.Drawing.Color.Snow;
            this.lblPageNumber.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPageNumber.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.lblPageNumber.Location = new System.Drawing.Point(3, 64);
            this.lblPageNumber.Name = "lblPageNumber";
            this.lblPageNumber.Size = new System.Drawing.Size(36, 16);
            this.lblPageNumber.TabIndex = 20;
            this.lblPageNumber.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tpnlGoogleDir
            // 
            this.tpnlGoogleDir.ColumnCount = 1;
            this.tpnlGoogleDir.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnlGoogleDir.Controls.Add(this.tpnlGoogleDirTop, 0, 0);
            this.tpnlGoogleDir.Controls.Add(this.pnlGoogleDirMain, 0, 1);
            this.tpnlGoogleDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnlGoogleDir.Location = new System.Drawing.Point(0, 0);
            this.tpnlGoogleDir.Margin = new System.Windows.Forms.Padding(0);
            this.tpnlGoogleDir.Name = "tpnlGoogleDir";
            this.tpnlGoogleDir.RowCount = 2;
            this.tpnlGoogleDir.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnlGoogleDir.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnlGoogleDir.Size = new System.Drawing.Size(781, 316);
            this.tpnlGoogleDir.TabIndex = 1;
            // 
            // tpnlGoogleDirTop
            // 
            this.tpnlGoogleDirTop.ColumnCount = 2;
            this.tpnlGoogleDirTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlGoogleDirTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlGoogleDirTop.Controls.Add(this.pnlGoogleDirLeft, 0, 0);
            this.tpnlGoogleDirTop.Controls.Add(this.pnlGoogleDirRight, 1, 0);
            this.tpnlGoogleDirTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnlGoogleDirTop.Location = new System.Drawing.Point(0, 0);
            this.tpnlGoogleDirTop.Margin = new System.Windows.Forms.Padding(0);
            this.tpnlGoogleDirTop.Name = "tpnlGoogleDirTop";
            this.tpnlGoogleDirTop.RowCount = 1;
            this.tpnlGoogleDirTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnlGoogleDirTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpnlGoogleDirTop.Size = new System.Drawing.Size(781, 30);
            this.tpnlGoogleDirTop.TabIndex = 0;
            // 
            // pnlGoogleDirLeft
            // 
            this.pnlGoogleDirLeft.Controls.Add(this.label20);
            this.pnlGoogleDirLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGoogleDirLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlGoogleDirLeft.Margin = new System.Windows.Forms.Padding(0);
            this.pnlGoogleDirLeft.Name = "pnlGoogleDirLeft";
            this.pnlGoogleDirLeft.Size = new System.Drawing.Size(390, 30);
            this.pnlGoogleDirLeft.TabIndex = 8;
            // 
            // label20
            // 
            this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.label20.Location = new System.Drawing.Point(1, 8);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(104, 12);
            this.label20.TabIndex = 82;
            this.label20.Text = "Mined Directories";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlGoogleDirRight
            // 
            this.pnlGoogleDirRight.Controls.Add(this.btnGoogleClearDir);
            this.pnlGoogleDirRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGoogleDirRight.Location = new System.Drawing.Point(390, 0);
            this.pnlGoogleDirRight.Margin = new System.Windows.Forms.Padding(0);
            this.pnlGoogleDirRight.Name = "pnlGoogleDirRight";
            this.pnlGoogleDirRight.Size = new System.Drawing.Size(391, 30);
            this.pnlGoogleDirRight.TabIndex = 9;
            // 
            // btnGoogleClearDir
            // 
            this.btnGoogleClearDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGoogleClearDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btnGoogleClearDir.ForeColor = System.Drawing.Color.Brown;
            this.btnGoogleClearDir.Location = new System.Drawing.Point(319, 0);
            this.btnGoogleClearDir.Name = "btnGoogleClearDir";
            this.btnGoogleClearDir.Size = new System.Drawing.Size(69, 28);
            this.btnGoogleClearDir.TabIndex = 10;
            this.btnGoogleClearDir.Text = "Clear List";
            this.btnGoogleClearDir.UseVisualStyleBackColor = true;
            this.btnGoogleClearDir.Click += new System.EventHandler(this.btnGoogleClearDir_Click);
            // 
            // pnlGoogleDirMain
            // 
            this.pnlGoogleDirMain.Controls.Add(this.lstGoogleDir);
            this.pnlGoogleDirMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGoogleDirMain.Location = new System.Drawing.Point(3, 33);
            this.pnlGoogleDirMain.Name = "pnlGoogleDirMain";
            this.pnlGoogleDirMain.Size = new System.Drawing.Size(775, 280);
            this.pnlGoogleDirMain.TabIndex = 11;
            // 
            // lstGoogleDir
            // 
            this.lstGoogleDir.BackColor = System.Drawing.Color.Snow;
            this.lstGoogleDir.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstGoogleDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstGoogleDir.FormattingEnabled = true;
            this.lstGoogleDir.ItemHeight = 12;
            this.lstGoogleDir.Location = new System.Drawing.Point(0, 0);
            this.lstGoogleDir.Name = "lstGoogleDir";
            this.lstGoogleDir.Size = new System.Drawing.Size(775, 278);
            this.lstGoogleDir.TabIndex = 12;
            // 
            // tpnlGoogleLink
            // 
            this.tpnlGoogleLink.ColumnCount = 1;
            this.tpnlGoogleLink.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnlGoogleLink.Controls.Add(this.tpnlGoogleLinkTop, 0, 0);
            this.tpnlGoogleLink.Controls.Add(this.pnlGoogleLinkMain, 0, 1);
            this.tpnlGoogleLink.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnlGoogleLink.Location = new System.Drawing.Point(0, 316);
            this.tpnlGoogleLink.Margin = new System.Windows.Forms.Padding(0);
            this.tpnlGoogleLink.Name = "tpnlGoogleLink";
            this.tpnlGoogleLink.RowCount = 2;
            this.tpnlGoogleLink.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnlGoogleLink.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnlGoogleLink.Size = new System.Drawing.Size(781, 316);
            this.tpnlGoogleLink.TabIndex = 2;
            // 
            // tpnlGoogleLinkTop
            // 
            this.tpnlGoogleLinkTop.ColumnCount = 2;
            this.tpnlGoogleLinkTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlGoogleLinkTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlGoogleLinkTop.Controls.Add(this.pnlGoogleLinkLeft, 0, 0);
            this.tpnlGoogleLinkTop.Controls.Add(this.pnlGoogleLinkRight, 1, 0);
            this.tpnlGoogleLinkTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnlGoogleLinkTop.Location = new System.Drawing.Point(0, 0);
            this.tpnlGoogleLinkTop.Margin = new System.Windows.Forms.Padding(0);
            this.tpnlGoogleLinkTop.Name = "tpnlGoogleLinkTop";
            this.tpnlGoogleLinkTop.RowCount = 1;
            this.tpnlGoogleLinkTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnlGoogleLinkTop.Size = new System.Drawing.Size(781, 30);
            this.tpnlGoogleLinkTop.TabIndex = 0;
            // 
            // pnlGoogleLinkLeft
            // 
            this.pnlGoogleLinkLeft.Controls.Add(this.label7);
            this.pnlGoogleLinkLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGoogleLinkLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlGoogleLinkLeft.Margin = new System.Windows.Forms.Padding(0);
            this.pnlGoogleLinkLeft.Name = "pnlGoogleLinkLeft";
            this.pnlGoogleLinkLeft.Size = new System.Drawing.Size(390, 30);
            this.pnlGoogleLinkLeft.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.label7.Location = new System.Drawing.Point(1, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 12);
            this.label7.TabIndex = 83;
            this.label7.Text = "Mined Links";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlGoogleLinkRight
            // 
            this.pnlGoogleLinkRight.Controls.Add(this.btnGoogleClearLink);
            this.pnlGoogleLinkRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGoogleLinkRight.Location = new System.Drawing.Point(390, 0);
            this.pnlGoogleLinkRight.Margin = new System.Windows.Forms.Padding(0);
            this.pnlGoogleLinkRight.Name = "pnlGoogleLinkRight";
            this.pnlGoogleLinkRight.Size = new System.Drawing.Size(391, 30);
            this.pnlGoogleLinkRight.TabIndex = 14;
            // 
            // btnGoogleClearLink
            // 
            this.btnGoogleClearLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGoogleClearLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btnGoogleClearLink.ForeColor = System.Drawing.Color.Brown;
            this.btnGoogleClearLink.Location = new System.Drawing.Point(319, 0);
            this.btnGoogleClearLink.Name = "btnGoogleClearLink";
            this.btnGoogleClearLink.Size = new System.Drawing.Size(69, 28);
            this.btnGoogleClearLink.TabIndex = 15;
            this.btnGoogleClearLink.Text = "Clear List";
            this.btnGoogleClearLink.UseVisualStyleBackColor = true;
            this.btnGoogleClearLink.Click += new System.EventHandler(this.btnGoogleClearLink_Click);
            // 
            // pnlGoogleLinkMain
            // 
            this.pnlGoogleLinkMain.Controls.Add(this.lstGoogleLink);
            this.pnlGoogleLinkMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGoogleLinkMain.Location = new System.Drawing.Point(3, 33);
            this.pnlGoogleLinkMain.Name = "pnlGoogleLinkMain";
            this.pnlGoogleLinkMain.Size = new System.Drawing.Size(775, 280);
            this.pnlGoogleLinkMain.TabIndex = 16;
            // 
            // lstGoogleLink
            // 
            this.lstGoogleLink.BackColor = System.Drawing.Color.Snow;
            this.lstGoogleLink.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstGoogleLink.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstGoogleLink.FormattingEnabled = true;
            this.lstGoogleLink.HorizontalExtent = 2048;
            this.lstGoogleLink.HorizontalScrollbar = true;
            this.lstGoogleLink.ItemHeight = 12;
            this.lstGoogleLink.Location = new System.Drawing.Point(0, 0);
            this.lstGoogleLink.Name = "lstGoogleLink";
            this.lstGoogleLink.ScrollAlwaysVisible = true;
            this.lstGoogleLink.Size = new System.Drawing.Size(775, 278);
            this.lstGoogleLink.TabIndex = 17;
            // 
            // pnlGoogleLeft
            // 
            this.pnlGoogleLeft.AutoScroll = true;
            this.pnlGoogleLeft.AutoScrollMinSize = new System.Drawing.Size(220, 670);
            this.pnlGoogleLeft.BackColor = System.Drawing.Color.Gray;
            this.pnlGoogleLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlGoogleLeft.Controls.Add(this.lblGoogleStatus);
            this.pnlGoogleLeft.Controls.Add(this.pictureBox4);
            this.pnlGoogleLeft.Controls.Add(this.btnGoogleQuit);
            this.pnlGoogleLeft.Controls.Add(this.btnStopGoole);
            this.pnlGoogleLeft.Controls.Add(this.btnGoogleStart);
            this.pnlGoogleLeft.Controls.Add(this.label54);
            this.pnlGoogleLeft.Controls.Add(this.prgGoogle);
            this.pnlGoogleLeft.Controls.Add(this.label55);
            this.pnlGoogleLeft.Controls.Add(this.label23);
            this.pnlGoogleLeft.Controls.Add(this.txtWords);
            this.pnlGoogleLeft.Controls.Add(this.txtGoogleKeyword);
            this.pnlGoogleLeft.Controls.Add(this.label22);
            this.pnlGoogleLeft.Controls.Add(this.txtGoogleTarget);
            this.pnlGoogleLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlGoogleLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlGoogleLeft.Name = "pnlGoogleLeft";
            this.pnlGoogleLeft.Size = new System.Drawing.Size(224, 712);
            this.pnlGoogleLeft.TabIndex = 1;
            // 
            // lblGoogleStatus
            // 
            this.lblGoogleStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGoogleStatus.Location = new System.Drawing.Point(5, 608);
            this.lblGoogleStatus.Name = "lblGoogleStatus";
            this.lblGoogleStatus.Size = new System.Drawing.Size(196, 60);
            this.lblGoogleStatus.TabIndex = 255;
            this.lblGoogleStatus.Text = "Not Running";
            this.lblGoogleStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox4.BackgroundImage")));
            this.pictureBox4.Location = new System.Drawing.Point(7, 5);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(108, 84);
            this.pictureBox4.TabIndex = 119;
            this.pictureBox4.TabStop = false;
            // 
            // label54
            // 
            this.label54.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label54.Location = new System.Drawing.Point(5, 670);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(196, 13);
            this.label54.TabIndex = 255;
            this.label54.Text = "Google Mining Progress:";
            this.label54.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // prgGoogle
            // 
            this.prgGoogle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.prgGoogle.BackColor = System.Drawing.Color.DarkGray;
            this.prgGoogle.Location = new System.Drawing.Point(7, 690);
            this.prgGoogle.Maximum = 1000;
            this.prgGoogle.Name = "prgGoogle";
            this.prgGoogle.Size = new System.Drawing.Size(194, 13);
            this.prgGoogle.TabIndex = 255;
            // 
            // label55
            // 
            this.label55.Location = new System.Drawing.Point(5, 100);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(40, 16);
            this.label55.TabIndex = 121;
            this.label55.Text = "Site";
            // 
            // label23
            // 
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.label23.Location = new System.Drawing.Point(5, 124);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(60, 16);
            this.label23.TabIndex = 79;
            this.label23.Text = "File Types";
            // 
            // label22
            // 
            this.label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.label22.Location = new System.Drawing.Point(5, 149);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(61, 16);
            this.label22.TabIndex = 77;
            this.label22.Text = "Keyword";
            // 
            // lblQuery
            // 
            this.lblQuery.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblQuery.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblQuery.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.lblQuery.Location = new System.Drawing.Point(8, 438);
            this.lblQuery.Name = "lblQuery";
            this.lblQuery.Size = new System.Drawing.Size(208, 17);
            this.lblQuery.TabIndex = 67;
            this.lblQuery.Text = "Google Query Status";
            this.lblQuery.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BackEndMiner
            // 
            this.BackEndMiner.BackColor = System.Drawing.Color.Gray;
            this.BackEndMiner.Controls.Add(this.pnl_BackEndMain);
            this.BackEndMiner.Controls.Add(this.panel4);
            this.BackEndMiner.Location = new System.Drawing.Point(4, 25);
            this.BackEndMiner.Name = "BackEndMiner";
            this.BackEndMiner.Size = new System.Drawing.Size(1005, 712);
            this.BackEndMiner.TabIndex = 0;
            this.BackEndMiner.Text = "BackEnd";
            this.BackEndMiner.UseVisualStyleBackColor = true;
            // 
            // pnl_BackEndMain
            // 
            this.pnl_BackEndMain.BackColor = System.Drawing.Color.DarkGray;
            this.pnl_BackEndMain.Controls.Add(this.tpnl_BackendMain);
            this.pnl_BackEndMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BackEndMain.Location = new System.Drawing.Point(224, 0);
            this.pnl_BackEndMain.Margin = new System.Windows.Forms.Padding(0);
            this.pnl_BackEndMain.Name = "pnl_BackEndMain";
            this.pnl_BackEndMain.Size = new System.Drawing.Size(781, 712);
            this.pnl_BackEndMain.TabIndex = 250;
            // 
            // tpnl_BackendMain
            // 
            this.tpnl_BackendMain.ColumnCount = 1;
            this.tpnl_BackendMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnl_BackendMain.Controls.Add(this.pnl_BackEndTop, 0, 0);
            this.tpnl_BackendMain.Controls.Add(this.pnl_BackEndBottom, 0, 1);
            this.tpnl_BackendMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_BackendMain.Location = new System.Drawing.Point(0, 0);
            this.tpnl_BackendMain.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_BackendMain.Name = "tpnl_BackendMain";
            this.tpnl_BackendMain.RowCount = 2;
            this.tpnl_BackendMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnl_BackendMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnl_BackendMain.Size = new System.Drawing.Size(781, 712);
            this.tpnl_BackendMain.TabIndex = 0;
            // 
            // pnl_BackEndTop
            // 
            this.pnl_BackEndTop.Controls.Add(this.tpnl_BackEndTop);
            this.pnl_BackEndTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BackEndTop.Location = new System.Drawing.Point(0, 0);
            this.pnl_BackEndTop.Margin = new System.Windows.Forms.Padding(0);
            this.pnl_BackEndTop.Name = "pnl_BackEndTop";
            this.pnl_BackEndTop.Size = new System.Drawing.Size(781, 356);
            this.pnl_BackEndTop.TabIndex = 150;
            // 
            // tpnl_BackEndTop
            // 
            this.tpnl_BackEndTop.ColumnCount = 3;
            this.tpnl_BackEndTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tpnl_BackEndTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tpnl_BackEndTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tpnl_BackEndTop.Controls.Add(this.pnl_BETopLeft1, 0, 0);
            this.tpnl_BackEndTop.Controls.Add(this.pbl_TopLeft3, 0, 1);
            this.tpnl_BackEndTop.Controls.Add(this.pnl_BETopMid1, 1, 0);
            this.tpnl_BackEndTop.Controls.Add(this.pnl_BETopMid3, 1, 1);
            this.tpnl_BackEndTop.Controls.Add(this.pnl_BETopRight1, 2, 0);
            this.tpnl_BackEndTop.Controls.Add(this.pnl_BETopRight3, 2, 1);
            this.tpnl_BackEndTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_BackEndTop.Location = new System.Drawing.Point(0, 0);
            this.tpnl_BackEndTop.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_BackEndTop.Name = "tpnl_BackEndTop";
            this.tpnl_BackEndTop.RowCount = 2;
            this.tpnl_BackEndTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnl_BackEndTop.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnl_BackEndTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpnl_BackEndTop.Size = new System.Drawing.Size(781, 356);
            this.tpnl_BackEndTop.TabIndex = 0;
            // 
            // pnl_BETopLeft1
            // 
            this.pnl_BETopLeft1.Controls.Add(this.btn_BEInDirImportM);
            this.pnl_BETopLeft1.Controls.Add(this.btn_BEImportInDirG);
            this.pnl_BETopLeft1.Controls.Add(this.label13);
            this.pnl_BETopLeft1.Controls.Add(this.btn_BEInDirClear);
            this.pnl_BETopLeft1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BETopLeft1.Location = new System.Drawing.Point(3, 3);
            this.pnl_BETopLeft1.Name = "pnl_BETopLeft1";
            this.pnl_BETopLeft1.Size = new System.Drawing.Size(254, 24);
            this.pnl_BETopLeft1.TabIndex = 28;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(-2, 4);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(64, 16);
            this.label13.TabIndex = 161;
            this.label13.Text = "Directories";
            // 
            // btn_BEInDirClear
            // 
            this.btn_BEInDirClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_BEInDirClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_BEInDirClear.ForeColor = System.Drawing.Color.Brown;
            this.btn_BEInDirClear.Location = new System.Drawing.Point(187, 0);
            this.btn_BEInDirClear.Name = "btn_BEInDirClear";
            this.btn_BEInDirClear.Size = new System.Drawing.Size(68, 21);
            this.btn_BEInDirClear.TabIndex = 32;
            this.btn_BEInDirClear.Text = "Clear List";
            this.btn_BEInDirClear.UseVisualStyleBackColor = true;
            this.btn_BEInDirClear.Click += new System.EventHandler(this.btn_BEInDirClear_Click);
            // 
            // pbl_TopLeft3
            // 
            this.pbl_TopLeft3.Controls.Add(this.txtInDirs);
            this.pbl_TopLeft3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbl_TopLeft3.Location = new System.Drawing.Point(3, 33);
            this.pbl_TopLeft3.Name = "pbl_TopLeft3";
            this.pbl_TopLeft3.Size = new System.Drawing.Size(254, 320);
            this.pbl_TopLeft3.TabIndex = 33;
            // 
            // txtInDirs
            // 
            this.txtInDirs.BackColor = System.Drawing.Color.Snow;
            this.txtInDirs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInDirs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInDirs.Location = new System.Drawing.Point(0, 0);
            this.txtInDirs.Name = "txtInDirs";
            this.txtInDirs.Size = new System.Drawing.Size(254, 320);
            this.txtInDirs.TabIndex = 34;
            this.txtInDirs.Text = "";
            this.txtInDirs.WordWrap = false;
            // 
            // pnl_BETopMid1
            // 
            this.pnl_BETopMid1.Controls.Add(this.btn_BEInFileClear);
            this.pnl_BETopMid1.Controls.Add(this.label15);
            this.pnl_BETopMid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BETopMid1.Location = new System.Drawing.Point(263, 3);
            this.pnl_BETopMid1.Name = "pnl_BETopMid1";
            this.pnl_BETopMid1.Size = new System.Drawing.Size(254, 24);
            this.pnl_BETopMid1.TabIndex = 35;
            // 
            // btn_BEInFileClear
            // 
            this.btn_BEInFileClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_BEInFileClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_BEInFileClear.ForeColor = System.Drawing.Color.Brown;
            this.btn_BEInFileClear.Location = new System.Drawing.Point(185, 0);
            this.btn_BEInFileClear.Name = "btn_BEInFileClear";
            this.btn_BEInFileClear.Size = new System.Drawing.Size(68, 21);
            this.btn_BEInFileClear.TabIndex = 37;
            this.btn_BEInFileClear.Text = "Clear List";
            this.btn_BEInFileClear.UseVisualStyleBackColor = true;
            this.btn_BEInFileClear.Click += new System.EventHandler(this.btn_BEInFileClear_Click);
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(-2, 4);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(142, 16);
            this.label15.TabIndex = 158;
            this.label15.Text = "Files";
            // 
            // pnl_BETopMid3
            // 
            this.pnl_BETopMid3.Controls.Add(this.txtInFiles);
            this.pnl_BETopMid3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BETopMid3.Location = new System.Drawing.Point(263, 33);
            this.pnl_BETopMid3.Name = "pnl_BETopMid3";
            this.pnl_BETopMid3.Size = new System.Drawing.Size(254, 320);
            this.pnl_BETopMid3.TabIndex = 38;
            // 
            // txtInFiles
            // 
            this.txtInFiles.BackColor = System.Drawing.Color.Snow;
            this.txtInFiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInFiles.Location = new System.Drawing.Point(0, 0);
            this.txtInFiles.Name = "txtInFiles";
            this.txtInFiles.Size = new System.Drawing.Size(254, 320);
            this.txtInFiles.TabIndex = 39;
            this.txtInFiles.Text = "";
            this.txtInFiles.WordWrap = false;
            // 
            // pnl_BETopRight1
            // 
            this.pnl_BETopRight1.Controls.Add(this.btn_BEInExtClear);
            this.pnl_BETopRight1.Controls.Add(this.label38);
            this.pnl_BETopRight1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BETopRight1.Location = new System.Drawing.Point(523, 3);
            this.pnl_BETopRight1.Name = "pnl_BETopRight1";
            this.pnl_BETopRight1.Size = new System.Drawing.Size(255, 24);
            this.pnl_BETopRight1.TabIndex = 40;
            // 
            // btn_BEInExtClear
            // 
            this.btn_BEInExtClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_BEInExtClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_BEInExtClear.ForeColor = System.Drawing.Color.Brown;
            this.btn_BEInExtClear.Location = new System.Drawing.Point(185, 0);
            this.btn_BEInExtClear.Name = "btn_BEInExtClear";
            this.btn_BEInExtClear.Size = new System.Drawing.Size(68, 21);
            this.btn_BEInExtClear.TabIndex = 42;
            this.btn_BEInExtClear.Text = "Clear List";
            this.btn_BEInExtClear.UseVisualStyleBackColor = true;
            this.btn_BEInExtClear.Click += new System.EventHandler(this.btn_BEInExtClear_Click);
            // 
            // label38
            // 
            this.label38.Location = new System.Drawing.Point(-2, 4);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(60, 16);
            this.label38.TabIndex = 174;
            this.label38.Text = "File types";
            // 
            // pnl_BETopRight3
            // 
            this.pnl_BETopRight3.Controls.Add(this.txtInFileTypes);
            this.pnl_BETopRight3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BETopRight3.Location = new System.Drawing.Point(523, 33);
            this.pnl_BETopRight3.Name = "pnl_BETopRight3";
            this.pnl_BETopRight3.Size = new System.Drawing.Size(255, 320);
            this.pnl_BETopRight3.TabIndex = 43;
            // 
            // txtInFileTypes
            // 
            this.txtInFileTypes.BackColor = System.Drawing.Color.Snow;
            this.txtInFileTypes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInFileTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInFileTypes.Location = new System.Drawing.Point(0, 0);
            this.txtInFileTypes.Name = "txtInFileTypes";
            this.txtInFileTypes.Size = new System.Drawing.Size(255, 320);
            this.txtInFileTypes.TabIndex = 44;
            this.txtInFileTypes.Text = "";
            this.txtInFileTypes.WordWrap = false;
            // 
            // pnl_BackEndBottom
            // 
            this.pnl_BackEndBottom.Controls.Add(this.tpnl_BEBottom1);
            this.pnl_BackEndBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BackEndBottom.Location = new System.Drawing.Point(3, 359);
            this.pnl_BackEndBottom.Name = "pnl_BackEndBottom";
            this.pnl_BackEndBottom.Size = new System.Drawing.Size(775, 350);
            this.pnl_BackEndBottom.TabIndex = 255;
            // 
            // tpnl_BEBottom1
            // 
            this.tpnl_BEBottom1.ColumnCount = 3;
            this.tpnl_BEBottom1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tpnl_BEBottom1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tpnl_BEBottom1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tpnl_BEBottom1.Controls.Add(this.tpnl_BEBottom2, 0, 0);
            this.tpnl_BEBottom1.Controls.Add(this.tpnl_BEBottomRight, 1, 0);
            this.tpnl_BEBottom1.Controls.Add(this.tpnl_BEFiles, 2, 0);
            this.tpnl_BEBottom1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_BEBottom1.Location = new System.Drawing.Point(0, 0);
            this.tpnl_BEBottom1.Name = "tpnl_BEBottom1";
            this.tpnl_BEBottom1.RowCount = 1;
            this.tpnl_BEBottom1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_BEBottom1.Size = new System.Drawing.Size(775, 350);
            this.tpnl_BEBottom1.TabIndex = 0;
            // 
            // tpnl_BEBottom2
            // 
            this.tpnl_BEBottom2.ColumnCount = 1;
            this.tpnl_BEBottom2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_BEBottom2.Controls.Add(this.pnl_BEBottomLeft1, 0, 0);
            this.tpnl_BEBottom2.Controls.Add(this.pnl_BEBottomLeft3, 0, 1);
            this.tpnl_BEBottom2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_BEBottom2.Location = new System.Drawing.Point(0, 0);
            this.tpnl_BEBottom2.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_BEBottom2.Name = "tpnl_BEBottom2";
            this.tpnl_BEBottom2.RowCount = 2;
            this.tpnl_BEBottom2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnl_BEBottom2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnl_BEBottom2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpnl_BEBottom2.Size = new System.Drawing.Size(258, 350);
            this.tpnl_BEBottom2.TabIndex = 0;
            // 
            // pnl_BEBottomLeft1
            // 
            this.pnl_BEBottomLeft1.Controls.Add(this.btn_BEOutDirClear);
            this.pnl_BEBottomLeft1.Controls.Add(this.label50);
            this.pnl_BEBottomLeft1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BEBottomLeft1.Location = new System.Drawing.Point(3, 3);
            this.pnl_BEBottomLeft1.Name = "pnl_BEBottomLeft1";
            this.pnl_BEBottomLeft1.Size = new System.Drawing.Size(252, 24);
            this.pnl_BEBottomLeft1.TabIndex = 0;
            // 
            // btn_BEOutDirClear
            // 
            this.btn_BEOutDirClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_BEOutDirClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_BEOutDirClear.ForeColor = System.Drawing.Color.Brown;
            this.btn_BEOutDirClear.Location = new System.Drawing.Point(187, 1);
            this.btn_BEOutDirClear.Name = "btn_BEOutDirClear";
            this.btn_BEOutDirClear.Size = new System.Drawing.Size(68, 21);
            this.btn_BEOutDirClear.TabIndex = 48;
            this.btn_BEOutDirClear.Text = "Clear List";
            this.btn_BEOutDirClear.UseVisualStyleBackColor = true;
            this.btn_BEOutDirClear.Click += new System.EventHandler(this.btn_BEOutDirClear_Click);
            // 
            // label50
            // 
            this.label50.Location = new System.Drawing.Point(-2, 5);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(201, 16);
            this.label50.TabIndex = 159;
            this.label50.Text = "Discovered Directories";
            // 
            // pnl_BEBottomLeft3
            // 
            this.pnl_BEBottomLeft3.Controls.Add(this.lstViewDirs);
            this.pnl_BEBottomLeft3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BEBottomLeft3.Location = new System.Drawing.Point(3, 33);
            this.pnl_BEBottomLeft3.Name = "pnl_BEBottomLeft3";
            this.pnl_BEBottomLeft3.Size = new System.Drawing.Size(252, 315);
            this.pnl_BEBottomLeft3.TabIndex = 49;
            // 
            // lstViewDirs
            // 
            this.lstViewDirs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstViewDirs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader4});
            this.lstViewDirs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstViewDirs.FullRowSelect = true;
            this.lstViewDirs.Location = new System.Drawing.Point(0, 0);
            this.lstViewDirs.MultiSelect = false;
            this.lstViewDirs.Name = "lstViewDirs";
            this.lstViewDirs.Size = new System.Drawing.Size(252, 315);
            this.lstViewDirs.SmallImageList = this.imageList1;
            this.lstViewDirs.TabIndex = 51;
            this.lstViewDirs.UseCompatibleStateImageBehavior = false;
            this.lstViewDirs.View = System.Windows.Forms.View.Details;
            this.lstViewDirs.DoubleClick += new System.EventHandler(this.lstViewDirs_DoubleClick);
            this.lstViewDirs.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstViewDirs_ColumnClick);
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Type";
            this.columnHeader7.Width = 40;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Directory name";
            this.columnHeader4.Width = 250;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.jpg");
            this.imageList1.Images.SetKeyName(1, "time.jpg");
            this.imageList1.Images.SetKeyName(2, "file.jpg");
            this.imageList1.Images.SetKeyName(3, "wikto3.jpg");
            this.imageList1.Images.SetKeyName(4, "wikto2.jpg");
            this.imageList1.Images.SetKeyName(5, "wikto1.jpg");
            // 
            // tpnl_BEBottomRight
            // 
            this.tpnl_BEBottomRight.ColumnCount = 1;
            this.tpnl_BEBottomRight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_BEBottomRight.Controls.Add(this.pnl_BEBottomRight1, 0, 0);
            this.tpnl_BEBottomRight.Controls.Add(this.pnl_BEBottomRight3, 0, 1);
            this.tpnl_BEBottomRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_BEBottomRight.Location = new System.Drawing.Point(258, 0);
            this.tpnl_BEBottomRight.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_BEBottomRight.Name = "tpnl_BEBottomRight";
            this.tpnl_BEBottomRight.RowCount = 2;
            this.tpnl_BEBottomRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnl_BEBottomRight.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnl_BEBottomRight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpnl_BEBottomRight.Size = new System.Drawing.Size(258, 350);
            this.tpnl_BEBottomRight.TabIndex = 1;
            // 
            // pnl_BEBottomRight1
            // 
            this.pnl_BEBottomRight1.Controls.Add(this.btn_BEOutIndexClear);
            this.pnl_BEBottomRight1.Controls.Add(this.label59);
            this.pnl_BEBottomRight1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BEBottomRight1.Location = new System.Drawing.Point(3, 3);
            this.pnl_BEBottomRight1.Name = "pnl_BEBottomRight1";
            this.pnl_BEBottomRight1.Size = new System.Drawing.Size(252, 24);
            this.pnl_BEBottomRight1.TabIndex = 0;
            // 
            // btn_BEOutIndexClear
            // 
            this.btn_BEOutIndexClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_BEOutIndexClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_BEOutIndexClear.ForeColor = System.Drawing.Color.Brown;
            this.btn_BEOutIndexClear.Location = new System.Drawing.Point(184, 0);
            this.btn_BEOutIndexClear.Name = "btn_BEOutIndexClear";
            this.btn_BEOutIndexClear.Size = new System.Drawing.Size(68, 21);
            this.btn_BEOutIndexClear.TabIndex = 52;
            this.btn_BEOutIndexClear.Text = "Clear List";
            this.btn_BEOutIndexClear.UseVisualStyleBackColor = true;
            this.btn_BEOutIndexClear.Click += new System.EventHandler(this.btn_BEOutIndexClear_Click);
            // 
            // label59
            // 
            this.label59.Location = new System.Drawing.Point(-2, 5);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(201, 16);
            this.label59.TabIndex = 160;
            this.label59.Text = "Indexable Directories";
            // 
            // pnl_BEBottomRight3
            // 
            this.pnl_BEBottomRight3.Controls.Add(this.lstViewIndexDirs);
            this.pnl_BEBottomRight3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BEBottomRight3.Location = new System.Drawing.Point(3, 33);
            this.pnl_BEBottomRight3.Name = "pnl_BEBottomRight3";
            this.pnl_BEBottomRight3.Size = new System.Drawing.Size(252, 315);
            this.pnl_BEBottomRight3.TabIndex = 53;
            // 
            // lstViewIndexDirs
            // 
            this.lstViewIndexDirs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstViewIndexDirs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader8,
            this.columnHeader5});
            this.lstViewIndexDirs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstViewIndexDirs.FullRowSelect = true;
            this.lstViewIndexDirs.Location = new System.Drawing.Point(0, 0);
            this.lstViewIndexDirs.MultiSelect = false;
            this.lstViewIndexDirs.Name = "lstViewIndexDirs";
            this.lstViewIndexDirs.Size = new System.Drawing.Size(252, 315);
            this.lstViewIndexDirs.SmallImageList = this.imageList1;
            this.lstViewIndexDirs.TabIndex = 55;
            this.lstViewIndexDirs.UseCompatibleStateImageBehavior = false;
            this.lstViewIndexDirs.View = System.Windows.Forms.View.Details;
            this.lstViewIndexDirs.DoubleClick += new System.EventHandler(this.lstViewDirs_DoubleClick);
            this.lstViewIndexDirs.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstViewDirs_ColumnClick);
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Type";
            this.columnHeader8.Width = 40;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Directory name";
            this.columnHeader5.Width = 250;
            // 
            // tpnl_BEFiles
            // 
            this.tpnl_BEFiles.ColumnCount = 1;
            this.tpnl_BEFiles.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_BEFiles.Controls.Add(this.pnl_BEFile1, 0, 0);
            this.tpnl_BEFiles.Controls.Add(this.pnl_BEFile3, 0, 1);
            this.tpnl_BEFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_BEFiles.Location = new System.Drawing.Point(516, 0);
            this.tpnl_BEFiles.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_BEFiles.Name = "tpnl_BEFiles";
            this.tpnl_BEFiles.RowCount = 2;
            this.tpnl_BEFiles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnl_BEFiles.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnl_BEFiles.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpnl_BEFiles.Size = new System.Drawing.Size(259, 350);
            this.tpnl_BEFiles.TabIndex = 2;
            // 
            // pnl_BEFile1
            // 
            this.pnl_BEFile1.Controls.Add(this.btn_BEOutFileClear);
            this.pnl_BEFile1.Controls.Add(this.label52);
            this.pnl_BEFile1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BEFile1.Location = new System.Drawing.Point(3, 3);
            this.pnl_BEFile1.Name = "pnl_BEFile1";
            this.pnl_BEFile1.Size = new System.Drawing.Size(253, 24);
            this.pnl_BEFile1.TabIndex = 0;
            // 
            // btn_BEOutFileClear
            // 
            this.btn_BEOutFileClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_BEOutFileClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_BEOutFileClear.ForeColor = System.Drawing.Color.Brown;
            this.btn_BEOutFileClear.Location = new System.Drawing.Point(186, 0);
            this.btn_BEOutFileClear.Name = "btn_BEOutFileClear";
            this.btn_BEOutFileClear.Size = new System.Drawing.Size(68, 21);
            this.btn_BEOutFileClear.TabIndex = 56;
            this.btn_BEOutFileClear.Text = "Clear List";
            this.btn_BEOutFileClear.UseVisualStyleBackColor = true;
            this.btn_BEOutFileClear.Click += new System.EventHandler(this.btn_BEOutFileClear_Click);
            // 
            // label52
            // 
            this.label52.Location = new System.Drawing.Point(-2, 5);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(141, 16);
            this.label52.TabIndex = 160;
            this.label52.Text = "Discovered Files";
            // 
            // pnl_BEFile3
            // 
            this.pnl_BEFile3.Controls.Add(this.lstViewFiles);
            this.pnl_BEFile3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_BEFile3.Location = new System.Drawing.Point(3, 33);
            this.pnl_BEFile3.Name = "pnl_BEFile3";
            this.pnl_BEFile3.Size = new System.Drawing.Size(253, 315);
            this.pnl_BEFile3.TabIndex = 57;
            // 
            // lstViewFiles
            // 
            this.lstViewFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader6});
            this.lstViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstViewFiles.FullRowSelect = true;
            this.lstViewFiles.Location = new System.Drawing.Point(0, 0);
            this.lstViewFiles.MultiSelect = false;
            this.lstViewFiles.Name = "lstViewFiles";
            this.lstViewFiles.Size = new System.Drawing.Size(253, 315);
            this.lstViewFiles.SmallImageList = this.imageList1;
            this.lstViewFiles.TabIndex = 59;
            this.lstViewFiles.UseCompatibleStateImageBehavior = false;
            this.lstViewFiles.View = System.Windows.Forms.View.Details;
            this.lstViewFiles.DoubleClick += new System.EventHandler(this.lstViewDirs_DoubleClick);
            this.lstViewFiles.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstViewDirs_ColumnClick);
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Type";
            this.columnHeader9.Width = 40;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Directory name";
            this.columnHeader6.Width = 250;
            // 
            // panel4
            // 
            this.panel4.AutoScroll = true;
            this.panel4.AutoScrollMinSize = new System.Drawing.Size(220, 670);
            this.panel4.AutoSize = true;
            this.panel4.BackColor = System.Drawing.Color.Gray;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.groupBox1);
            this.panel4.Controls.Add(this.btnBackEndPause);
            this.panel4.Controls.Add(this.chkPreserve);
            this.panel4.Controls.Add(this.btn_BESkiptoDirs);
            this.panel4.Controls.Add(this.btn_BEQuit);
            this.panel4.Controls.Add(this.btn_BEStop);
            this.panel4.Controls.Add(this.btn_BEStart);
            this.panel4.Controls.Add(this.btn_BESkiptoFiles);
            this.panel4.Controls.Add(this.btn_BackEndExport);
            this.panel4.Controls.Add(this.groupBox10);
            this.panel4.Controls.Add(this.groupBox11);
            this.panel4.Controls.Add(this.groupBox12);
            this.panel4.Controls.Add(this.label56);
            this.panel4.Controls.Add(this.pictureBox3);
            this.panel4.Controls.Add(this.label58);
            this.panel4.Controls.Add(this.prgsFiles);
            this.panel4.Controls.Add(this.prgsDirs);
            this.panel4.Controls.Add(this.lblStatus);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(224, 712);
            this.panel4.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nudTimeAnomaly);
            this.groupBox1.Controls.Add(this.chkTimeAnomalies);
            this.groupBox1.Location = new System.Drawing.Point(7, 531);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox1.Size = new System.Drawing.Size(212, 38);
            this.groupBox1.TabIndex = 257;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Time anomalies";
            // 
            // nudTimeAnomaly
            // 
            this.nudTimeAnomaly.BackColor = System.Drawing.Color.Snow;
            this.nudTimeAnomaly.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudTimeAnomaly.Enabled = false;
            this.nudTimeAnomaly.Location = new System.Drawing.Point(95, 14);
            this.nudTimeAnomaly.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudTimeAnomaly.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nudTimeAnomaly.Name = "nudTimeAnomaly";
            this.nudTimeAnomaly.ReadOnly = true;
            this.nudTimeAnomaly.Size = new System.Drawing.Size(100, 18);
            this.nudTimeAnomaly.TabIndex = 21;
            this.nudTimeAnomaly.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // chkTimeAnomalies
            // 
            this.chkTimeAnomalies.AutoSize = true;
            this.chkTimeAnomalies.BackColor = System.Drawing.Color.Transparent;
            this.chkTimeAnomalies.Location = new System.Drawing.Point(8, 17);
            this.chkTimeAnomalies.Name = "chkTimeAnomalies";
            this.chkTimeAnomalies.Size = new System.Drawing.Size(60, 16);
            this.chkTimeAnomalies.TabIndex = 12;
            this.chkTimeAnomalies.Text = "Activate";
            this.toolTip1.SetToolTip(this.chkTimeAnomalies, "Report findings with with unordinary time responses.");
            this.chkTimeAnomalies.UseVisualStyleBackColor = true;
            this.chkTimeAnomalies.CheckedChanged += new System.EventHandler(this.chkTimeAnomalies_CheckedChanged);
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.skinButton4);
            this.groupBox10.Controls.Add(this.skinButton5);
            this.groupBox10.Controls.Add(this.skinButton6);
            this.groupBox10.Controls.Add(this.radioHEAD);
            this.groupBox10.Controls.Add(this.radioGET);
            this.groupBox10.Controls.Add(this.btn_BEClearDB);
            this.groupBox10.Controls.Add(this.chkBackEndAI);
            this.groupBox10.Controls.Add(this.NUPDOWNBackEnd);
            this.groupBox10.Controls.Add(this.panel2);
            this.groupBox10.Location = new System.Drawing.Point(7, 356);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(212, 169);
            this.groupBox10.TabIndex = 18;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Trigger Control";
            // 
            // NUPDOWNBackEnd
            // 
            this.NUPDOWNBackEnd.BackColor = System.Drawing.Color.Snow;
            this.NUPDOWNBackEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NUPDOWNBackEnd.DecimalPlaces = 3;
            this.NUPDOWNBackEnd.Enabled = false;
            this.NUPDOWNBackEnd.Increment = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            this.NUPDOWNBackEnd.Location = new System.Drawing.Point(8, 36);
            this.NUPDOWNBackEnd.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.NUPDOWNBackEnd.Name = "NUPDOWNBackEnd";
            this.NUPDOWNBackEnd.Size = new System.Drawing.Size(100, 18);
            this.NUPDOWNBackEnd.TabIndex = 20;
            this.NUPDOWNBackEnd.Value = new decimal(new int[] {
            800,
            0,
            0,
            196608});
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.DarkGray;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.txtErrorCodeDir);
            this.panel2.Controls.Add(this.txtErrorCodeFile);
            this.panel2.Location = new System.Drawing.Point(4, 91);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(202, 52);
            this.panel2.TabIndex = 148;
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.DarkGray;
            this.label11.Location = new System.Drawing.Point(109, 5);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(81, 16);
            this.label11.TabIndex = 67;
            this.label11.Text = "Trig Codes(Dir)";
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.DarkGray;
            this.label14.Location = new System.Drawing.Point(109, 29);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(81, 16);
            this.label14.TabIndex = 82;
            this.label14.Text = "Trig Codes (File)";
            // 
            // groupBox11
            // 
            this.groupBox11.Controls.Add(this.btn_BELoadDirs);
            this.groupBox11.Controls.Add(this.btn_BEUpdateFromSP);
            this.groupBox11.Controls.Add(this.cmbBackEndUpdate);
            this.groupBox11.Controls.Add(this.btn_BELoadExts);
            this.groupBox11.Controls.Add(this.btn_BELoadFiles);
            this.groupBox11.Location = new System.Drawing.Point(7, 250);
            this.groupBox11.Name = "groupBox11";
            this.groupBox11.Size = new System.Drawing.Size(212, 100);
            this.groupBox11.TabIndex = 12;
            this.groupBox11.TabStop = false;
            this.groupBox11.Text = "Directory/File/Extension";
            // 
            // groupBox12
            // 
            this.groupBox12.Controls.Add(this.label9);
            this.groupBox12.Controls.Add(this.label8);
            this.groupBox12.Controls.Add(this.txtIPPort);
            this.groupBox12.Controls.Add(this.chkBackEnduseSSLport);
            this.groupBox12.Controls.Add(this.txtIPNumber);
            this.groupBox12.Location = new System.Drawing.Point(7, 172);
            this.groupBox12.Name = "groupBox12";
            this.groupBox12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox12.Size = new System.Drawing.Size(212, 72);
            this.groupBox12.TabIndex = 8;
            this.groupBox12.TabStop = false;
            this.groupBox12.Text = "Target selection";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(120, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 12);
            this.label9.TabIndex = 62;
            this.label9.Text = "Port";
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(120, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(84, 12);
            this.label8.TabIndex = 61;
            this.label8.Text = "IP/DNS name";
            // 
            // label56
            // 
            this.label56.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label56.Location = new System.Drawing.Point(5, 646);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(202, 13);
            this.label56.TabIndex = 255;
            this.label56.Text = "Directory Progress:";
            this.label56.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox3.BackgroundImage")));
            this.pictureBox3.Location = new System.Drawing.Point(7, 5);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(108, 84);
            this.pictureBox3.TabIndex = 119;
            this.pictureBox3.TabStop = false;
            // 
            // label58
            // 
            this.label58.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label58.Location = new System.Drawing.Point(5, 678);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(202, 13);
            this.label58.TabIndex = 255;
            this.label58.Text = "File Progress:";
            this.label58.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // prgsFiles
            // 
            this.prgsFiles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.prgsFiles.BackColor = System.Drawing.Color.DarkGray;
            this.prgsFiles.Location = new System.Drawing.Point(7, 694);
            this.prgsFiles.Maximum = 100000;
            this.prgsFiles.Name = "prgsFiles";
            this.prgsFiles.Size = new System.Drawing.Size(200, 13);
            this.prgsFiles.Step = 1;
            this.prgsFiles.TabIndex = 255;
            // 
            // prgsDirs
            // 
            this.prgsDirs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.prgsDirs.BackColor = System.Drawing.Color.DarkGray;
            this.prgsDirs.Location = new System.Drawing.Point(7, 662);
            this.prgsDirs.Maximum = 10000;
            this.prgsDirs.Name = "prgsDirs";
            this.prgsDirs.Size = new System.Drawing.Size(200, 13);
            this.prgsDirs.Step = 1;
            this.prgsDirs.TabIndex = 255;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.BackColor = System.Drawing.Color.Snow;
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatus.Location = new System.Drawing.Point(7, 628);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(200, 14);
            this.lblStatus.TabIndex = 255;
            this.lblStatus.Text = "Status";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // NiktoIsh
            // 
            this.NiktoIsh.BackColor = System.Drawing.Color.Gray;
            this.NiktoIsh.Controls.Add(this.pnl_WiktoMain);
            this.NiktoIsh.Controls.Add(this.panel5);
            this.NiktoIsh.Location = new System.Drawing.Point(4, 25);
            this.NiktoIsh.Name = "NiktoIsh";
            this.NiktoIsh.Size = new System.Drawing.Size(1005, 712);
            this.NiktoIsh.TabIndex = 2;
            this.NiktoIsh.Text = "Wikto";
            this.NiktoIsh.UseVisualStyleBackColor = true;
            // 
            // pnl_WiktoMain
            // 
            this.pnl_WiktoMain.BackColor = System.Drawing.Color.DarkGray;
            this.pnl_WiktoMain.Controls.Add(this.tpnl_WiktoMain);
            this.pnl_WiktoMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_WiktoMain.Location = new System.Drawing.Point(224, 0);
            this.pnl_WiktoMain.Margin = new System.Windows.Forms.Padding(0);
            this.pnl_WiktoMain.Name = "pnl_WiktoMain";
            this.pnl_WiktoMain.Size = new System.Drawing.Size(781, 712);
            this.pnl_WiktoMain.TabIndex = 149;
            // 
            // tpnl_WiktoMain
            // 
            this.tpnl_WiktoMain.ColumnCount = 3;
            this.tpnl_WiktoMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tpnl_WiktoMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tpnl_WiktoMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tpnl_WiktoMain.Controls.Add(this.tpnl_WiktoB1, 0, 1);
            this.tpnl_WiktoMain.Controls.Add(this.tpnl_WiktoB2, 1, 1);
            this.tpnl_WiktoMain.Controls.Add(this.tpnl_WiktoB3, 2, 1);
            this.tpnl_WiktoMain.Controls.Add(this.tpnl_WiktoT3, 2, 0);
            this.tpnl_WiktoMain.Controls.Add(this.tpnl_WiktoT2, 1, 0);
            this.tpnl_WiktoMain.Controls.Add(this.tpnl_WiktoT1, 0, 0);
            this.tpnl_WiktoMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_WiktoMain.Location = new System.Drawing.Point(0, 0);
            this.tpnl_WiktoMain.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_WiktoMain.Name = "tpnl_WiktoMain";
            this.tpnl_WiktoMain.RowCount = 2;
            this.tpnl_WiktoMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnl_WiktoMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tpnl_WiktoMain.Size = new System.Drawing.Size(781, 712);
            this.tpnl_WiktoMain.TabIndex = 0;
            // 
            // tpnl_WiktoB1
            // 
            this.tpnl_WiktoB1.ColumnCount = 1;
            this.tpnl_WiktoB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_WiktoB1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpnl_WiktoB1.Controls.Add(this.panpnl_WiktoBL1, 0, 0);
            this.tpnl_WiktoB1.Controls.Add(this.panpnl_WiktoBL2, 0, 1);
            this.tpnl_WiktoB1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_WiktoB1.Location = new System.Drawing.Point(0, 356);
            this.tpnl_WiktoB1.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_WiktoB1.Name = "tpnl_WiktoB1";
            this.tpnl_WiktoB1.RowCount = 2;
            this.tpnl_WiktoB1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnl_WiktoB1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnl_WiktoB1.Size = new System.Drawing.Size(312, 356);
            this.tpnl_WiktoB1.TabIndex = 3;
            // 
            // panpnl_WiktoBL1
            // 
            this.panpnl_WiktoBL1.Controls.Add(this.label60);
            this.panpnl_WiktoBL1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panpnl_WiktoBL1.Location = new System.Drawing.Point(3, 3);
            this.panpnl_WiktoBL1.Name = "panpnl_WiktoBL1";
            this.panpnl_WiktoBL1.Size = new System.Drawing.Size(306, 24);
            this.panpnl_WiktoBL1.TabIndex = 33;
            // 
            // label60
            // 
            this.label60.Location = new System.Drawing.Point(-2, 0);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(161, 16);
            this.label60.TabIndex = 181;
            this.label60.Text = "Nikto Results";
            // 
            // panpnl_WiktoBL2
            // 
            this.panpnl_WiktoBL2.Controls.Add(this.lvw_NiktoResults);
            this.panpnl_WiktoBL2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panpnl_WiktoBL2.Location = new System.Drawing.Point(3, 33);
            this.panpnl_WiktoBL2.Name = "panpnl_WiktoBL2";
            this.panpnl_WiktoBL2.Size = new System.Drawing.Size(306, 321);
            this.panpnl_WiktoBL2.TabIndex = 34;
            // 
            // lvw_NiktoResults
            // 
            this.lvw_NiktoResults.BackColor = System.Drawing.Color.Snow;
            this.lvw_NiktoResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvw_NiktoResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvw_NiktoResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvw_NiktoResults.FullRowSelect = true;
            this.lvw_NiktoResults.Location = new System.Drawing.Point(0, 0);
            this.lvw_NiktoResults.MultiSelect = false;
            this.lvw_NiktoResults.Name = "lvw_NiktoResults";
            this.lvw_NiktoResults.Size = new System.Drawing.Size(306, 321);
            this.lvw_NiktoResults.SmallImageList = this.imageList1;
            this.lvw_NiktoResults.TabIndex = 35;
            this.lvw_NiktoResults.UseCompatibleStateImageBehavior = false;
            this.lvw_NiktoResults.View = System.Windows.Forms.View.Details;
            this.lvw_NiktoResults.DoubleClick += new System.EventHandler(this.lvw_NiktoResults_DoubleClick);
            this.lvw_NiktoResults.Resize += new System.EventHandler(this.ResizeListViews);
            this.lvw_NiktoResults.SelectedIndexChanged += new System.EventHandler(this.populateNiktoDescvuln);
            this.lvw_NiktoResults.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lstViewDirs_ColumnClick);
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "Type";
            this.columnHeader10.Width = 40;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Weight";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Trigger";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Request";
            // 
            // tpnl_WiktoB2
            // 
            this.tpnl_WiktoB2.ColumnCount = 1;
            this.tpnl_WiktoB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_WiktoB2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpnl_WiktoB2.Controls.Add(this.panpnl_WiktoBM1, 0, 0);
            this.tpnl_WiktoB2.Controls.Add(this.panpnl_WiktoBM2, 0, 1);
            this.tpnl_WiktoB2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_WiktoB2.Location = new System.Drawing.Point(312, 356);
            this.tpnl_WiktoB2.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_WiktoB2.Name = "tpnl_WiktoB2";
            this.tpnl_WiktoB2.RowCount = 2;
            this.tpnl_WiktoB2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnl_WiktoB2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnl_WiktoB2.Size = new System.Drawing.Size(234, 356);
            this.tpnl_WiktoB2.TabIndex = 4;
            // 
            // panpnl_WiktoBM1
            // 
            this.panpnl_WiktoBM1.Controls.Add(this.label61);
            this.panpnl_WiktoBM1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panpnl_WiktoBM1.Location = new System.Drawing.Point(3, 3);
            this.panpnl_WiktoBM1.Name = "panpnl_WiktoBM1";
            this.panpnl_WiktoBM1.Size = new System.Drawing.Size(228, 24);
            this.panpnl_WiktoBM1.TabIndex = 36;
            // 
            // label61
            // 
            this.label61.Location = new System.Drawing.Point(-2, 0);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(161, 16);
            this.label61.TabIndex = 181;
            this.label61.Text = "HTTP Request";
            // 
            // panpnl_WiktoBM2
            // 
            this.panpnl_WiktoBM2.Controls.Add(this.txtNiktoReq);
            this.panpnl_WiktoBM2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panpnl_WiktoBM2.Location = new System.Drawing.Point(3, 33);
            this.panpnl_WiktoBM2.Name = "panpnl_WiktoBM2";
            this.panpnl_WiktoBM2.Size = new System.Drawing.Size(228, 321);
            this.panpnl_WiktoBM2.TabIndex = 37;
            // 
            // txtNiktoReq
            // 
            this.txtNiktoReq.BackColor = System.Drawing.Color.Snow;
            this.txtNiktoReq.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNiktoReq.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNiktoReq.Location = new System.Drawing.Point(0, 0);
            this.txtNiktoReq.Multiline = true;
            this.txtNiktoReq.Name = "txtNiktoReq";
            this.txtNiktoReq.ReadOnly = true;
            this.txtNiktoReq.Size = new System.Drawing.Size(228, 321);
            this.txtNiktoReq.TabIndex = 38;
            // 
            // tpnl_WiktoB3
            // 
            this.tpnl_WiktoB3.ColumnCount = 1;
            this.tpnl_WiktoB3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tpnl_WiktoB3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpnl_WiktoB3.Controls.Add(this.panpnl_WiktoBR1, 0, 0);
            this.tpnl_WiktoB3.Controls.Add(this.panpnl_WiktoBR2, 0, 1);
            this.tpnl_WiktoB3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_WiktoB3.Location = new System.Drawing.Point(546, 356);
            this.tpnl_WiktoB3.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_WiktoB3.Name = "tpnl_WiktoB3";
            this.tpnl_WiktoB3.RowCount = 2;
            this.tpnl_WiktoB3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnl_WiktoB3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnl_WiktoB3.Size = new System.Drawing.Size(235, 356);
            this.tpnl_WiktoB3.TabIndex = 5;
            // 
            // panpnl_WiktoBR1
            // 
            this.panpnl_WiktoBR1.Controls.Add(this.label62);
            this.panpnl_WiktoBR1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panpnl_WiktoBR1.Location = new System.Drawing.Point(3, 3);
            this.panpnl_WiktoBR1.Name = "panpnl_WiktoBR1";
            this.panpnl_WiktoBR1.Size = new System.Drawing.Size(255, 24);
            this.panpnl_WiktoBR1.TabIndex = 39;
            // 
            // label62
            // 
            this.label62.Location = new System.Drawing.Point(-2, 0);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(161, 16);
            this.label62.TabIndex = 181;
            this.label62.Text = "HTTP Response";
            // 
            // panpnl_WiktoBR2
            // 
            this.panpnl_WiktoBR2.Controls.Add(this.txtNiktoRes);
            this.panpnl_WiktoBR2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panpnl_WiktoBR2.Location = new System.Drawing.Point(3, 33);
            this.panpnl_WiktoBR2.Name = "panpnl_WiktoBR2";
            this.panpnl_WiktoBR2.Size = new System.Drawing.Size(255, 321);
            this.panpnl_WiktoBR2.TabIndex = 40;
            // 
            // txtNiktoRes
            // 
            this.txtNiktoRes.BackColor = System.Drawing.Color.Snow;
            this.txtNiktoRes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNiktoRes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNiktoRes.HideSelection = false;
            this.txtNiktoRes.Location = new System.Drawing.Point(0, 0);
            this.txtNiktoRes.Multiline = true;
            this.txtNiktoRes.Name = "txtNiktoRes";
            this.txtNiktoRes.ReadOnly = true;
            this.txtNiktoRes.Size = new System.Drawing.Size(255, 321);
            this.txtNiktoRes.TabIndex = 41;
            // 
            // tpnl_WiktoT3
            // 
            this.tpnl_WiktoT3.ColumnCount = 1;
            this.tpnl_WiktoT3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tpnl_WiktoT3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpnl_WiktoT3.Controls.Add(this.pnl_WiktoTR1, 0, 0);
            this.tpnl_WiktoT3.Controls.Add(this.pnl_WiktoTR2, 0, 1);
            this.tpnl_WiktoT3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_WiktoT3.Location = new System.Drawing.Point(546, 0);
            this.tpnl_WiktoT3.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_WiktoT3.Name = "tpnl_WiktoT3";
            this.tpnl_WiktoT3.RowCount = 2;
            this.tpnl_WiktoT3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnl_WiktoT3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnl_WiktoT3.Size = new System.Drawing.Size(235, 356);
            this.tpnl_WiktoT3.TabIndex = 2;
            // 
            // pnl_WiktoTR1
            // 
            this.pnl_WiktoTR1.Controls.Add(this.label26);
            this.pnl_WiktoTR1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_WiktoTR1.Location = new System.Drawing.Point(3, 3);
            this.pnl_WiktoTR1.Name = "pnl_WiktoTR1";
            this.pnl_WiktoTR1.Size = new System.Drawing.Size(255, 24);
            this.pnl_WiktoTR1.TabIndex = 30;
            // 
            // label26
            // 
            this.label26.Location = new System.Drawing.Point(0, 3);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(178, 16);
            this.label26.TabIndex = 179;
            this.label26.Text = "Nikto Database Description:";
            // 
            // pnl_WiktoTR2
            // 
            this.pnl_WiktoTR2.Controls.Add(this.lvw_NiktoDesc);
            this.pnl_WiktoTR2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_WiktoTR2.Location = new System.Drawing.Point(3, 33);
            this.pnl_WiktoTR2.Name = "pnl_WiktoTR2";
            this.pnl_WiktoTR2.Size = new System.Drawing.Size(255, 320);
            this.pnl_WiktoTR2.TabIndex = 31;
            // 
            // lvw_NiktoDesc
            // 
            this.lvw_NiktoDesc.BackColor = System.Drawing.Color.Snow;
            this.lvw_NiktoDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvw_NiktoDesc.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col_ndbd,
            this.col_ndv});
            this.lvw_NiktoDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvw_NiktoDesc.FullRowSelect = true;
            this.lvw_NiktoDesc.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvw_NiktoDesc.Location = new System.Drawing.Point(0, 0);
            this.lvw_NiktoDesc.Name = "lvw_NiktoDesc";
            this.lvw_NiktoDesc.Size = new System.Drawing.Size(255, 320);
            this.lvw_NiktoDesc.TabIndex = 32;
            this.lvw_NiktoDesc.UseCompatibleStateImageBehavior = false;
            this.lvw_NiktoDesc.View = System.Windows.Forms.View.Details;
            this.lvw_NiktoDesc.Resize += new System.EventHandler(this.ResizeListViews);
            // 
            // col_ndbd
            // 
            this.col_ndbd.Width = 100;
            // 
            // col_ndv
            // 
            this.col_ndv.Width = 500;
            // 
            // tpnl_WiktoT2
            // 
            this.tpnl_WiktoT2.ColumnCount = 1;
            this.tpnl_WiktoT2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_WiktoT2.Controls.Add(this.pnl_WiktoTM1, 0, 0);
            this.tpnl_WiktoT2.Controls.Add(this.pnl_WiktoTM2, 0, 1);
            this.tpnl_WiktoT2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_WiktoT2.Location = new System.Drawing.Point(312, 0);
            this.tpnl_WiktoT2.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_WiktoT2.Name = "tpnl_WiktoT2";
            this.tpnl_WiktoT2.RowCount = 2;
            this.tpnl_WiktoT2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnl_WiktoT2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnl_WiktoT2.Size = new System.Drawing.Size(234, 356);
            this.tpnl_WiktoT2.TabIndex = 1;
            // 
            // pnl_WiktoTM1
            // 
            this.pnl_WiktoTM1.Controls.Add(this.label12);
            this.pnl_WiktoTM1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_WiktoTM1.Location = new System.Drawing.Point(3, 3);
            this.pnl_WiktoTM1.Name = "pnl_WiktoTM1";
            this.pnl_WiktoTM1.Size = new System.Drawing.Size(228, 24);
            this.pnl_WiktoTM1.TabIndex = 27;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(-2, 3);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(161, 16);
            this.label12.TabIndex = 180;
            this.label12.Text = "Nikto Database Entries";
            // 
            // pnl_WiktoTM2
            // 
            this.pnl_WiktoTM2.Controls.Add(this.lvw_NiktoDb);
            this.pnl_WiktoTM2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_WiktoTM2.Location = new System.Drawing.Point(3, 33);
            this.pnl_WiktoTM2.Name = "pnl_WiktoTM2";
            this.pnl_WiktoTM2.Size = new System.Drawing.Size(228, 320);
            this.pnl_WiktoTM2.TabIndex = 28;
            // 
            // lvw_NiktoDb
            // 
            this.lvw_NiktoDb.AutoArrange = false;
            this.lvw_NiktoDb.BackColor = System.Drawing.Color.Snow;
            this.lvw_NiktoDb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvw_NiktoDb.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col_desc,
            this.col_target});
            this.lvw_NiktoDb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvw_NiktoDb.FullRowSelect = true;
            this.lvw_NiktoDb.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvw_NiktoDb.HideSelection = false;
            this.lvw_NiktoDb.LabelWrap = false;
            this.lvw_NiktoDb.Location = new System.Drawing.Point(0, 0);
            this.lvw_NiktoDb.MultiSelect = false;
            this.lvw_NiktoDb.Name = "lvw_NiktoDb";
            this.lvw_NiktoDb.ShowGroups = false;
            this.lvw_NiktoDb.Size = new System.Drawing.Size(228, 320);
            this.lvw_NiktoDb.TabIndex = 29;
            this.lvw_NiktoDb.UseCompatibleStateImageBehavior = false;
            this.lvw_NiktoDb.View = System.Windows.Forms.View.Details;
            this.lvw_NiktoDb.Resize += new System.EventHandler(this.ResizeListViews);
            this.lvw_NiktoDb.SelectedIndexChanged += new System.EventHandler(this.populateNiktoDesc);
            // 
            // col_desc
            // 
            this.col_desc.Text = "Trigger";
            this.col_desc.Width = 100;
            // 
            // col_target
            // 
            this.col_target.Text = "Request";
            this.col_target.Width = 100;
            // 
            // tpnl_WiktoT1
            // 
            this.tpnl_WiktoT1.ColumnCount = 1;
            this.tpnl_WiktoT1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_WiktoT1.Controls.Add(this.pnl_WiktoTL1, 0, 0);
            this.tpnl_WiktoT1.Controls.Add(this.pnl_WiktoTL3, 0, 1);
            this.tpnl_WiktoT1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_WiktoT1.Location = new System.Drawing.Point(0, 0);
            this.tpnl_WiktoT1.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_WiktoT1.Name = "tpnl_WiktoT1";
            this.tpnl_WiktoT1.RowCount = 2;
            this.tpnl_WiktoT1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnl_WiktoT1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnl_WiktoT1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpnl_WiktoT1.Size = new System.Drawing.Size(312, 356);
            this.tpnl_WiktoT1.TabIndex = 0;
            // 
            // pnl_WiktoTL1
            // 
            this.pnl_WiktoTL1.Controls.Add(this.btn_EditDirs);
            this.pnl_WiktoTL1.Controls.Add(this.btn_WiktoImportBackEnd);
            this.pnl_WiktoTL1.Controls.Add(this.btn_WiktoImportMirror);
            this.pnl_WiktoTL1.Controls.Add(this.btn_WiktoImportGoogle);
            this.pnl_WiktoTL1.Controls.Add(this.btn_WiktoClearCGI);
            this.pnl_WiktoTL1.Controls.Add(this.label5);
            this.pnl_WiktoTL1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_WiktoTL1.Location = new System.Drawing.Point(3, 3);
            this.pnl_WiktoTL1.Name = "pnl_WiktoTL1";
            this.pnl_WiktoTL1.Size = new System.Drawing.Size(306, 24);
            this.pnl_WiktoTL1.TabIndex = 19;
            // 
            // btn_EditDirs
            // 
            this.btn_EditDirs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_EditDirs.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_EditDirs.Location = new System.Drawing.Point(78, 0);
            this.btn_EditDirs.Name = "btn_EditDirs";
            this.btn_EditDirs.Size = new System.Drawing.Size(48, 21);
            this.btn_EditDirs.TabIndex = 182;
            this.btn_EditDirs.Text = "Edit";
            this.btn_EditDirs.UseVisualStyleBackColor = true;
            this.btn_EditDirs.Click += new System.EventHandler(this.btn_EditDirs_Click);
            // 
            // btn_WiktoClearCGI
            // 
            this.btn_WiktoClearCGI.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_WiktoClearCGI.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btn_WiktoClearCGI.ForeColor = System.Drawing.Color.Brown;
            this.btn_WiktoClearCGI.Location = new System.Drawing.Point(276, 0);
            this.btn_WiktoClearCGI.Name = "btn_WiktoClearCGI";
            this.btn_WiktoClearCGI.Size = new System.Drawing.Size(30, 21);
            this.btn_WiktoClearCGI.TabIndex = 24;
            this.btn_WiktoClearCGI.Text = "CL";
            this.btn_WiktoClearCGI.UseVisualStyleBackColor = true;
            this.btn_WiktoClearCGI.Click += new System.EventHandler(this.btn_WiktoClearCGI_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(0, 3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 16);
            this.label5.TabIndex = 181;
            this.label5.Text = "CGI Directories";
            // 
            // pnl_WiktoTL3
            // 
            this.pnl_WiktoTL3.Controls.Add(this.lst_NiktoCGI);
            this.pnl_WiktoTL3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_WiktoTL3.Location = new System.Drawing.Point(3, 33);
            this.pnl_WiktoTL3.Name = "pnl_WiktoTL3";
            this.pnl_WiktoTL3.Size = new System.Drawing.Size(306, 320);
            this.pnl_WiktoTL3.TabIndex = 25;
            // 
            // lst_NiktoCGI
            // 
            this.lst_NiktoCGI.BackColor = System.Drawing.Color.Snow;
            this.lst_NiktoCGI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lst_NiktoCGI.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lst_NiktoCGI.FormattingEnabled = true;
            this.lst_NiktoCGI.ItemHeight = 12;
            this.lst_NiktoCGI.Location = new System.Drawing.Point(0, 0);
            this.lst_NiktoCGI.Name = "lst_NiktoCGI";
            this.lst_NiktoCGI.Size = new System.Drawing.Size(306, 314);
            this.lst_NiktoCGI.TabIndex = 26;
            // 
            // panel5
            // 
            this.panel5.AutoScroll = true;
            this.panel5.AutoScrollMinSize = new System.Drawing.Size(220, 670);
            this.panel5.BackColor = System.Drawing.Color.Gray;
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.btnPauseWikto);
            this.panel5.Controls.Add(this.label2);
            this.panel5.Controls.Add(this.prgNik);
            this.panel5.Controls.Add(this.prgNiktoWork);
            this.panel5.Controls.Add(this.btn_NiktoLoad);
            this.panel5.Controls.Add(this.skinButtonGreen2);
            this.panel5.Controls.Add(this.lblNiktoAI);
            this.panel5.Controls.Add(this.groupBox13);
            this.panel5.Controls.Add(this.groupBox14);
            this.panel5.Controls.Add(this.pictureBox2);
            this.panel5.Controls.Add(this.skinButtonRed1);
            this.panel5.Controls.Add(this.btn_WiktoStop);
            this.panel5.Controls.Add(this.btn_WiktoStart);
            this.panel5.Controls.Add(this.label3);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(224, 712);
            this.panel5.TabIndex = 1;
            // 
            // btnPauseWikto
            // 
            this.btnPauseWikto.Enabled = false;
            this.btnPauseWikto.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold);
            this.btnPauseWikto.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btnPauseWikto.Location = new System.Drawing.Point(121, 86);
            this.btnPauseWikto.Name = "btnPauseWikto";
            this.btnPauseWikto.Size = new System.Drawing.Size(86, 28);
            this.btnPauseWikto.TabIndex = 257;
            this.btnPauseWikto.Text = "Pause";
            this.btnPauseWikto.UseVisualStyleBackColor = true;
            this.btnPauseWikto.Click += new System.EventHandler(this.btnPauseWikto_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(5, 642);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(196, 13);
            this.label2.TabIndex = 255;
            this.label2.Text = "Wikto Progress:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // prgNik
            // 
            this.prgNik.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.prgNik.BackColor = System.Drawing.Color.DarkGray;
            this.prgNik.Location = new System.Drawing.Point(7, 658);
            this.prgNik.Maximum = 1000;
            this.prgNik.Name = "prgNik";
            this.prgNik.Size = new System.Drawing.Size(194, 13);
            this.prgNik.TabIndex = 255;
            // 
            // prgNiktoWork
            // 
            this.prgNiktoWork.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.prgNiktoWork.BackColor = System.Drawing.Color.DarkGray;
            this.prgNiktoWork.Location = new System.Drawing.Point(7, 690);
            this.prgNiktoWork.Maximum = 10;
            this.prgNiktoWork.Name = "prgNiktoWork";
            this.prgNiktoWork.Size = new System.Drawing.Size(194, 13);
            this.prgNiktoWork.TabIndex = 255;
            // 
            // lblNiktoAI
            // 
            this.lblNiktoAI.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNiktoAI.BackColor = System.Drawing.Color.Snow;
            this.lblNiktoAI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNiktoAI.Enabled = false;
            this.lblNiktoAI.Location = new System.Drawing.Point(7, 622);
            this.lblNiktoAI.Name = "lblNiktoAI";
            this.lblNiktoAI.Size = new System.Drawing.Size(194, 20);
            this.lblNiktoAI.TabIndex = 255;
            this.lblNiktoAI.Text = "Error messages";
            this.lblNiktoAI.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox13
            // 
            this.groupBox13.Controls.Add(this.btnClearNiktoAI);
            this.groupBox13.Controls.Add(this.btnNiktoRestFuzz);
            this.groupBox13.Controls.Add(this.btnNiktoShowAll);
            this.groupBox13.Controls.Add(this.chkOptimizedNikto);
            this.groupBox13.Controls.Add(this.btnNiktoFuzzUpdate);
            this.groupBox13.Controls.Add(this.NUPDOWNfuzz);
            this.groupBox13.Location = new System.Drawing.Point(7, 214);
            this.groupBox13.Name = "groupBox13";
            this.groupBox13.Size = new System.Drawing.Size(208, 88);
            this.groupBox13.TabIndex = 9;
            this.groupBox13.TabStop = false;
            this.groupBox13.Text = "Trigger Control";
            // 
            // groupBox14
            // 
            this.groupBox14.Controls.Add(this.chkuseSSLWikto);
            this.groupBox14.Controls.Add(this.txtNiktoPort);
            this.groupBox14.Controls.Add(this.txtNiktoTarget);
            this.groupBox14.Controls.Add(this.label28);
            this.groupBox14.Controls.Add(this.label29);
            this.groupBox14.Location = new System.Drawing.Point(7, 138);
            this.groupBox14.Name = "groupBox14";
            this.groupBox14.Size = new System.Drawing.Size(208, 70);
            this.groupBox14.TabIndex = 5;
            this.groupBox14.TabStop = false;
            this.groupBox14.Text = "Target selection";
            // 
            // label28
            // 
            this.label28.Location = new System.Drawing.Point(124, 44);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(32, 12);
            this.label28.TabIndex = 80;
            this.label28.Text = "Port";
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(122, 19);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(80, 16);
            this.label29.TabIndex = 79;
            this.label29.Text = "IP / DNS name";
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox2.BackgroundImage")));
            this.pictureBox2.Location = new System.Drawing.Point(7, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(108, 84);
            this.pictureBox2.TabIndex = 119;
            this.pictureBox2.TabStop = false;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(7, 674);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(194, 13);
            this.label3.TabIndex = 255;
            this.label3.Text = "Work Progress:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GoogleHacks
            // 
            this.GoogleHacks.BackColor = System.Drawing.Color.Gray;
            this.GoogleHacks.Controls.Add(this.pnl_GHMain);
            this.GoogleHacks.Controls.Add(this.panel6);
            this.GoogleHacks.Location = new System.Drawing.Point(4, 25);
            this.GoogleHacks.Name = "GoogleHacks";
            this.GoogleHacks.Size = new System.Drawing.Size(1005, 712);
            this.GoogleHacks.TabIndex = 4;
            this.GoogleHacks.Text = "GoogleHacks";
            this.GoogleHacks.UseVisualStyleBackColor = true;
            // 
            // pnl_GHMain
            // 
            this.pnl_GHMain.BackColor = System.Drawing.Color.DarkGray;
            this.pnl_GHMain.Controls.Add(this.tpnl_HMain);
            this.pnl_GHMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_GHMain.Location = new System.Drawing.Point(224, 0);
            this.pnl_GHMain.Name = "pnl_GHMain";
            this.pnl_GHMain.Size = new System.Drawing.Size(781, 712);
            this.pnl_GHMain.TabIndex = 149;
            // 
            // tpnl_HMain
            // 
            this.tpnl_HMain.ColumnCount = 1;
            this.tpnl_HMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_HMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpnl_HMain.Controls.Add(this.tpnl_GHManQuery, 0, 1);
            this.tpnl_HMain.Controls.Add(this.pbl_GoogleHackDb, 0, 0);
            this.tpnl_HMain.Controls.Add(this.pnl_GHDesc, 0, 2);
            this.tpnl_HMain.Controls.Add(this.tpnl_GHResults, 0, 3);
            this.tpnl_HMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_HMain.Location = new System.Drawing.Point(0, 0);
            this.tpnl_HMain.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_HMain.Name = "tpnl_HMain";
            this.tpnl_HMain.RowCount = 4;
            this.tpnl_HMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tpnl_HMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnl_HMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tpnl_HMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.34F));
            this.tpnl_HMain.Size = new System.Drawing.Size(781, 712);
            this.tpnl_HMain.TabIndex = 0;
            // 
            // tpnl_GHManQuery
            // 
            this.tpnl_GHManQuery.ColumnCount = 2;
            this.tpnl_GHManQuery.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_GHManQuery.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tpnl_GHManQuery.Controls.Add(this.pnl_GHMQ1, 0, 0);
            this.tpnl_GHManQuery.Controls.Add(this.pnl_GHMQ2, 1, 0);
            this.tpnl_GHManQuery.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_GHManQuery.Location = new System.Drawing.Point(0, 227);
            this.tpnl_GHManQuery.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_GHManQuery.Name = "tpnl_GHManQuery";
            this.tpnl_GHManQuery.RowCount = 1;
            this.tpnl_GHManQuery.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_GHManQuery.Size = new System.Drawing.Size(781, 30);
            this.tpnl_GHManQuery.TabIndex = 0;
            // 
            // pnl_GHMQ1
            // 
            this.pnl_GHMQ1.Controls.Add(this.txtGoogleHackOnceOff);
            this.pnl_GHMQ1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_GHMQ1.Location = new System.Drawing.Point(3, 3);
            this.pnl_GHMQ1.Name = "pnl_GHMQ1";
            this.pnl_GHMQ1.Size = new System.Drawing.Size(675, 24);
            this.pnl_GHMQ1.TabIndex = 9;
            // 
            // txtGoogleHackOnceOff
            // 
            this.txtGoogleHackOnceOff.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGoogleHackOnceOff.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGoogleHackOnceOff.Location = new System.Drawing.Point(0, 0);
            this.txtGoogleHackOnceOff.Name = "txtGoogleHackOnceOff";
            this.txtGoogleHackOnceOff.Size = new System.Drawing.Size(675, 18);
            this.txtGoogleHackOnceOff.TabIndex = 10;
            this.txtGoogleHackOnceOff.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // pnl_GHMQ2
            // 
            this.pnl_GHMQ2.Controls.Add(this.btn_GHManualQuery);
            this.pnl_GHMQ2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_GHMQ2.Location = new System.Drawing.Point(684, 3);
            this.pnl_GHMQ2.Name = "pnl_GHMQ2";
            this.pnl_GHMQ2.Size = new System.Drawing.Size(94, 24);
            this.pnl_GHMQ2.TabIndex = 11;
            // 
            // pbl_GoogleHackDb
            // 
            this.pbl_GoogleHackDb.Controls.Add(this.lstGoogleHack);
            this.pbl_GoogleHackDb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbl_GoogleHackDb.Location = new System.Drawing.Point(3, 3);
            this.pbl_GoogleHackDb.Name = "pbl_GoogleHackDb";
            this.pbl_GoogleHackDb.Size = new System.Drawing.Size(775, 221);
            this.pbl_GoogleHackDb.TabIndex = 7;
            // 
            // lstGoogleHack
            // 
            this.lstGoogleHack.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstGoogleHack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstGoogleHack.ItemHeight = 12;
            this.lstGoogleHack.Location = new System.Drawing.Point(0, 0);
            this.lstGoogleHack.Name = "lstGoogleHack";
            this.lstGoogleHack.Size = new System.Drawing.Size(775, 218);
            this.lstGoogleHack.TabIndex = 8;
            this.lstGoogleHack.MouseEnter += new System.EventHandler(this.GHkeepscrolling);
            this.lstGoogleHack.SelectedIndexChanged += new System.EventHandler(this.populateGoogleHackDesc);
            this.lstGoogleHack.MouseLeave += new System.EventHandler(this.GHstopscrolling);
            // 
            // pnl_GHDesc
            // 
            this.pnl_GHDesc.Controls.Add(this.txtGoogleHackDesc);
            this.pnl_GHDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_GHDesc.Location = new System.Drawing.Point(3, 260);
            this.pnl_GHDesc.Name = "pnl_GHDesc";
            this.pnl_GHDesc.Size = new System.Drawing.Size(775, 221);
            this.pnl_GHDesc.TabIndex = 13;
            // 
            // txtGoogleHackDesc
            // 
            this.txtGoogleHackDesc.BackColor = System.Drawing.Color.Snow;
            this.txtGoogleHackDesc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtGoogleHackDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGoogleHackDesc.Location = new System.Drawing.Point(0, 0);
            this.txtGoogleHackDesc.Multiline = true;
            this.txtGoogleHackDesc.Name = "txtGoogleHackDesc";
            this.txtGoogleHackDesc.ReadOnly = true;
            this.txtGoogleHackDesc.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtGoogleHackDesc.Size = new System.Drawing.Size(775, 221);
            this.txtGoogleHackDesc.TabIndex = 14;
            // 
            // tpnl_GHResults
            // 
            this.tpnl_GHResults.ColumnCount = 1;
            this.tpnl_GHResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_GHResults.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tpnl_GHResults.Controls.Add(this.tpnl_GHResultsTop, 0, 0);
            this.tpnl_GHResults.Controls.Add(this.pnl_GHRes3, 0, 1);
            this.tpnl_GHResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_GHResults.Location = new System.Drawing.Point(0, 484);
            this.tpnl_GHResults.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_GHResults.Name = "tpnl_GHResults";
            this.tpnl_GHResults.RowCount = 2;
            this.tpnl_GHResults.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tpnl_GHResults.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tpnl_GHResults.Size = new System.Drawing.Size(781, 228);
            this.tpnl_GHResults.TabIndex = 3;
            // 
            // tpnl_GHResultsTop
            // 
            this.tpnl_GHResultsTop.ColumnCount = 2;
            this.tpnl_GHResultsTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_GHResultsTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tpnl_GHResultsTop.Controls.Add(this.pnl_GHRes1, 0, 0);
            this.tpnl_GHResultsTop.Controls.Add(this.pnl_GHRes2, 1, 0);
            this.tpnl_GHResultsTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tpnl_GHResultsTop.Location = new System.Drawing.Point(0, 0);
            this.tpnl_GHResultsTop.Margin = new System.Windows.Forms.Padding(0);
            this.tpnl_GHResultsTop.Name = "tpnl_GHResultsTop";
            this.tpnl_GHResultsTop.RowCount = 1;
            this.tpnl_GHResultsTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tpnl_GHResultsTop.Size = new System.Drawing.Size(781, 30);
            this.tpnl_GHResultsTop.TabIndex = 0;
            // 
            // pnl_GHRes1
            // 
            this.pnl_GHRes1.Controls.Add(this.label4);
            this.pnl_GHRes1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_GHRes1.Location = new System.Drawing.Point(3, 3);
            this.pnl_GHRes1.Name = "pnl_GHRes1";
            this.pnl_GHRes1.Size = new System.Drawing.Size(675, 24);
            this.pnl_GHRes1.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.Location = new System.Drawing.Point(0, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(426, 13);
            this.label4.TabIndex = 166;
            this.label4.Text = "Results:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnl_GHRes2
            // 
            this.pnl_GHRes2.Controls.Add(this.btn_GHClearResults);
            this.pnl_GHRes2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_GHRes2.Location = new System.Drawing.Point(684, 3);
            this.pnl_GHRes2.Name = "pnl_GHRes2";
            this.pnl_GHRes2.Size = new System.Drawing.Size(94, 24);
            this.pnl_GHRes2.TabIndex = 16;
            // 
            // btn_GHClearResults
            // 
            this.btn_GHClearResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btn_GHClearResults.Location = new System.Drawing.Point(0, 0);
            this.btn_GHClearResults.Name = "btn_GHClearResults";
            this.btn_GHClearResults.Size = new System.Drawing.Size(94, 24);
            this.btn_GHClearResults.TabIndex = 17;
            this.btn_GHClearResults.Text = "Clear List";
            this.btn_GHClearResults.UseVisualStyleBackColor = true;
            this.btn_GHClearResults.Click += new System.EventHandler(this.btn_GHClearResults_Click);
            // 
            // pnl_GHRes3
            // 
            this.pnl_GHRes3.Controls.Add(this.lstGoogleHackResults);
            this.pnl_GHRes3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_GHRes3.Location = new System.Drawing.Point(3, 33);
            this.pnl_GHRes3.Name = "pnl_GHRes3";
            this.pnl_GHRes3.Size = new System.Drawing.Size(775, 193);
            this.pnl_GHRes3.TabIndex = 18;
            // 
            // lstGoogleHackResults
            // 
            this.lstGoogleHackResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstGoogleHackResults.ContextMenu = this.cntxtGoogleHacks;
            this.lstGoogleHackResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstGoogleHackResults.ItemHeight = 12;
            this.lstGoogleHackResults.Location = new System.Drawing.Point(0, 0);
            this.lstGoogleHackResults.Name = "lstGoogleHackResults";
            this.lstGoogleHackResults.Size = new System.Drawing.Size(775, 182);
            this.lstGoogleHackResults.TabIndex = 19;
            this.lstGoogleHackResults.SelectedIndexChanged += new System.EventHandler(this.populateGoogleHackDescFromResults);
            // 
            // panel6
            // 
            this.panel6.AutoScroll = true;
            this.panel6.AutoScrollMinSize = new System.Drawing.Size(220, 670);
            this.panel6.BackColor = System.Drawing.Color.Gray;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.lblGoogleHackEst);
            this.panel6.Controls.Add(this.lblGoogleHackPage);
            this.panel6.Controls.Add(this.btn_GHLoadDatabase);
            this.panel6.Controls.Add(this.label1);
            this.panel6.Controls.Add(this.lblGoogleHackStatus);
            this.panel6.Controls.Add(this.prgGHQuick);
            this.panel6.Controls.Add(this.prgsGoogleHackAll);
            this.panel6.Controls.Add(this.txtGoogleHackTarget);
            this.panel6.Controls.Add(this.pictureBox7);
            this.panel6.Controls.Add(this.btn_GHQuit);
            this.panel6.Controls.Add(this.btn_GHStop);
            this.panel6.Controls.Add(this.btn_GHStart);
            this.panel6.Controls.Add(this.label21);
            this.panel6.Controls.Add(this.label24);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(224, 712);
            this.panel6.TabIndex = 1;
            // 
            // lblGoogleHackEst
            // 
            this.lblGoogleHackEst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGoogleHackEst.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblGoogleHackEst.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGoogleHackEst.Location = new System.Drawing.Point(149, 626);
            this.lblGoogleHackEst.Name = "lblGoogleHackEst";
            this.lblGoogleHackEst.Size = new System.Drawing.Size(52, 16);
            this.lblGoogleHackEst.TabIndex = 255;
            this.lblGoogleHackEst.Text = "Estimate";
            this.lblGoogleHackEst.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblGoogleHackPage
            // 
            this.lblGoogleHackPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblGoogleHackPage.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblGoogleHackPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGoogleHackPage.Location = new System.Drawing.Point(7, 626);
            this.lblGoogleHackPage.Name = "lblGoogleHackPage";
            this.lblGoogleHackPage.Size = new System.Drawing.Size(52, 16);
            this.lblGoogleHackPage.TabIndex = 255;
            this.lblGoogleHackPage.Text = "Page";
            this.lblGoogleHackPage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(7, 642);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 13);
            this.label1.TabIndex = 255;
            this.label1.Text = "Google Hack:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblGoogleHackStatus
            // 
            this.lblGoogleHackStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGoogleHackStatus.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblGoogleHackStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGoogleHackStatus.Location = new System.Drawing.Point(7, 586);
            this.lblGoogleHackStatus.Name = "lblGoogleHackStatus";
            this.lblGoogleHackStatus.Size = new System.Drawing.Size(194, 25);
            this.lblGoogleHackStatus.TabIndex = 255;
            this.lblGoogleHackStatus.Text = "Google Query Status";
            this.lblGoogleHackStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // prgGHQuick
            // 
            this.prgGHQuick.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.prgGHQuick.BackColor = System.Drawing.Color.DarkGray;
            this.prgGHQuick.Location = new System.Drawing.Point(7, 690);
            this.prgGHQuick.Maximum = 10;
            this.prgGHQuick.Name = "prgGHQuick";
            this.prgGHQuick.Size = new System.Drawing.Size(194, 13);
            this.prgGHQuick.TabIndex = 255;
            // 
            // prgsGoogleHackAll
            // 
            this.prgsGoogleHackAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.prgsGoogleHackAll.BackColor = System.Drawing.Color.DarkGray;
            this.prgsGoogleHackAll.Location = new System.Drawing.Point(7, 658);
            this.prgsGoogleHackAll.Name = "prgsGoogleHackAll";
            this.prgsGoogleHackAll.Size = new System.Drawing.Size(194, 13);
            this.prgsGoogleHackAll.TabIndex = 255;
            // 
            // pictureBox7
            // 
            this.pictureBox7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox7.BackgroundImage")));
            this.pictureBox7.Location = new System.Drawing.Point(7, 5);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(108, 84);
            this.pictureBox7.TabIndex = 119;
            this.pictureBox7.TabStop = false;
            // 
            // label21
            // 
            this.label21.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label21.Location = new System.Drawing.Point(7, 674);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(194, 13);
            this.label21.TabIndex = 255;
            this.label21.Text = "Google Hack Quick:";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(5, 100);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(40, 16);
            this.label24.TabIndex = 121;
            this.label24.Text = "Target";
            // 
            // pnl_configleft
            // 
            this.pnl_configleft.BackColor = System.Drawing.Color.Gray;
            this.pnl_configleft.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnl_configleft.Controls.Add(this.pnl_ConfigMain);
            this.pnl_configleft.Controls.Add(this.panel1);
            this.pnl_configleft.ForeColor = System.Drawing.SystemColors.ControlText;
            this.pnl_configleft.Location = new System.Drawing.Point(4, 25);
            this.pnl_configleft.Name = "pnl_configleft";
            this.pnl_configleft.Size = new System.Drawing.Size(1005, 712);
            this.pnl_configleft.TabIndex = 3;
            this.pnl_configleft.Text = "SystemConfig";
            this.pnl_configleft.UseVisualStyleBackColor = true;
            // 
            // pnl_ConfigMain
            // 
            this.pnl_ConfigMain.Controls.Add(this.tab_configMain);
            this.pnl_ConfigMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnl_ConfigMain.Location = new System.Drawing.Point(224, 0);
            this.pnl_ConfigMain.Name = "pnl_ConfigMain";
            this.pnl_ConfigMain.Size = new System.Drawing.Size(777, 708);
            this.pnl_ConfigMain.TabIndex = 252;
            // 
            // tab_configMain
            // 
            this.tab_configMain.Controls.Add(this.cfg_DB);
            this.tab_configMain.Controls.Add(this.cfg_Header);
            this.tab_configMain.Controls.Add(this.cfg_Google);
            this.tab_configMain.Controls.Add(this.cfg_Proxy);
            this.tab_configMain.Controls.Add(this.cfg_Spider);
            this.tab_configMain.Controls.Add(this.cfg_Timing);
            this.tab_configMain.Controls.Add(this.cfg_Update);
            this.tab_configMain.Controls.Add(this.cfg_Startup);
            this.tab_configMain.Controls.Add(this.cfg_Help);
            this.tab_configMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tab_configMain.Location = new System.Drawing.Point(0, 0);
            this.tab_configMain.Name = "tab_configMain";
            this.tab_configMain.SelectedIndex = 0;
            this.tab_configMain.Size = new System.Drawing.Size(777, 708);
            this.tab_configMain.TabIndex = 8;
            // 
            // cfg_DB
            // 
            this.cfg_DB.BackColor = System.Drawing.Color.DarkGray;
            this.cfg_DB.Controls.Add(this.btn_browseghdb);
            this.cfg_DB.Controls.Add(this.btn_browsenikto);
            this.cfg_DB.Controls.Add(this.btn_CfgLocateNDb);
            this.cfg_DB.Controls.Add(this.btn_CfgLocateGDb);
            this.cfg_DB.Controls.Add(this.label27);
            this.cfg_DB.Controls.Add(this.label25);
            this.cfg_DB.Controls.Add(this.txtDBLocationGH);
            this.cfg_DB.Controls.Add(this.txtDBlocationNikto);
            this.cfg_DB.Location = new System.Drawing.Point(4, 21);
            this.cfg_DB.Name = "cfg_DB";
            this.cfg_DB.Padding = new System.Windows.Forms.Padding(3);
            this.cfg_DB.Size = new System.Drawing.Size(769, 683);
            this.cfg_DB.TabIndex = 4;
            this.cfg_DB.Text = "Database Locations";
            this.cfg_DB.UseVisualStyleBackColor = true;
            // 
            // btn_browseghdb
            // 
            this.btn_browseghdb.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_browseghdb.Location = new System.Drawing.Point(597, 31);
            this.btn_browseghdb.Name = "btn_browseghdb";
            this.btn_browseghdb.Size = new System.Drawing.Size(60, 22);
            this.btn_browseghdb.TabIndex = 14;
            this.btn_browseghdb.Text = "Browse...";
            this.btn_browseghdb.UseVisualStyleBackColor = true;
            this.btn_browseghdb.Click += new System.EventHandler(this.btn_browseghdb_Click);
            // 
            // btn_browsenikto
            // 
            this.btn_browsenikto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_browsenikto.Location = new System.Drawing.Point(597, 7);
            this.btn_browsenikto.Name = "btn_browsenikto";
            this.btn_browsenikto.Size = new System.Drawing.Size(60, 22);
            this.btn_browsenikto.TabIndex = 13;
            this.btn_browsenikto.Text = "Browse...";
            this.btn_browsenikto.UseVisualStyleBackColor = true;
            this.btn_browsenikto.Click += new System.EventHandler(this.btn_browsenikto_Click);
            // 
            // label27
            // 
            this.label27.Location = new System.Drawing.Point(6, 36);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(137, 12);
            this.label27.TabIndex = 9;
            this.label27.Text = "Google Hack Database:";
            // 
            // label25
            // 
            this.label25.Location = new System.Drawing.Point(6, 12);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(137, 12);
            this.label25.TabIndex = 8;
            this.label25.Text = "Nikto Database:";
            // 
            // cfg_Header
            // 
            this.cfg_Header.BackColor = System.Drawing.Color.DarkGray;
            this.cfg_Header.Controls.Add(this.txtHeader);
            this.cfg_Header.Location = new System.Drawing.Point(4, 22);
            this.cfg_Header.Name = "cfg_Header";
            this.cfg_Header.Padding = new System.Windows.Forms.Padding(3);
            this.cfg_Header.Size = new System.Drawing.Size(769, 682);
            this.cfg_Header.TabIndex = 5;
            this.cfg_Header.Text = "HTTP Header";
            this.cfg_Header.UseVisualStyleBackColor = true;
            // 
            // cfg_Google
            // 
            this.cfg_Google.BackColor = System.Drawing.Color.DarkGray;
            this.cfg_Google.Controls.Add(this.btnSpudLocate);
            this.cfg_Google.Controls.Add(this.txtSpudDirectory);
            this.cfg_Google.Controls.Add(this.label19);
            this.cfg_Google.Controls.Add(this.btn_StartAura);
            this.cfg_Google.Controls.Add(this.updownGoogleDepth);
            this.cfg_Google.Controls.Add(this.label18);
            this.cfg_Google.Location = new System.Drawing.Point(4, 22);
            this.cfg_Google.Name = "cfg_Google";
            this.cfg_Google.Padding = new System.Windows.Forms.Padding(3);
            this.cfg_Google.Size = new System.Drawing.Size(769, 682);
            this.cfg_Google.TabIndex = 1;
            this.cfg_Google.Text = "Spud API";
            this.cfg_Google.UseVisualStyleBackColor = true;
            // 
            // btnSpudLocate
            // 
            this.btnSpudLocate.Location = new System.Drawing.Point(376, 39);
            this.btnSpudLocate.Name = "btnSpudLocate";
            this.btnSpudLocate.Size = new System.Drawing.Size(73, 23);
            this.btnSpudLocate.TabIndex = 138;
            this.btnSpudLocate.Text = "Locate";
            this.btnSpudLocate.UseVisualStyleBackColor = true;
            this.btnSpudLocate.Click += new System.EventHandler(this.btnSpudLocate_Click);
            // 
            // txtSpudDirectory
            // 
            this.txtSpudDirectory.Location = new System.Drawing.Point(145, 41);
            this.txtSpudDirectory.Name = "txtSpudDirectory";
            this.txtSpudDirectory.Size = new System.Drawing.Size(225, 18);
            this.txtSpudDirectory.TabIndex = 137;
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(17, 41);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(122, 18);
            this.label19.TabIndex = 136;
            this.label19.Text = "Spud Directory";
            // 
            // btn_StartAura
            // 
            this.btn_StartAura.Location = new System.Drawing.Point(17, 71);
            this.btn_StartAura.Name = "btn_StartAura";
            this.btn_StartAura.Size = new System.Drawing.Size(172, 23);
            this.btn_StartAura.TabIndex = 135;
            this.btn_StartAura.Text = "Start SPUD";
            this.btn_StartAura.UseVisualStyleBackColor = true;
            this.btn_StartAura.Click += new System.EventHandler(this.btn_Spud_Click);
            // 
            // updownGoogleDepth
            // 
            this.updownGoogleDepth.BackColor = System.Drawing.Color.Snow;
            this.updownGoogleDepth.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.updownGoogleDepth.Location = new System.Drawing.Point(145, 20);
            this.updownGoogleDepth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.updownGoogleDepth.Name = "updownGoogleDepth";
            this.updownGoogleDepth.Size = new System.Drawing.Size(44, 18);
            this.updownGoogleDepth.TabIndex = 133;
            this.updownGoogleDepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.updownGoogleDepth, "The number of levels to drill down to in the Google Results");
            this.updownGoogleDepth.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(17, 20);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(122, 18);
            this.label18.TabIndex = 134;
            this.label18.Text = "Pages(max)";
            // 
            // cfg_Proxy
            // 
            this.cfg_Proxy.BackColor = System.Drawing.Color.DarkGray;
            this.cfg_Proxy.Controls.Add(this.chkProxyPresent);
            this.cfg_Proxy.Controls.Add(this.txtProxySettings);
            this.cfg_Proxy.Controls.Add(this.label17);
            this.cfg_Proxy.Location = new System.Drawing.Point(4, 22);
            this.cfg_Proxy.Name = "cfg_Proxy";
            this.cfg_Proxy.Padding = new System.Windows.Forms.Padding(3);
            this.cfg_Proxy.Size = new System.Drawing.Size(769, 682);
            this.cfg_Proxy.TabIndex = 0;
            this.cfg_Proxy.Text = "Proxy Settings";
            this.cfg_Proxy.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(4, 12);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(122, 16);
            this.label17.TabIndex = 112;
            this.label17.Text = "Proxy IP:port";
            // 
            // cfg_Spider
            // 
            this.cfg_Spider.BackColor = System.Drawing.Color.DarkGray;
            this.cfg_Spider.Controls.Add(this.chk_ignoreidx);
            this.cfg_Spider.Controls.Add(this.txt_idxflags);
            this.cfg_Spider.Controls.Add(this.label42);
            this.cfg_Spider.Controls.Add(this.txt_excdirs);
            this.cfg_Spider.Controls.Add(this.label40);
            this.cfg_Spider.Controls.Add(this.label36);
            this.cfg_Spider.Controls.Add(this.nud_contentsize);
            this.cfg_Spider.Controls.Add(this.label35);
            this.cfg_Spider.Controls.Add(this.txt_content);
            this.cfg_Spider.Controls.Add(this.label33);
            this.cfg_Spider.Controls.Add(this.label63);
            this.cfg_Spider.Controls.Add(this.NUPDOWNspider);
            this.cfg_Spider.Controls.Add(this.label64);
            this.cfg_Spider.Controls.Add(this.label67);
            this.cfg_Spider.Controls.Add(this.txt_ConfigSpiderExclude);
            this.cfg_Spider.Controls.Add(this.txt_ConfigSpiderExtension);
            this.cfg_Spider.Location = new System.Drawing.Point(4, 22);
            this.cfg_Spider.Name = "cfg_Spider";
            this.cfg_Spider.Padding = new System.Windows.Forms.Padding(3);
            this.cfg_Spider.Size = new System.Drawing.Size(769, 682);
            this.cfg_Spider.TabIndex = 9;
            this.cfg_Spider.Text = "Spider";
            this.cfg_Spider.UseVisualStyleBackColor = true;
            // 
            // chk_ignoreidx
            // 
            this.chk_ignoreidx.AutoSize = true;
            this.chk_ignoreidx.BackColor = System.Drawing.Color.Transparent;
            this.chk_ignoreidx.Checked = true;
            this.chk_ignoreidx.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_ignoreidx.Location = new System.Drawing.Point(8, 108);
            this.chk_ignoreidx.Name = "chk_ignoreidx";
            this.chk_ignoreidx.Size = new System.Drawing.Size(164, 16);
            this.chk_ignoreidx.TabIndex = 162;
            this.chk_ignoreidx.Text = "Do not spider indexable directories";
            this.chk_ignoreidx.UseVisualStyleBackColor = true;
            this.chk_ignoreidx.CheckedChanged += new System.EventHandler(this.chk_ignoreidx_CheckedChanged);
            // 
            // txt_idxflags
            // 
            this.txt_idxflags.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_idxflags.BackColor = System.Drawing.Color.Snow;
            this.txt_idxflags.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_idxflags.Enabled = false;
            this.txt_idxflags.Location = new System.Drawing.Point(134, 130);
            this.txt_idxflags.Name = "txt_idxflags";
            this.txt_idxflags.Size = new System.Drawing.Size(508, 18);
            this.txt_idxflags.TabIndex = 161;
            this.txt_idxflags.Text = "?C=N;O=A,?C=N;O=D,?C=M;O=A,?C=M;O=D,?C=S;O=A,?C=S;O=D,?C=D;O=A,?C=D;O=D";
            this.txt_idxflags.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(6, 132);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(62, 12);
            this.label42.TabIndex = 160;
            this.label42.Text = "Index Sorting:";
            // 
            // txt_excdirs
            // 
            this.txt_excdirs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_excdirs.BackColor = System.Drawing.Color.Snow;
            this.txt_excdirs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_excdirs.Location = new System.Drawing.Point(134, 81);
            this.txt_excdirs.Name = "txt_excdirs";
            this.txt_excdirs.Size = new System.Drawing.Size(508, 18);
            this.txt_excdirs.TabIndex = 159;
            this.txt_excdirs.Text = "/images/,/js/,/img/";
            this.txt_excdirs.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(6, 84);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(85, 12);
            this.label40.TabIndex = 158;
            this.label40.Text = "Exclude Directories";
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(260, 156);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(16, 12);
            this.label36.TabIndex = 157;
            this.label36.Text = "kB";
            // 
            // nud_contentsize
            // 
            this.nud_contentsize.BackColor = System.Drawing.Color.Snow;
            this.nud_contentsize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nud_contentsize.Location = new System.Drawing.Point(134, 178);
            this.nud_contentsize.Maximum = new decimal(new int[] {
            2048,
            0,
            0,
            0});
            this.nud_contentsize.Name = "nud_contentsize";
            this.nud_contentsize.Size = new System.Drawing.Size(120, 18);
            this.nud_contentsize.TabIndex = 156;
            this.nud_contentsize.Value = new decimal(new int[] {
            256,
            0,
            0,
            0});
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(6, 180);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(115, 12);
            this.label35.TabIndex = 155;
            this.label35.Text = "Maximum Content Length:";
            // 
            // txt_content
            // 
            this.txt_content.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_content.BackColor = System.Drawing.Color.Snow;
            this.txt_content.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_content.Location = new System.Drawing.Point(134, 58);
            this.txt_content.Name = "txt_content";
            this.txt_content.Size = new System.Drawing.Size(508, 18);
            this.txt_content.TabIndex = 154;
            this.txt_content.Text = "text/*";
            this.txt_content.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.txt_content, "Only spider content with this content type (ie: text/html,text/*,application/pdf," +
                    "application/*,etc)");
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Location = new System.Drawing.Point(6, 60);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(101, 12);
            this.label33.TabIndex = 153;
            this.label33.Text = "Include Content-Types:";
            // 
            // label63
            // 
            this.label63.Location = new System.Drawing.Point(6, 156);
            this.label63.Name = "label63";
            this.label63.Size = new System.Drawing.Size(72, 16);
            this.label63.TabIndex = 150;
            this.label63.Text = "Threads:";
            // 
            // label64
            // 
            this.label64.AutoSize = true;
            this.label64.Location = new System.Drawing.Point(6, 36);
            this.label64.Name = "label64";
            this.label64.Size = new System.Drawing.Size(89, 12);
            this.label64.TabIndex = 152;
            this.label64.Text = "Exclude Extensions:";
            // 
            // label67
            // 
            this.label67.AutoSize = true;
            this.label67.Location = new System.Drawing.Point(6, 12);
            this.label67.Name = "label67";
            this.label67.Size = new System.Drawing.Size(64, 12);
            this.label67.TabIndex = 151;
            this.label67.Text = "Exclude Filter:";
            // 
            // cfg_Timing
            // 
            this.cfg_Timing.BackColor = System.Drawing.Color.DarkGray;
            this.cfg_Timing.Controls.Add(this.updownRetryTCP);
            this.cfg_Timing.Controls.Add(this.updownTimeOutTCP);
            this.cfg_Timing.Controls.Add(this.label6);
            this.cfg_Timing.Controls.Add(this.label16);
            this.cfg_Timing.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cfg_Timing.Location = new System.Drawing.Point(4, 22);
            this.cfg_Timing.Name = "cfg_Timing";
            this.cfg_Timing.Padding = new System.Windows.Forms.Padding(3);
            this.cfg_Timing.Size = new System.Drawing.Size(769, 682);
            this.cfg_Timing.TabIndex = 2;
            this.cfg_Timing.Text = "Timing";
            this.cfg_Timing.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(122, 12);
            this.label6.TabIndex = 128;
            this.label6.Text = "Retry";
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(6, 12);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(122, 12);
            this.label16.TabIndex = 127;
            this.label16.Text = "Timeout (ms)";
            // 
            // cfg_Update
            // 
            this.cfg_Update.BackColor = System.Drawing.Color.DarkGray;
            this.cfg_Update.Controls.Add(this.label49);
            this.cfg_Update.Controls.Add(this.label48);
            this.cfg_Update.Controls.Add(this.label46);
            this.cfg_Update.Controls.Add(this.txtURLUpdateGHDB);
            this.cfg_Update.Controls.Add(this.txtURLUpdateNiktoDB);
            this.cfg_Update.Controls.Add(this.txtURLUpdate);
            this.cfg_Update.Location = new System.Drawing.Point(4, 22);
            this.cfg_Update.Name = "cfg_Update";
            this.cfg_Update.Padding = new System.Windows.Forms.Padding(3);
            this.cfg_Update.Size = new System.Drawing.Size(769, 682);
            this.cfg_Update.TabIndex = 6;
            this.cfg_Update.Text = "Update Sites";
            this.cfg_Update.UseVisualStyleBackColor = true;
            // 
            // label49
            // 
            this.label49.Location = new System.Drawing.Point(6, 60);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(128, 16);
            this.label49.TabIndex = 110;
            this.label49.Text = "GoogleHack DB";
            // 
            // label48
            // 
            this.label48.Location = new System.Drawing.Point(6, 36);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(128, 16);
            this.label48.TabIndex = 109;
            this.label48.Text = "Nikto DB";
            // 
            // label46
            // 
            this.label46.Location = new System.Drawing.Point(6, 12);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(128, 16);
            this.label46.TabIndex = 108;
            this.label46.Text = "BackEnd DB";
            // 
            // cfg_Startup
            // 
            this.cfg_Startup.BackColor = System.Drawing.Color.DarkGray;
            this.cfg_Startup.Controls.Add(this.chk_StartWiz);
            this.cfg_Startup.Controls.Add(this.btn_ShowNews);
            this.cfg_Startup.Controls.Add(this.chk_ShowStart);
            this.cfg_Startup.Location = new System.Drawing.Point(4, 22);
            this.cfg_Startup.Name = "cfg_Startup";
            this.cfg_Startup.Size = new System.Drawing.Size(769, 682);
            this.cfg_Startup.TabIndex = 10;
            this.cfg_Startup.Text = "Wikto Start Up";
            // 
            // chk_StartWiz
            // 
            this.chk_StartWiz.AutoSize = true;
            this.chk_StartWiz.BackColor = System.Drawing.Color.Transparent;
            this.chk_StartWiz.Location = new System.Drawing.Point(6, 36);
            this.chk_StartWiz.Name = "chk_StartWiz";
            this.chk_StartWiz.Size = new System.Drawing.Size(128, 16);
            this.chk_StartWiz.TabIndex = 31;
            this.chk_StartWiz.Text = "Always start with Wizard";
            this.chk_StartWiz.UseVisualStyleBackColor = true;
            this.chk_StartWiz.CheckedChanged += new System.EventHandler(this.chk_StartWiz_CheckedChanged);
            // 
            // cfg_Help
            // 
            this.cfg_Help.BackColor = System.Drawing.Color.DarkGray;
            this.cfg_Help.Controls.Add(this.lbl_About);
            this.cfg_Help.Controls.Add(this.label47);
            this.cfg_Help.Controls.Add(this.lbl_ResearchMail);
            this.cfg_Help.Controls.Add(this.label41);
            this.cfg_Help.Controls.Add(this.lbl_WiktoHome);
            this.cfg_Help.Controls.Add(this.label39);
            this.cfg_Help.Location = new System.Drawing.Point(4, 22);
            this.cfg_Help.Name = "cfg_Help";
            this.cfg_Help.Padding = new System.Windows.Forms.Padding(3);
            this.cfg_Help.Size = new System.Drawing.Size(769, 682);
            this.cfg_Help.TabIndex = 8;
            this.cfg_Help.Text = "Help";
            this.cfg_Help.UseVisualStyleBackColor = true;
            // 
            // lbl_About
            // 
            this.lbl_About.AutoSize = true;
            this.lbl_About.Location = new System.Drawing.Point(6, 84);
            this.lbl_About.Name = "lbl_About";
            this.lbl_About.Size = new System.Drawing.Size(359, 108);
            this.lbl_About.TabIndex = 5;
            this.lbl_About.Text = resources.GetString("lbl_About.Text");
            // 
            // label47
            // 
            this.label47.AutoSize = true;
            this.label47.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label47.Location = new System.Drawing.Point(6, 60);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(71, 12);
            this.label47.TabIndex = 4;
            this.label47.Text = "About Wikto:";
            // 
            // lbl_ResearchMail
            // 
            this.lbl_ResearchMail.AutoSize = true;
            this.lbl_ResearchMail.Location = new System.Drawing.Point(125, 36);
            this.lbl_ResearchMail.Name = "lbl_ResearchMail";
            this.lbl_ResearchMail.Size = new System.Drawing.Size(114, 12);
            this.lbl_ResearchMail.TabIndex = 3;
            this.lbl_ResearchMail.TabStop = true;
            this.lbl_ResearchMail.Text = "research@sensepost.com";
            this.lbl_ResearchMail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GotoLink);
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(6, 36);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(95, 12);
            this.label41.TabIndex = 2;
            this.label41.Text = "SensePost Research:";
            // 
            // lbl_WiktoHome
            // 
            this.lbl_WiktoHome.AutoSize = true;
            this.lbl_WiktoHome.Location = new System.Drawing.Point(192, 12);
            this.lbl_WiktoHome.Name = "lbl_WiktoHome";
            this.lbl_WiktoHome.Size = new System.Drawing.Size(186, 12);
            this.lbl_WiktoHome.TabIndex = 1;
            this.lbl_WiktoHome.TabStop = true;
            this.lbl_WiktoHome.Text = "http://www.sensepost.com/research/wikto";
            this.lbl_WiktoHome.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.GotoLink);
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(6, 12);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(147, 12);
            this.label39.TabIndex = 0;
            this.label39.Text = "Complete online documentation at:";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoScrollMinSize = new System.Drawing.Size(220, 670);
            this.panel1.BackColor = System.Drawing.Color.Gray;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btn_CnfQuit);
            this.panel1.Controls.Add(this.label37);
            this.panel1.Controls.Add(this.lblConfigFileLocation);
            this.panel1.Controls.Add(this.btn_CnfReset);
            this.panel1.Controls.Add(this.btn_CnfSave);
            this.panel1.Controls.Add(this.btn_CnfLoad);
            this.panel1.Controls.Add(this.pictureBox5);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(224, 708);
            this.panel1.TabIndex = 1;
            // 
            // label37
            // 
            this.label37.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label37.Location = new System.Drawing.Point(5, 643);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(198, 13);
            this.label37.TabIndex = 255;
            this.label37.Text = "Currently Running:";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblConfigFileLocation
            // 
            this.lblConfigFileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblConfigFileLocation.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblConfigFileLocation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblConfigFileLocation.Location = new System.Drawing.Point(3, 656);
            this.lblConfigFileLocation.Name = "lblConfigFileLocation";
            this.lblConfigFileLocation.Size = new System.Drawing.Size(200, 47);
            this.lblConfigFileLocation.TabIndex = 255;
            this.lblConfigFileLocation.Text = "Running on defaults";
            this.lblConfigFileLocation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox5.BackgroundImage")));
            this.pictureBox5.Location = new System.Drawing.Point(7, 5);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(108, 84);
            this.pictureBox5.TabIndex = 119;
            this.pictureBox5.TabStop = false;
            // 
            // tab_wizard
            // 
            this.tab_wizard.BackColor = System.Drawing.Color.DarkGray;
            this.tab_wizard.Location = new System.Drawing.Point(4, 25);
            this.tab_wizard.Name = "tab_wizard";
            this.tab_wizard.Size = new System.Drawing.Size(1005, 712);
            this.tab_wizard.TabIndex = 6;
            this.tab_wizard.Text = "Scan Wizard";
            this.tab_wizard.UseVisualStyleBackColor = true;
            // 
            // fdlLoadBackEndDirs
            // 
            this.fdlLoadBackEndDirs.CheckFileExists = false;
            this.fdlLoadBackEndDirs.CheckPathExists = false;
            this.fdlLoadBackEndDirs.DefaultExt = "txt";
            this.fdlLoadBackEndDirs.InitialDirectory = "c:\\";
            // 
            // fdlLoadBackEndFiles
            // 
            this.fdlLoadBackEndFiles.DefaultExt = "txt";
            this.fdlLoadBackEndFiles.InitialDirectory = "c:\\";
            // 
            // fdlLoadBackEndExt
            // 
            this.fdlLoadBackEndExt.DefaultExt = "txt";
            this.fdlLoadBackEndExt.InitialDirectory = "c:\\";
            // 
            // fdlExportBackEnd
            // 
            this.fdlExportBackEnd.DefaultExt = "csv";
            this.fdlExportBackEnd.FileName = "wiktobackend.csv";
            this.fdlExportBackEnd.Filter = "CSV files | *.csv";
            this.fdlExportBackEnd.InitialDirectory = "c:\\";
            // 
            // fdlLoadNiktoDB
            // 
            this.fdlLoadNiktoDB.DefaultExt = "txt";
            this.fdlLoadNiktoDB.InitialDirectory = "c:\\";
            // 
            // fdlExportWikto
            // 
            this.fdlExportWikto.DefaultExt = "csv";
            this.fdlExportWikto.FileName = "wikto.csv";
            this.fdlExportWikto.Filter = "CSV files | *.csv";
            this.fdlExportWikto.InitialDirectory = "c:\\";
            // 
            // fdlGoogleHacksOpenDB
            // 
            this.fdlGoogleHacksOpenDB.Filter = "XML files|*.xml";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.CheckFileExists = false;
            this.openFileDialog1.Filter = "Wikto Conf files |*.wkt|All files|*.*";
            // 
            // frm_Wikto
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.ClientSize = new System.Drawing.Size(1016, 741);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.splitter1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frm_Wikto";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Wikto by SensePost ";
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResizeEnd += new System.EventHandler(this.ResizeListViews);
            this.Load += new System.EventHandler(this.frm_Wikto_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NUPDOWNspider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownRetryTCP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownTimeOutTCP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUPDOWNfuzz)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.Mirror.ResumeLayout(false);
            this.tpnlMirror.ResumeLayout(false);
            this.tpnlMirrorLinks.ResumeLayout(false);
            this.tpnlMirrorLinkTop.ResumeLayout(false);
            this.pnlMirrorLinkLeft.ResumeLayout(false);
            this.pnlMrrorLinkRight.ResumeLayout(false);
            this.tpnlMirrorDir.ResumeLayout(false);
            this.tpnlMirrorDirTop.ResumeLayout(false);
            this.pnlMirrorDirLeft.ResumeLayout(false);
            this.pnlMirrorDirRight.ResumeLayout(false);
            this.pnlMirrorLeft.ResumeLayout(false);
            this.pnlMirrorLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            this.Googler.ResumeLayout(false);
            this.tpnlGoogleMain.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tpnlGoogleDir.ResumeLayout(false);
            this.tpnlGoogleDirTop.ResumeLayout(false);
            this.pnlGoogleDirLeft.ResumeLayout(false);
            this.pnlGoogleDirRight.ResumeLayout(false);
            this.pnlGoogleDirMain.ResumeLayout(false);
            this.tpnlGoogleLink.ResumeLayout(false);
            this.tpnlGoogleLinkTop.ResumeLayout(false);
            this.pnlGoogleLinkLeft.ResumeLayout(false);
            this.pnlGoogleLinkRight.ResumeLayout(false);
            this.pnlGoogleLinkMain.ResumeLayout(false);
            this.pnlGoogleLeft.ResumeLayout(false);
            this.pnlGoogleLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.BackEndMiner.ResumeLayout(false);
            this.BackEndMiner.PerformLayout();
            this.pnl_BackEndMain.ResumeLayout(false);
            this.tpnl_BackendMain.ResumeLayout(false);
            this.pnl_BackEndTop.ResumeLayout(false);
            this.tpnl_BackEndTop.ResumeLayout(false);
            this.pnl_BETopLeft1.ResumeLayout(false);
            this.pbl_TopLeft3.ResumeLayout(false);
            this.pnl_BETopMid1.ResumeLayout(false);
            this.pnl_BETopMid3.ResumeLayout(false);
            this.pnl_BETopRight1.ResumeLayout(false);
            this.pnl_BETopRight3.ResumeLayout(false);
            this.pnl_BackEndBottom.ResumeLayout(false);
            this.tpnl_BEBottom1.ResumeLayout(false);
            this.tpnl_BEBottom2.ResumeLayout(false);
            this.pnl_BEBottomLeft1.ResumeLayout(false);
            this.pnl_BEBottomLeft3.ResumeLayout(false);
            this.tpnl_BEBottomRight.ResumeLayout(false);
            this.pnl_BEBottomRight1.ResumeLayout(false);
            this.pnl_BEBottomRight3.ResumeLayout(false);
            this.tpnl_BEFiles.ResumeLayout(false);
            this.pnl_BEFile1.ResumeLayout(false);
            this.pnl_BEFile3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeAnomaly)).EndInit();
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUPDOWNBackEnd)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox11.ResumeLayout(false);
            this.groupBox12.ResumeLayout(false);
            this.groupBox12.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.NiktoIsh.ResumeLayout(false);
            this.pnl_WiktoMain.ResumeLayout(false);
            this.tpnl_WiktoMain.ResumeLayout(false);
            this.tpnl_WiktoB1.ResumeLayout(false);
            this.panpnl_WiktoBL1.ResumeLayout(false);
            this.panpnl_WiktoBL2.ResumeLayout(false);
            this.tpnl_WiktoB2.ResumeLayout(false);
            this.panpnl_WiktoBM1.ResumeLayout(false);
            this.panpnl_WiktoBM2.ResumeLayout(false);
            this.panpnl_WiktoBM2.PerformLayout();
            this.tpnl_WiktoB3.ResumeLayout(false);
            this.panpnl_WiktoBR1.ResumeLayout(false);
            this.panpnl_WiktoBR2.ResumeLayout(false);
            this.panpnl_WiktoBR2.PerformLayout();
            this.tpnl_WiktoT3.ResumeLayout(false);
            this.pnl_WiktoTR1.ResumeLayout(false);
            this.pnl_WiktoTR2.ResumeLayout(false);
            this.tpnl_WiktoT2.ResumeLayout(false);
            this.pnl_WiktoTM1.ResumeLayout(false);
            this.pnl_WiktoTM2.ResumeLayout(false);
            this.tpnl_WiktoT1.ResumeLayout(false);
            this.pnl_WiktoTL1.ResumeLayout(false);
            this.pnl_WiktoTL3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.groupBox13.ResumeLayout(false);
            this.groupBox13.PerformLayout();
            this.groupBox14.ResumeLayout(false);
            this.groupBox14.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.GoogleHacks.ResumeLayout(false);
            this.pnl_GHMain.ResumeLayout(false);
            this.tpnl_HMain.ResumeLayout(false);
            this.tpnl_GHManQuery.ResumeLayout(false);
            this.pnl_GHMQ1.ResumeLayout(false);
            this.pnl_GHMQ1.PerformLayout();
            this.pnl_GHMQ2.ResumeLayout(false);
            this.pbl_GoogleHackDb.ResumeLayout(false);
            this.pnl_GHDesc.ResumeLayout(false);
            this.pnl_GHDesc.PerformLayout();
            this.tpnl_GHResults.ResumeLayout(false);
            this.tpnl_GHResultsTop.ResumeLayout(false);
            this.pnl_GHRes1.ResumeLayout(false);
            this.pnl_GHRes2.ResumeLayout(false);
            this.pnl_GHRes3.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            this.pnl_configleft.ResumeLayout(false);
            this.pnl_ConfigMain.ResumeLayout(false);
            this.tab_configMain.ResumeLayout(false);
            this.cfg_DB.ResumeLayout(false);
            this.cfg_DB.PerformLayout();
            this.cfg_Header.ResumeLayout(false);
            this.cfg_Header.PerformLayout();
            this.cfg_Google.ResumeLayout(false);
            this.cfg_Google.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updownGoogleDepth)).EndInit();
            this.cfg_Proxy.ResumeLayout(false);
            this.cfg_Proxy.PerformLayout();
            this.cfg_Spider.ResumeLayout(false);
            this.cfg_Spider.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_contentsize)).EndInit();
            this.cfg_Timing.ResumeLayout(false);
            this.cfg_Update.ResumeLayout(false);
            this.cfg_Update.PerformLayout();
            this.cfg_Startup.ResumeLayout(false);
            this.cfg_Startup.PerformLayout();
            this.cfg_Help.ResumeLayout(false);
            this.cfg_Help.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

        #region Static Thread - Main Application Entry-Point
        [STAThread]
		static void Main()
        {
			Application.Run(new frm_Wikto());
        }
        #endregion

        #region Wikto Load - Initialisation Routines
        private void frm_Wikto_Load(object sender, System.EventArgs e)
        {
            txtHeader.Text = "Accept: */*\r\nAccept-Language: en-us\r\nConnection: close\r\nUser-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)\r\nHost: localhost";
            btnStopGoole.Enabled = false;
            LoadConfigInitial();
            if (bl_ShowStart)
            {
                News thenews = new News(this);
                thenews.ShowDialog();
                thenews.Dispose();
                thenews = null;
            }
            LoadConfigInitial();
            if (bl_ShowStart)
                this.chk_ShowStart.Checked = true;
            else
                this.chk_ShowStart.Checked = false;
            if (bl_ShowStartWiz)
                this.chk_StartWiz.Checked = true;
            else
                this.chk_StartWiz.Checked = false;

            if (isFirstTimeRun && !bl_ShowStartWiz)
            {
                MessageBox.Show("This is either the first time Wikto is being run or your configuration file has been deleted or moved.\n\nThe default settings have been loaded.\n\nPlease check this configuration now.", "Wikto Configuration");
                String tmpPath = Application.ExecutablePath;
                String sbPath = "";
                String[] arrPath = tmpPath.Split('\\');
                int tmpi;
                for (tmpi = 0; tmpi < arrPath.Length - 1; tmpi++)
                {
                    if (sbPath.Length == 0)
                        sbPath = arrPath[tmpi];
                    else
                        sbPath = sbPath + "\\" + arrPath[tmpi];
                }
                txtDBlocationNikto.Text = sbPath + "\\databases\\nikto-scan_database.db";
                txtDBLocationGH.Text = sbPath + "\\database\\GHDB.xml";
                tabControl1.SelectedIndex = 5;
                tabControl1.TabPages[5].Refresh();
            }
            lbl_WiktoHome.Links.Add(0, lbl_WiktoHome.Text.Length, "http://www.sensepost.com/research/wikto");
            lbl_ResearchMail.Links.Add(0, lbl_ResearchMail.Text.Length, "mailto:research@sensepost.com");
            lbl_About.Text = lbl_About.Text.Replace("?", Application.ProductVersion.ToString());
            if (bl_ShowStartWiz)
            {
                Wizard thewiz = new Wizard(this);
                this.tabControl1.SelectedIndex = 0;
                thewiz.ShowDialog();
                thewiz.Dispose();
                thewiz = null;
            }
            // We check to ensure that these things are populated...
            if (txtInDirs.Text.Length == 0) txtInDirs.Text = ReadBackEndFile("dirs");
            if (txtInFiles.Text.Length == 0) txtInFiles.Text = ReadBackEndFile("files");
            if (txtInFileTypes.Text.Length == 0) txtInFileTypes.Text = ReadBackEndFile("extensions");
        }
        #endregion

        #region File I/O Routines
        private string ReadBackEndFile(string fileend)
        {
            string returnstring = "";
            StreamReader freader = null;
            string[] tmparray = Application.ExecutablePath.Split('\\');
            string tmpdir = "";
            tmparray[tmparray.Length - 1] = "databases";
            foreach (string s in tmparray)
            {
                if (tmpdir.Length == 0) tmpdir = s;
                else tmpdir = tmpdir + '\\' + s;
            }
            try
            {
                freader = new StreamReader(tmpdir + "\\backend_" + fileend + ".txt");
            }
            catch
            {
                return "";
            }

            string readline = "";
            while ((readline = freader.ReadLine()) != null)
            {
                if (returnstring == "") returnstring = readline;
                else returnstring = returnstring + "\r\n" + readline;

            }
            freader.Close();
            return returnstring;
        }

        private void btnExport_Click(object sender, System.EventArgs e)
        {
            DialogResult res = fdlExportBackEnd.ShowDialog();
            if (res == DialogResult.OK)
            {
                if (fdlExportBackEnd.FileName.Length > 0)
                {
                    try
                    {

                        string[] Directories = new string[5000];
                        string[] Files = new string[5000];

                        //First we write the directories
                        //Directories = txtOutDirs.Text.Split('\n');
                        //Files = txtOutFiles.Text.Split('\n');

                        if (Files.Length + Directories.Length == 2)
                        {
                            MessageBox.Show("No results yet - nothing to export!", "Error");
                        }
                        else
                        {

                            StreamWriter fileWrite = new StreamWriter(fdlExportBackEnd.FileName);
                            // Standard Discovered Directories...
                            fileWrite.WriteLine("#Directories");
                            int zz = 0;
                            for (zz = 0; zz < lstViewDirs.Items.Count; zz++)
                            {
                                if (lstViewDirs.Items[zz].ToString().Length > 0)
                                {
                                    fileWrite.WriteLine("{0},{1}", txtIPNumber.Text, lstViewDirs.Items[zz].ToString());
                                }
                            }
                            //Indexable Directories...
                            fileWrite.WriteLine("#Indexable");
                            for (zz = 0; zz < lstViewIndexDirs.Items.Count; zz++)
                            {
                                if (lstViewIndexDirs.Items[zz].ToString().Length > 0)
                                {
                                    fileWrite.WriteLine("{0},{1}", txtIPNumber.Text, lstViewIndexDirs.Items[zz].ToString());
                                }
                            }
                            // Standard Discovered Files
                            fileWrite.WriteLine("#Files");
                            for (zz = 0; zz < lstViewFiles.Items.Count; zz++)
                            {
                                if (lstViewFiles.Items[zz].ToString().Length > 0)
                                {
                                    fileWrite.WriteLine("{0},{1}", txtIPNumber.Text, lstViewFiles.Items[zz].ToString());
                                }
                            }
                            /*foreach (string DirectoryItem in Directories) {
                                string wDirectoryItem=DirectoryItem.Replace("\r","");
                                if (wDirectoryItem.Length > 0) {
                                    fileWrite.WriteLine("{0},{1}",txtIPNumber.Text,wDirectoryItem);
                                }
                            }
                            //Now we write the files
                            foreach (string FileItem in Files) {
                                string wFileItem=FileItem.Replace("\r","");
                                if (wFileItem.Length > 0) {
                                    fileWrite.WriteLine("{0},{1}",txtIPNumber.Text,wFileItem);
                                }
                            }*/

                            fileWrite.Close();
                            MessageBox.Show("Results successfully exported to " + fdlExportBackEnd.FileName, "Info");
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Problem writing file\n" + ex.Message, "Error reading file");
                    }
                }
            }
        }

        private void btnLoadDirs_Click(object sender, System.EventArgs e)
        {

            fdlLoadBackEndDirs.ShowDialog();
            if (fdlLoadBackEndDirs.FileName.Length > 0)
            {
                try
                {
                    StreamReader fileRead = new StreamReader(fdlLoadBackEndDirs.FileName);

                    string readline = "";
                    while ((readline = fileRead.ReadLine()) != null)
                    {
                        txtInDirs.Text += readline + "\r\n";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problem reading file\n" + ex.Message, "Error reading file");
                }
            }
        }

        private void btnLoadFiles_Click(object sender, System.EventArgs e)
        {

            fdlLoadBackEndFiles.ShowDialog();
            if (fdlLoadBackEndFiles.FileName.Length > 0)
            {
                try
                {
                    StreamReader fileRead = new StreamReader(fdlLoadBackEndFiles.FileName);
                    string readline = "";
                    while ((readline = fileRead.ReadLine()) != null)
                    {
                        txtInFiles.Text += readline + "\r\n";
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Problem reading file\n" + ex.Message, "Error reading file");
                }
            }
        }

        private void btnLoadFileformat_Click(object sender, System.EventArgs e)
        {

            fdlLoadBackEndExt.ShowDialog();
            if (fdlLoadBackEndExt.FileName.Length > 0)
            {
                try
                {
                    StreamReader fileRead = new StreamReader(fdlLoadBackEndExt.FileName);
                    string readline = "";
                    while ((readline = fileRead.ReadLine()) != null)
                    {
                        txtInFileTypes.Text += readline + "\r\n";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Problem reading file\n" + ex.Message, "Error reading file");
                }
            }
        }
        #endregion

        #region Main Scanner Routines
        #region Spider Routines
        private void btn_InsieWinsieSpider(object sender, EventArgs e)
        {
            bool Error = true;
            try
            {
                System.Convert.ToInt32(txt_SpiderPort.Text);
                Error = false;
            }
            catch
            {
                MessageBox.Show("Please enter a valid Port Number.", "Port Error");
            }
            if (!Error)
            {
                DelegateToSpider delHTStartDelegate = new DelegateToSpider(GoInsieWinsie);
                AsyncCallback callBackWhenDone = new AsyncCallback(this.EndInsieWinsie);
                delHTStartDelegate.BeginInvoke(callBackWhenDone, null);
            }
        }

        private void GoInsieWinsie()
        {
            this.Invoke(this.dlgControlDisable, new Object[] { this.btnHTStart, false });
            this.Invoke(this.dlgControlDisable, new Object[] { this.btnHTStop, true });
            this.Invoke(this.dlgControlListCls, new Object[] { this.lstMirrorDirs });
            this.Invoke(this.dlgControlListCls, new Object[] { this.lstMirrorLinks });
            this.Invoke(this.dlgControlTextSet, new Object[] { this.lblMirrorStatus, "Spidering Target..." });
            this.Invoke(this.dlgControlProgVal, new Object[] { this.prgHT, 0 });
            System.Uri uri;
            String Hostname = this.Invoke(this.dlgControlTextGet, new Object[] { this.txtHTTarget }).ToString();
            String Port = this.Invoke(this.dlgControlTextGet, new Object[] { this.txt_SpiderPort }).ToString();
            if (chk_SpiderSSL.Checked)
            {
                uri = new System.Uri("https://" + Hostname + ":" + Port);
            }
            else
            {
                uri = new System.Uri("http://" + Hostname + ":" + Port);
            }
            //System.Uri uri = new System.Uri("http://" + this.Invoke(this.dlgControlTextGet, new Object[] { this.txtHTTarget }).ToString());
            Boolean excludeidx = false;
            String excludes = "";
            if (chk_ignoreidx.Checked)
            {
                excludes = this.txt_ConfigSpiderExclude.Text;
                excludeidx = true;
            }
            else
            {
                excludes = this.txt_ConfigSpiderExclude.Text + "," + this.txt_idxflags.Text;
                excludeidx = false;
            }
            spd = new Spider(excludes, this.txt_ConfigSpiderExtension.Text, this.txt_content.Text, (int)this.nud_contentsize.Value, this.txt_excdirs.Text, excludeidx);
            spd.ReportTo = this;
            spd.Start(uri, System.Convert.ToInt32(NUPDOWNspider.Value));
        }

        private void EndInsieWinsie(IAsyncResult arResult)
        {
            this.Invoke(this.dlgControlDisable, new Object[] { this.btnHTStart, true });
            this.Invoke(this.dlgControlDisable, new Object[] { this.btnHTStop, false });
            this.Invoke(this.dlgControlTextSet, new Object[] { this.lblMirrorStatus, "Not Running" });
            this.Invoke(this.dlgControlProgMax, new Object[] { this.prgHT, 100 });
            this.Invoke(this.dlgControlProgVal, new Object[] { this.prgHT, 0 });
            try
            {
                spd.Quit = true;
                spd.End();
                spd.reset();
                spd = null;
                GC.Collect();
            }
            catch { GC.Collect(); }
        }

        private void chk_SpiderSSL_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_SpiderSSL.Checked)
            {
                txt_SpiderPort.Text = "443";
            }
            else
            {
                txt_SpiderPort.Text = "80";
            }
        }

        public void WriteExcluded(string url)
        {
            String Moo = ParseDirectores(url);
            try
            {
                if (Moo != "")
                    this.Invoke(this.dlgControlListUnq, new Object[] { this.lstMirrorDirs, Moo });
                this.Invoke(this.dlgControlProgMax, new Object[] { this.prgHT, spd.TotalResults });
                this.Invoke(this.dlgControlProgVal, new Object[] { this.prgHT, spd.FoundResults });
            }
            catch { }
        }

        public void SetLastUrl(string url)
        {
            try
            {
                this.Invoke(this.dlgControlListUnq, new Object[] { this.lstMirrorLinks, url });
                this.Invoke(this.dlgControlProgMax, new Object[] { this.prgHT, spd.TotalResults });
                this.Invoke(this.dlgControlProgVal, new Object[] { this.prgHT, spd.FoundResults });
            }
            catch { }
        }

        private void btnHTStop_Click(object sender, EventArgs e)
        {
            try
            {
                spd.reset();
                spd.Quit = true;
                spd.End();
                //spd.Quit = true;
                //spd.End();
                //spd.reset();
                spd = null;
                GC.Collect();
            }
            catch { GC.Collect(); }
        }
        #endregion

        #region Google Routines
        private void UpdateGoogleUI()
        {
            lblQuery.Text = "Done";
            btnGoogleStart.Enabled = true;
            btnStopGoole.Enabled = false;

        }

        private void populatekeyword(object sender, System.EventArgs e)
        {
            txtGoogleKeyword.Text = txtGoogleTarget.Text.Replace("www.", "").Replace(".com", "").Replace(".net", "");
        }

        private void btnGoogleStart_Click(object sender, System.EventArgs e)
        {
            stoppitgoogle = false;
            lstGoogleLink.Items.Clear();
            lstGoogleDir.Items.Clear();
            btnGoogleStart.Enabled = false;
            BeginGoogleTaskThread();
        }

        public void BeginGoogleTaskThread()
        {
            //DelegateToGoogleTask delGoogleTaskDelegate = new DelegateToGoogleTask(LongTaskGoogle);
            DelegateToGoogleTask delGoogleTaskDelegate = new DelegateToGoogleTask(LongTaskSpud);
            AsyncCallback callGoogleBackWhenDone = new AsyncCallback(this.EndGoogleTaskThread);
            delGoogleTaskDelegate.BeginInvoke(callGoogleBackWhenDone, null);
        }

        public void EndGoogleTaskThread(IAsyncResult arResult)
        {
            MethodInvoker miGoogle = new MethodInvoker(this.UpdateGoogleUI);
            this.BeginInvoke(miGoogle);
        }

        private void btnSpudLocate_Click(object sender, EventArgs e)
        {
            DialogResult didheopen = folderBrowserDialog1.ShowDialog();
            if (didheopen != DialogResult.OK)
            {
                return;
            }
            if (folderBrowserDialog1.SelectedPath.Length > 0)
            {
                txtSpudDirectory.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        public void btn_Spud_Click(object sender, EventArgs e)
        {
            //check if spud is running first (exception not in SPUD itself yet)
            Process[] processSpud = Process.GetProcessesByName("SPUD");

            if (processSpud.Length > 0)
            {
                MessageBox.Show("SPUD is already running", "SPUD");
                return;
            }

            #region Registry Start

            //string spudname = "";
            //try
            //{
            //    RegistryKey NewKey = Registry.CurrentUser;
            //    NewKey = NewKey.OpenSubKey(@"SOFTWARE\Microsoft\Installer\Assemblies", false);
            //    string[] subkeynames = NewKey.GetSubKeyNames();
            //    foreach (string subkeyname in subkeynames)
            //    {
            //        if (subkeyname.ToUpper().Contains("SENSEPOST") && subkeyname.ToUpper().Contains("SPUDEXE"))
            //        {
            //            spudname = subkeyname.Replace('|', '\\');
            //            spudname = spudname.ToLower();
            //            spudname = spudname.Replace("spudexelib.dll", "spud.exe");
            //        }
            //    }
            //    NewKey.Close();
            //}
            //catch
            //{
            //    spudname = "";
            //}
            //if (spudname == "")
            //{
            //    try
            //    {
            //        RegistryKey NewKey = Registry.LocalMachine;
            //        NewKey = NewKey.OpenSubKey(@"SOFTWARE\Classes\Installer\Assemblies", false);
            //        string[] subkeynames = NewKey.GetSubKeyNames();
            //        foreach (string subkeyname in subkeynames)
            //        {
            //            if (subkeyname.ToUpper().Contains("SENSEPOST") && subkeyname.ToUpper().Contains("SPUDEXE"))
            //            {
            //                spudname = subkeyname.Replace('|', '\\');
            //                spudname = spudname.ToLower();
            //                spudname = spudname.Replace("spudexelib.dll", "spud.exe");
            //            }
            //        }
            //        NewKey.Close();
            //    }
            //    catch
            //    {
            //        spudname = "";
            //    }
            //}
            //if (spudname == "")
            //{
            //    MessageBox.Show("SensePost SPUD is not installed or could not be located.", "SPUD Error");
            //    string targetURL = "http://www.sensepost.com/research/spud";
            //    System.Diagnostics.Process.Start(targetURL);
            //}
            //else
            //{
            //    try
            //    {
            //        String[] moo = spudname.Split('\\');
            //        int moolength = moo.Length - 3;
            //        String DirStart = "";
            //        for (int i = 0; i <= moolength; i++)
            //        {
            //            if (DirStart.Length == 0) DirStart = moo[i];
            //            else DirStart = DirStart + "\\" + moo[i];
            //        }
            //        System.Diagnostics.ProcessStartInfo inf = new System.Diagnostics.ProcessStartInfo();
            //        inf.WorkingDirectory = DirStart;
            //        inf.FileName = spudname;
            //        System.Diagnostics.Process.Start(inf);
            //        //System.Diagnostics.Process.Start(auraname);
            //    }
            //    catch
            //    {
            //        MessageBox.Show("SensePost SPUD (" + spudname + ") could not be started.", "SPUD Error");
            //        string targetURL = "http://www.sensepost.com/research/spud";
            //        System.Diagnostics.Process.Start(targetURL);
            //    }
            //}

            #endregion

            if (txtSpudDirectory.Text.Equals(String.Empty))
            {
                MessageBox.Show("Please locate the Spud directory first", "SPUD");
            }
            else
            {
                try
                {
                    System.Diagnostics.ProcessStartInfo inf = new System.Diagnostics.ProcessStartInfo();
                    inf.WorkingDirectory = txtSpudDirectory.Text.Trim();
                    inf.FileName = "bin/spud.exe";
                    System.Diagnostics.Process.Start(inf);
                }
                catch
                {
                    MessageBox.Show("Spud could not be started, sorry");
                }
            }
        }

        public void LongTaskSpud()
        {
            this.Invoke(this.dlgControlerSetReadonly, new Object[] { this.txtGoogleTarget, true });
            this.Invoke(this.dlgControlerSetReadonly, new Object[] { this.txtWords, true });
            this.Invoke(this.dlgControlerSetReadonly, new Object[] { this.txtGoogleKeyword, true });
            stoppitgoogle = false;


            /*com.google.api.GoogleSearchService myGoogleservice = new com.google.api.GoogleSearchService();
            com.google.api.GoogleSearchResult myGoogleresult = new com.google.api.GoogleSearchResult();
            com.google.api.ResultElement myElement = new com.google.api.ResultElement();
            string googleKey = txtGoogleKey.Text;*/

            int wordcount = 0;
            string[] googleWords = new string[100];
            ArrayList uniqueURLs = new ArrayList();

            this.Invoke(this.dlgControlDisable, new Object[] { this.btnGoogleStart, false });
            this.Invoke(this.dlgControlDisable, new Object[] { this.btnStopGoole, true });

            googleWords = this.Invoke(this.dlgControlTextGet, new Object[] { this.txtWords }).ToString().Split(',');
            wordcount = googleWords.Length;

            int the_depth = System.Convert.ToInt32(this.Invoke(this.dlgControllNupGet, new Object[] { this.updownGoogleDepth }));
            String the_targt = this.Invoke(this.dlgControlTextGet, new Object[] { this.txtGoogleTarget }).ToString();
            String the_keywd = this.Invoke(this.dlgControlTextGet, new Object[] { this.txtGoogleKeyword }).ToString();

            this.Invoke(this.dlgControlProgVal, new Object[] { this.prgGoogle, 0 });
            this.Invoke(this.dlgControlProgMax, new Object[] { this.prgGoogle, wordcount });

            foreach (string googleWord in googleWords)
            {
                string query = the_keywd + " filetype:" + googleWord + " site:" + the_targt;
                this.Invoke(this.dlgControlTextSet, new Object[] { this.txtGoogleQuery, query });
                this.Invoke(this.dlgControlTextSet, new Object[] { this.lblGoogleStatus, query });

                int j;

                for (j = 0; j <= the_depth && (stoppitgoogle == false); j += 10)
                {
                    com.sensepost.spud.obj.Service the_srv = new SensePost.Wikto.com.sensepost.spud.obj.Service();
                    com.sensepost.spud.obj.StructuredResult the_res = new SensePost.Wikto.com.sensepost.spud.obj.StructuredResult();


                    this.Invoke(this.dlgControlTextSet, new Object[] { this.lblPageNumber, j.ToString() });
                    try
                    {
                        the_res = the_srv.GetStructResult(query, j, 10, false);
                        //myGoogleresult = myGoogleservice.doGoogleSearch(googleKey, query, j, 10, false, "", false, "", "latin1", "latin1");
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(this.dlgControlListAdd, new Object[] { this.lstGoogleLink, ex.Message });
                        //if (ex.Message.IndexOf("Invalid authorization key:") > 0)
                        //{
                        //    MessageBox.Show("Your Google API key appear to be invalid.\nPlease enter a valid key in System Config section.");
                        //    stoppitgoogle = true;
                        //}
                    }
                    this.Invoke(this.dlgControlTextSet, new Object[] { this.lblEstimate, the_res.ResultTotal.ToString() });

                    if (the_res.ResultItems != null)
                    {
                        foreach (com.sensepost.spud.obj.StructuredResultElement the_elm in the_res.ResultItems)
                        {
                            this.Invoke(this.dlgControlListUnq, new Object[] { this.lstGoogleLink, the_elm.ResultUrl.ToString() });
                            parseGoogleCGIs(the_elm.ResultUrl.ToString());
                        }
                    }

                }
                this.Invoke(this.dlgControlProgInc, new Object[] { this.prgGoogle, 1 });
            }
            this.Invoke(this.dlgControlProgVal, new Object[] { this.prgGoogle, 0 });
        }

        private void parseGoogleCGIs(string URL)
        {
            try
            {
                URL = URL.ToLower();
                string reconstruct = "";
                string[] parts = new string[200];
                parts = URL.Split('/');

                ArrayList partsrec = new ArrayList();
                parts = URL.Split('/');
                if (parts.Length > 4)
                {
                    for (int i = 3; i < parts.Length - 1; i++)
                    {
                        reconstruct = "";
                        for (int j = 3; j <= i; j++)
                        {
                            if (parts[j].Length > 0)
                            {
                                reconstruct += "/" + parts[j];
                            }
                        }
                        if (reconstruct.Length > 0)
                        {
                            partsrec.Add(reconstruct);
                        }
                    }
                }

                string[] allCGIs = new string[20000];
                foreach (string foundCGI in partsrec)
                {
                    this.Invoke(this.dlgControlListUnq, new Object[] { this.lstGoogleDir, foundCGI });
                }
            }
            catch
            {
                //eek we do nothing...tsk tsk
            }

        }
        #endregion

        #region BackEnd Routines
        private void UpdateUI()
        {
            lblStatus.Text = "Done";
            btn_BEStart.Enabled = true;
            btn_BEStop.Enabled = false;
            btn_BESkiptoDirs.Enabled = false;
            btn_BESkiptoFiles.Enabled = false;
        }

        private void btnStart_Click(object sender, System.EventArgs e)
        {
            btn_BEClearDB.PerformClick();
            stoppit = false;
            skipDirectories = false;
            if (!chkPreserve.Checked)
            {
                lstViewFiles.Items.Clear();
                lstViewDirs.Items.Clear();
                lstViewIndexDirs.Items.Clear();
                backend_dirresults = new BackEndMining[200000];
                backend_filresults = new BackEndMining[200000];
                backend_dirresults[0].location = "/";
                backend_dirresults[0].ai = "0";
                backend_dirresults[0].indexable = false;
                backend_dirresults[0].responsecode = "200";
            }
            btnBackEndPause.Enabled = true;
            BeginLongTaskThread();
        }

        public void BeginLongTaskThread()
        {
            DelegateToLongTask delLongTaskDelegate = new DelegateToLongTask(LongTaskinClass);
            AsyncCallback callBackWhenDone = new AsyncCallback(this.EndLongTaskThread);
            delLongTaskDelegate.BeginInvoke(callBackWhenDone, null);
        }

        public void EndLongTaskThread(IAsyncResult arResult)
        {
            MethodInvoker mi = new MethodInvoker(this.UpdateUI);
            this.BeginInvoke(mi);
        }

        public void LongTaskinClass()
        {
            // -------- This remains unchanged...
            this.Invoke(this.dlgControlDisable, new Object[] { this.btn_BEStart, false });
            this.Invoke(this.dlgControlDisable, new Object[] { this.btn_BEStop, true });
            this.Invoke(this.dlgControlDisable, new Object[] { this.btn_BESkiptoDirs, true });
            this.Invoke(this.dlgControlDisable, new Object[] { this.btn_BESkiptoFiles, true });
            this.Invoke(this.dlgControlProgVal, new Object[] { this.prgsDirs, 0 });
            this.Invoke(this.dlgControlProgVal, new Object[] { this.prgsFiles, 0 });

            // -------- This is new new block of code...
            // In this case, we're going to seperate the Files and Directories blocks.
            string[] Directories = new string[100000];
            string[] NewDirectories = new string[100000];
            ArrayList TestedDirectories = new ArrayList();
            int n_Found = -1;
            int n_BootStrap = 0;
            maxBackEndAI = 0;
            minBackEndAI = 100;
            string w2NewDirectory = "";
            int n_DirCount = 1;
            int n_FilCount = 0;

            //if (chkPreserve.Checked) n_DirCount = backend_dirresults.Length;
            //if (chkPreserve.Checked) n_FilCount = backend_filresults.Length;

            //a value of 0.8 seems to return less FP's...
            //if (Convert.ToDouble(NUPDOWNBackEnd.Value) < 0.9 && chkBackEndAI.Checked)
            //{
            //    MessageBox.Show("The new content compare algorithm is much more accurate.\n\nWe suggest a trigger value of around 0.9", "AI Brute Forcing");
            //}
            if (!chkPreserve.Checked)
            {
                this.Invoke(this.dlgControlViewCls, new Object[] { this.lstViewDirs });

                ListViewItem lvi = new ListViewItem(" ");
                lvi.ImageIndex = 0;
                lvi.SubItems.Add("/");
                this.Invoke(this.dlgControlViewUnq, new Object[] { this.lstViewDirs, lvi });
            }
            while (n_Found != 0)
            {
                n_Found = 0;
                // We get the list of directories...
                NewDirectories = this.Invoke(this.dlgControlViewArr, new Object[] { this.lstViewDirs, 0 }).ToString().Split('\n');
                // We create the root directory if no directories are contained in the list...
                if (NewDirectories.Length == 0) { NewDirectories[0] = "/"; }

                // Now we start with the funky mining processes...
                foreach (string NewDirectory in NewDirectories)
                {
                    stopdir = false;
                    if (stoppit)
                    {
                        n_Found = 0;
                        break;
                    }

                    while (pauseBackEnd)
                    {
                        System.Threading.Thread.Sleep(1000);
                        if (stoppit)
                        {
                            pauseBackEnd = false;
                            n_Found = 0;
                            break;
                        }
                    }

                    string wNewDirectory = NewDirectory.Replace("\r", "");
                    if ((n_BootStrap != 0) && (wNewDirectory.Equals("/")))
                    {
                        wNewDirectory = wNewDirectory.Replace("/", "");
                    }

                    if (wNewDirectory.Length > 0)
                    {
                        //First we test the directories
                        Directories = this.Invoke(this.dlgControlTextGet, new Object[] { this.txtInDirs }).ToString().Split('\n');
                        this.Invoke(this.dlgControlProgVal, new Object[] { this.prgsDirs, 0 });
                        this.Invoke(this.dlgControlProgMax, new Object[] { this.prgsDirs, Directories.Length });
                        if (!TestedDirectories.Contains(wNewDirectory))
                        {
                            foreach (string DirectoryItem in Directories)
                            {
                                string wDirectoryItem = DirectoryItem.Replace("\r", "");
                                if (wDirectoryItem.Length > 0)
                                {
                                    if (stoppit || skipDirectories || stopdir)
                                    {
                                        break;
                                    }

                                    while (pauseBackEnd)
                                    {
                                        System.Threading.Thread.Sleep(1000);
                                        if (stoppit || skipDirectories || stopdir)
                                        {
                                            pauseBackEnd = false;
                                            n_Found = 0;
                                            break;
                                        }
                                    }

                                    this.Invoke(this.dlgControlProgInc, new Object[] { this.prgsDirs, 1 });
                                    if (wNewDirectory.Equals("/"))
                                    {
                                        w2NewDirectory = wNewDirectory.Replace("/", "");
                                    }
                                    else
                                    {
                                        w2NewDirectory = wNewDirectory;
                                    }

                                    string status = ("Dir:" + w2NewDirectory + wDirectoryItem + "/").Replace("//", "/");
                                    this.Invoke(this.dlgControlTextSet, new Object[] { this.lblStatus, status });

                                    //TIMER: get average response time start
                                    DateTime startTime = DateTime.Now;

                                    // And here's where the actual test occurrs...
                                    bool istrue = BuildBFDirReq(txtIPNumber.Text, txtIPPort.Text, buildRequest(w2NewDirectory + wDirectoryItem, "", ""), txtErrorCodeDir.Text, (int)updownTimeOutTCP.Value, n_DirCount, w2NewDirectory + wDirectoryItem);

                                    //TIMER: get average response time end
                                    TimeSpan ts = DateTime.Now - startTime;
                                    
                                    //i have to create two listviewitems because if i clone the original one (for indexable dirs) the font screws up...?? weird
                                    ListViewItem lvi = new ListViewItem(" ");
                                    ListViewItem lviIndex = new ListViewItem(" ");

                                    if (istrue)
                                    {
                                        lvi.ImageIndex = 0;
                                        lviIndex.ImageIndex = 0;

                                        string itemtext = (w2NewDirectory + wDirectoryItem + "/").Replace("//", "/");

                                        lvi.SubItems.Add(itemtext);
                                        lviIndex.SubItems.Add(itemtext);

                                        if (isitIndexable)
                                        {
                                            ListViewItem indexItem = lvi;
                                            this.Invoke(this.dlgControlViewUnq, new object[] { this.lstViewIndexDirs, lviIndex });
                                        }

                                        this.Invoke(this.dlgControlViewUnq, new object[] { this.lstViewDirs, lvi });
                                        n_Found++;
                                    }
                                    else
                                    {
                                        //check for weird response times in AI
                                        if ((chkTimeAnomalies.Checked) && (n_DirCount != 1))
                                        {
                                            if ((ts.Milliseconds < (previousTime / (double)nudTimeAnomaly.Value)) || (ts.Milliseconds > (previousTime * (double)nudTimeAnomaly.Value)))
                                            {
                                                lvi.ImageIndex = 1;
                                                lvi.SubItems.Add(w2NewDirectory + wDirectoryItem + "  ["+ ts.Milliseconds.ToString() +"ms]");
                                                this.Invoke(this.dlgControlViewUnq, new object[] { this.lstViewDirs, lvi });
                                            }
                                        }
                                    }
                                    previousTime = ts.Milliseconds;
                                }
                                n_DirCount++;
                            }
                        }
                        TestedDirectories.Add(w2NewDirectory);
                    }
                }
                n_BootStrap = 1;
            }
            string tmp = "";
            tmp = this.Invoke(this.dlgControlViewArr, new Object[] { this.lstViewDirs, 0 }).ToString();
            Directories = tmp.Split('\n');
            tmp = "";
            tmp = this.Invoke(this.dlgControlTextGet, new Object[] { this.txtInFiles }).ToString();
            string[] Files = tmp.Split('\n');
            tmp = "";
            tmp = this.Invoke(this.dlgControlTextGet, new Object[] { this.txtInFileTypes }).ToString();
            string[] FileTypes = tmp.Split('\n');
            tmp = "";
            this.Invoke(this.dlgControlProgMax, new Object[] { this.prgsFiles, Directories.Length * Files.Length * FileTypes.Length });

            //move to files
            foreach (string DirectoryItem in Directories)
            {
                if (stoppit) { break; }
                stopdir = false;

                foreach (string FileItem in Files)
                {
                    if (stoppit || stopdir) { break; }

                    foreach (string FileTypeItem in FileTypes)
                    {
                        if (stoppit || stopdir) { break; }

                        while (pauseBackEnd)
                        {
                            System.Threading.Thread.Sleep(1000);
                            if (stoppit || stopdir) 
                            {
                                pauseBackEnd = false;
                                break; 
                            }
                        }

                        this.Invoke(this.dlgControlProgInc, new Object[] { this.prgsFiles, prgsFiles.Maximum / (Directories.Length * Files.Length * FileTypes.Length) });

                        string wFileTypeItem = FileTypeItem.Replace("\r", "");
                        string wDirectoryItem = DirectoryItem.Replace("\r", "");
                        string wFileItem = FileItem.Replace("\r", "");

                        if (wDirectoryItem.Length > 0 && wFileItem.Length > 0 && wFileTypeItem.Length > 0)
                        {
                            if (wDirectoryItem.EndsWith("/"))
                            {
                                this.Invoke(this.dlgControlTextSet, new Object[] { this.lblStatus, "File: " + txtIPNumber.Text + ":" + wDirectoryItem + wFileItem + "." + wFileTypeItem });
                            }
                            else
                            {
                                this.Invoke(this.dlgControlTextSet, new Object[] { this.lblStatus, "File: " + txtIPNumber.Text + ":" + wDirectoryItem + "/" + wFileItem + "." + wFileTypeItem });
                            }
                            
                            //TIMER: get average response time start
                            DateTime startTime = DateTime.Now;

                            //this is where the file exist check is done
                            bool istrue = BuildBFFileRequest(txtIPNumber.Text, txtIPPort.Text, buildRequest(wDirectoryItem, wFileItem, wFileTypeItem),txtErrorCodeFile.Text,(int)updownTimeOutTCP.Value, n_FilCount, wFileItem + "." + wFileTypeItem, DirectoryItem);

                            //TIMER: get average response time end
                            TimeSpan ts = DateTime.Now - startTime;
                            ListViewItem lvi = new ListViewItem(" ");

                            if (istrue)
                            {
                                if (wDirectoryItem.EndsWith("/"))
                                {
                                    lvi.SubItems.Add(wDirectoryItem + wFileItem + "." + wFileTypeItem);
                                }
                                else
                                {
                                    lvi.SubItems.Add(wDirectoryItem + "/" + wFileItem + "." + wFileTypeItem);
                                }
                                lvi.ImageIndex = 2;
                                this.Invoke(this.dlgControlViewUnq, new object[] { this.lstViewFiles, lvi });
                            }
                            else
                            {
                                //check for weird response times in AI
                                if ((chkTimeAnomalies.Checked) && (n_FilCount != 1))
                                {
                                    if ((ts.Milliseconds < (previousTime / (double)nudTimeAnomaly.Value)) || (ts.Milliseconds > (previousTime * (double)nudTimeAnomaly.Value)))
                                    {
                                        if (wDirectoryItem.EndsWith("/"))
                                        {
                                            lvi.SubItems.Add(wDirectoryItem + wFileItem + "." + wFileTypeItem + "  [" + ts.Milliseconds.ToString() + "ms]");
                                        }
                                        else
                                        {
                                            lvi.SubItems.Add(wDirectoryItem + "/" + wFileItem + "." + wFileTypeItem + "  [" + ts.Milliseconds.ToString() + "ms]");
                                        }

                                        lvi.ImageIndex = 1;
                                        this.Invoke(this.dlgControlViewAdd, new object[] { this.lstViewFiles,lvi });
                                    }
                                }
                            }
                            previousTime = ts.Milliseconds;
                            n_FilCount++;
                        }
                    }
                }
            }
            this.Invoke(this.dlgControlDisable, new object[] { btnBackEndPause, false });
            // ------ END HERE.
        }

        private void lvw_NiktoResults_DoubleClick(object sender, EventArgs e)
        {
            ListView lv = (ListView)sender;
            ListView.SelectedListViewItemCollection items = lv.SelectedItems;

            ListViewItem lvi = items[0];
            string adds = "";

            if(chkuseSSLWikto.Checked)
                adds = "s";

            string url = "http" + adds + "://" + txtNiktoTarget.Text + lvi.SubItems[3].Text;
            System.Diagnostics.Process.Start(url);
        }

        private void btnBackEndPause_Click(object sender, EventArgs e)
        {
            pauseBackEnd = !pauseBackEnd;

            if (pauseBackEnd)
                btnBackEndPause.Text = "Resume";
            else
                btnBackEndPause.Text = "Pause";
        }

        private void chkTimeAnomalies_CheckedChanged(object sender, EventArgs e)
        {
            nudTimeAnomaly.Enabled = chkTimeAnomalies.Checked;
        }

        private void lstViewDirs_DoubleClick(object sender, EventArgs e)
        {
            ListView lv = (ListView)sender;
            ListView.SelectedListViewItemCollection items = lv.SelectedItems;

            ListViewItem lvi = items[0];

            if (lvi.ImageIndex != 1)
            {
                string adds = "";

                if (chkuseSSLWikto.Checked)
                    adds = "s";

                string url = "http" + adds + "://" + txtIPNumber.Text + lvi.SubItems[1].Text;
                System.Diagnostics.Process.Start(url);
            }
        }

        public bool BuildBFDirReq(string ipRaw, string portRaw, string requestRaw, string errorCodes, int TimeOut, int DirNum, string MyDir)
        {

            if (chkProxyPresent.Checked)
            {
                string[] proxyItems = new string[2];
                proxyItems = txtProxySettings.Text.Split(':');
                ipRaw = proxyItems[0];
                portRaw = proxyItems[1];
            }
            string response = "";
            if (chkBackEnduseSSLport.Checked)
            {
                response = sendraw(ipRaw, portRaw, requestRaw, 1024, TimeOut, true);
            }
            else
            {
                response = sendraw(ipRaw, portRaw, requestRaw, 1024, TimeOut);
            }

            //lets check indexability first
            if (response.IndexOf("ndex of") >= 0 || response.IndexOf("To Parent Directory") >= 0)
            {
                isitIndexable = true;
            }
            else
            {
                isitIndexable = false;
            }

            if (chkBackEndAI.Checked)
            {
                //AI stuff
                //string[] parts = new string[2000];
                //parts=requestRaw.Split(' ');
                double result;
                niktoRequests niktoset;
                niktoset.description = "FPtestdir";
                niktoset.request = requestRaw;
                niktoset.trigger = "";
                niktoset.type = "FPtestdir";
                niktoset.method = "GET";
                niktoset.sensepostreq = "";
                string[] urlparts = new string[20];
                urlparts = MyDir.Split('/');
                string finalresult = "";
                for (int a = 1; a < urlparts.Length - 1; a++)
                {
                    finalresult += "/" + urlparts[a];
                }
                // Here we will build the dummy directory request...
                if (urlparts[urlparts.Length - 1].IndexOf(".") > 0)
                {
                    string[] splitter = urlparts[urlparts.Length - 1].Split('.');
                    finalresult = finalresult + "/" + splitter[0] + "SensePostNotThereNoNo." + splitter[1];
                }
                else
                {
                    finalresult = finalresult + "/" + urlparts[urlparts.Length - 1].Substring(0, 1) + "SensePostNotThereNoNo" + urlparts[urlparts.Length - 1].Substring(urlparts[urlparts.Length - 1].Length - 1, 1);
                }
                result = testniktoFP(ipRaw, portRaw, niktoset, finalresult, response);
                string[] responseline = new string[5];
                responseline = response.Split('\n');
                string[] GetTheCode = responseline[0].Split(' ');
                backend_dirresults[DirNum].location = MyDir;
                backend_dirresults[DirNum].ai = result.ToString();
                backend_dirresults[DirNum].responsecode = GetTheCode[1];
                backend_dirresults[DirNum].indexable = isitIndexable;
                if ((result < Convert.ToDouble(NUPDOWNBackEnd.Value)) && result >= 0.00)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //normal stuff
                string[] responseline = new string[5];
                string[] errorcodes = new string[20];
                responseline = response.Split('\n');
                errorcodes = errorCodes.Split(',');
                string[] GetTheCode = responseline[0].Split(' ');
                backend_dirresults[DirNum].location = MyDir;
                backend_dirresults[DirNum].ai = "";
                backend_dirresults[DirNum].responsecode = GetTheCode[1];
                backend_dirresults[DirNum].indexable = isitIndexable;
                foreach (string errorcodeItem in errorcodes)
                {
                    if (responseline[0].IndexOf(errorcodeItem) > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool BuildBFFileRequest(string ipRaw, string portRaw, string requestRaw, string errorCodes, int TimeOut, int FileNum, string MyFile, string MyDir)
        {
            if (chkProxyPresent.Checked)
            {
                string[] proxyItems = new string[2];
                proxyItems = txtProxySettings.Text.Split(':');
                ipRaw = proxyItems[0];
                portRaw = proxyItems[1];
            }
            string response = "";
            if (chkBackEnduseSSLport.Checked)
            {
                response = sendraw(ipRaw, portRaw, requestRaw, 1024, TimeOut, true);
            }
            else
            {
                response = sendraw(ipRaw, portRaw, requestRaw, 1024, TimeOut);
            }

            if (chkBackEndAI.Checked == true)
            {
                string[] parts = new string[2000];
                parts = requestRaw.Split(' ');
                double result;
                niktoRequests niktoset;

                //true=file false=dir
                niktoset.description = "FPtestfile";
                niktoset.request = requestRaw;
                niktoset.trigger = "";
                niktoset.type = "FPtestfile";
                niktoset.method = "GET";
                niktoset.sensepostreq = "";
                string TestFile = "";
                if (!MyDir.EndsWith("/"))
                    MyDir = MyDir + "/";
                if (MyFile.StartsWith("."))
                {
                    if (MyFile.Length < 3)
                    {
                        TestFile = MyDir + MyFile.Substring(0, MyFile.Length) + "SensePostNotThereNoNo" + MyFile.Substring(0, MyFile.Length);
                    }
                    else
                    {
                        TestFile = MyDir + MyFile.Substring(0, 3) + "SensePostNotThereNoNo" + MyFile.Substring(MyFile.Length - 3, 3);
                    }
                }
                else if (MyFile.IndexOf(".") > 0)
                {
                    string[] splitter = MyFile.Split('.');
                    if (MyFile.Length < 3)
                    {
                        TestFile = MyDir + MyFile.Substring(0, MyFile.Length) + "SensePostNotThereNoNo." + splitter[1];
                    }
                    else
                    {
                        TestFile = MyDir + MyFile.Substring(0, 3) + "SensePostNotThereNoNo." + splitter[1];
                    }
                }
                else
                {
                    if (MyFile.Length < 3)
                    {
                        TestFile = MyDir + MyFile.Substring(0, MyFile.Length) + "SensePostNotThereNoNo" + MyFile.Substring(0, MyFile.Length);
                    }
                    else
                    {
                        TestFile = MyDir + MyFile.Substring(0, 3) + "SensePostNotThereNoNo" + MyFile.Substring(MyFile.Length - 3, 3);
                    }
                }
                result = testniktoFP(ipRaw, portRaw, niktoset, TestFile, response);
                string[] responseline = new string[5];
                responseline = response.Split('\n');
                string[] GetTheCode = responseline[0].Split(' ');
                backend_filresults[FileNum].location = MyDir + MyFile;
                backend_filresults[FileNum].ai = result.ToString();
                backend_filresults[FileNum].responsecode = GetTheCode[1];
                backend_filresults[FileNum].indexable = false;
                if ((result < Convert.ToDouble(NUPDOWNBackEnd.Value)) && result >= 0.00)
                {
                    return true;
                }
                else { return false; }

            }
            else
            {
                //normal stuff
                string[] responseline = new string[5];
                string[] errorcodes = new string[20];

                responseline = response.Split('\n');
                string[] GetTheCode = responseline[0].Split(' ');
                backend_filresults[FileNum].location = MyDir + MyFile;
                backend_filresults[FileNum].ai = "";
                backend_filresults[FileNum].responsecode = GetTheCode[1];
                backend_filresults[FileNum].indexable = false;
                errorcodes = errorCodes.Split(',');
                foreach (string errorcodeItem in errorcodes)
                {
                    if (responseline[0].IndexOf(errorcodeItem) > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private void ToggleBackendAI(object sender, System.EventArgs e)
        {
            if (chkBackEndAI.Checked == true)
            {
                NUPDOWNBackEnd.Enabled = true;
                txtErrorCodeDir.Enabled = false;
                txtErrorCodeFile.Enabled = false;
                radioHEAD.Checked = false;
                radioGET.Checked = true;
                radioHEAD.Enabled = false;
            }
            if (chkBackEndAI.Checked == false)
            {
                NUPDOWNBackEnd.Enabled = false;
                txtErrorCodeDir.Enabled = true;
                txtErrorCodeFile.Enabled = true;
                radioHEAD.Enabled = true;
            }
        }

        private void btnClearBackEndDB_Click(object sender, System.EventArgs e)
        {
            for (int i = 0; i < globalFPb; i++)
            {
                backend_FP[i].URLlocation = "";
                backend_FP[i].filetype = "";
                backend_FP[i].HTTPblob = "";
            }
            //lblBackEndAI.Text="Fingerpint DB cleared";
            globalFPb = 0;
            minBackEndAI = 100;
            maxBackEndAI = 0;
        }

        private void lstViewDirs_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            ListView myListView = (ListView)sender;
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            myListView.Sort();

        }

        private void chkBackEnduseSSLport_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBackEnduseSSLport.Checked)
            {
                txtIPPort.Text = "443";
            }
            else
            {
                txtIPPort.Text = "80";
            }
        }
        #endregion

        #region Nikto Routines
        private void btnStartNikto_Click(object sender, System.EventArgs e)
        {
            NiktoOptimised = new Hashtable();
            btn_WiktoStart.Enabled = false;
            stopnikto = false;
            lvw_NiktoResults.Items.Clear();
            txtNiktoReq.Clear();
            txtNiktoRes.Clear();
            btnPauseWikto.Enabled = true;
            BeginNiktoTaskThread();
        }

        private void UpdateNiktoUI()
        {
            btn_WiktoStart.Enabled = true;
            btn_WiktoStop.Enabled = false;
        }

        public void BeginNiktoTaskThread()
        {
            DelegateToNiktoTask delNiktoTaskDelegate = new DelegateToNiktoTask(LongTaskNikto);
            AsyncCallback callNiktoBackWhenDone = new AsyncCallback(this.EndNiktoTaskThread);
            delNiktoTaskDelegate.BeginInvoke(callNiktoBackWhenDone, null);
        }

        public void EndNiktoTaskThread(IAsyncResult arResult)
        {
            MethodInvoker miNikto = new MethodInvoker(this.UpdateNiktoUI);
            this.BeginInvoke(miNikto);
        }

        private String GetDummyReq(String niktoreq)
        {
            string originalReq = niktoreq;

            bool StartsWithSlash = false;
            bool EndsWithSlash = false;
            // Here's how we get this...
            // 1 - If the url starts or ends with "/", we remove them...
            if (niktoreq == "/")
            {
                StartsWithSlash = true;
                niktoreq = "";
                EndsWithSlash = true;
            }
            else if (niktoreq.StartsWith("/?"))
            {
                StartsWithSlash = true;
                EndsWithSlash = true;
            }
            else
            {
                if (niktoreq.StartsWith("/"))
                {
                    niktoreq = niktoreq.Substring(1, niktoreq.Length - 1);
                    StartsWithSlash = true;
                }
                if (niktoreq.EndsWith("/"))
                {
                    niktoreq = niktoreq.Substring(0, niktoreq.Length - 1);
                    EndsWithSlash = true;
                }
            }
            // 2 - If the url contains more than one ?, we're going to replace those at the end with SENSEPOSTPLACEHOLDER
            int q1 = 0;
            q1 = niktoreq.IndexOf("?");
            if (q1 > -1)
            {
                String[] tmp = niktoreq.Split('?');
                //String[] tmp = niktoreq.Split("?");
                niktoreq = "";
                bool FoundFirst = false;
                int idx = 0;
                foreach (String tmp2 in tmp)
                {
                    if (niktoreq == "" && idx == 0)
                    {
                        niktoreq = tmp2;
                    }
                    else
                    {
                        if (!FoundFirst)
                        {
                            niktoreq += ("?" + tmp2);
                            FoundFirst = true;
                        }
                        else { niktoreq += ("SENSEPOSTPLACEHOLDER" + tmp2); }
                    }
                    idx++;
                }
            }
            // 3 - Now, we split the request into URL, QUERYSTRING at the ?
            String[] url_qur = niktoreq.Split('?');
            // 4 - We now split the URL section at the directory delimiters...
            String[] dir_url = url_qur[0].Split('/');
            // 5 - And our last directory section will be the path we're interested in...
            String LastSect = dir_url[dir_url.Length - 1];
            // 6 - Now, we want out request to start and end the same as the original request.
            // So we concatenate the first three chars of the original, and the last 4 of the original onto our string...
            // But we first have to ensure that the string is long enough...
            String MyReq = "";
            if (LastSect.Length >= 4)
                MyReq = LastSect.Substring(0, 3) + "SensePostNotThereNoNo" + LastSect.Substring(LastSect.Length - 4, 4);
            else
                MyReq = LastSect + "SensePostNotThereNoNo" + LastSect;
            // 7 - Now, we reassamble the directory structure...
            String Dirs = "";
            int i = 0;
            for (i = 0; i < dir_url.Length - 1; i++)
            {
                if (Dirs == "") Dirs = dir_url[i];
                else Dirs = Dirs + "/" + dir_url[i];
            }
            Dirs = Dirs + "/" + MyReq;
            if (StartsWithSlash) Dirs = "/" + Dirs;
            if (EndsWithSlash) Dirs = Dirs + "/";
            // 8 - And readd the query string if neccessary
            if (url_qur.Length > 1) Dirs = Dirs + "?" + url_qur[1];
            // 9 - And replace all SENSEPOSTPLACEHOLDERS with ?
            Dirs = Dirs.Replace("SENSEPOSTPLACEHOLDER", "?");
            // 10 - And a slash at start / end if there was one originally...
            // And now we have our dummy request...

            //indicate requests that *might* be FP's
            string[] checkfordoubles = new string[] { "?", "%20", "." };
            string[] checkforsingles = new string[] {};
            WeirdDummy = false;

            //check for multiple occurances of string
            for (int k = 0; k < checkfordoubles.Length; k++)
            {
                if (originalReq.IndexOf(checkfordoubles[k]) > -1)   //string is in request
                {
                    if (originalReq.LastIndexOf(checkfordoubles[k]) != originalReq.IndexOf(checkfordoubles[k])) //there is another string in the request
                    {
                        WeirdDummy = true;
                        break;
                    }
                }
            }

            //check for a single accurance of string
            for (int k = 0; k < checkforsingles.Length; k++)
            {
                if (originalReq.IndexOf(checkforsingles[k]) > -1)
                {
                    WeirdDummy = true;
                    break;
                }
            }

            Regex RgxUrl = new Regex("(([a-zA-Z][0-9a-zA-Z+\\-\\.]*:)?/{0,2}[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?(#[0-9a-zA-Z;/?:@&=+$\\.\\-_!~*'()%]+)?");
            if(!RgxUrl.IsMatch(txtNiktoTarget.Text + Dirs))
            {
                WeirdDummy = true;
            }

            return Dirs;
        }

        private bool IsNumeric(String niktotrig)
        {
            niktotrig = niktotrig.ToLower();
            int i = 0;
            bool returner = true;
            for (i = 0; i < niktotrig.Length; i++)
            {
                String tmp = niktotrig.Substring(i, 1);
                if ((tmp != "0") &&
                    (tmp != "1") &&
                    (tmp != "2") &&
                    (tmp != "3") &&
                    (tmp != "4") &&
                    (tmp != "5") &&
                    (tmp != "6") &&
                    (tmp != "7") &&
                    (tmp != "8") &&
                    (tmp != "9"))
                    returner = false;
            }
            return returner;
        }

        private String GetNiktoRequest(niktoRequests request, bool real)
        {
            String method = request.method;
            String returner = "";
            if (real)
            {
                if (chkProxyPresent.Checked) returner = method + " http://" + txtNiktoTarget.Text + request.request + " HTTP/1.0\r\n";
                else returner = method + " " + request.request + " HTTP/1.0\r\n";
            }
            else
            {
                if (chkProxyPresent.Checked) returner = method + " http://" + txtNiktoTarget.Text + request.sensepostreq + " HTTP/1.0\r\n";
                else returner = method + " " + request.sensepostreq + " HTTP/1.0\r\n";
            }
            returner = returner + "\r\n\r\n";
            return returner;
        }

        private String GetNiktoFake(niktoRequests request)
        {
            String whatdoisend = GetNiktoRequest(request, false);
            String whatdoiget = "";
            String AddyItem = "";
            String PortItem = "";
            if (chkProxyPresent.Checked)
            {
                string[] proxyItems = new string[2];
                proxyItems = txtProxySettings.Text.Split(':');
                AddyItem = proxyItems[0];
                PortItem = proxyItems[1];
            }
            else
            {
                AddyItem = txtNiktoTarget.Text;
                PortItem = txtNiktoPort.Text;
            }
            if (chkuseSSLWikto.Checked)
            {
                whatdoiget = sendraw(AddyItem, PortItem, whatdoisend, 1024, (int)updownTimeOutTCP.Value, true);
            }
            else
            {
                whatdoiget = sendraw(AddyItem, PortItem, whatdoisend, 1024, (int)updownTimeOutTCP.Value);
            }
            return whatdoiget;
        }

        private String GetNiktoReal(niktoRequests request)
        {
            String whatdoisend = GetNiktoRequest(request, true);
            String whatdoiget = "";
            String AddyItem = "";
            String PortItem = "";
            if (chkProxyPresent.Checked)
            {
                string[] proxyItems = new string[2];
                proxyItems = txtProxySettings.Text.Split(':');
                AddyItem = proxyItems[0];
                PortItem = proxyItems[1];
            }
            else
            {
                AddyItem = txtNiktoTarget.Text;
                PortItem = txtNiktoPort.Text;
            }
            if (chkuseSSLWikto.Checked)
            {
                whatdoiget = sendraw(AddyItem, PortItem, whatdoisend, 1024, (int)updownTimeOutTCP.Value, true);
            }
            else
            {
                whatdoiget = sendraw(AddyItem, PortItem, whatdoisend, 1024, (int)updownTimeOutTCP.Value);
            }
            return whatdoiget;
        }

        private Double GetNiktoBlobDiff(String FakeResp, String RealResp)
        {
            if ((FakeResp.Length == 0) || (RealResp.Length == 0))
                return 100;
            FakeResp = FakeResp.Replace('\r', ' ');
            String HtmFake = StripNiktoHeader(FakeResp.Split('\n'));
            RealResp = RealResp.Replace('\r', ' ');
            String HtmReal = StripNiktoHeader(RealResp.Split('\n'));
            int wordamount = 0;
            int wordmatch = 0;
            //float Returner = 0.0;
            String[] WordFake = HtmFake.Split(' ');
            foreach (String tmp in WordFake) if (tmp.Length > 0) wordamount++;
            String[] WordReal = HtmReal.Split(' ');
            foreach (String tmp in WordReal) if (tmp.Length > 0) wordamount++;
            foreach (String tmp_fake in WordFake)
            {
                foreach (String tmp_real in WordReal)
                {
                    if ((tmp_fake == tmp_real) && (tmp_fake.Length > 0) && (tmp_real.Length > 0))
                    {
                        wordmatch++;
                        break;
                    }
                }
            }
            Double returner =((2 * System.Convert.ToDouble(wordmatch)) / System.Convert.ToDouble(wordamount));
            return returner;
        }

        private String StripNiktoHeader(String[] response_array)
        {
            String Returner = "";
            bool HeaderDone = false;
            bool AddMe = true;
            foreach (String tmp in response_array)
            {
                AddMe = true;
                if (tmp.Length < 3)
                {
                    HeaderDone = true;
                    AddMe = false;
                }
                if (HeaderDone && AddMe)
                    Returner += tmp;
            }
            return Returner;
        }

        private void LongTaskNikto()
        {
            int a = 0;
            //double result;
            this.Invoke(this.dlgControlDisable, new Object[] { this.btn_WiktoStop, true });
            this.Invoke(this.dlgControlProgMax, new Object[] { this.prgNik, numberofNiktorequests });
            this.Invoke(this.dlgControlProgVal, new Object[] { this.prgNik, 0 });
            this.Invoke(this.dlgControlTextSet, new Object[] { this.lblNiktoAI, "" });
            niktoResultCounter = 0;
            if (numberofNiktorequests == 0)
            {
                MessageBox.Show("You havent loaded the Nikto database", "Nikto Database Not Loaded");
            }
            else
            {
                for (a = 0; a < numberofNiktorequests && stopnikto == false; a++)
                {
                    if (stopnikto == true)
                    {
                        break;
                    }

                    while (pauseWikto)
                    {
                        System.Threading.Thread.Sleep(1000);
                        if (stopnikto)
                        {
                            pauseWikto = false;
                            break;
                        }
                    }

                    this.Invoke(this.dlgControlListView, new Object[] { this.lvw_NiktoDb, a });
                    this.Invoke(this.dlgControlViewSel, new Object[] { this.lvw_NiktoDb, a });
                    this.Invoke(this.dlgControlProgVal, new Object[] { this.prgNiktoWork, 0 });

                    // We have two options here...
                    // Either we're looking for a string trigger,
                    // or we're looking for a status code trigger.
                    // For status code triggers, we want to get a dummy object + the real thing
                    // And then do a BLOB diff between them
                    // If its a numeric code, its going to be three digits long and ONLY contain numbers...

                    //status code trigger
                    if ((niktoRequest[a].trigger.Length == 3) && IsNumeric(niktoRequest[a].trigger))
                    {
                        // This is a CGI request - we go through all CGI directories and test the Nikto DB...
                        if ((niktoRequest[a].request.IndexOf("@CGIDIRS") > -1) || (niktoRequest[a].request.IndexOf("@ADMIN") > -1))
                        {
                            string[] CGIdirs = this.Invoke(this.dlgControlListArr, new Object[] { this.lst_NiktoCGI }).ToString().Split('\n');
                            foreach (string CGIdir in CGIdirs)
                            {
                                if (stopnikto)
                                { 
                                    break;
                                }

                                while (pauseWikto)
                                {
                                    System.Threading.Thread.Sleep(1000);
                                    if (stopnikto)
                                    {
                                        pauseWikto = false;
                                        break;
                                    }
                                }

                                string nCGIdir = CGIdir.Replace("\r", "");
                                if (nCGIdir.Length > 1)
                                {
                                    niktoRequests newNiktoRequest;
                                    newNiktoRequest = niktoRequest[a];
                                    newNiktoRequest.request = newNiktoRequest.request.Replace("@CGIDIRS", nCGIdir + "/");
                                    newNiktoRequest.request = newNiktoRequest.request.Replace("@ADMIN", nCGIdir + "/");
                                    if ((newNiktoRequest.sensepostreq == "") || ((!newNiktoRequest.sensepostreq.Contains("@CGIDIRS") && (!newNiktoRequest.sensepostreq.Contains("@ADMIN")))))
                                        newNiktoRequest.sensepostreq = GetDummyReq(newNiktoRequest.request);
                                    else
                                    {
                                        newNiktoRequest.sensepostreq = newNiktoRequest.sensepostreq.Replace("@CGIDIRS", nCGIdir + "/");
                                        newNiktoRequest.sensepostreq = newNiktoRequest.sensepostreq.Replace("@ADMIN", nCGIdir + "/");
                                    }
                                    // We have our /CGIDIR/request and /CGIDIR/dummyrequest.  We test them now...
                                    // If its optimised, we check if there is a BLOB result in the db...
                                    String RealResp = "";
                                    String FakeResp = "";

                                    if (chkOptimizedNikto.Checked)
                                    {
                                        if (NiktoOptimised.ContainsKey(newNiktoRequest.sensepostreq))
                                            FakeResp = NiktoOptimised[newNiktoRequest.sensepostreq].ToString();
                                        else
                                        {
                                            FakeResp = GetNiktoFake(newNiktoRequest);
                                            NiktoOptimised.Add(newNiktoRequest.sensepostreq, FakeResp);
                                        }
                                    }
                                    else
                                    {
                                        FakeResp = GetNiktoFake(newNiktoRequest);
                                    }
                                    RealResp = GetNiktoReal(newNiktoRequest);
                                    Double nikto_score = GetNiktoBlobDiff(FakeResp, RealResp);
                                    nikto_result[niktoResultCounter].rawrequest = newNiktoRequest.request;
                                    nikto_result[niktoResultCounter].theNiktoRequest = niktoRequest[a];
                                    nikto_result[niktoResultCounter].rawresult = RealResp;
                                    nikto_result[niktoResultCounter].fuzzValue = nikto_score;
                                    nikto_result[niktoResultCounter].theoriginalrequest = niktoRequest[a].request;
                                    if (nikto_score < Convert.ToDouble(NUPDOWNfuzz.Value) && nikto_score >= 0.0000000000)
                                    {
                                        ListViewItem lvi = new ListViewItem(" ");

                                        if (WeirdDummy)
                                        {
                                            lvi.ImageIndex = 3;
                                            WeirdDummy = false;
                                        }
                                        else
                                        {
                                            lvi.ImageIndex = 4;
                                        }

                                        lvi.SubItems.Add(nikto_score.ToString());
                                        lvi.SubItems.Add(newNiktoRequest.trigger);
                                        lvi.SubItems.Add(newNiktoRequest.request);

                                        //this.Invoke(this.dlgControlViewAdd, new Object[] { this.lvw_NiktoResults, new ListViewItem(NiktoItemToAdd) });
                                        this.Invoke(this.dlgControlViewAdd, new Object[] { this.lvw_NiktoResults, lvi });
                                    }
                                    niktoResultCounter++;
                                }
                            }
                        }
                        else
                        {
                            // This is a single request - we test it once...
                            niktoRequests newNiktoRequest;
                            newNiktoRequest = niktoRequest[a];
                            if (newNiktoRequest.sensepostreq == "")
                                newNiktoRequest.sensepostreq = GetDummyReq(newNiktoRequest.request);
                            String RealResp = "";
                            String FakeResp = "";
                            if (chkOptimizedNikto.Checked)
                            {
                                if (NiktoOptimised.ContainsKey(newNiktoRequest.sensepostreq))
                                    FakeResp = NiktoOptimised[newNiktoRequest.sensepostreq].ToString();
                                else
                                {
                                    FakeResp = GetNiktoFake(newNiktoRequest);
                                    NiktoOptimised.Add(newNiktoRequest.sensepostreq, FakeResp);
                                }
                            }
                            else
                            {
                                FakeResp = GetNiktoFake(newNiktoRequest);
                            }
                            RealResp = GetNiktoReal(newNiktoRequest);
                            Double nikto_score = GetNiktoBlobDiff(FakeResp, RealResp);
                            nikto_result[niktoResultCounter].rawrequest = newNiktoRequest.request;
                            nikto_result[niktoResultCounter].theNiktoRequest = niktoRequest[a];
                            nikto_result[niktoResultCounter].rawresult = RealResp;
                            nikto_result[niktoResultCounter].fuzzValue = nikto_score;
                            nikto_result[niktoResultCounter].theoriginalrequest = niktoRequest[a].request;
                            if (nikto_score < Convert.ToDouble(NUPDOWNfuzz.Value) && nikto_score >= 0.0000000000)
                            {
                                //status code, but body differs from fake request
                                ListViewItem lvi = new ListViewItem(" ");

                                if (WeirdDummy)
                                {
                                    lvi.ImageIndex = 3;
                                    WeirdDummy = false;
                                }
                                else
                                {
                                    lvi.ImageIndex = 4;
                                }

                                lvi.SubItems.Add(nikto_score.ToString());
                                lvi.SubItems.Add(newNiktoRequest.trigger);
                                lvi.SubItems.Add(newNiktoRequest.request);

                                //this.Invoke(this.dlgControlViewAdd, new Object[] { this.lvw_NiktoResults, new ListViewItem(NiktoItemToAdd) });
                                this.Invoke(this.dlgControlViewAdd, new Object[] { this.lvw_NiktoResults, lvi });
                            }
                            niktoResultCounter++;
                        }

                    }
                    else
                    {
                        // We're talking about a match trigger - we'll get the request
                        if ((niktoRequest[a].request.IndexOf("@CGIDIRS") > -1) || (niktoRequest[a].request.IndexOf("@ADMIN") > -1))
                        {
                            string[] CGIdirs = this.Invoke(this.dlgControlListArr, new Object[] { this.lst_NiktoCGI }).ToString().Split('\n');
                            foreach (string CGIdir in CGIdirs)
                            {
                                if (stopnikto == true)
                                {
                                    break;
                                }

                                while (pauseWikto)
                                {
                                    System.Threading.Thread.Sleep(1000);
                                    if (stopnikto)
                                    {
                                        pauseWikto = false;
                                        break;
                                    }
                                }

                                string nCGIdir = CGIdir.Replace("\r", "");
                                if (nCGIdir.Length > 1)
                                {
                                    niktoRequests newNiktoRequest;
                                    newNiktoRequest = niktoRequest[a];
                                    newNiktoRequest.request = newNiktoRequest.request.Replace("@CGIDIRS", nCGIdir + "/");
                                    newNiktoRequest.request = newNiktoRequest.request.Replace("@ADMIN", nCGIdir + "/");
                                    String RealResp = "";
                                    RealResp = GetNiktoReal(newNiktoRequest);
                                    Double nikto_score = 100;
                                    if (RealResp.IndexOf(newNiktoRequest.trigger) > -1) nikto_score = 0.01;
                                    nikto_result[niktoResultCounter].rawrequest = newNiktoRequest.request;
                                    nikto_result[niktoResultCounter].theNiktoRequest = niktoRequest[a];
                                    nikto_result[niktoResultCounter].rawresult = RealResp;
                                    nikto_result[niktoResultCounter].fuzzValue = nikto_score;
                                    nikto_result[niktoResultCounter].theoriginalrequest = niktoRequest[a].request;
                                    if (nikto_score < Convert.ToDouble(NUPDOWNfuzz.Value) && nikto_score >= 0.0000000000)
                                    {
                                        //String[] NiktoItemToAdd = new String[3];
                                        //NiktoItemToAdd[0] = nikto_score.ToString();
                                        //NiktoItemToAdd[1] = newNiktoRequest.trigger;
                                        //NiktoItemToAdd[2] = newNiktoRequest.request;

                                        ListViewItem lvi = new ListViewItem(" ");
                                        lvi.ImageIndex = 5;

                                        lvi.SubItems.Add(nikto_score.ToString());
                                        lvi.SubItems.Add(newNiktoRequest.trigger);
                                        lvi.SubItems.Add(newNiktoRequest.request);

                                        //this.Invoke(this.dlgControlViewAdd, new Object[] { this.lvw_NiktoResults, new ListViewItem(NiktoItemToAdd) });
                                        this.Invoke(this.dlgControlViewAdd, new Object[] { this.lvw_NiktoResults, lvi });
                                    }
                                    niktoResultCounter++;
                                }
                            }
                        }
                        else
                        {
                            // This is a single request - we test it once...
                            niktoRequests newNiktoRequest;
                            newNiktoRequest = niktoRequest[a];
                            String RealResp = "";
                            RealResp = GetNiktoReal(newNiktoRequest);
                            Double nikto_score = 100;
                            if (RealResp.IndexOf(newNiktoRequest.trigger) > -1) nikto_score = 0.01;
                            nikto_result[niktoResultCounter].rawrequest = newNiktoRequest.request;
                            nikto_result[niktoResultCounter].theNiktoRequest = niktoRequest[a];
                            nikto_result[niktoResultCounter].rawresult = RealResp;
                            nikto_result[niktoResultCounter].fuzzValue = nikto_score;
                            nikto_result[niktoResultCounter].theoriginalrequest = niktoRequest[a].request;
                            if (nikto_score < Convert.ToDouble(NUPDOWNfuzz.Value) && nikto_score >= 0.0000000000)
                            {
                                //String[] NiktoItemToAdd = new String[3];
                                //NiktoItemToAdd[0] = nikto_score.ToString();
                                //NiktoItemToAdd[1] = newNiktoRequest.trigger;
                                //NiktoItemToAdd[2] = newNiktoRequest.request;

                                //text trigger
                                ListViewItem lvi = new ListViewItem(" ");
                                lvi.ImageIndex = 5;

                                lvi.SubItems.Add(nikto_score.ToString());
                                lvi.SubItems.Add(newNiktoRequest.trigger);
                                lvi.SubItems.Add(newNiktoRequest.request);

                                //this.Invoke(this.dlgControlViewAdd, new Object[] { this.lvw_NiktoResults, new ListViewItem(NiktoItemToAdd) });
                                this.Invoke(this.dlgControlViewAdd, new Object[] { this.lvw_NiktoResults, lvi });
                            }
                            niktoResultCounter++;
                        }
                    }
                    this.Invoke(this.dlgControlProgVal, new Object[] { this.prgNiktoWork, 10 });
                    this.Invoke(this.dlgControlProgInc, new Object[] { this.prgNik, prgNik.Maximum / numberofNiktorequests });
                }
            }
            this.Invoke(this.dlgControlDisable, new object[] { btnPauseWikto, false });
        }

        private void btnNiktoLoad_Click_1(object sender, System.EventArgs e)
        {

            string filename = "";
            //lets ask him if he want the default file
            DialogResult yesno = MessageBox.Show("Do you want to load your default database file?", "Choose Nikto Database", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (yesno == DialogResult.Yes)
            {
                //check if its exists
                if (File.Exists(txtDBlocationNikto.Text) == true)
                {
                    filename = txtDBlocationNikto.Text;
                }
                else
                {
                    yesno = MessageBox.Show("The default Nikto Database does not exists.\n\nDo you want to download it?", "Download Nikto Database", System.Windows.Forms.MessageBoxButtons.YesNo);
                    if (yesno == DialogResult.Yes)
                    {
                        btnDBUpdateNikto_Click(null, null);
                    }
                    else { return; }

                    fdlLoadNiktoDB.InitialDirectory = txtDBlocationNikto.Text;
                    DialogResult didheopen = fdlLoadNiktoDB.ShowDialog();
                    if (didheopen != DialogResult.OK)
                    {
                        return;
                    }

                    if (fdlLoadNiktoDB.FileName.Length > 0)
                    {
                        filename = fdlLoadNiktoDB.FileName;
                    }
                }
            }
            else
            {
                fdlLoadNiktoDB.InitialDirectory = txtDBlocationNikto.Text;
                DialogResult didheopen = fdlLoadNiktoDB.ShowDialog();
                if (didheopen != DialogResult.OK)
                {
                    return;
                }

                if (fdlLoadNiktoDB.FileName.Length > 0)
                {
                    filename = fdlLoadNiktoDB.FileName;
                }
            }

            try
            {
                StreamReader fileRead = new StreamReader(filename);
                string oreadline = "";
                string readline = "";
                string[] splititems = new string[5];
                int number = 0;
                lvw_NiktoDb.Items.Clear();
                while ((oreadline = fileRead.ReadLine()) != null)
                {
                    readline = oreadline.Replace("\",\"", "^");
                    // we are not interested in the XSS stuff..
                    try
                    {
                        if ((readline.IndexOf('#') == -1) &&
                            (readline.IndexOf("<script>alert") == -1) &&
                            (readline.IndexOf("javascript:alert") == -1) &&
                            readline.Length > 2)
                        {

                            splititems = readline.Split('^');


                            niktoRequest[number].type = splititems[0].Replace("\"", "");
                            niktoRequest[number].request = splititems[1].Replace("\"", ""); ;
                            niktoRequest[number].trigger = splititems[2].Replace("\"", ""); ;
                            niktoRequest[number].method = splititems[3].Replace("\"", ""); ;
                            niktoRequest[number].description = splititems[4].Replace("\"", "");
                            if (splititems.Length > 5)
                                niktoRequest[number].sensepostreq = splititems[5].Replace("\"", "");
                            else
                                niktoRequest[number].sensepostreq = "";

                            // Check for a match on the request
                            Regex r = new Regex(@"JUNK\((?<junk_length>\d+)\)");
                            Match m = r.Match(niktoRequest[number].request);
                            if (m.Success)
                            {
                                // Match found - replace it with junk
                                string length = m.Result("${junk_length}");
                                niktoRequest[number].request.Replace("/abcd/", "/xxx/");
                                string junkstring = niktoRequest[number].request.Replace("JUNK(" + length.ToString() + ")",
                                    RandomStringGenerator(Int32.Parse(length)));
                                niktoRequest[number].request = junkstring;
                            }

                            string niktoItemToAdd = niktoRequest[number].trigger + "\t" + niktoRequest[number].request;
                            String[] MyTest = new String[2];
                            MyTest[0] = niktoRequest[number].trigger;
                            MyTest[1] = niktoRequest[number].request;
                            ListViewItem lvi = new ListViewItem(MyTest);
                            lvw_NiktoDb.Items.Add(lvi);
                            number++;
                        }
                    }
                    catch { }
                }
                numberofNiktorequests = number;
                fileRead.Close();
                MessageBox.Show("Loaded " + numberofNiktorequests.ToString() + " tests from Nikto scan database", "Info");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problem reading file\n" + ex.Message, "Error reading file");
            }

        }

        private void populateNiktoDesc(object sender, System.EventArgs e)
        {
            int a = 0;
            String[] NiktoKey = new String[2];
            foreach (ListViewItem lvi in lvw_NiktoDb.SelectedItems)
            {
                a = lvi.Index;
                break;
            }
            NiktoKey[0] = lvw_NiktoDb.Items[a].SubItems[0].Text;
            NiktoKey[1] = lvw_NiktoDb.Items[a].SubItems[1].Text;
            //string niktoKey=lstboxNikto.SelectedItem.ToString();
            //string[] niktoItems = new string[2];
            //niktoItems = niktoKey.Split('\t');

            for (a = 0; a < numberofNiktorequests; a++)
            {
                if ((niktoRequest[a].request.CompareTo(NiktoKey[1]) == 0) &&
                    (niktoRequest[a].trigger.CompareTo(NiktoKey[0]) == 0))
                {
                    lvw_NiktoDesc.Items.Clear();
                    String[] myitems = new String[2];
                    myitems[0] = "Description: ";
                    myitems[1] = niktoRequest[a].description;
                    lvw_NiktoDesc.Items.Add(new ListViewItem(myitems));
                    myitems[0] = "Request: ";
                    myitems[1] = niktoRequest[a].request;
                    lvw_NiktoDesc.Items.Add(new ListViewItem(myitems));
                    myitems[0] = "Trigger: ";
                    myitems[1] = niktoRequest[a].trigger;
                    lvw_NiktoDesc.Items.Add(new ListViewItem(myitems));
                    myitems[0] = "Method: ";
                    myitems[1] = niktoRequest[a].method;
                    lvw_NiktoDesc.Items.Add(new ListViewItem(myitems));
                }
            }
        }

        private void populateNiktoDescvuln(object sender, System.EventArgs e)
        {
            int a = 0;
            foreach (ListViewItem lvi in lvw_NiktoResults.SelectedItems)
            {
                a = lvi.Index;
                break;
            }
            string[] niktoItems = new string[3];
            niktoItems[0] = lvw_NiktoResults.Items[a].SubItems[1].Text;
            niktoItems[1] = lvw_NiktoResults.Items[a].SubItems[2].Text;
            niktoItems[2] = lvw_NiktoResults.Items[a].SubItems[3].Text;

            //find the corresponding result..
            for (int b = 0; b < niktoResultCounter; b++)
            {
                if ((nikto_result[b].rawrequest.CompareTo(niktoItems[2]) == 0) &&
                    (nikto_result[b].theNiktoRequest.trigger.CompareTo(niktoItems[1]) == 0))
                {
                    txtNiktoReq.Text = nikto_result[b].rawrequest;
                    txtNiktoRes.Text = nikto_result[b].rawresult;
                    lvw_NiktoDesc.Items.Clear();
                    String[] myitems = new String[2];
                    myitems[0] = "Description: ";
                    myitems[1] = nikto_result[b].theNiktoRequest.description;
                    lvw_NiktoDesc.Items.Add(new ListViewItem(myitems));
                    myitems[0] = "Request: ";
                    myitems[1] = nikto_result[b].theNiktoRequest.request;
                    lvw_NiktoDesc.Items.Add(new ListViewItem(myitems));
                    myitems[0] = "Trigger: ";
                    myitems[1] = nikto_result[b].theNiktoRequest.trigger;
                    lvw_NiktoDesc.Items.Add(new ListViewItem(myitems));
                    myitems[0] = "Method: ";
                    myitems[1] = nikto_result[b].theNiktoRequest.method;
                    lvw_NiktoDesc.Items.Add(new ListViewItem(myitems));
                    if ((nikto_result[b].theNiktoRequest.trigger.Length != 3) && !IsNumeric(nikto_result[b].theNiktoRequest.trigger))
                        if (txtNiktoRes.Text.IndexOf(nikto_result[b].theNiktoRequest.trigger) > -1)
                        {
                            txtNiktoRes.SelectionStart = txtNiktoRes.Text.IndexOf(nikto_result[b].theNiktoRequest.trigger);
                            txtNiktoRes.SelectionLength = nikto_result[b].theNiktoRequest.trigger.Length;
                        }
                    break;
                }

            }
        }

        public double testNiktoRequest(string ipRaw, string portRaw, string requestRaw, niktoRequests niktoset, int TimeOut, string SensePostReq)
        {

            if (chkProxyPresent.Checked)
            {
                string[] proxyItems = new string[2];
                proxyItems = txtProxySettings.Text.Split(':');
                ipRaw = proxyItems[0];
                portRaw = proxyItems[1];
            }

            nikto_result[niktoResultCounter].rawrequest = requestRaw;
            nikto_result[niktoResultCounter].theNiktoRequest = niktoset;

            if (stopscroll == false)
            {
                this.Invoke(this.dlgControlTextSet, new Object[] { this.txtNiktoReq, nikto_result[niktoResultCounter].rawrequest });
            }

            this.Invoke(this.dlgControlProgVal, new Object[] { this.prgNiktoWork, 0 });
            string response = "";
            if (chkuseSSLWikto.Checked)
            {
                response = sendraw(ipRaw, portRaw, requestRaw, 1024, TimeOut, true);
            }
            else
            {
                response = sendraw(ipRaw, portRaw, requestRaw, 1024, TimeOut);
            }
            this.Invoke(this.dlgControlProgVal, new Object[] { this.prgNiktoWork, 10 });

            nikto_result[niktoResultCounter].rawresult = response;


            if (stopscroll == false)
            {
                this.Invoke(this.dlgControlTextSet, new Object[] { this.txtNiktoRes, nikto_result[niktoResultCounter].rawresult });
            }

            string[] responseline = new string[5];
            responseline = response.Split('\n');
            try
            {
                if (Convert.ToInt16(niktoset.trigger) < 1000)
                {
                    //normal checking
                    //fuzzing checking
                    double testResults = testniktoFP(txtNiktoTarget.Text, txtNiktoPort.Text, niktoset, niktoset.request, response);
                    nikto_result[niktoResultCounter].fuzzValue = testResults;
                    niktoResultCounter++;
                    return testResults;
                }
            }
            catch
            {
                //they have a string we need to look for...
                if (response.IndexOf(niktoset.trigger) > 0)
                {
                    nikto_result[niktoResultCounter].fuzzValue = 0.001;
                    niktoResultCounter++;
                    return 0.01;
                }
                else
                {
                    //fuzzing checking
                    double testResults = testniktoFP(txtNiktoTarget.Text, txtNiktoPort.Text, niktoset, niktoset.request, response);
                    nikto_result[niktoResultCounter].fuzzValue = testResults;
                    niktoResultCounter++;
                    return testResults;
                }
            }
            //...but it didnt match
            nikto_result[niktoResultCounter].fuzzValue = 1.00;
            niktoResultCounter++;
            return 1.00;
        }

        public string stestNiktoRequest(string ipRaw, string portRaw, string requestRaw, niktoRequests niktoset, int TimeOut)
        {

            if (chkProxyPresent.Checked)
            {
                string[] proxyItems = new string[2];
                proxyItems = txtProxySettings.Text.Split(':');
                ipRaw = proxyItems[0];
                portRaw = proxyItems[1];
            }
            string response = "";
            //this need fixing!!!
            if (chkuseSSLWikto.Checked || chkBackEnduseSSLport.Checked)
            {
                response = sendraw(ipRaw, portRaw, requestRaw, 1024, TimeOut, true);
            }
            else
            {
                response = sendraw(ipRaw, portRaw, requestRaw, 1024, TimeOut);
            }

            return response;
        }

        public string buildNiktoRequest(niktoRequests niktoset)
        {

            string methodGETHEAD = niktoset.method;
            string actualrequest = "";
            if (chkProxyPresent.Checked)
            {
                actualrequest = methodGETHEAD + " http://" + txtNiktoTarget.Text + niktoset.request + " HTTP/1.0\r\n";
            }
            else actualrequest = methodGETHEAD + " " + niktoset.request + " HTTP/1.0\r\n";

            actualrequest += txtHeader.Text + "\r\n\r\n";
            return actualrequest;

        }

        private double testniktoFP(string ipRaw, string portRaw, niktoRequests niktoset, string request, string reply)
        {
            string location = extractLocation(request);
            string filetype = extractFileType(request);

            string blobFromDB = getBlob(ipRaw, portRaw, niktoset, filetype, location);
            if (blobFromDB.Length > 0)
            {
                double result = compareBlobs(blobFromDB, reply);
                if (niktoset.type.CompareTo("FPtestdir") == 0 || niktoset.type.CompareTo("FPtestfile") == 0)
                {
                    if (result >= maxBackEndAI)
                    {
                        maxBackEndAI = result;
                    }
                    if (result <= minBackEndAI)
                    {
                        minBackEndAI = result;
                    }

                }
                else
                {
                }

                return result;
            }
            else return -1.0;
        }

        private void btnClearNiktoAI_Click(object sender, System.EventArgs e)
        {
            NiktoOptimised = new Hashtable();
            lblNiktoAI.Text = "Fingerpint DB cleared";
            globalFP = 0;
        }

        private void btnNiktoFuzzUpdate_Click(object sender, System.EventArgs e)
        {
            lvw_NiktoResults.Items.Clear();
            //lstBoxNiktoResults.Items.Clear();
            for (int a = 0; a <= niktoResultCounter; a++)
            {
                if (nikto_result[a].fuzzValue <= Convert.ToDouble(NUPDOWNfuzz.Value) &&
                    nikto_result[a].fuzzValue > 0)
                {
                    //String[] NiktoItemToAdd = new String[3];
                    //NiktoItemToAdd[0] = nikto_result[a].fuzzValue;
                    //NiktoItemToAdd[1] = newNiktoRequest.trigger;
                    //NiktoItemToAdd[2] = newNiktoRequest.request;

                    ListViewItem lvi = new ListViewItem(" ");
                    lvi.ImageIndex = 0;

                    lvi.SubItems.Add(nikto_result[a].fuzzValue.ToString());
                    lvi.SubItems.Add(nikto_result[a].theNiktoRequest.trigger.ToString());
                    lvi.SubItems.Add(nikto_result[a].theNiktoRequest.request.ToString());

                    //this.Invoke(this.dlgControlViewAdd, new Object[] { this.lvw_NiktoResults, new ListViewItem(NiktoItemToAdd) });
                    this.Invoke(this.dlgControlViewAdd, new Object[] { this.lvw_NiktoResults, lvi });
                }
            }

        }

        private void btnNiktoShowAll_Click(object sender, System.EventArgs e)
        {
            NiktoFuzzNValueNow = NUPDOWNfuzz.Value;
            NUPDOWNfuzz.Value = 1000;
            btnNiktoFuzzUpdate_Click(sender, e);
        }

        private void btnNiktoRestFuzz_Click(object sender, System.EventArgs e)
        {
            NUPDOWNfuzz.Value = Convert.ToDecimal(NiktoFuzzNValueNow);
            btnNiktoFuzzUpdate_Click(sender, e);
        }

        private void btnNiktoExport_Click(object sender, System.EventArgs e)
        {
            //fdlExportWikto.ShowDialog();
            DialogResult res = fdlExportWikto.ShowDialog();
            if (res == DialogResult.OK)
            {
                if (fdlExportWikto.FileName.Length > 0)
                {
                    try
                    {
                        if (niktoResultCounter < 1)
                        {
                            MessageBox.Show("No results yet - nothing to export!", "Error");
                        }
                        else
                        {
                            StreamWriter fileWrite = new StreamWriter(fdlExportWikto.FileName);
                            for (int a = 0; a < niktoResultCounter; a++)
                            {
                                if (nikto_result[a].fuzzValue > 0 && nikto_result[a].fuzzValue < Convert.ToDouble(NUPDOWNfuzz.Value))
                                {
                                    fileWrite.WriteLine("{0},{1},{2},{3},{4}", txtNiktoTarget.Text,
                                        nikto_result[a].theNiktoRequest.request,
                                        nikto_result[a].theNiktoRequest.trigger,
                                        nikto_result[a].theNiktoRequest.type,
                                        nikto_result[a].theNiktoRequest.description);
                                }
                            }
                            fileWrite.Close();
                            MessageBox.Show("Results successfully exported to " + fdlExportWikto.FileName, "Info");
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Problem writing file\n" + ex.Message, "Error reading file");
                    }
                }
            }
        }

        private void ToggleNiktoAI(object sender, System.EventArgs e)
        {
            btnClearNiktoAI.Enabled = true;
            btnNiktoFuzzUpdate.Enabled = true;
            btnNiktoRestFuzz.Enabled = true;
            btnNiktoShowAll.Enabled = true;
            NUPDOWNfuzz.Enabled = true;
            lblNiktoAI.Enabled = true;
        }

        private void chkuseSSLWikto_CheckedChanged(object sender, EventArgs e)
        {
            if (chkuseSSLWikto.Checked)
            {
                txtNiktoPort.Text = "443";
            }
            else
            {
                txtNiktoPort.Text = "80";
            }
        }
        #endregion

        #region Google Hack Routines
        private void btnStartGoogleHack_Click(object sender, System.EventArgs e)
        {
            btn_GHStart.Enabled = false;
            lstGoogleHackResults.Items.Clear();
            stopGH = false;
            BeginGoogleHackTaskThread();
        }

        public void BeginGoogleHackTaskThread()
        {
            //DelegateToGoogleHackTask delGoogleHackTaskDelegate = new DelegateToGoogleHackTask(LongTaskGoogleHack);
            DelegateToGoogleHackTask delGoogleHackTaskDelegate = new DelegateToGoogleHackTask(LongTaskSpudHack);
            AsyncCallback callGoogleHackWhenDone = new AsyncCallback(this.EndGoogleHackTaskThread);
            delGoogleHackTaskDelegate.BeginInvoke(callGoogleHackWhenDone, null);
        }

        public void EndGoogleHackTaskThread(IAsyncResult arResult)
        {
            this.Invoke(this.dlgControlerSetReadonly, new Object[] { this.txtGoogleHackTarget, false });
            MethodInvoker miGoogleHack = new MethodInvoker(this.UpdateGoogleHackUI);
            this.BeginInvoke(miGoogleHack);
        }

        private void UpdateGoogleHackUI()
        {
            btn_GHStart.Enabled = true;
            btn_GHStop.Enabled = false;
        }

        void LongTaskSpudHack()
        {
            //this is actually just sad to see code duplicated..

            if (this.TheGoogleHackDatabase.Count == 0)
            {
                MessageBox.Show("Please first load the GHDB - you have zero entries", "Error");
                return;
            }
            stopGH = false;
            //om.google.api.GoogleSearchService myGoogleservice = new com.google.api.GoogleSearchService();
            //com.google.api.GoogleSearchResult myGoogleresult = new com.google.api.GoogleSearchResult();
            //com.google.api.ResultElement myElement = new com.google.api.ResultElement();

            //string googleKey = txtGoogleKey.Text;
            this.Invoke(this.dlgControlerSetReadonly, new Object[] { this.txtGoogleHackTarget, true });
            this.Invoke(this.dlgControlDisable, new Object[] { this.btn_GHStart, false });
            this.Invoke(this.dlgControlDisable, new Object[] { this.btn_GHStop, true });
            this.Invoke(this.dlgControlProgMax, new Object[] { this.prgsGoogleHackAll, this.TheGoogleHackDatabase.Count });
            this.Invoke(this.dlgControlProgVal, new Object[] { this.prgsGoogleHackAll, 0 });

            for (int l = 0; l < this.TheGoogleHackDatabase.Count && (stopGH == false); l++)
            {

                GoogleHackDB_type GoogleHack = (GoogleHackDB_type)TheGoogleHackDatabase[l];

                this.Invoke(this.dlgControlProgInc, new Object[] { this.prgsGoogleHackAll, 1 });
                //prgsGoogleHackAll.Increment(1);
                if (GHstopscroll == false)
                {
                    this.Invoke(this.dlgControlListSel, new Object[] { this.lstGoogleHack, 1 });
                    //lstGoogleHack.SelectedIndex=l;
                }
                String the_target = this.Invoke(this.dlgControlTextGet, new Object[] { txtGoogleHackTarget }).ToString();
                string query = "site:" + the_target + " " + GoogleHack.querystring;
                this.Invoke(this.dlgControlTextSet, new Object[] { this.lblGoogleHackStatus, query });
                //lblGoogleHackStatus.Text=query;
                int j;
                int the_depth = System.Convert.ToInt32(this.Invoke(this.dlgControllNupGet, new Object[] { this.updownGoogleDepth }));
                ArrayList collectedURLS = new ArrayList();
                for (j = 0; j <= the_depth && (stopGH == false); j += 10)
                {
                    bool DoIGo = true;
                    com.sensepost.spud.obj.Service the_srv = new SensePost.Wikto.com.sensepost.spud.obj.Service();
                    com.sensepost.spud.obj.StructuredResult the_res = new SensePost.Wikto.com.sensepost.spud.obj.StructuredResult();

                    this.Invoke(this.dlgControlProgVal, new Object[] { this.prgGHQuick, 10 });

                    this.Invoke(this.dlgControlTextSet, new Object[] { this.lblGoogleHackPage, j.ToString() });
                    //lblGoogleHackPage.Text=j.ToString();
                    try
                    {
                        the_res = the_srv.GetStructResult(query, j, 10, true);
                        //myGoogleresult = myGoogleservice.doGoogleSearch
                        //    (googleKey, query, j, 10, false, "", false, "", "latin1", "latin1");
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(this.dlgControlListAdd, new Object[] { this.lstGoogleHackResults, ex.Message.ToString() });
                        //lstGoogleHackResults.Items.Add(ex.Message);
                        //if (ex.Message.IndexOf("Invalid authorization key:") > 0)
                        //{
                        //    MessageBox.Show("Your Google API key appear to be invalid.\nPlease enter a valid key in System Config section.");
                        //    stopGH = true;
                        //}
                        DoIGo = false;
                    }
                    this.Invoke(this.dlgControlProgVal, new Object[] { this.prgGHQuick, 0 });

                    this.Invoke(this.dlgControlTextSet, new Object[] { this.lblGoogleHackEst, the_res.ResultTotal.ToString() });
                    //lblGoogleHackEst.Text=myGoogleresult.estimatedTotalResultsCount.ToString();

                    if (DoIGo)
                    {
                        foreach (com.sensepost.spud.obj.StructuredResultElement the_elm in the_res.ResultItems)
                        {
                            collectedURLS.Add(the_elm.ResultUrl.ToString());
                        }
                    }
                    //for (i = 0; i < 10 && (stopGH == false); i++)
                    //{
                    //    try
                    //    {
                    //        myElement = myGoogleresult.resultElements[i];
                    //        collectedURLS.Add(myElement.URL.ToString());
                    //    }
                    //    catch
                    //    {
                    //        this.Invoke(this.dlgControlTextSet, new Object[] { this.lblGoogleHackStatus, "Done" });
                    //        //lblGoogleHackStatus.Text ="Done";
                    //        j = 99999999;
                    //        break;
                    //    }
                    // }
                }
                if (collectedURLS.Count > 0)
                {
                    this.Invoke(this.dlgControlListAdd, new Object[] { lstGoogleHackResults, "" });
                    this.Invoke(this.dlgControlListAdd, new Object[] { lstGoogleHackResults, GoogleHack.signatureReferenceNumber + "\t" + GoogleHack.querystring });
                    this.Invoke(this.dlgControlListAdd, new Object[] { lstGoogleHackResults, "---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------" });
                    //lstGoogleHackResults.BackColor=System.Drawing.Color.White;
                    foreach (string Url in collectedURLS)
                    {
                        this.Invoke(this.dlgControlListAdd, new Object[] { lstGoogleHackResults, Url });
                        //lstGoogleHackResults.Items.Add(Url);
                    }
                }
            }
        }

        private void btnLoadGoogleHack_Click(object sender, System.EventArgs e)
        {
            string filename = "";
            //lets ask him if he want the default file
            DialogResult yesno = MessageBox.Show("Do you want to load your default database file?", "Choose Google Hack Database", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (yesno == DialogResult.Yes)
            {
                //check if its exists
                if (File.Exists(txtDBLocationGH.Text) == true)
                {
                    filename = txtDBLocationGH.Text;
                }
                else
                {
                    yesno = MessageBox.Show("The default Google Hack Database does not exists.\n\nDo you want to download it?", "Download Google Hack Database", System.Windows.Forms.MessageBoxButtons.YesNo);
                    if (yesno == DialogResult.Yes)
                    {
                        btnDBUpdateGH_Click(null, null);
                    }
                    else { return; }

                    fdlGoogleHacksOpenDB.InitialDirectory = txtDBLocationGH.Text;
                    DialogResult didheopen = fdlGoogleHacksOpenDB.ShowDialog();
                    if (didheopen != DialogResult.OK)
                    {
                        return;
                    }

                    if (fdlGoogleHacksOpenDB.FileName.Length > 0)
                    {
                        filename = fdlGoogleHacksOpenDB.FileName;
                    }
                }
            }
            else
            {
                fdlGoogleHacksOpenDB.InitialDirectory = txtDBLocationGH.Text;
                DialogResult didheopen = fdlGoogleHacksOpenDB.ShowDialog();
                if (didheopen != DialogResult.OK)
                {
                    return;
                }

                if (fdlGoogleHacksOpenDB.FileName.Length > 0)
                {
                    filename = fdlGoogleHacksOpenDB.FileName;
                }
            }


            if (filename.Length > 0)
            {

                lstGoogleHack.Items.Clear();
                this.TheGoogleHackDatabase = new ArrayList();

                // this really sucks..but seems the only way we can get the XML in
                // let's hope johnny does not change the DB anytime soon
                XmlReader reader = new XmlTextReader(filename);

                reader.Read();
                reader.ReadStartElement("searchEngineSignature");
                try
                {
                    while (1 == 1)
                    {
                        GoogleHackDB_type GoogleHack = new GoogleHackDB_type();
                        reader.ReadStartElement("signature");
                        reader.ReadStartElement("ghdb_id");
                        GoogleHack.signatureReferenceNumber = reader.ReadString();
                        reader.ReadEndElement();
                        reader.ReadStartElement("category");
                        GoogleHack.category = reader.ReadString();
                        reader.ReadEndElement();
                        reader.ReadStartElement("querystring");
                        GoogleHack.querystring = reader.ReadString();
                        reader.ReadEndElement();
                        reader.ReadStartElement("shortDescription");
                        GoogleHack.shortDescription = reader.ReadString();
                        reader.ReadEndElement();
                        reader.ReadStartElement("textualDescription");
                        GoogleHack.textualDescription = reader.ReadString();
                        reader.ReadEndElement();
                        reader.ReadEndElement();
                        this.TheGoogleHackDatabase.Add(GoogleHack);
                    }
                }
                catch
                {
                    reader.Close();
                    MessageBox.Show("Loaded " + this.TheGoogleHackDatabase.Count + " entries", "Info");
                }
                for (int i = 0; i < this.TheGoogleHackDatabase.Count; i++)
                {
                    GoogleHackDB_type GoogleHack = (GoogleHackDB_type)this.TheGoogleHackDatabase[i];
                    lstGoogleHack.Items.Add(GoogleHack.signatureReferenceNumber + "\t" + GoogleHack.querystring);
                }
            }
        }

        private void cntxtGoogleHacks_Popup(object sender, System.EventArgs e)
        {
            MessageBox.Show("Context coming soon! :)");
        }

        private void populateGoogleHackDesc(object sender, System.EventArgs e)
        {
            string GHKey = lstGoogleHack.SelectedItem.ToString();
            string[] GHItems = new string[2];
            GHItems = GHKey.Split('\t');

            for (int a = 0; a < numberofGoogleHacks; a++)
            {
                if (GoogleHack[a].signatureReferenceNumber.CompareTo(GHItems[0]) == 0)
                {
                    txtGoogleHackDesc.Text = "Google string:\r\n" + GoogleHack[a].querystring + "\r\n\r\nRef ID: " + GoogleHack[a].signatureReferenceNumber + "\r\n\r\nDescription:\r\n" + GoogleHack[a].textualDescription + "\r\n\r\nCategory:\r\n" + GoogleHack[a].category;
                    txtGoogleHackOnceOff.Text = GoogleHack[a].querystring;
                }
            }
        }

        private void populateGoogleHackDescFromResults(object sender, System.EventArgs e)
        {
            string GHKey = lstGoogleHackResults.SelectedItem.ToString();
            string[] GHItems = new string[2];
            GHItems = GHKey.Split('\t');
            int flag = 0;
            //check if its a signature
            for (int a = 0; a < numberofGoogleHacks; a++)
            {
                if (GoogleHack[a].signatureReferenceNumber.CompareTo(GHItems[0]) == 0)
                {
                    txtGoogleHackDesc.Text = "Google string:\r\n" + GoogleHack[a].querystring + "\r\n\r\nRef ID: " + GoogleHack[a].signatureReferenceNumber + "\r\n\r\nDescription:\r\n" + GoogleHack[a].textualDescription + "\r\n\r\nCategory:\r\n" + GoogleHack[a].category;
                    txtGoogleHackOnceOff.Text = GoogleHack[a].querystring;
                    flag = 1;
                    break;
                }
            }
            if (flag == 0)
            {
                //then its a URL
                if (GHKey.Length < 270 && GHKey.Length > 5)
                {
                    txtGoogleHackDesc.Text = "URL:\r\n" + GHKey;
                    txtGoogleHackOnceOff.Text = GHKey;
                }
            }
        }

        private void btnGoogleHackOnceOff_Click(object sender, System.EventArgs e)
        {

            //ermm..
            stopGH = false;
            btn_GHStop.Enabled = true;
            btn_GHStart.Enabled = false;
            //string googleKey = txtGoogleKey.Text;
            ArrayList collectedURLS = new ArrayList();

            string query = txtGoogleHackOnceOff.Text;
            lblGoogleHackStatus.Text = query;
            int j;

            for (j = 0; j <= (int)updownGoogleDepth.Value && (stopGH == false); j += 10)
            {
                com.sensepost.spud.obj.Service the_srv = new SensePost.Wikto.com.sensepost.spud.obj.Service();
                com.sensepost.spud.obj.StructuredResult the_res = new SensePost.Wikto.com.sensepost.spud.obj.StructuredResult();

                prgGHQuick.Value = 10;
                lblGoogleHackPage.Text = j.ToString();

                try
                {
                    the_res = the_srv.GetStructResult(query, j, 10, false);
                    //myGoogleresult = myGoogleservice.doGoogleSearch(googleKey, query, j, 10, false, "", false, "", "latin1", "latin1");
                }
                catch (Exception ex)
                {
                    lstGoogleHackResults.Items.Add(ex.Message);
                }
                prgGHQuick.Value = 0;
                //lblGoogleHackEst.Text = myGoogleresult.estimatedTotalResultsCount.ToString();

                lstGoogleHackResults.Items.Add(" ");
                lstGoogleHackResults.Items.Add("Once off:\t" + query);
                lstGoogleHackResults.Items.Add("---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                lstGoogleHackResults.BackColor = System.Drawing.Color.White;
                foreach (com.sensepost.spud.obj.StructuredResultElement the_elm in the_res.ResultItems)
                {
                    lstGoogleHackResults.Items.Add(the_elm.ResultUrl.ToString());
                }
                btn_GHStop.Enabled = false;
                btn_GHStart.Enabled = true;
            }
        }

        #endregion

        #endregion

        #region Update Procs
        private void btnBackEndUpdateDirs_Click(object sender, System.EventArgs e)
        {

            try
            {
                WebClient myWebClient;
                string download = "";

                if (cmbBackEndUpdate.Items.Count < 2)
                {
                    try
                    {
                        myWebClient = new WebClient();
                        lblStatus.Text = "Getting update list from SensePost...";
                        try
                        {
                            byte[] myDataBuffer = myWebClient.DownloadData(txtURLUpdate.Text + "/back-end-categories.txt");
                            download = Encoding.ASCII.GetString(myDataBuffer);
                        }
                        catch
                        {
                            MessageBox.Show("Could not get update categories", "Error Getting Update Categories");
                            return;
                        }
                        cmbBackEndUpdate.Items.Clear();
                        cmbBackEndUpdate.Text = "Choose one";
                        lblStatus.Text = "Please choose a category from the combo box and click on Update";

                        string[] newcats = download.Split('\n');
                        cmbBackEndUpdate.Items.Clear();
                        foreach (string cat in newcats)
                        {
                            cmbBackEndUpdate.Items.Add(cat);
                        }
                    }
                    catch { }
                }
                else
                {
                    getandupdate(txtInDirs, txtURLUpdate.Text + cmbBackEndUpdate.SelectedItem.ToString() + "/backend-dirs.txt");
                    getandupdate(txtInFiles, txtURLUpdate.Text + cmbBackEndUpdate.SelectedItem.ToString() + "/backend-filenames.txt");
                    getandupdate(txtInFileTypes, txtURLUpdate.Text + cmbBackEndUpdate.SelectedItem.ToString() + "/backend-extensions.txt");
                    lblStatus.Text = "Wikto Backend DB updated..";
                }
            }
            catch { }

        }

        private void getandupdate(System.Windows.Forms.RichTextBox box, string URL)
        {
            string download = "";
            WebClient myWebClient = new WebClient();
            lblStatus.Text = "Updating the Wikto Backend DB...";
            try
            {
                byte[] myDataBuffer = myWebClient.DownloadData(URL);
                download = Encoding.ASCII.GetString(myDataBuffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot update:\n\n" + ex.ToString());
                return;
            }
            box.Clear();
            string[] newdirs = download.Replace("\r\n", "\n").Split('\n');
            foreach (string dir in newdirs)
            {
                if (dir.Length > 1)
                {
                    box.AppendText(dir + "\r\n");
                }
            }
        }

        private void btnDBUpdateGH_Click(object sender, System.EventArgs e)
        {

            WebClient myWebClient = new WebClient();
            //myWebClient.

            if (File.Exists(txtDBLocationGH.Text) == true)
            {
                string DBdate = File.GetLastWriteTime(txtDBLocationGH.Text).ToString();
                DialogResult yesno = MessageBox.Show("DB exists. Creation date is:\n" + DBdate + "\n Do you want to override the DB with a fresh copy?", "Warning", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                if (yesno == DialogResult.Cancel) { return; }
                if (yesno == DialogResult.No)
                {

                    fdlLoadBackEndDirs.Title = "Choose location to save DB";
                    fdlLoadBackEndDirs.CheckFileExists = false;

                    fdlLoadBackEndDirs.Filter = "XML|*.xml|All|*.*";
                    DialogResult didheopen = fdlLoadBackEndDirs.ShowDialog();
                    if (didheopen != DialogResult.OK)
                    {
                        return;
                    }

                    if (fdlLoadBackEndDirs.FileName.Length > 0)
                    {
                        txtDBLocationGH.Text = fdlLoadBackEndDirs.FileName;
                    }
                }
            }
            else
            {
                fdlLoadBackEndDirs.Filter = "XML|*.xml|All|*.*";
                DialogResult didheopen = fdlLoadBackEndDirs.ShowDialog();
                if (didheopen != DialogResult.OK)
                {
                    return;
                }

                if (fdlLoadBackEndDirs.FileName.Length > 0)
                {
                    txtDBLocationGH.Text = fdlLoadBackEndDirs.FileName;
                }
            }

            //ok lets attempt to get the file
            try
            {
                File.Delete(txtDBLocationGH.Text);
                MessageBox.Show("About to download the Google Hack DB\nThis might take a while...\nNow click on OK and wait for the next pop-up..\n");
                WebResponse response = null;
                Stream stream = null;
                StreamReader reader = null;
                try
                {
                    Uri m_uri = new Uri(txtURLUpdateGHDB.Text);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(m_uri);
                    request.Timeout = 10000;
                    request.AllowAutoRedirect = true;
                    response = request.GetResponse();
                    stream = response.GetResponseStream();
                    string buffer = "", line;
                    reader = new StreamReader(stream);
                    while ((line = reader.ReadLine()) != null)
                    {
                        buffer += line;
                    }
                    if (buffer.Contains("<meta HTTP-EQUIV="))
                    {
                        String[] tmpur1 = buffer.Split('<');
                        String theurl = "";
                        foreach (String tmpitm1 in tmpur1)
                        {
                            if (tmpitm1.StartsWith("meta HTTP-EQUIV=\"REFRESH\" CONTENT=\"0; "))
                            {
                                String[] nxtur1 = tmpitm1.Split('0');
                                String[] tmpur2 = nxtur1[1].Split('"');
                                theurl = tmpur2[0].Replace("; URL=", "");
                                break;
                            }
                        }
                        if (theurl.Length == 0)
                        {
                            MessageBox.Show("Invalid META response received.");
                            return;
                        }
                        m_uri = new Uri(theurl);
                        request = (HttpWebRequest)WebRequest.Create(m_uri);
                        request.Timeout = 10000;
                        request.AllowAutoRedirect = true;
                        response = request.GetResponse();
                        stream = response.GetResponseStream();
                        buffer = "";
                        line = "";
                        reader = new StreamReader(stream);
                        while ((line = reader.ReadLine()) != null)
                        {
                            buffer += line;
                        }
                    }
                    StreamWriter sw = new StreamWriter(txtDBLocationGH.Text + "NEW");
                    sw.Write(buffer);
                    sw.Flush();
                    sw.Close();
                    File.Delete(txtDBLocationGH.Text);
                    File.Copy(txtDBLocationGH.Text + "NEW", txtDBLocationGH.Text);
                }
                catch
                {
                    MessageBox.Show("Cannot get GHDB file from " + txtDBLocationGH.Text);
                    return;
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                    if (stream != null)
                        stream.Close();
                    if (response != null)
                        response.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot get GHDB file from " + txtDBLocationGH.Text + ":\n\n" + ex.ToString());
                return;
            }
            MessageBox.Show("Download complete!\nRemember to save your config if this is the first time you have downloaded the DB", "Info");

        }

        private void btnDBUpdateNikto_Click(object sender, System.EventArgs e)
        {

            WebClient myWebClient = new WebClient();

            if (File.Exists(txtDBlocationNikto.Text) == true)
            {
                string DBdate = File.GetLastWriteTime(txtDBlocationNikto.Text).ToString();
                DialogResult yesno = MessageBox.Show("DB exists. Creation date is:\n" + DBdate + "\n Do you want to override the DB with a fresh copy?", "Warning", System.Windows.Forms.MessageBoxButtons.YesNoCancel);
                if (yesno == DialogResult.Cancel) { return; }
                if (yesno == DialogResult.No)
                {

                    fdlLoadBackEndDirs.Title = "Choose location to save DB";
                    fdlLoadBackEndDirs.CheckFileExists = false;

                    fdlLoadBackEndDirs.Filter = "DB|*.db|All|*.*";
                    DialogResult didheopen = fdlLoadBackEndDirs.ShowDialog();
                    if (didheopen != DialogResult.OK)
                    {
                        return;
                    }

                    if (fdlLoadBackEndDirs.FileName.Length > 0)
                    {
                        txtDBlocationNikto.Text = fdlLoadBackEndDirs.FileName;
                    }
                }
            }
            else
            {
                fdlLoadBackEndDirs.Filter = "DB|*.db|All|*.*";
                DialogResult didheopen = fdlLoadBackEndDirs.ShowDialog();
                if (didheopen != DialogResult.OK)
                {
                    return;
                }

                if (fdlLoadBackEndDirs.FileName.Length > 0)
                {
                    txtDBlocationNikto.Text = fdlLoadBackEndDirs.FileName;
                }
            }

            //ok lets attempt to get the file
            try
            {
                File.Delete(txtDBlocationNikto.Text);
                MessageBox.Show("About to download the Nikto DB\nThis might take a while...\nNow click on OK and wait for the next pop-up..\n");
                myWebClient.DownloadFile(txtURLUpdateNiktoDB.Text, txtDBlocationNikto.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot get GHDB file from " + txtDBlocationNikto.Text + ":\n\n" + ex.ToString());
                return;
            }
            MessageBox.Show("Download complete\nRemember to save your config if this is the first time you have downloaded the DB", "Info");
        }

        #endregion

        #region Configuration Persistence
        private void btnConfigSave_Click(object sender, System.EventArgs e)
        {
            string filename = "";

            DialogResult didheopen = openFileDialog1.ShowDialog();
            if (didheopen != DialogResult.OK)
            {
                return;
            }
            if (openFileDialog1.FileName.Length > 0)
            {
                filename = openFileDialog1.FileName;
            }
            isFirstTimeRun = false;

            StreamWriter writefile = new StreamWriter(filename);
            ArrayList config = new ArrayList();
            //now.....we have to add all them shit - this is a misssssion

            //From the config screen
            config.Add("Config-ProxyDetails!" + txtProxySettings.Text);
            config.Add("Config-ProxyEnable!" + chkProxyPresent.Checked.ToString());
            config.Add("Config-GoogleDepth!" + updownGoogleDepth.Value.ToString());
            config.Add("Config-RetryTCP!" + updownRetryTCP.Value.ToString());
            config.Add("Config-TimeoutTCP!" + updownTimeOutTCP.Value.ToString());
            config.Add("Config-DB-NiktoLocation!" + txtDBlocationNikto.Text);
            config.Add("Config-DB-GHLocation!" + txtDBLocationGH.Text);
            config.Add("Config-SPUDDirectory!" + txtSpudDirectory.Text);
            config.Add("Config-UpdateSite!" + txtURLUpdate.Text);
            config.Add("Config-ConfigFile!" + filename);

            //from Googler
            config.Add("Googler-Filetypes!" + txtWords.Text);

            //from Backend......
            config.Add("BackEnd-GET!" + radioGET.Checked.ToString());
            config.Add("BackEnd-HEAD!" + radioHEAD.Checked.ToString());
            config.Add("BackEnd-TriggerDir!" + txtErrorCodeDir.Text);
            config.Add("BackEnd-TriggerFiles!" + txtErrorCodeFile.Text);
            config.Add("BackEnd-UseAI!" + chkBackEndAI.Checked.ToString());
            config.Add("BackEnd-AITriggerLevel!" + NUPDOWNBackEnd.Value.ToString());
            config.Add("BackEnd-Preserve!" + chkPreserve.Checked.ToString());

            //from Spider...
            config.Add("Spider-Exclude!" + txt_ConfigSpiderExclude.Text.ToString());
            config.Add("Spider-Extensions!" + txt_ConfigSpiderExtension.Text.ToString());
            config.Add("Spider-Threads!" + NUPDOWNspider.Value.ToString());

            //From Wikto
            config.Add("Wikto-Optimized!" + chkOptimizedNikto.Checked.ToString());
            config.Add("Wikto-AITriggerLevel!" + NUPDOWNfuzz.Value.ToString());

            //write the backend files - dirs
            string[] backenddirs = txtInDirs.Text.Replace("\r\n", "\n").Split('\n');
            foreach (string item in backenddirs)
            {
                config.Add("BackEnd-Directories!" + item);
            }

            //write the backend files - files
            string[] backendfiles = txtInFiles.Text.Replace("\r\n", "\n").Split('\n');
            foreach (string item in backendfiles)
            {
                config.Add("BackEnd-FileNames!" + item);
            }

            //write the backend files
            string[] backendext = txtInFileTypes.Text.Replace("\r\n", "\n").Split('\n');
            foreach (string item in backendext)
            {
                config.Add("BackEnd-Extensions!" + item);
            }


            foreach (string item in config)
            {
                writefile.WriteLine(item);
            }
            writefile.Close();
            setregistry(filename);


            lblConfigFileLocation.Text = filename;
        }

        private void setregistry(string filename)
        {
            //now we have to write the filename location to the registry
            RegistryKey OurKey = Registry.LocalMachine;
            OurKey = OurKey.OpenSubKey("SOFTWARE", true);
            OurKey.CreateSubKey("SensePost");
            OurKey.CreateSubKey(@"SensePost\Wikto");

            RegistryKey NewKey = Registry.LocalMachine;
            NewKey = NewKey.OpenSubKey(@"SOFTWARE\SensePost\Wikto", true);
            try
            {
                NewKey.SetValue("ConfigFile", filename);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problems writing to registry:\n\n" + ex.ToString());
            }
            NewKey.Close();
        }

        private void setregistrystart(bool ShouldIStart)
        {
            //now we have to write the filename location to the registry
            RegistryKey OurKey = Registry.LocalMachine;
            OurKey = OurKey.OpenSubKey("SOFTWARE", true);
            OurKey.CreateSubKey("SensePost");
            OurKey.CreateSubKey(@"SensePost\Wikto");

            RegistryKey NewKey = Registry.LocalMachine;
            NewKey = NewKey.OpenSubKey(@"SOFTWARE\SensePost\Wikto", true);
            try
            {
                if (ShouldIStart) NewKey.SetValue("ShowStart", "1");
                else NewKey.SetValue("ShowStart", "0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problems writing to registry:\n\n" + ex.ToString());
            }
            NewKey.Close();
        }

        private void setregistrywizard(bool ShouldIStart)
        {
            //now we have to write the filename location to the registry
            RegistryKey OurKey = Registry.LocalMachine;
            OurKey = OurKey.OpenSubKey("SOFTWARE", true);
            OurKey.CreateSubKey("SensePost");
            OurKey.CreateSubKey(@"SensePost\Wikto");

            RegistryKey NewKey = Registry.LocalMachine;
            NewKey = NewKey.OpenSubKey(@"SOFTWARE\SensePost\Wikto", true);
            try
            {
                if (ShouldIStart) NewKey.SetValue("ShowWizard", "1");
                else NewKey.SetValue("ShowWizard", "0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Problems writing to registry:\n\n" + ex.ToString());
            }
            NewKey.Close();
        }

        private void button2_Click_2(object sender, System.EventArgs e)
        {
            DialogResult yesno = MessageBox.Show("This will terminate Wikto.\nOn restart Wikto will start with factory defaults.\n\n Are you sure this is what you want?", "Warning", System.Windows.Forms.MessageBoxButtons.YesNo);
            if (yesno == DialogResult.Yes)
            {
                resetregistry();
                Application.Exit();
            }
        }

        private void resetregistry()
        {
            //clear the whole thing...
            try
            {
                RegistryKey OurKey = Registry.LocalMachine;
                OurKey.DeleteSubKeyTree(@"SOFTWARE\SensePost");
                OurKey.Close();
            }
            catch { }
        }

        private void LoadActualConfig(string filename)
        {

            StreamReader freader = null;
            try
            {
                freader = new StreamReader(filename);
            }
            catch
            {
                isFirstTimeRun = true;
            }

            if (isFirstTimeRun == true)
            {
                return;
            }
            txtInDirs.Clear();
            txtInFiles.Clear();
            txtInFileTypes.Clear();

            try
            {
                string readline = "";
                while ((readline = freader.ReadLine()) != null)
                {
                    string[] namevalue = new string[2];
                    namevalue = readline.Split('!');
                    switch (namevalue[0])
                    {

                        case "Config-ProxyDetails":
                            txtProxySettings.Text = namevalue[1];
                            break;
                        case "Config-ProxyEnable":
                            chkProxyPresent.Checked = Convert.ToBoolean(namevalue[1]);
                            break;
                        case "Config-GoogleDepth":
                            updownGoogleDepth.Value = Convert.ToInt16(namevalue[1]);
                            break;
                        case "Config-RetryTCP":
                            updownRetryTCP.Value = Convert.ToInt16(namevalue[1]);
                            break;
                        case "Config-TimeoutTCP":
                            updownTimeOutTCP.Value = Convert.ToInt16(namevalue[1]);
                            break;
                        case "Config-UpdateSite":
                            txtURLUpdate.Text = namevalue[1];
                            break;
                        case "Config-DB-NiktoLocation":
                            txtDBlocationNikto.Text = namevalue[1];
                            break;
                        case "Config-DB-GHLocation":
                            txtDBLocationGH.Text = namevalue[1];
                            break;
                        case "Googler-Filetypes":
                            txtWords.Text = namevalue[1];
                            break;
                        case "BackEnd-GET":
                            radioGET.Checked = Convert.ToBoolean(namevalue[1]);
                            break;
                        case "BackEnd-HEAD":
                            radioHEAD.Checked = Convert.ToBoolean(namevalue[1]);
                            break;
                        case "BackEnd-TriggerDir":
                            txtErrorCodeDir.Text = namevalue[1];
                            break;
                        case "BackEnd-TriggerFiles":
                            txtErrorCodeFile.Text = namevalue[1];
                            break;
                        case "BackEnd-UseAI":
                            chkBackEndAI.Checked = Convert.ToBoolean(namevalue[1]);
                            break;
                        case "BackEnd-AITriggerLevel":
                            NUPDOWNBackEnd.Value = Convert.ToDecimal(namevalue[1]);
                            break;
                        case "BackEnd-Preserve":
                            chkPreserve.Checked = Convert.ToBoolean(namevalue[1]);
                            break;
                        case "Wikto-Optimized":
                            chkOptimizedNikto.Checked = Convert.ToBoolean(namevalue[1]);
                            break;
                        case "Wikto-AITriggerLevel":
                            NUPDOWNfuzz.Value = Convert.ToDecimal(namevalue[1]);
                            break;
                        case "Config-SPUDDirectory":
                            txtSpudDirectory.Text = namevalue[1];
                            break;
                        case "BackEnd-Directories":
                            txtInDirs.AppendText(namevalue[1] + "\r\n");
                            break;
                        case "BackEnd-FileNames":
                            txtInFiles.AppendText(namevalue[1] + "\r\n");
                            break;
                        case "BackEnd-Extensions":
                            txtInFileTypes.AppendText(namevalue[1] + "\r\n");
                            break;
                        case "Spider-Exclude":
                            txt_ConfigSpiderExclude.Text = namevalue[1];
                            break;
                        case "Spider-Extensions":
                            txt_ConfigSpiderExtension.Text = namevalue[1];
                            break;
                        case "Spider-Threads":
                            try
                            {
                                NUPDOWNspider.Value = Convert.ToInt16(namevalue[1]);
                            }
                            catch
                            {
                                NUPDOWNspider.Value = 20;
                            }
                            break;
                        default:
                            break;
                    }
                }
                setregistry(filename);
                freader.Close();
                lblConfigFileLocation.Text = filename;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Reading config file:\n\n" + ex.ToString());
            }
        }

        private void LoadConfigInitial()
        {

            string filename = "";
            string ShowStart = "";
            string ShowWiz = "";
            RegistryKey NewKey = Registry.LocalMachine;
            NewKey = NewKey.OpenSubKey(@"SOFTWARE\SensePost\Wikto", true);
            try
            {
                filename = (string)NewKey.GetValue("ConfigFile");
            }
            catch
            {
                return;
            }
            try
            {
                ShowStart = (string)NewKey.GetValue("ShowStart");
                if (ShowStart == "1")
                    bl_ShowStart = true;
                else
                    bl_ShowStart = false;
            }
            catch { bl_ShowStart = true; }
            try
            {
                ShowWiz = (string)NewKey.GetValue("ShowWizard");
                if (ShowWiz == "1")
                    bl_ShowStartWiz = true;
                else
                    bl_ShowStartWiz = false;
            }
            catch { bl_ShowStartWiz = true; }

            if (filename == null)
            {
                return;
            }

            isFirstTimeRun = false;
            LoadActualConfig(filename);

        }

        private void btnLoadConfig_Click(object sender, System.EventArgs e)
        {

            string filename = "";
            DialogResult didheopen = openFileDialog1.ShowDialog();
            if (didheopen != DialogResult.OK)
            {
                return;
            }
            if (openFileDialog1.FileName.Length > 0)
            {
                filename = openFileDialog1.FileName;
            }
            if (filename.Length > 0)
            {
                lblConfigFileLocation.Text = filename;
                isFirstTimeRun = false;
                LoadActualConfig(filename);
            }
        }
        #endregion

        #region Thread Delegate Target Voids
        private String DelgateDeclarationTextGet(System.Windows.Forms.Control myctl)
        {
            return (myctl.Text);
        }

        private void DelgateDeclarationTextSet(System.Windows.Forms.Control myctl, String s)
        {
            myctl.Text = s;
        }

        private void DelgateDeclarationTextApp(System.Windows.Forms.TextBox myctl, String s)
        {
            myctl.AppendText(s);
        }

        private void DelgateDeclarationDisable(System.Windows.Forms.Control myctl, bool b)
        {
            myctl.Enabled = b;
        }

        private void DelgateDeclarationListCls(System.Windows.Forms.ListBox myctl)
        {
            myctl.Items.Clear();
        }

        private void DelgateDeclarationListAdd(System.Windows.Forms.ListBox myctl, String s)
        {
            myctl.Items.Add(s);
        }

        private void DelgateDeclarationListUnq(System.Windows.Forms.ListBox myctl, String s)
        {
            int i;
            bool found = false;
            for (i = 0; i < myctl.Items.Count; i++)
            {
                if (myctl.Items[i].ToString() == s)
                {
                    found = true;
                    i = myctl.Items.Count + 1;
                }
            }
            if (!found)
                myctl.Items.Add(s);
        }

        private String DelgateDeclarationListArr(System.Windows.Forms.ListBox myctl)
        {
            String Returner = "";
            int i;
            for (i = 0; i < myctl.Items.Count; i++)
            {
                Returner = Returner + myctl.Items[i].ToString() + "\n";
            }
            return (Returner);
        }

        private void DelgateDeclarationListSel(System.Windows.Forms.ListBox myctl, int i)
        {
            myctl.SelectedIndex = i;
        }

        private void DelgateDeclarationViewSel(System.Windows.Forms.ListView myctl, int i)
        {
            myctl.Items[i].Selected = true;
        }

        private void DelgateDeclarationViewAdd(System.Windows.Forms.ListView myctl, System.Windows.Forms.ListViewItem s)
        {
            myctl.Items.Add(s);
        }

        private void DelgateDeclarationViewCls(System.Windows.Forms.ListView myctl)
        {
            myctl.Items.Clear();
        }

        private void DelgateDeclarationViewUnq(System.Windows.Forms.ListView myctl, ListViewItem s)
        {
            bool found = false;
            foreach (ListViewItem lvi in myctl.Items)
            {
                if (lvi.Equals(s))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                myctl.Items.Add(s);
            }
        }

        private String DelgateDeclarationViewArr(System.Windows.Forms.ListView myctl, int ImageIndex)
        {
            String Returner = "";
            foreach (ListViewItem lvi in myctl.Items)
            {
                if (ImageIndex == -1)
                {
                    Returner += lvi.SubItems[1].Text + "\n";
                }
                else
                {
                    if (lvi.ImageIndex == ImageIndex)
                    {
                        Returner += lvi.SubItems[1].Text + "\n";
                    }
                }
            }
            return (Returner);
        }

        private void DelgateDeclarationProgMax(System.Windows.Forms.ProgressBar myctl, int i)
        {
            myctl.Maximum = i;
        }

        private void DelgateDeclarationProgVal(System.Windows.Forms.ProgressBar myctl, int i)
        {
            myctl.Value = i;
        }

        private void DelgateDeclarationProgInc(System.Windows.Forms.ProgressBar myctl, int i)
        {
            myctl.Increment(i);
        }

        private bool DelgateDeclarationChckGet(System.Windows.Forms.CheckBox myctl)
        {
            return myctl.Checked;
        }

        private void DelgateDeclarationLviewSet(System.Windows.Forms.ListView myctl, int i)
        {
            myctl.Items[i].Selected = true;
            myctl.Items[i].EnsureVisible();
        }

        private void DelgateDeclarationReadOnlySet(System.Windows.Forms.TextBox myctl, bool b)
        {
            myctl.ReadOnly = b;
        }

        private String DelgateDeclarationNupGet(System.Windows.Forms.NumericUpDown myctl)
        {
            return (myctl.Value.ToString());
        }

        private int DelgateDeclarationProgMaxGet(System.Windows.Forms.ProgressBar myctl)
        {
            return (myctl.Maximum);
        }

        #endregion

        #region Miscellaneous HTTP Routines
        private void populateGETHEAD(object sender, System.EventArgs e) {
			if (chkProxyPresent.Checked==true){
				radioGET.Checked=true;
				radioHEAD.Checked=false;
			}
        }

        public string sendraw(string ipRaw, string portRaw, string payloadRaw, int size, int TimeOut)
        {
            int retry = (int)updownRetryTCP.Value;

            while (retry > 0)
            {
                try
                {

                    TcpClient tcpclnt = new TcpClient();
                    tcpclnt.ReceiveTimeout = TimeOut;
                    tcpclnt.SendTimeout = TimeOut;
                    tcpclnt.LingerState.LingerTime = 1;
                    LingerOption lingerOption = new LingerOption(false, 1);
                    tcpclnt.LingerState = lingerOption;
                    tcpclnt.NoDelay = true;

                    tcpclnt.Connect(ipRaw, Int32.Parse(portRaw));

                    Stream stm = tcpclnt.GetStream();
                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(payloadRaw);
                    stm.Write(ba, 0, ba.Length);

                    byte[] bb = new byte[size];

                    int k = 1;
                    string response = "";
                    while (k > 0)
                    {
                        k = stm.Read(bb, 0, size);
                        for (int i = 0; i < k; i++)
                            response += Convert.ToChar(bb[i]);
                    }

                    tcpclnt.Close();
                    //this is need else we get CLOSE_WAITS - not nice..but works well
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    return response;
                }

                catch
                {

                    retry--;
                    this.Invoke(this.dlgControlTextSet, new Object[] { this.lblStatus, "Network problem - retry:" + Convert.ToString(retry) });
                    this.Invoke(this.dlgControlTextSet, new Object[] { this.lblNiktoAI, "Network problem - retry:" + Convert.ToString(retry) });
                    Thread.Sleep(1000);
                }
            }

            return "Retry count (" + updownRetryTCP.Value.ToString() + ") exceeded";
        }

        public string sendraw(string ipRaw, string portRaw, string payloadRaw, int size, int TimeOut, bool useSSL)
        {
            int retry = (int)updownRetryTCP.Value;
            IPHostEntry IPHost = Dns.GetHostEntry(ipRaw);
            //IPHostEntry IPHost = Dns.Resolve(ipRaw); 
            string[] aliases = IPHost.Aliases;
            IPAddress[] addr = IPHost.AddressList;
            IPEndPoint iep;
            SecureProtocol sp;
            sp = SecureProtocol.Ssl3;
            SecureSocket s = null;
            SecurityOptions options = new SecurityOptions(sp);
            options.Certificate = null;
            options.Protocol = SecureProtocol.Ssl3;
            options.Entity = ConnectionEnd.Client;
            options.CommonName = ipRaw;
            options.VerificationType = CredentialVerification.None;
            options.Flags = SecurityFlags.IgnoreMaxProtocol;
            options.AllowedAlgorithms = SslAlgorithms.ALL;

            while (retry > 0)
            {
                try
                {

                    s = new SecureSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp, options);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 4000);
                    s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 4000);

                    iep = new IPEndPoint(addr[0], Convert.ToInt32(portRaw));


                    s.Connect(iep);


                    ASCIIEncoding asen = new ASCIIEncoding();
                    byte[] ba = asen.GetBytes(payloadRaw);
                    s.Send(ba, ba.Length, System.Net.Sockets.SocketFlags.None);

                    byte[] bb = new byte[size];


                    string response = "";
                    int k = 1;
                    while (k > 0)
                    {
                        k = s.Receive(bb, size, System.Net.Sockets.SocketFlags.None);
                        for (int i = 0; i < k; i++)
                        {
                            response += Convert.ToChar(bb[i]);
                        }
                    }
                    s.Close();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    return response;

                }


                catch
                {
                    retry--;
                    lblStatus.Text = "Network problem - retrying\r\n";
                    lblNiktoAI.Text = "Network problem - retrying\r\n";
                    Thread.Sleep(1000);
                }
            }

            return "Retry count (" + updownRetryTCP.Value.ToString() + ") exceeded";
        }

        public static string RandomStringGenerator(int intLen)
        {
            Random r = new Random();
            StringBuilder random_string = new StringBuilder(intLen, intLen);
            for (int i = 0; i < intLen; i++)
                random_string.Append((char)((int)(26 * r.NextDouble()) + 65));
            return random_string.ToString();
        }

        public string buildRequest(string DirectoryItem, string FileItem, string FileTypeItem)
        {
            string myCombo = "";
            string methodGETHEAD = "HEAD";

            if (FileTypeItem.Length > 0)
            {
                myCombo = DirectoryItem + "/" + FileItem + "." + FileTypeItem;
            }
            else
            {
                myCombo = DirectoryItem + "/" + FileItem;
            }

            if ((DirectoryItem.Length == 1) && (FileTypeItem.Length > 0))
            {
                myCombo = DirectoryItem + FileItem + "." + FileTypeItem;
            }
            else if ((DirectoryItem.Length == 1) && (FileTypeItem.Length == 0))
            {
                myCombo = DirectoryItem + FileItem;

            }

            if (radioGET.Checked == true)
            {
                methodGETHEAD = "GET";
            }
            else methodGETHEAD = "HEAD";

            string actualrequest = "";
            if (chkProxyPresent.Checked)
            {
                if (chkuseSSLWikto.Checked || chkBackEnduseSSLport.Checked)
                {
                    actualrequest = methodGETHEAD + " https://" + txtIPNumber.Text + myCombo + " HTTP/1.0\r\n";
                }
                else
                {
                    actualrequest = methodGETHEAD + " http://" + txtIPNumber.Text + myCombo + " HTTP/1.0\r\n";
                }
            }
            else actualrequest = methodGETHEAD + " " + myCombo + " HTTP/1.0\r\n";

            actualrequest += txtHeader.Text + "\r\n\r\n";
            return actualrequest;
        }

        /*public bool testRequest(string ipRaw, string portRaw, string requestRaw, string errorCodes, int TimeOut, bool fileordir)
        {

            if (chkProxyPresent.Checked)
            {
                string[] proxyItems = new string[2];
                proxyItems = txtProxySettings.Text.Split(':');
                ipRaw = proxyItems[0];
                portRaw = proxyItems[1];
            }
            string response = "";
            if (chkBackEnduseSSLport.Checked)
            {
                response = sendraw(ipRaw, portRaw, requestRaw, 1024, TimeOut, true);
            }
            else
            {
                response = sendraw(ipRaw, portRaw, requestRaw, 1024, TimeOut);
            }

            //lets check indexability first
            if (response.IndexOf("ndex of") >= 0 || response.IndexOf("To Parent Directory") >= 0)
            {
                isitIndexable = true;
            }
            else
            {
                isitIndexable = false;
            }

            if (chkBackEndAI.Checked == true)
            {

                //AI stuff
                string[] parts = new string[2000];
                parts = requestRaw.Split(' ');
                double result;
                niktoRequests niktoset;

                //true=file false=dir
                if (fileordir == false)
                {
                    niktoset.description = "FPtestdir";
                    niktoset.request = requestRaw;
                    niktoset.trigger = "";
                    niktoset.type = "FPtestdir";
                    niktoset.method = parts[0];
                    niktoset.sensepostreq = "";

                    string[] urlparts = new string[20];
                    urlparts = parts[1].Split('/');
                    string finalresult = "";
                    for (int a = 1; a < urlparts.Length - 2; a++)
                    {
                        finalresult += "/" + urlparts[a];
                    }

                    result = testniktoFP(ipRaw, portRaw, niktoset, finalresult + "/mooforgetit", response);
                }
                else
                {
                    niktoset.description = "FPtestfile";
                    niktoset.request = requestRaw;
                    niktoset.trigger = "";
                    niktoset.type = "FPtestfile";
                    niktoset.method = parts[0];
                    niktoset.sensepostreq = "";
                    result = testniktoFP(ipRaw, portRaw, niktoset, parts[1], response);
                }
                if ((result < Convert.ToDouble(NUPDOWNBackEnd.Value)) && result >= 0.00)
                {
                    return true;
                }
                else { return false; }

            }
            else
            {
                //normal stuff
                string[] responseline = new string[5];
                string[] errorcodes = new string[20];

                responseline = response.Split('\n');

                errorcodes = errorCodes.Split(',');
                foreach (string errorcodeItem in errorcodes)
                {
                    if (responseline[0].IndexOf(errorcodeItem) > 0)
                    {
                        return true;
                    }
                }
                return false;
            }
        }*/

        public string crop_header(string withheader)
        {
            lock (this)
            {
                int startbody = withheader.IndexOf("\r\n\r\n");
                if (startbody == -1 || startbody == withheader.Length) startbody = 0;
                string returner = withheader.Substring(startbody);
                return returner;
            }
        }

        public double compareBlobs(string blobA, string blobB)
        {

            string cA = crop_header(blobA);
            string cB = crop_header(blobB);

            if (cA.Equals(cB))
            {
                return 1;
            }

            string[] wordsblobA = new string[cA.Split(' ').Length];
            string[] wordsblobB = new string[cB.Split(' ').Length];

            wordsblobA = cA.Split(' ');
            wordsblobB = cB.Split(' ');

            int matchcount = 0;
            foreach (string wordA in wordsblobA)
            {
                foreach (string wordB in wordsblobB)
                {
                    if (wordA.CompareTo(wordB) == 0)
                    {
                        matchcount++;
                        break;
                    }
                }
            }
            return (Math.Round((double)(matchcount * 2.0) / ((double)wordsblobA.Length + (double)wordsblobB.Length), 5));
        }

        private string extractFileType(string request)
        {
            string[] header = new string[5];
            header = request.Split('\n');
            try
            {
                string[] types = new string[20];
                types = header[0].Split('?');

                string[] dot = new string[20];
                dot = types[0].Split('.');
                if (dot[dot.Length - 1].Length > 5)
                {
                    return "default";
                }
                else return dot[dot.Length - 1];
            }
            catch
            {
                return "default";
            }
        }

        private string extractLocation(string request)
        {
            string[] header = new string[5];
            header = request.Split('\n');
            header[0] = header[0].Replace("..", "||");

            string[] directories = new string[20];

            directories = header[0].Split('?', '!', '#', '$', '^', '&', '\\', '*', ',');

            string[] dirparts = new string[2000];
            dirparts = directories[0].Split('/');
            string returner = "";

            int endplace = 2;
            try
            {
                if (directories[0].ToCharArray()[directories[0].Length - 1].CompareTo('/') == 0)
                {
                    endplace = 3;
                }
            }
            catch { }


            for (int i = 0; i <= dirparts.Length - endplace; i++)
            {
                returner += dirparts[i] + "/";
            }
            returner = returner.Replace("||", "..");
            return returner;
        }

        private string ParseDirectores(string request)
        {
            Uri myuri = new Uri(request);
            string returner = "";
            string Parts1 = myuri.PathAndQuery.ToString();
            String[] Parts2 = Parts1.Split('?', '#');
            String[] Parts3 = Parts2[0].Split('/');
            if (Parts3[Parts3.Length - 1].IndexOf('.') > 0)
            {
                for (int i = 0; i < Parts3.Length - 1; i++)
                {
                    returner = "/" + Parts3[i];
                }
            }
            else
            {
                returner = Parts2[0];
            }
            if (returner == "/")
                returner = "";
            return (returner);
        }

        private string generateBlob(string target, string port, niktoRequests niktoset, string filetype, string filelocation)
        {

            niktoRequests FPtest;
            FPtest.method = niktoset.method;
            FPtest.description = "FP test item";
            FPtest.type = "FP test item";
            FPtest.trigger = "";
            FPtest.sensepostreq = "";

            if (filetype.CompareTo("default") != 0)
            {
                FPtest.request = filelocation + "noteverthere." + filetype;
            }
            else FPtest.request = filelocation + "noteverthere/";

            string result = stestNiktoRequest(target, port, buildNiktoRequest(FPtest), FPtest, 6000);

            if (niktoset.type.CompareTo("FPtestfile") == 0 || niktoset.type.CompareTo("FPtestdir") == 0)
            {
                backend_FP[globalFPb].URLlocation = filelocation;
                backend_FP[globalFPb].HTTPblob = result;
                backend_FP[globalFPb].filetype = filetype;
                backend_FP[globalFPb].method = FPtest.method;
                globalFPb++;
            }
            else
            {
                nikto_FP[globalFP].URLlocation = filelocation;
                nikto_FP[globalFP].HTTPblob = result;
                nikto_FP[globalFP].filetype = filetype;
                nikto_FP[globalFP].method = FPtest.method;
                globalFP++;
            }

            return result;

        }

        private string getBlob(string ipRaw, string portRaw, niktoRequests niktoset, string filetype, string filelocation)
        {

            if (niktoset.type.CompareTo("FPtestfile") == 0 || niktoset.type.CompareTo("FPtestdir") == 0)
            {
                for (int i = 0; i < globalFPb; i++)
                {
                    if ((backend_FP[i].URLlocation.CompareTo(filelocation) == 0) &&
                        (backend_FP[i].filetype.CompareTo(filetype) == 0) &&
                        (backend_FP[i].method.CompareTo(niktoset.method) == 0))
                    {
                        //if (chkBackEndShowAI.Checked){
                        //	lblBackEndAI.Text+="Blob found in DB!\r\n";
                        //}
                        return backend_FP[i].HTTPblob;
                    }
                }
            }
            else
            {
                for (int i = 0; i < globalFP; i++)
                {
                    if ((nikto_FP[i].URLlocation.CompareTo(filelocation) == 0) &&
                        (nikto_FP[i].filetype.CompareTo(filetype) == 0) &&
                        (nikto_FP[i].method.CompareTo(niktoset.method) == 0))
                    {
                        //lblNiktoAI.Text+="Blob found in DB!\r\n";
                        return nikto_FP[i].HTTPblob;
                    }
                }
            }
            //if we end up here we know we must go get a new one
            if (niktoset.type.CompareTo("FPtestfile") == 0)
            {
                //if (chkBackEndShowAI.Checked){
                //	lblBackEndAI.Text+="Not found in DB - getting it...\r\n";
                //}
            }
            else
                if (niktoset.type.CompareTo("FPtestdir") != 0)
                {
                    //if (chkBackEndShowAI.Checked){
                    //	lblNiktoAI.Text+="Fingerprint not found in DB - getting it...\r\n\r\n";
                    //}
                }
            return generateBlob(ipRaw, portRaw, niktoset, filetype, filelocation);
        }

        private ArrayList dedupe_AR(ArrayList lots)
        {
            Hashtable work = new Hashtable();
            ArrayList returner = new ArrayList();

            foreach (string entry in lots)
            {
                if (entry.Length > 0)
                {
                    try
                    {
                        work.Add(entry, entry);
                    }
                    catch { } //duplicate entry
                }
            }

            foreach (string moo in work.Keys)
            {
                returner.Add(moo);
            }
            return returner;
        }
        #endregion

        #region We'll come back to these...
        private void populateHeader(object sender, System.EventArgs e) {
			txtHeader.Text="Accept: */*\r\nAccept-Language: en-us\r\nConnection: close\r\nUser-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)\r\nHost: "+txtIPNumber.Text;
		}
		private void populateHeaderfromNikto(object sender, System.EventArgs e) {
			txtHeader.Text="Accept: */*\r\nAccept-Language: en-us\r\nConnection: close\r\nUser-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)\r\nHost: "+txtNiktoTarget.Text;
		}
		
		private void button7_Click(object sender, System.EventArgs e) {
			txtInDirs.Clear();
		}
		private void btnClearFiles_Click(object sender, System.EventArgs e) {
			txtInFileTypes.Clear();
		}
		private void btnClearFileTypes_Click(object sender, System.EventArgs e) {
			txtInFiles.Clear();
		}
		private void btnGoogleMinedDirClear_Click(object sender, System.EventArgs e) {
			//txtGoogleCGImined.Clear();
		}
		private void button8_Click(object sender, System.EventArgs e) {
			lstGoogleHackResults.Items.Clear();
		}
		private void btnHTCleanLinks_Click(object sender, System.EventArgs e)
		{
            lstMirrorLinks.Items.Clear();
		}

		private void btnHTClearDirs_Click(object sender, System.EventArgs e)
		{
		}

		private void btnSkipDirs_Click(object sender, System.EventArgs e) 
		{
			skipDirectories=true;
		}
		private void btnSkipFile_Click(object sender, System.EventArgs e) {
			stopdir=true;
		}
		private void btnStopNikto_Click(object sender, System.EventArgs e) 
        {
			stopnikto=true;
            pauseWikto = false;
            btnPauseWikto.Text = "Pause";
            btnPauseWikto.Enabled = false;
        }
        private void btnPauseWikto_Click(object sender, EventArgs e)
        {
            pauseWikto = !pauseWikto;

            if (pauseWikto)
                btnPauseWikto.Text = "Resume";
            else
                btnPauseWikto.Text = "Pause";
        }

		private void btnStopGoole_Click(object sender, System.EventArgs e) {
			stoppitgoogle=true;
		}
		private void btnStop_Click(object sender, System.EventArgs e) 
        {
			stoppit=true;
            pauseBackEnd = false;
            btnBackEndPause.Text = "Pause";
            btnBackEndPause.Enabled = false;
		}
		private void btnQuit3_Click(object sender, System.EventArgs e) {
			Quitter();
		}
		private void button5_Click(object sender, System.EventArgs e) {
			Quitter();
		}
		private void button3_Click(object sender, System.EventArgs e) {
			Quitter();
		}
		private void btnQuit5_Click(object sender, System.EventArgs e) {
			Quitter();
		}
		private void button1_Click_1(object sender, System.EventArgs e)
		{
			Quitter();
		}
		
		private void button2_Click(object sender, System.EventArgs e) {
			stoppit=true;
		}
		private void btnStopGoogleHack_Click(object sender, System.EventArgs e) {
			stopGH=true;
            this.Invoke(this.dlgControlerSetReadonly, new Object[] { this.txtGoogleHackTarget, false });
		}

		private void stopscrolling(object sender, System.EventArgs e) {
			stopscroll=true;
		}
		private void keepscrolling(object sender, System.EventArgs e) {
			stopscroll=false;
		}
		private void GHstopscrolling(object sender, System.EventArgs e) {
			GHstopscroll=true;
		}
		private void GHkeepscrolling(object sender, System.EventArgs e) {
			GHstopscroll=false;
        }
        #endregion

        #region Scan Wizard Routines
        private void CheckIfIShouldStartTheWizard(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedIndex == 6)
            {
                Wizard thewiz = new Wizard(this);
                this.tabControl1.SelectedIndex = 0;
                thewiz.ShowDialog();
                thewiz.Dispose();
                thewiz = null;
            }
        }

        public void SetWizScanData(string thehost, string theport, string thessl, string thegoogle, string theuseproxy, string theproxy, string theuseai)
        {
            if (thessl.ToUpper() == "HTTP")
            {
                chk_SpiderSSL.Checked = false;
                chkBackEnduseSSLport.Checked = false;
                chkuseSSLWikto.Checked = false;
            }
            else
            {
                chk_SpiderSSL.Checked = true;
                chkBackEnduseSSLport.Checked = true;
                chkuseSSLWikto.Checked = true;
            }
            if (thegoogle.ToUpper() == "YES")
            {
                txtGoogleTarget.Text = thehost;
                txtGoogleHackTarget.Text = thehost;
                txtGoogleKeyword.Text = txtGoogleTarget.Text.Replace("www.", "").Replace(".com", "").Replace(".net", "");
            }
            else
            {
                txtGoogleTarget.Text = "";
                txtGoogleHackTarget.Text = "";
            }
            txtIPNumber.Text = thehost;
            txtIPPort.Text = theport;
            txtNiktoTarget.Text = thehost;
            txtNiktoPort.Text = theport;
            txtHTTarget.Text = thehost;
            txt_SpiderPort.Text = theport;
            if (theuseai.ToUpper() == "YES")
            {
                chkBackEndAI.Checked = true;
            }
            else
            {
                chkBackEndAI.Checked = false;
            }
            if (theuseproxy.ToUpper() == "YES")
            {
                chkProxyPresent.Checked = true;
                txtProxySettings.Text = theproxy;
            }
            else
            {
                chkProxyPresent.Checked = false;
                txtProxySettings.Text = "";
            }
            //txtGoogleKey.Text = thegoogleapi;
        }

        private void chk_StartWiz_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_StartWiz.Checked)
            {
                bl_ShowStartWiz = true;
                setregistrywizard(true);
            }
            else
            {
                bl_ShowStartWiz = false;
                setregistrywizard(false);
            }
        }

         #endregion

        #region News Form Routines
        #endregion

        #region Url Handlers (URL Labels etc)
        private void GotoLink(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string target = e.Link.LinkData as string;
            System.Diagnostics.Process.Start(target);
        }
        #endregion

        #region Button Target voids
        private void chkProxyPresent_CheckedChanged(object sender, System.EventArgs e) {
			if (chkProxyPresent.Checked==true){
				MessageBox.Show("Remember - proxy wont work with SSL...yet!");
			}
		}

        private void skinButtonRed3_Click(object sender, EventArgs e)
        {
            lstMirrorDirs.Items.Clear();
        }

        private void btnGoogleClearDir_Click(object sender, EventArgs e)
        {
            lstGoogleDir.Items.Clear();
        }

        private void btnGoogleClearLink_Click(object sender, EventArgs e)
        {
            lstGoogleLink.Items.Clear();
        }

        private void btn_BEInDirClear_Click(object sender, EventArgs e)
        {
            txtInDirs.Text = "";
        }

        private void btn_BEInFileClear_Click(object sender, EventArgs e)
        {
            txtInFiles.Text = "";
        }

        private void btn_BEInExtClear_Click(object sender, EventArgs e)
        {
            txtInFileTypes.Text = "";
        }

        private void btn_BEOutFileClear_Click(object sender, EventArgs e)
        {
            lstViewFiles.Items.Clear();
        }

        private void btn_BEOutIndexClear_Click(object sender, EventArgs e)
        {
            lstViewIndexDirs.Items.Clear();
        }

        private void btn_BEOutDirClear_Click(object sender, EventArgs e)
        {
            lstViewDirs.Items.Clear();
        }

        private void btn_BEInDirImportM_Click(object sender, EventArgs e)
        {
            if ((!txtInDirs.Text.EndsWith("\n")) && (!txtInDirs.Text.EndsWith("\r")) && (txtInDirs.Text.Length != 0))
                txtInDirs.Text += "\n";
            int i;
            for (i = 0; i < lstMirrorDirs.Items.Count; i++)
            {
                String ItemToAdd = "";
                if (lstMirrorDirs.Items[i].ToString().StartsWith("/"))
                    ItemToAdd = lstMirrorDirs.Items[i].ToString();
                else
                    ItemToAdd = "/" + lstMirrorDirs.Items[i].ToString();
                if (ItemToAdd.EndsWith("/"))
                    ItemToAdd = ItemToAdd.TrimEnd('/');
                txtInDirs.Text += ItemToAdd + "\n";
            }
        }

        private void btn_BEImportInDirG_Click(object sender, EventArgs e)
        {
            if ((!txtInDirs.Text.EndsWith("\n")) && (!txtInDirs.Text.EndsWith("\r")) && (txtInDirs.Text.Length != 0))
                txtInDirs.Text += "\n";
            int i;
            for (i = 0; i < lstGoogleDir.Items.Count; i++)
            {
                String ItemToAdd = "";
                if (lstGoogleDir.Items[i].ToString().StartsWith("/"))
                    ItemToAdd = lstGoogleDir.Items[i].ToString();
                else
                    ItemToAdd = "/" + lstGoogleDir.Items[i].ToString();
                if (ItemToAdd.EndsWith("/"))
                    ItemToAdd = ItemToAdd.TrimEnd('/');
                txtInDirs.Text += ItemToAdd + "\n";
            }
        }

        private void btn_WiktoImportBackEnd_Click(object sender, EventArgs e)
        {
            //int i;
            //for (i = 0; i < lstViewDirs.Items.Count; i++)
            //{
            //    String ItemToAdd = "";
            //    if (lstViewDirs.Items[i].ToString().StartsWith("/"))
            //        ItemToAdd = lstViewDirs.Items[i].ToString();
            //    else
            //        ItemToAdd = "/" + lstViewDirs.Items[i].ToString();
            //    DelgateDeclarationListUnq(this.lst_NiktoCGI, ItemToAdd.TrimEnd('/'));
            //}

            foreach (ListViewItem lvi in lstViewDirs.Items)
            {
                if (lvi.ImageIndex == 0)
                {
                    string ItemToAdd = lvi.SubItems[1].Text;

                    if (!ItemToAdd.StartsWith("/"))
                        ItemToAdd = "/" + ItemToAdd;

                    DelgateDeclarationListUnq(this.lst_NiktoCGI, ItemToAdd.TrimEnd('/'));
                }
            }
        }

        private void btn_WiktoImportMirror_Click(object sender, EventArgs e)
        {
            int i;
            for (i = 0; i < lstMirrorDirs.Items.Count; i++)
            {
                String ItemToAdd = "";
                if (lstMirrorDirs.Items[i].ToString().StartsWith("/"))
                    ItemToAdd = lstMirrorDirs.Items[i].ToString();
                else
                    ItemToAdd = "/" + lstMirrorDirs.Items[i].ToString();
                DelgateDeclarationListUnq(this.lst_NiktoCGI, ItemToAdd.TrimEnd('/'));
            }
        }

        private void btn_WiktoImportGoogle_Click(object sender, EventArgs e)
        {
            int i;
            for (i = 0; i < lstGoogleDir.Items.Count; i++)
            {
                String ItemToAdd = "";
                if (lstGoogleDir.Items[i].ToString().StartsWith("/"))
                    ItemToAdd = lstGoogleDir.Items[i].ToString();
                else
                    ItemToAdd = "/" + lstGoogleDir.Items[i].ToString();
                DelgateDeclarationListUnq(this.lst_NiktoCGI, ItemToAdd.TrimEnd('/'));
            }
        }

        private void btn_WiktoClearCGI_Click(object sender, EventArgs e)
        {
            lst_NiktoCGI.Items.Clear();
        }

        private void ResizeListViews(object sender, EventArgs e)
        {
            lvw_NiktoDb.Columns[0].Width = System.Convert.ToInt32(lvw_NiktoDb.Width * 0.5);
            lvw_NiktoDb.Columns[1].Width = System.Convert.ToInt32(lvw_NiktoDb.Width * 0.5);

            lvw_NiktoResults.Columns[0].Width = System.Convert.ToInt32(lvw_NiktoResults.Width / 5);
            lvw_NiktoResults.Columns[1].Width = System.Convert.ToInt32(lvw_NiktoResults.Width / 4);
            lvw_NiktoResults.Columns[2].Width = System.Convert.ToInt32(lvw_NiktoResults.Width / 4);
            lvw_NiktoResults.Columns[3].Width = System.Convert.ToInt32(lvw_NiktoResults.Width / 4);
        }

        private void btn_GHClearResults_Click(object sender, EventArgs e)
        {
            lstGoogleHackResults.Items.Clear();
        }

        private void cmbBackEndUpdate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBackEndUpdate.Items.Count < 2)
            {
                try
                {
                    btn_BEUpdateFromSP.PerformClick();
                    btn_BEUpdateFromSP.Focus();
                }
                catch { }
            }
        }

        public bool GetProxyEnabled()
        {
            bool MyProxy = (bool)this.Invoke(this.dlgControlChckGet, new Object[] { this.chkProxyPresent });
            return MyProxy;
        }

        public string GetProxySettings()
        {
            string MyProxy = this.Invoke(this.dlgControlTextGet, new Object[] { this.txtProxySettings }).ToString();
            return MyProxy;
        }

        private void skinButton6_Click(object sender, EventArgs e)
        {
            lstViewDirs.Items.Clear();
            lstViewIndexDirs.Items.Clear();
            lstViewFiles.Items.Clear();
            if (chkBackEndAI.Checked)
            {
                foreach (BackEndMining bem in backend_dirresults)
                {
                    if (bem.location != null)
                    {
                        try
                        {
                            if (System.Convert.ToDouble(bem.ai) <= System.Convert.ToDouble(NUPDOWNBackEnd.Value))
                            {
                                lstViewDirs.Items.Add(bem.location);
                                if (bem.indexable)
                                    lstViewIndexDirs.Items.Add(bem.location);
                            }
                        }
                        catch { }
                    }
                }
                foreach (BackEndMining bem in backend_filresults)
                {
                    if (bem.location != null)
                    {
                        try
                        {
                            if (System.Convert.ToDouble(bem.ai) <= System.Convert.ToDouble(NUPDOWNBackEnd.Value))
                            {
                                lstViewFiles.Items.Add(bem.location);
                            }
                        }
                        catch { }
                    }
                }
            }
            else
            {
                foreach (BackEndMining bem in backend_dirresults)
                {
                    if (bem.location != null)
                    {
                        if (txtErrorCodeDir.Text.Contains(bem.responsecode))
                        //if (txtErrorCodeDir.Text.IndexOf(bem.responsecode) > 0)
                        {
                            lstViewDirs.Items.Add(bem.location);
                            if (bem.indexable)
                                lstViewIndexDirs.Items.Add(bem.location);
                        }
                    }
                }
                foreach (BackEndMining bem in backend_filresults)
                {
                    if (bem.location != null)
                    {
                        if (txtErrorCodeFile.Text.IndexOf(bem.responsecode) > 0)
                        {
                            lstViewFiles.Items.Add(bem.location);
                        }
                    }
                }
            }
        }

        private void skinButton5_Click(object sender, EventArgs e)
        {
            lstViewDirs.Items.Clear();
            lstViewIndexDirs.Items.Clear();
            lstViewFiles.Items.Clear();
            foreach (BackEndMining bem in backend_dirresults)
            {
                if (bem.location != null)
                {
                    lstViewDirs.Items.Add(bem.location);
                    if (bem.indexable)
                        lstViewIndexDirs.Items.Add(bem.location);
                }
            }
            foreach (BackEndMining bem in backend_filresults)
            {
                if (bem.location != null)
                {
                    lstViewFiles.Items.Add(bem.location);
                }
            }
        }

        private void skinButton4_Click(object sender, EventArgs e)
        {
            NUPDOWNBackEnd.Value = BFNupVal;
            txtErrorCodeFile.Text = BFFilErr;
            txtErrorCodeDir.Text = BFDirErr;
            chkBackEndAI.Checked = true;
            ToggleBackendAI(null, null);
            skinButton6.PerformClick();
        }

        private void skinButton3_Click(object sender, EventArgs e)
        {
            News thenewsform = new News(this);
            thenewsform.ShowDialog();
            thenewsform.Dispose();
            thenewsform = null;
        }

        private void chk_ShowStart_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_ShowStart.Checked)
            {
                bl_ShowStart = true;
                setregistrystart(true);
            }
            else
            {
                bl_ShowStart = false;
                setregistrystart(false);
            }
        }     

        private void btn_browsenikto_Click(object sender, EventArgs e)
        {
            fdlLoadNiktoDB.FileName = "";
            string[] tmparray = Application.ExecutablePath.Split('\\');
            string tmpdir = "";
            tmparray[tmparray.Length - 1] = "databases";
            foreach (string s in tmparray)
            {
                if (tmpdir.Length == 0) tmpdir = s;
                else tmpdir = tmpdir + '\\' + s;
            }
            fdlLoadNiktoDB.InitialDirectory = tmpdir;
            DialogResult didheopen = fdlLoadNiktoDB.ShowDialog();
            if (didheopen != DialogResult.OK)
            {
                return;
            }
            else
            {
                txtDBlocationNikto.Text = fdlLoadNiktoDB.FileName;
            }
        }

        private void btn_browseghdb_Click(object sender, EventArgs e)
        {
            fdlLoadNiktoDB.FileName = "";
            string[] tmparray = Application.ExecutablePath.Split('\\');
            string tmpdir = "";
            tmparray[tmparray.Length - 1] = "databases";
            foreach (string s in tmparray)
            {
                if (tmpdir.Length == 0) tmpdir = s;
                else tmpdir = tmpdir + '\\' + s;
            }
            fdlLoadNiktoDB.InitialDirectory = tmpdir;
            DialogResult didheopen = fdlLoadNiktoDB.ShowDialog();
            if (didheopen != DialogResult.OK)
            {
                return;
            }
            else
            {
                txtDBLocationGH.Text = fdlLoadNiktoDB.FileName;
            }
        }

        private void chk_ignoreidx_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_ignoreidx.Checked)
                txt_idxflags.Enabled = false;
            else
                txt_idxflags.Enabled = true;
        }
        

        private void btn_EditDirs_Click(object sender, EventArgs e)
        {
            EditItems the_from = new EditItems(this, lst_NiktoCGI);
            the_from.ShowDialog();
            the_from.Dispose();
        }

        #endregion

        #region REMOVED SUBS...
        /*private void button2_Click_1(object sender, System.EventArgs e)
		{
			string[] mirrordirs=new string[2000];
			//mirrordirs=txtHTDirs.Text.Replace("\r\n","\n").Split('\n');
			
			foreach (string dir in mirrordirs)
			{
				if (dir.Length>0)
				{
					txtInDirs.AppendText("/"+dir.Trim('/')+"\r\n");
				}
			}
		}*/
        #endregion

    }
    #endregion

}
