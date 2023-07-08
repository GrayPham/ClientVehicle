using Connect.Common;
using Connect.Common.Collection;
using Connect.Common.Contract;
using Connect.Common.Helper;
using Connect.Common.Interface;
using Connect.Common.Logging;
using Connect.RemoteDataProvider.Common;
using Connect.RemoteDataProvider.Interface;
using Connect.SocketClient;
using Newtonsoft.Json;
using nsConnect.RemoteDataProvider.Client;
using nsFramework.Common.Pattern;
using Parking.App.Common;
using Parking.App.Common.ApiMethod;
using Parking.App.Common.Constants;
using Parking.App.Common.Helper;
using Parking.App.Common.ViewModels;
using Parking.App.Contract.Common;
using Parking.App.Contract.Service;
using Parking.App.Contract.Setting;
using Parking.App.Interface.Common;
using Parking.App.Service.Common;
using Parking.App.Service.Handlers;
using Parking.Contract.Common;
using Server.Contract.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Parking.App.Factory
{
    public class ProgramFactory : IClientHandler
    {

        #region Singleton
        public event GeneralEventHandler<string> ClientConnected;
        public event GeneralEventHandler<string> ClientDisconnected;
        public event GeneralEventHandler<string> ClientReConnected;

        private void OnClientConnected(string data)
        {
            if (ClientConnected != null) ClientConnected(this, new EventArgs<string>(data));
        }
        private void OnClientDisconnected(string data)
        {
            if (ClientDisconnected != null) ClientDisconnected(this, new EventArgs<string>(data));
        }
        private void OnClientReConnected(string data)
        {
            if (ClientReConnected != null) ClientReConnected(this, new EventArgs<string>(data));
        }
        public virtual IProgramController ProgramController { get; set; }
        public static ProgramFactory Instance
        {
            get { return Nested.Factory; }
        }

        private class Nested
        {
            internal static readonly ProgramFactory Factory;
            internal static readonly bool IsCreated = false;
            // Explicit static constructor to tell C# compiler
            // not to mark type as before field initialize

            static Nested()
            {
                Factory = new ProgramFactory();
                IsCreated = true;
            }
        }


        #endregion

        #region Member
        protected readonly ILog _log = new DirectLog(null, Singleton<StandardConsole>.Instance);
        protected readonly ISocketClient _socketClientServer;
        protected readonly ISessionHandler _clientSessionHandler;

        protected readonly IPort _serialPort;
        protected string _ipServer = "";
        protected int _port = 9000;
        protected string _path = System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        public string IPServer => _ipServer;
        public ILog Log
        {
            get { return _log; }
        }
        public ISocketClient SocketClientServer
        {
            get { return _socketClientServer; }
        }
        public ISessionHandler ClientSessionHandler
        {
            get { return _clientSessionHandler; }
        }

        public string sPath => _path;
        #endregion

        //---------------------------------------------------------------

        #region Service
        private readonly ILoginService _loginService;
        private readonly tblClientSoundMgtService _tblClientSoundMgtService;
        private readonly tblAdMgtService _tblAdMgtService;
        private readonly tblDeviceRequestConnectService _tblDeviceRequestConnectService;
        private readonly tblUserService _tblUserService;
        private readonly tblUserPhotoService _tblUserPhotoService;
        private readonly tblStoreDeviceService _tblStoreDeviceService;
        private readonly tblUserMgtStoreService _tblUserMgtStoreService;
        private readonly tblStoreEnvironmentSettingService _tblStoreEnvironmentSettingService;
        private readonly tblStoreMasterService _tblStoreMasterService;
        private IList<tblClientSoundMgtInfo> _listSetting;
        private readonly InfoCollection<tblAdMgtInfo> _tblAdMgtInfos = new InfoCollection<tblAdMgtInfo>();
        private readonly XMLReader _xml = new XMLReader();
        private string _pathAdMgt = "";
        #endregion

        //---------------------------------------------------------------

        #region Constructor

        public ProgramFactory()
        {
            try
            {
                _path = System.IO.Path.GetDirectoryName(new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                _log = new DirectLog(_path, null, Singleton<StandardConsole>.Instance);
                _pathAdMgt = System.IO.Path.Combine(_path, "AdMgt");
                #region appsettings
                var _settingInfo = _xml.ReadJsonInfo<SettingInfo>(@"\Setting\appsettings.json");
                if (_settingInfo != null)
                {
                    ApplicationInfo.PortUser = _settingInfo.PortUser;
                    ApplicationInfo.IPServer = _settingInfo.IPServer;
                    ApplicationInfo.VersionName = _settingInfo.VersionName;
                    ApplicationInfo.VersionCode = _settingInfo.VersionCode;
                    ApplicationInfo.VersionBy = _settingInfo.VersionBy;
                    ApplicationInfo.VersionDate = _settingInfo.VersionDate;
                    ApplicationInfo.Company = _settingInfo.Company;
                    ApplicationInfo.Copyright = _settingInfo.Copyright;
                    ApplicationInfo.FirstName = _settingInfo.FirstName;
                    ApplicationInfo.LastName = _settingInfo.LastName;
                    ApplicationInfo.AppName = _settingInfo.AppName;
                    ApplicationInfo.Author = _settingInfo.Author;
                    ApplicationInfo.GuiD = _settingInfo.GuiD;
                    ApplicationInfo.Phone = _settingInfo.Phone;
                    ApplicationInfo.LicenseKey = _settingInfo.LicenseKey;
                    ApplicationInfo.SoftwareKey = _settingInfo.SoftwareKey;
                    ApplicationInfo.PCID = _settingInfo.PCID;
                    ApplicationInfo.CpuIdKey = _settingInfo.CpuIdKey;
                    ApplicationInfo.InstallID = _settingInfo.InstallID;

                    Constants.PhoneNumberValidation = _settingInfo.PhoneNumberValidation;
                    Constants.PhoneAuthFailure = _settingInfo.PhoneAuthFailure;
                    Constants.WrongCapcha = _settingInfo.WrongCapcha;
                    Constants.IsSussessGetData = _settingInfo.IsSussessGetData;
                    Constants.CodeSussesGetData = _settingInfo.CodeSussesGetData;
                    Constants.RightOTP = _settingInfo.RightOTP;
                    Constants.TypeImg = _settingInfo.TypeImg;
                    Constants.TempMember = _settingInfo.TempMember;
                    Constants.OfficeMember = _settingInfo.OfficeMember;
                    Constants.CardMethod = _settingInfo.CardMethod;
                    Constants.PhoneMethod = _settingInfo.PhoneMethod;
                    Constants.AuthMethodHeader1 = _settingInfo.AuthMethodHeader1;
                    Constants.AuthMethodHeader2 = _settingInfo.AuthMethodHeader2;
                    Constants.CheckUserInforHeader1 = _settingInfo.CheckUserInforHeader1;
                    Constants.CheckUserInforHeader2 = _settingInfo.CheckUserInforHeader2;
                    Constants.IdCardHeader1 = _settingInfo.IdCardHeader1;
                    Constants.IdCardHeader2 = _settingInfo.IdCardHeader2;
                    Constants.InputPhoneNumberHeader1 = _settingInfo.InputPhoneNumberHeader1;
                    Constants.InputPhoneNumberHeader2 = _settingInfo.InputPhoneNumberHeader2;
                    Constants.PhoneAuthHeader1 = _settingInfo.PhoneAuthHeader1;
                    Constants.PhoneAuthHeader2 = _settingInfo.PhoneAuthHeader2;
                    Constants.ScannerPhotoCardHeader1 = _settingInfo.ScannerPhotoCardHeader1;
                    Constants.ScannerPhotoCardHeader2 = _settingInfo.ScannerPhotoCardHeader2;
                    Constants.ApiServerURL = _settingInfo.ApiServerURL;
                    Constants.ApiVersionURL = _settingInfo.ApiVersionURL;
                    Constants.ApiOcrURL = _settingInfo.ApiOcrURL;
                    Constants.OcrSecretCode = _settingInfo.OcrSecretCode;
                    Constants.ApiQrURL = _settingInfo.ApiQrURL;
                    Constants.SRP001 = _settingInfo.SRP001;
                    Constants.URIActiveMq = _settingInfo.URIActiveMq;
                    Constants.UiPassTextFolder = _settingInfo.UiPassTextFolder;
                    Constants.UiPassImageFolder = _settingInfo.UiPassImageFolder;
                    Constants.PostCallUri = _settingInfo.PostCallUri;
                    Constants.KcbModule = _settingInfo.KcbModule;
                    Constants.PrevArrow = _settingInfo.PrevArrow;
                    Constants.NextArrow = _settingInfo.NextArrow;
                    Constants.isEnableAutomaticalyRunApp = _settingInfo.isEnableAutomaticalyRunApp;
                    Constants.TokenTelegram = _settingInfo.TokenTelegram;
                    Constants.IdUserTelegram = _settingInfo.IdUserTelegram;
                    Constants.PathAutoUpdate = _settingInfo.PathAutoUpdate;
                    Constants.FileAutoUpdate = _settingInfo.FileAutoUpdate;
                    Constants.AppAutoUpdate = _settingInfo.AppAutoUpdate;

                    Constants.Title1 = _settingInfo.Title1;
                    Constants.Title2 = _settingInfo.Title2;
                    Constants.Title3 = _settingInfo.Title3;
                    Constants.CameraTop = _settingInfo.CameraTop;
                    Constants.CameraBot = _settingInfo.CameraBot;
                }
                #endregion

                _ipServer = ApplicationInfo.IPServer;
                _port = ApplicationInfo.PortUser;
                _clientSessionHandler = new SessionHandler(_log, ApplicationInfo.eClientType);
                ApplicationInfo.PortUser = _port;
                var ipaddress = IPAddress.Parse(_ipServer);
                var endpoint = new IPEndPoint(ipaddress, _port);
                _socketClientServer = new AsyncSocketClient<StandardTcpClientHandler>(endpoint, _log);
                _socketClientServer.Connected += (sender, e) =>
                {
                    _clientSessionHandler.Connected((IPort)e.Data);
                    OnClientConnected(e.Data.ToString());
                };
                _socketClientServer.Disconnected += (send, e) =>
                {
                    _clientSessionHandler.Disconnected(); OnClientDisconnected("");
                };
                _socketClientServer.ReConnected += (send, e) => { OnClientReConnected(""); };

                var loginService = new RemoteLoginService(_log);
                _loginService = loginService;
                loginService.RegisterType();
                _clientSessionHandler.Register(loginService);
                //_socketClientServer.Connect();

                _listSetting = _xml.ReadXml<tblClientSoundMgtInfo>(@"\Setting\SettingView.xml");
                var tblAdMgtInfos = _xml.ReadXml<tblAdMgtInfo>(@"\Setting\AdMgtInfos.xml");
                if (tblAdMgtInfos != null && tblAdMgtInfos.Any())
                {
                    _tblAdMgtInfos.AddList(tblAdMgtInfos);
                }

                RemoteCacheDataService<tblClientSoundMgtInfo>.RegisterType(_log);
                _tblClientSoundMgtService = new tblClientSoundMgtService(_log, true, SSignature.tblClientSoundMgtService, SSignature.ToString(SSignature.tblClientSoundMgtService));
                _clientSessionHandler.Register(_tblClientSoundMgtService);

                RemoteCacheDataService<tblAdMgtInfo>.RegisterType(_log);
                _tblAdMgtService = new tblAdMgtService(_log, false, SSignature.tblAdMgtService, SSignature.ToString(SSignature.tblAdMgtService));
                _clientSessionHandler.Register(_tblAdMgtService);

                RemoteCacheDataService<tblDeviceRequestConnectInfo>.RegisterType(_log);
                _tblDeviceRequestConnectService = new tblDeviceRequestConnectService(_log, false, SSignature.tblAdMgtService, SSignature.ToString(SSignature.tblAdMgtService));
                _clientSessionHandler.Register(_tblDeviceRequestConnectService);

                RemoteCacheDataService<tblUserInfo>.RegisterType(_log);
                _tblUserService = new tblUserService(_log, false, SSignature.tblUserService, SSignature.ToString(SSignature.tblUserService));
                _clientSessionHandler.Register(_tblUserService);

                RemoteCacheDataService<tblUserPhotoInfo>.RegisterType(_log);
                _tblUserPhotoService = new tblUserPhotoService(_log, false, SSignature.tblUserPhotoService, SSignature.ToString(SSignature.tblUserPhotoService));
                _clientSessionHandler.Register(_tblUserPhotoService);

                RemoteCacheDataService<tblStoreDeviceInfo>.RegisterType(_log);
                _tblStoreDeviceService = new tblStoreDeviceService(_log, false, SSignature.tblStoreDeviceService, SSignature.ToString(SSignature.tblStoreDeviceService));
                _clientSessionHandler.Register(_tblStoreDeviceService);

                RemoteCacheDataService<tblUserMgtStoreInfo>.RegisterType(_log);
                _tblUserMgtStoreService = new tblUserMgtStoreService(_log, false, SSignature.tblUserMgtStoreService, SSignature.ToString(SSignature.tblUserMgtStoreService));
                _clientSessionHandler.Register(_tblUserMgtStoreService);
                RemoteCacheDataService<tblStoreEnvironmentSettingInfo>.RegisterType(_log);
                _tblStoreEnvironmentSettingService = new tblStoreEnvironmentSettingService(_log, false, SSignature.tblStoreEnvironmentSettingService, SSignature.ToString(SSignature.tblStoreEnvironmentSettingService));
                _clientSessionHandler.Register(_tblStoreEnvironmentSettingService);

                RemoteCacheDataService<tblStoreMasterInfo>.RegisterType(_log);
                _tblStoreMasterService = new tblStoreMasterService(_log, false, SSignature.tblStoreMasterService, SSignature.ToString(SSignature.tblStoreMasterService));
                _clientSessionHandler.Register(_tblStoreMasterService);
                GetStoreNoAsync();
                var _storeMasterClient = _tblStoreMasterService.RegisterClient(_tblStoreMasterService.GetType().Name, StoreMasterSynchronized);
                GetCommonSub();

            }
            catch (Exception ex)
            {
                _log.SError(this.GetType().Name, ex.Message, ex.StackTrace, ex.Message);
            }

        }
        private void StoreMasterSynchronized(object sender, EventArgs<int> e)
        {

        }
        private void DeleteFileFromFolder(tblAdMgtInfo info)
        {

        }

        private void WirteFile(tblAdMgtInfo info)
        {
            try
            {
                var ext = Path.GetExtension(info.AttachFilePath);
                if (string.IsNullOrWhiteSpace(ext))
                {
                    ext = ".png";
                }
                var fileName = info.AdName + ext;
                var re = ApiMethod.DownFileAdmgt(info.AdNo, System.IO.Path.Combine(_pathAdMgt, fileName));
                if (re.Status)
                {
                    info.LocalPath = fileName;
                }
            }
            catch (Exception ex)
            {
                _log.SError(this.GetType().Name, ex.Message, ex.StackTrace, ex.Message);
            }

        }
        private void DeleteFile(tblAdMgtInfo info)
        {
            try
            {
                var ext = Path.GetExtension(info.AttachFilePath);
                if (!string.IsNullOrWhiteSpace(info.LocalPath))
                {
                    File.Delete(info.LocalPath);
                }
            }
            catch (Exception ex)
            {
                _log.SError(this.GetType().Name, ex.Message, ex.StackTrace, ex.Message);
            }

        }
        private void AdMgtSynchronized(object sender, EventArgs<int> e)
        {
            try
            {
                var req = new RequestInfo() { TypeCode = "CheckSynchronized", Data = _tblAdMgtInfos, GroupCode = ConfigClass.StoreNo };
                _tblAdMgtService.RequestSynchronized(0, req).ContinueWith(x =>
                {
                    if (!x.IsFaulted)
                    {
                        if (x.Result.Status)
                        {
                            var list = JsonHelper.JsonToListInfo<tblAdMgtInfo>(string.Empty + x.Result.Data);
                            if (list != null && list.Any())
                            {
                                foreach (var item in list)
                                {
                                    string fileName = item.AdName;
                                    string directoryPath = Helpers.fullPathMainForm + @"AdMgt\";

                                    string[] filePaths = Directory.GetFiles(directoryPath, fileName + ".*", SearchOption.TopDirectoryOnly);

                                    if (filePaths.Length > 0)
                                    {
                                        File.Delete(filePaths[0]);
                                    }
                                    var info = _tblAdMgtInfos.FirstOrDefault(f => f.AdNo == item.AdNo);
                                    if (info != null)
                                    {
                                        info.Copy(item);
                                        WirteFile(info);
                                        _tblAdMgtInfos.Update(info);
                                        _tblAdMgtService.Set_UpdateCompleted(item);
                                    }
                                    else
                                    {
                                        WirteFile(item);
                                        _tblAdMgtInfos.Add(item);
                                        _tblAdMgtService.Set_AddCompleted(item);
                                    }
                                }
                                _xml.WriteXmlList<tblAdMgtInfo>(_tblAdMgtInfos.Cast<tblAdMgtInfo>().ToList(), @"\Setting\AdMgtInfos.xml");
                            }
                        }
                        // to do
                    }
                });
            }
            catch (Exception ex)
            {
                _log.SError(this.GetType().Name, ex.Message, ex.StackTrace, ex.Message);
            }
        }
        private void AdMgt_Added(object sender, EventArgs<tblAdMgtInfo> e)
        {
            try
            {
                string fileName = e.Data.AdName;
                string directoryPath = Helpers.fullPathMainForm + @"AdMgt\";

                string[] filePaths = Directory.GetFiles(directoryPath, fileName + ".*", SearchOption.TopDirectoryOnly);

                if (filePaths.Length > 0)
                {
                    File.Delete(filePaths[0]);
                }

                if (e.Data == null)
                    return;
                var info = _tblAdMgtInfos.FirstOrDefault(t => t.AdNo == e.Data.AdNo);
                if (info != null)
                {
                    info.Copy(e.Data);
                    WirteFile(info);
                    _tblAdMgtInfos.Add(info);
                    _xml.WriteXmlList<tblAdMgtInfo>(_tblAdMgtInfos.Cast<tblAdMgtInfo>().ToList(), @"\Setting\AdMgtInfos.xml");
                }
                else
                {
                    WirteFile(e.Data);
                    _tblAdMgtInfos.Add(e.Data);
                    _xml.WriteXmlList<tblAdMgtInfo>(_tblAdMgtInfos.Cast<tblAdMgtInfo>().ToList(), @"\Setting\AdMgtInfos.xml");
                }
                _tblAdMgtService.Set_AddCompleted(e.Data);
            }
            catch (Exception ex)
            {
                _log.SError(this.GetType().Name, ex.Message, ex.StackTrace, ex.Message);
            }
        }
        private void AdMgt_Updated(object sender, EventArgs<tblAdMgtInfo> e)
        {
            try
            {
                if (e.Data != null)
                {
                    string fileName = e.Data.AdName;
                    string directoryPath = Helpers.fullPathMainForm + @"AdMgt\";

                    string[] filePaths = Directory.GetFiles(directoryPath, fileName + ".*", SearchOption.TopDirectoryOnly);

                    if (filePaths.Length > 0)
                    {
                        File.Delete(filePaths[0]);
                    }
                    var info = _tblAdMgtInfos.FirstOrDefault(t => t.AdNo == e.Data.AdNo);
                    if (info != null)
                    {
                        info.Copy(e.Data);
                        WirteFile(info);
                        _tblAdMgtInfos.Add(info);
                        _xml.WriteXmlList<tblAdMgtInfo>(_tblAdMgtInfos.Cast<tblAdMgtInfo>().ToList(), @"\Setting\AdMgtInfos.xml");
                    }
                    else
                    {
                        WirteFile(e.Data);
                        _tblAdMgtInfos.Add(e.Data);
                        _xml.WriteXmlList<tblAdMgtInfo>(_tblAdMgtInfos.Cast<tblAdMgtInfo>().ToList(), @"\Setting\AdMgtInfos.xml");
                    }
                    _tblAdMgtService.Set_UpdateCompleted(e.Data);
                }
            }
            catch (Exception ex)
            {
                _log.SError(this.GetType().Name, ex.Message, ex.StackTrace, ex.Message);
            }
        }
        private void AdMgt_Removed(object sender, EventArgs<object> e)
        {
            try
            {
                if (e.Data != null)
                {
                    var info = _tblAdMgtInfos.FirstOrDefault(t => t.AdNo == Helpers.OToInt(e.Data));
                    if (info != null)
                    {
                        _tblAdMgtInfos.Remove(info);
                        DeleteFile(info);
                        _xml.WriteXmlList<tblAdMgtInfo>(_tblAdMgtInfos.Cast<tblAdMgtInfo>().ToList(), @"\Setting\AdMgtInfos.xml");
                    }
                    _tblAdMgtService.Set_RemoveCompleted(e.Data);
                }
            }
            catch (Exception ex)
            {
                _log.SError(this.GetType().Name, ex.Message, ex.StackTrace, ex.Message);
            }
        }

        private async Task GetStoreNoAsync()
        {
            try
            {
                var deviceKey = Helpers.GetPhysicalAddress();
                var _requestInfo = new RequestInfo()
                {
                    Top = 9999,
                    Owner = SessionDatas.GetOwner(),
                    UserName = SessionDatas.GetLoginUser(),
                    UserID = "" + SessionDatas.GetUserID(),
                    BranchID = SessionDatas.GetBranchID(),
                    BranchCode = SessionDatas.GetBranchCode(),
                    TSQL = "",
                    ModelName = "",
                    GroupCode = deviceKey,
                };

                var result = await ApiMethod.RequestPostAsync(SSignature.tblStoreDeviceService, FunctionCode.RequestGetDataIsActivityByGroupCode, _requestInfo);

                if (result.Status)
                {
                    ConfigClass.PublicIp = Helpers.GetPublicIp();
                    _log.Info("test" + ConfigClass.PublicIp);
                    List<tblStoreDeviceInfo> storeData = JsonHelper.JsonToListInfo<tblStoreDeviceInfo>(string.Empty + result.Data);
                    if (storeData != null && storeData.Any())
                    {
                        var storeNo = storeData.FirstOrDefault(x => x.DeviceKeyNo == deviceKey && (string.Empty + x.DeviceType).ToUpper() == Constants.DeviceType && x.DevicePublicIP.Trim() == ConfigClass.PublicIp.Trim());

                        ConfigClass.StoreNo = storeNo?.StoreNo ?? 0;
                        ConfigClass.StoreDeviceNo = storeNo?.StoreDeviceNo ?? 0;
                        ConfigClass.DeviceKey = Helpers.GetPhysicalAddress();
                        _log.Info("test" + ConfigClass.StoreNo);

                        // synchronous ad
                        //var _adMgtClient = _tblAdMgtService.RegisterClient(_tblAdMgtService.GetType().Name, AdMgtSynchronized);
                        //_tblAdMgtService.SetAddedListener(_adMgtClient, AdMgt_Added);
                        //_tblAdMgtService.SetUpdatedListener(_adMgtClient, AdMgt_Updated);
                        //_tblAdMgtService.SetRemovedListener(_adMgtClient, AdMgt_Removed);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.SError(this.GetType().Name, ex.Message, ex.StackTrace, ex.Message);
            }
        }

        private async void GetCommonSub()
        {
            tblCommonSubInfo info = new tblCommonSubInfo();
            Datas data = new Datas
            {
                Data = info
            };

            var dataSend = new DataRequest()
            {
                Signature = 116,
                DataLength = 0,
                FrameID = 0,
                FunctionCode = 12292,
                Data = data
            };
            var data2 = await ApiMethod.PostCall(dataSend);
            RequestInfo test = JsonConvert.DeserializeObject<RequestInfo>(data2.Content.ReadAsStringAsync().Result);

            List<tblCommonSubInfo> commonSub = Helpers.ConvertObjectToListModel<tblCommonSubInfo>(test.Data);

            tblCommonSubInfo RejectCode = commonSub.FirstOrDefault(x => x.CommonSubCode == Constants.RejectCode);
            tblCommonSubInfo ApproveCode = commonSub.FirstOrDefault(x => x.CommonSubCode == Constants.ApproveCode);

            if (data2.StatusCode == HttpStatusCode.OK)
            {
                ConfigClass.RejectMessage = RejectCode.CommonSubName1;
                ConfigClass.ApproveMessage = ApproveCode.CommonSubName1;
            }
        }
        #endregion

        //---------------------------------------------------------------

        #region Property
        public ILoginService loginService { get { return _loginService; } }
        public ICacheDataService<tblClientSoundMgtInfo> tblClientSoundMgtService { get { return _tblClientSoundMgtService; } }
        public tblAdMgtService tblAdMgtService { get { return _tblAdMgtService; } }
        public ICacheDataService<tblAdMgtInfo> tblAdMgtServiceInfo { get { return _tblAdMgtService; } }
        public ICacheDataService<tblDeviceRequestConnectInfo> tblDeviceRequestConnectService => _tblDeviceRequestConnectService;
        public ICacheDataService<tblUserInfo> tblUserService { get { return _tblUserService; } }
        public ICacheDataService<tblUserPhotoInfo> tblUserPhotoService { get { return _tblUserPhotoService; } }
        public ICacheDataService<tblStoreDeviceInfo> tblStoreDeviceService { get { return _tblStoreDeviceService; } }
        public ICacheDataService<tblUserMgtStoreInfo> tblUserMgtStoreService { get { return _tblUserMgtStoreService; } }
        public ICacheDataService<tblStoreEnvironmentSettingInfo> tblStoreEnvironmentSettingService { get { return _tblStoreEnvironmentSettingService; } }
        public ICacheDataService<tblStoreMasterInfo> tblStoreMasterService { get { return _tblStoreMasterService; } }
        public IList<tblClientSoundMgtInfo> tblClientSoundMgtInfos => _listSetting;
        #endregion

        //---------------------------------------------------------------
    }
}
