#define NOUNITY // COMMENT IF UNITY
#define SYNCUDP // COMMENT IF TCP ONLY
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Linq;
using Nettention.Proud;
#if (!NOUNITY)
using UnityEngine;
#else
using System.Diagnostics;
#endif

// MSB Network Module
// 191101
// ReSharper disable once CheckNamespace
namespace MSBNetwork
{
    /// <summary>
    /// User Data Class
    /// Use for only network encrypt, decrypt
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class UserData
    {
        public int userNumber;
        public string userID;
        public string userNick;
        public int userRank;
        public int userMoney;
        public int userCash;
        public int userWeapon;
        public int userSkin;

        public UserData(int _userNUM, string _userID, string _userNICK, int _userRank, int _userWeapon, int _userSkin)
        {
            userNumber = _userNUM;
            userID = _userID;
            userNick = _userNICK;
            userRank = _userRank;
            userWeapon = _userWeapon;
            userSkin = _userSkin;
        }

        public UserData(int _userNUM, string _userID, string _userNICK, int _userRank, int _userMoney, int _userCash, int _userWeapon, int _userSkin)
        {
            userNumber = _userNUM;
            userID = _userID;
            userNick = _userNICK;
            userRank = _userRank;
            userMoney = _userMoney;
            userCash = _userCash;
            userWeapon = _userWeapon;
            userSkin = _userSkin;
        }
    }

    /// <summary>
    /// Game Status Sync
    /// Server -> Client
    /// </summary>
    internal static class OnGameInfo
    {
        public const string TAG_USER_NUM = "NUM";
        public const string TAG_USER_ID = "ID";
        public const string TAG_USER_NICK = "NICK";
        public const string TAG_USER_RANK = "RANK";
        public const string TAG_USER_WEAPON = "WEAPON";
        public const string TAG_USER_SKIN = "SKIN";
    }

    /// <summary>
    /// Network Module
    /// Singleton
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class NetworkModule
    {
        private static NetworkModule INSTANCE;

        public interface OnServerConnectListener
        {
            /// <summary>
            /// 서버 접속 결과를 받습니다
            /// </summary>
            /// <param name="result">접속 성공여부</param>
            /// <param name="message">이벤트 메시지</param>
            void OnServerConnection(bool result, string message);
        }

        public interface OnLoginResultListener
        {
            /// <summary>
            /// 서버에서 로그인 결과를 받습니다
            /// </summary>
            /// <param name="_result">로그인 성공여부</param>
            /// <param name="_user">로그인한 유저 데이터</param>
            /// <param name="_game">진행중이었던 게임방 번호 or -1</param>
            /// <param name="_message">로그인 메시지</param>
            void OnLoginResult(bool _result, UserData _user, int _game, string _message);
        }

        public interface OnStatusResultListener
        {
            /// <summary>
            /// 서버에서 회원정보 결과를 받습니다
            /// </summary>
            /// <param name="_result">회원정보 성공여부</param>
            /// <param name="_user">회원정보 유저 데이터</param>
            /// <param name="_game">진행중인 게임방 번호 or -1</param>
            /// <param name="_message">회원정보 메시지</param>
            void OnStatusResult(bool _result, UserData _user, int _game, string _message);
        }
        
        public interface OnSystemResultListener
        {
            /// <summary>
            /// 서버에서 시스템 데이터를 받습니다
            /// </summary>
            /// <param name="_result">시스템 성공여부</param>
            void OnSystemResult(bool _result);
        }

        public interface OnGameMatchedListener
        {
            /// <summary>
            /// 서버에서 게임 매칭이 되었을 때 이벤트를 받습니다
            /// </summary>
            /// <param name="_result">게임 매칭 여부</param>
            /// <param name="_room">게임방 인덱스</param>
            /// <param name="_message">게임 매칭 메시지</param>
            void OnGameMatched(bool _result, int _room, string _message);
        }

        public interface OnGameUserMoveListener
        {
            /// <summary>
            /// 서버에서 같은 게임방 내 유저의 이동 이벤트를 받습니다
            /// </summary>
            /// <param name="_data">이벤트 데이터</param>
            void OnGameUserMove(object _data);
        }

        public interface OnGameUserSyncListener
        {
            /// <summary>
            /// 서버에서 같은 게임방 내 유저의 싱크 이벤트를 받습니다
            /// </summary>
            /// <param name="_data">이벤트 데이터</param>
            void OnGameUserSync(object _data);
        }

        public interface OnGameInfoListener
        {
            /// <summary>
            /// 서버에서 게임방에 대한 기본 정보를 받습니다
            /// </summary>
            /// <param name="_result">팀전 매칭 여부</param>
            /// <param name="_room">게임방 인덱스</param>
            /// <param name="_mode">솔로(0) / 팀전(1) 여부</param>
            /// <param name="_users">게임방 참여 유저정보</param>
            /// <param name="_message">정보 메시지</param>
            void OnGameInfo(bool _result, int _room, int _mode, LinkedList<UserData> _users, string _message);
        }

        public interface OnGameStatusListener
        {
            /// <summary>
            /// 서버에서 게임방에 대한 카운트다운 이벤트를 받습니다
            /// </summary>
            /// <param name="count">카운트다운 초</param>
            void OnGameEventCount(int count);

            /// <summary>
            /// 서버에서 게임방에 대한 시간 이벤트를 받습니다
            /// </summary>
            /// <param name="time">시간 초</param>
            void OnGameEventTime(int time);

            /// <summary>
            /// 서버에서 게임방에 대한 레디 이벤트를 받습니다
            /// </summary>
            /// <param name="readyData">레디 데이터</param>
            void OnGameEventReady(string readyData);

            /// <summary>
            /// 서버에서 게임방에 대한 점수 이벤트를 받습니다
            /// </summary>
            /// <param name="blueKill">블루 킬</param>
            /// <param name="blueDeath">블루 데스</param>
            /// <param name="bluePoint">블루 점수</param>
            /// <param name="redKill">레드 킬</param>
            /// <param name="redDeath">레드 데스</param>
            /// <param name="redPoint">레드 점수</param>
            void OnGameEventScore(int blueKill, int blueDeath, int bluePoint, int redKill, int redDeath, int redPoint);

            /// <summary>
            /// 서버에서 게임방에 대한 메시지 이벤트를 받습니다
            /// </summary>
            /// <param name="type">메시지 종류</param>
            /// <param name="message">메시지 데이터</param>
            void OnGameEventMessage(int type, string message);
        }

        public interface OnGameEventListener
        {
            /// <summary>
            /// 서버에서 게임방에 대한 체력 이벤트를 받습니다
            /// </summary>
            /// <param name="num">대상 유저번호</param>
            /// <param name="health">대상 현재 체력</param>
            void OnGameEventHealth(int num, int health);

            /// <summary>
            /// 서버에서 게임방에 대한 데미지 이벤트를 받습니다
            /// </summary>
            /// <param name="from">데미지를 가한 유저</param>
            /// <param name="to">데미지를 받은 유저</param>
            /// <param name="amount">데미지량</param>
            /// <param name="option">기타 데이터</param>
            void OnGameEventDamage(int from, int to, int amount, string option);

            /// <summary>
            /// 서버에서 게임방에 대한 오브젝트 이벤트를 받습니다
            /// </summary>
            /// <param name="num">대상 오브젝트번호</param>
            /// <param name="health">대상 현재 체력</param>
            void OnGameEventObject(int num, int health);

            /// <summary>
            /// 서버에서 게임방에 대한 아이템 이벤트를 받습니다
            /// </summary>
            /// <param name="type">아이템 타입 (0:스코어 / 1:힐팩)</param>
            /// <param name="num">대상 아이템번호</param>
            /// <param name="action">0:스폰 / 유저번호</param>
            void OnGameEventItem(int type, int num, int action);

            /// <summary>
            /// 서버에서 게임방에 대한 처치 이벤트를 받습니다
            /// </summary>
            /// <param name="from">처치자</param>
            /// <param name="to">사망자</param>
            /// <param name="option">기타 데이터</param>
            void OnGameEventKill(int from, int to, string option);

            /// <summary>
            /// 서버에서 게임방에 대한 리스폰 이벤트를 받습니다
            /// </summary>
            /// <param name="num">대상 유저번호</param>
            /// <param name="time">부활 카운트다운</param>
            void OnGameEventRespawn(int num, int time);
        }

        public interface OnGameResultListener
        {
            /// <summary>
            /// 서버에서 게임 결과 이벤트를 받습니다
            /// </summary>
            /// <param name="data">이벤트 데이터</param>
            void OnGameResult(string data);
        }

        private static List<OnServerConnectListener> onServerConnectListeners;
        private static List<OnLoginResultListener> onLoginResultListeners;
        private static List<OnStatusResultListener> onStatusResultListeners;
        private static List<OnSystemResultListener> onSystemResultListeners;
        private static List<OnGameMatchedListener> onGameMatchedListeners;
        private static List<OnGameUserMoveListener> onGameUserMoveListeners;
        private static List<OnGameUserSyncListener> onGameUserSyncListeners;
        private static List<OnGameInfoListener> onGameInfoListeners;
        private static List<OnGameStatusListener> onGameStatusListeners;
        private static List<OnGameEventListener> onGameEventListeners;
        private static List<OnGameResultListener> onGameResultListeners;

        private NetworkModule()
        {
            onServerConnectListeners = new List<OnServerConnectListener>();
            onLoginResultListeners = new List<OnLoginResultListener>();
            onStatusResultListeners = new List<OnStatusResultListener>();
            onSystemResultListeners = new List<OnSystemResultListener>();
            onGameMatchedListeners = new List<OnGameMatchedListener>();
            onGameUserMoveListeners = new List<OnGameUserMoveListener>();
            onGameUserSyncListeners = new List<OnGameUserSyncListener>();
            onGameInfoListeners = new List<OnGameInfoListener>();
            onGameStatusListeners = new List<OnGameStatusListener>();
            onGameEventListeners = new List<OnGameEventListener>();
            onGameResultListeners = new List<OnGameResultListener>();
        }

        public static NetworkModule GetInstance()
        {
            return INSTANCE ??= new NetworkModule();
        }

        private const string DEFAULT_SERVER_IP = "127.0.0.1";
        private const int DEFAULT_SERVER_PORT = 8888;
        private static string SERVER_IP;
        private static int SERVER_PORT;
        private System.Guid guidVersion = new System.Guid("{0x27ad1634,0x381e,0x4228,{0x98,0xa,0xda,0xc8,0xeb,0x5f,0x4e,0x83}}");

        NetClient networkClient = new NetClient();

        internal MSBC2S.Proxy netC2SProxy = new MSBC2S.Proxy();
        internal MSBS2C.Stub netS2CStub = new MSBS2C.Stub();

        /// <summary>
        /// 서버에 접속합니다
        /// </summary>
        /// <param name="_ip">(선택) 서버 IP 주소</param>
        /// <param name="_port">(선택) 서버 PORT</param>
        public void Connect(string _ip = DEFAULT_SERVER_IP, int _port = DEFAULT_SERVER_PORT)
        {
            SERVER_IP = _ip;
            SERVER_PORT = _port;

            NetConnectionParam netConnection = new NetConnectionParam();
            netConnection.serverIP = SERVER_IP;
            netConnection.serverPort = (ushort) SERVER_PORT;
            netConnection.protocolVersion = new Nettention.Proud.Guid(guidVersion);

            networkClient.AttachProxy(netC2SProxy);
            networkClient.AttachStub(netS2CStub);

            networkClient.JoinServerCompleteHandler = (errorInfo, reply) =>
            {
                if (errorInfo.errorType == ErrorType.Ok)
                {
                    if (onServerConnectListeners != null && onServerConnectListeners.Count > 0)
                    {
                        foreach (OnServerConnectListener listener in onServerConnectListeners)
                        {
                            if (listener == null) continue;
                            listener.OnServerConnection(true, string.Empty);
                        }
                    }
                }
                else
                {
                    if (onServerConnectListeners != null && onServerConnectListeners.Count > 0)
                    {
                        foreach (OnServerConnectListener listener in onServerConnectListeners)
                        {
                            if (listener == null) continue;
                            listener.OnServerConnection(false, errorInfo.ToString());
                        }
                    }
                }
            };

            networkClient.LeaveServerHandler = errorInfo =>
            {
                if (onServerConnectListeners != null && onServerConnectListeners.Count > 0)
                {
                    foreach (OnServerConnectListener listener in onServerConnectListeners)
                    {
                        if (listener == null) continue;
                        listener.OnServerConnection(false, errorInfo.ToString());
                    }
                }
            };

            InitializeListeners();

            networkClient.Connect(netConnection);
        }

        /// <summary>
        /// 접속을 종료합니다
        /// 통신 스레드를 종료합니다
        /// </summary>
        public void Disconnect()
        {
            networkClient.Dispose();
        }

        public void OnUpdate()
        {
            networkClient.FrameMove();
        }

        /// <summary>
        /// 서버로부터의 이벤트 수신을 처리합니다.
        /// </summary>
        private void InitializeListeners()
        {
            netS2CStub.OnLoginResult = OnEventUserLogin;
            netS2CStub.OnStatusResult = OnEventUserStatus;
            netS2CStub.OnSystemResult = OnEventSystem;
            netS2CStub.OnGameMatched = OnEventGameQueue;
            netS2CStub.OnGameInfo = OnEventGameInfo;
            netS2CStub.OnGameStatusCountdown = OnEventGameStatusCountdown;
            netS2CStub.OnGameStatusTime = OnEventGameStatusTime;
            netS2CStub.OnGameStatusReady = OnEventGameStatusReady;
            netS2CStub.OnGameStatusScore = OnEventGameStatusScore;
            netS2CStub.OnGameStatusMessage = OnEventGameStatusMessage;
            netS2CStub.OnGameEventHealth = OnEventGameEventHealth;
            netS2CStub.OnGameEventDamage = OnEventGameEventDamage;
            netS2CStub.OnGameEventObject = OnEventGameEventObject;
            netS2CStub.OnGameEventItem = OnEventGameEventItem;
            netS2CStub.OnGameEventKill = OnEventGameEventKill;
            netS2CStub.OnGameEventRespawn = OnEventGameEventRespawn;
            netS2CStub.OnGameResult = OnEventGameResult;
            netS2CStub.OnGameUserMove = OnEventGameUserMove;
            netS2CStub.OnGameUserSync = OnEventGameUserSync;
        }

        /// <summary>
        /// 서버에 Login 요청을 전송합니다
        /// 등록된 OnLoginResultListener 에 서버 응답이 수신됩니다
        /// </summary>
        /// <param name="_id">유저 아이디</param>
        /// <param name="_pw">유저 비밀번호</param>
        public void RequestUserLogin(string _id, string _pw)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("RequestUserLogin");
#else
                Debug.WriteLine("RequestUserLogin");
#endif
                JObject data = new JObject {{"id", _id}, {"pw", _pw}};
                netC2SProxy.OnLoginRequest(HostID.HostID_Server, RmiContext.ReliableSend, data.ToString());
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("RequestUserLogin ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("RequestUserLogin ERROR");
                Debug.WriteLine(e);
#endif
            }
        }

        /// <summary>
        /// 서버에 Status 요청을 전송합니다
        /// 등록된 OnStatusResultListener 에 서버 응답이 수신됩니다
        /// </summary>
        /// <param name="_id">유저 아이디</param>
        public void RequestUserStatus(string _id)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("RequestUserStatus");
#else
                Debug.WriteLine("RequestUserStatus");
#endif
                JObject data = new JObject {{"id", _id}};
                netC2SProxy.OnStatusRequest(HostID.HostID_Server, RmiContext.ReliableSend, data.ToString());
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("RequestUserStatus ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("RequestUserStatus ERROR");
                Debug.WriteLine(e);
#endif
            }
        }
        
        /// <summary>
        /// 서버에 System 요청을 전송합니다
        /// 등록된 OnSystemResultListener 에 서버 응답이 수신됩니다
        /// </summary>
        /// <param name="_id">유저 아이디</param>
        public void RequestUserSystem(string _id)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("RequestUserSystem");
#else
                Debug.WriteLine("RequestUserSystem");
#endif
                JObject data = new JObject {{"id", _id}};
                netC2SProxy.OnSystemRequest(HostID.HostID_Server, RmiContext.ReliableSend, data.ToString());
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("RequestUserSystem ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("RequestUserSystem ERROR");
                Debug.WriteLine(e);
#endif
            }
        }

        /// <summary>
        /// 서버에 Game Queue 요청을 전송합니다
        /// 등록된 OnGameMatchedListener 에 서버 응답이 수신됩니다
        /// </summary>
        /// <param name="_weapon">유저 선택 무기</param>
        /// <param name="_skin">유저 선택 스킨</param>
        public void RequestGameQueue(int _weapon, int _skin)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("RequestGameQueue");
#else
                Debug.WriteLine("RequestGameQueue");
#endif
                JObject data = new JObject {{"weapon", _weapon}, {"skin", _skin}};
                netC2SProxy.OnGameQueueRequest(HostID.HostID_Server, RmiContext.ReliableSend, data.ToString());
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("RequestGameQueue ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("RequestGameQueue ERROR");
                Debug.WriteLine(e);
#endif
            }
        }

        /// <summary>
        /// 서버에 Game Info 요청을 전송합니다
        /// 등록된 OnGameInfoListener 에 서버로부터 게임방 데이터가 수신됩니다
        /// </summary>
        /// <param name="_gameRoom">게임방 인덱스</param>
        public void RequestGameInfo(int _gameRoom)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("RequestGameInfo");
#else
                Debug.WriteLine("RequestGameInfo");
#endif
                netC2SProxy.OnGameInfoRequest(HostID.HostID_Server, RmiContext.ReliableSend, _gameRoom, string.Empty);
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("RequestGameInfo ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("RequestGameInfo ERROR");
                Debug.WriteLine(e);
#endif
            }
        }

        /// <summary>
        /// 서버에 Game Move 이벤트를 전송합니다
        /// 등록된 OnGameMoveListener 에 서버로부터 싱크 데이터가 수신됩니다
        /// </summary>
        /// <param name="_gameRoom">게임방 인덱스</param>
        /// <param name="_data">싱크 데이터</param>
        public void RequestGameUserMove(int _gameRoom, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("RequestGameUserMove");
#else
                Debug.WriteLine("RequestGameUserMove");
#endif
#if (SYNCUDP)
                netC2SProxy.OnGameUserMove(HostID.HostID_Server, RmiContext.UnreliableSend, _gameRoom, _data);
#else
                netC2SProxy.OnGameUserMove(HostID.HostID_Server, RmiContext.ReliableSend, _gameRoom, _data);
#endif
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("RequestGameUserMove ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("RequestGameUserMove ERROR");
                Debug.WriteLine(e);
#endif
            }
        }

        /// <summary>
        /// 서버에 Game Sync 이벤트를 전송합니다
        /// 등록된 OnGameSyncListener 에 서버로부터 싱크 데이터가 수신됩니다
        /// </summary>
        /// <param name="_gameRoom">게임방 인덱스</param>
        /// <param name="_data">싱크 데이터</param>
        public void RequestGameUserSync(int _gameRoom, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("RequestGameUserSync");
#else
                Debug.WriteLine("RequestGameUserSync");
#endif
#if (SYNCUDP)
                netC2SProxy.OnGameUserSync(HostID.HostID_Server, RmiContext.UnreliableSend, _gameRoom, _data);
#else
                netC2SProxy.OnGameUserSync(HostID.HostID_Server, RmiContext.ReliableSend, _gameRoom, _data);
#endif
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("RequestGameUserSync ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("RequestGameUserSync ERROR");
                Debug.WriteLine(e);
#endif
            }
        }

        /// <summary>
        /// 서버에 Game Action Ready 이벤트를 전송합니다
        /// </summary>
        /// <param name="_gameRoom">게임방 인덱스</param>
        public void RequestGameUserActionReady(int _gameRoom)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("RequestGameUserActionReady");
#else
                Debug.WriteLine("RequestGameUserActionReady");
#endif
                netC2SProxy.OnGameActionReady(HostID.HostID_Server, RmiContext.ReliableSend, _gameRoom, string.Empty);
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("RequestGameUserActionReady ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("RequestGameUserActionReady ERROR");
                Debug.WriteLine(e);
#endif
            }
        }

        /// <summary>
        /// 서버에 Game Action Damage 이벤트를 전송합니다
        /// </summary>
        /// <param name="_gameRoom">게임방 인덱스</param>
        /// <param name="_target">대상 인덱스</param>
        /// <param name="_amount">피해량 데이터</param>
        /// <param name="_option">기타 데이터</param>
        public void RequestGameUserActionDamage(int _gameRoom, int _target, int _amount, string _option)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("RequestGameUserActionDamage");
#else
                Debug.WriteLine("RequestGameUserActionDamage");
#endif
                JObject data = new JObject();
                data.Add("target", _target);
                data.Add("amount", _amount);
                data.Add("option", _option);
                netC2SProxy.OnGameActionDamage(HostID.HostID_Server, RmiContext.ReliableSend, _gameRoom, data.ToString());
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("RequestGameUserActionDamage ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("RequestGameUserActionDamage ERROR");
                Debug.WriteLine(e);
#endif
            }
        }

        /// <summary>
        /// 서버에 Game Action Object 이벤트를 전송합니다
        /// </summary>
        /// <param name="_gameRoom">게임방 인덱스</param>
        /// <param name="_target">대상 인덱스</param>
        /// <param name="_amount">피해량 데이터</param>
        public void RequestGameUserActionObject(int _gameRoom, int _target, int _amount)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("RequestGameUserActionObject");
#else
                Debug.WriteLine("RequestGameUserActionObject");
#endif
                JObject data = new JObject();
                data.Add("target", _target);
                data.Add("amount", _amount);
                netC2SProxy.OnGameActionObject(HostID.HostID_Server, RmiContext.ReliableSend, _gameRoom, data.ToString());
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("RequestGameUserActionObject ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("RequestGameUserActionObject ERROR");
                Debug.WriteLine(e);
#endif
            }
        }

        /// <summary>
        /// 서버에 Game Action Item 이벤트를 전송합니다
        /// </summary>
        /// <param name="_gameRoom">게임방 인덱스</param>
        /// <param name="_type">아이템 타입 (0:스코어 / 1:힐팩)</param>
        /// <param name="_target">대상 인덱스</param>
        public void RequestGameUserActionItem(int _gameRoom, int _type, int _target)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("RequestGameUserActionItem");
#else
                Debug.WriteLine("RequestGameUserActionItem");
#endif
                JObject data = new JObject();
                data.Add("type", _type);
                data.Add("target", _target);
                netC2SProxy.OnGameActionItem(HostID.HostID_Server, RmiContext.ReliableSend, _gameRoom, data.ToString());
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("RequestGameUserActionItem ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("RequestGameUserActionItem ERROR");
                Debug.WriteLine(e);
#endif
            }
        }

        private static bool OnEventUserLogin(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventUserLogin");
#else
                Debug.WriteLine("OnEventUserLogin");
#endif
                JObject data = JObject.Parse(_data);
                int result = data.GetValue("result").Value<int>();
                int user_num = data.GetValue("num").Value<int>();
                string user_id = data.GetValue("id").ToString();
                string user_nick = data.GetValue("nick").ToString();
                int user_rank = data.GetValue("rank").Value<int>();
                int user_money = data.GetValue("money").Value<int>();
                int user_cash = data.GetValue("cash").Value<int>();
                int user_weapon = data.GetValue("weapon").Value<int>();
                int user_skin = data.GetValue("skin").Value<int>();
                int game = data.GetValue("game").Value<int>();
                string message = data.GetValue("message").ToString();
                UserData dataUser = new UserData(user_num, user_id, user_nick, user_rank, user_money, user_cash, user_weapon, user_skin);
                if (onLoginResultListeners != null && onLoginResultListeners.Count > 0)
                {
                    foreach (OnLoginResultListener listener in onLoginResultListeners)
                    {
                        listener?.OnLoginResult(result == 1, dataUser, game, message);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventUserLogin ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventUserLogin ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventUserStatus(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventUserStatus");
                Debug.Log(_data);
#else
                Debug.WriteLine("OnEventUserStatus");
#endif
                JObject data = JObject.Parse(_data);
                int result = data.GetValue("result").Value<int>();
                int user_num = data.GetValue("num").Value<int>();
                string user_id = data.GetValue("id").ToString();
                string user_nick = data.GetValue("nick").ToString();
                int user_rank = data.GetValue("rank").Value<int>();
                int user_money = data.GetValue("money").Value<int>();
                int user_cash = data.GetValue("cash").Value<int>();
                int user_weapon = data.GetValue("weapon").Value<int>();
                int user_skin = data.GetValue("skin").Value<int>();
                int game = data.GetValue("game").Value<int>();
                string message = data.GetValue("message").ToString();
                UserData dataUser = new UserData(user_num, user_id, user_nick, user_rank, user_money, user_cash, user_weapon, user_skin);
                if (onStatusResultListeners != null && onStatusResultListeners.Count > 0)
                {
                    foreach (OnStatusResultListener listener in onStatusResultListeners)
                    {
                        listener?.OnStatusResult(result == 1, dataUser, game, message);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventUserStatus ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventUserStatus ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }
        
        private static bool OnEventSystem(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventSystem");
                Debug.Log(_data);
#else
                Debug.WriteLine("OnEventSystem");
#endif
                // TODO
                if (onSystemResultListeners != null && onSystemResultListeners.Count > 0)
                {
                    foreach (OnSystemResultListener listener in onSystemResultListeners)
                    {
                        listener?.OnSystemResult(true);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventSystem ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventSystem ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameQueue(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameQueue");
#else
                Debug.WriteLine("OnEventGameQueue");
#endif
                JObject data = JObject.Parse(_data);
                int result = data.GetValue("result").Value<int>();
                int room = data.GetValue("room").Value<int>();
                string message = data.GetValue("message").ToString();
                if (onGameMatchedListeners != null && onGameMatchedListeners.Count > 0)
                {
                    foreach (OnGameMatchedListener listener in onGameMatchedListeners)
                    {
                        listener?.OnGameMatched(result == 1, room, message);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameQueue ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameQueue ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameInfo(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameInfo");
#else
                Debug.WriteLine("OnEventGameInfo");
#endif
                JObject data = JObject.Parse(_data);
                int result = data.GetValue("result").Value<int>();
                int room = data.GetValue("room").Value<int>();
                int mode = data.GetValue("mode").Value<int>();
                JArray dataUsersRaw = data.GetValue("users").Value<JArray>();
                LinkedList<UserData> dataUsers = new LinkedList<UserData>();
                foreach (var jToken in dataUsersRaw)
                {
                    var dataUserRaw = (JObject) jToken;
                    int dataNum = (int) dataUserRaw.GetValue(OnGameInfo.TAG_USER_NUM);
                    string dataID = (string) dataUserRaw.GetValue(OnGameInfo.TAG_USER_ID);
                    string dataNick = (string) dataUserRaw.GetValue(OnGameInfo.TAG_USER_NICK);
                    int dataRank = (int) dataUserRaw.GetValue(OnGameInfo.TAG_USER_RANK);
                    int dataWeapon = (int) dataUserRaw.GetValue(OnGameInfo.TAG_USER_WEAPON);
                    int dataSkin = (int) dataUserRaw.GetValue(OnGameInfo.TAG_USER_SKIN);
                    UserData dataUser = new UserData(dataNum, dataID, dataNick, dataRank, dataWeapon, dataSkin);
                    dataUsers.AddLast(dataUser);
                }
                if (onGameInfoListeners != null && onGameInfoListeners.Count > 0)
                {
                    foreach (OnGameInfoListener listener in onGameInfoListeners)
                    {
                        listener?.OnGameInfo(result == 1, room, mode, dataUsers, string.Empty);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameInfo ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameInfo ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameStatusCountdown(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameStatusCountdown");
#else
                Debug.WriteLine("OnEventGameStatusCountdown");
#endif
                JObject data = JObject.Parse(_data);
                int count = data.GetValue("count").Value<int>();
                if (onGameStatusListeners != null && onGameStatusListeners.Count > 0)
                {
                    foreach (OnGameStatusListener listener in onGameStatusListeners)
                    {
                        listener?.OnGameEventCount(count);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameStatusCountdown ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameStatusCountdown ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameStatusTime(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameStatusTime");
#else
                Debug.WriteLine("OnEventGameStatusTime");
#endif
                JObject data = JObject.Parse(_data);
                int time = data.GetValue("time").Value<int>();
                if (onGameStatusListeners != null && onGameStatusListeners.Count > 0)
                {
                    foreach (OnGameStatusListener listener in onGameStatusListeners)
                    {
                        if (listener == null) continue;
                        listener.OnGameEventTime(time);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameStatusTime ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameStatusTime ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameStatusReady(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameStatusReady");
#else
                Debug.WriteLine("OnEventGameStatusReady");
#endif
                JObject data = JObject.Parse(_data);
                string readyData = data.GetValue("readyData").ToString();
                if (onGameStatusListeners != null && onGameStatusListeners.Count > 0)
                {
                    foreach (OnGameStatusListener listener in onGameStatusListeners)
                    {
                        listener?.OnGameEventReady(readyData);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameStatusReady ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameStatusReady ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameStatusScore(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameStatusScore");
#else
                Debug.WriteLine("OnEventGameStatusScore");
#endif
                JObject data = JObject.Parse(_data);
                int blueKill = data.GetValue("blueKill").Value<int>();
                int blueDeath = data.GetValue("blueDeath").Value<int>();
                int bluePoint = data.GetValue("bluePoint").Value<int>();
                int redKill = data.GetValue("redKill").Value<int>();
                int redDeath = data.GetValue("redDeath").Value<int>();
                int redPoint = data.GetValue("redPoint").Value<int>();
                if (onGameStatusListeners != null && onGameStatusListeners.Count > 0)
                {
                    foreach (OnGameStatusListener listener in onGameStatusListeners)
                    {
                        listener?.OnGameEventScore(blueKill, blueDeath, bluePoint, redKill, redDeath, redPoint);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameStatusScore ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameStatusScore ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameStatusMessage(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameStatus");
#else
                Debug.WriteLine("OnEventGameStatus");
#endif
                JObject data = JObject.Parse(_data);
                int type = data.GetValue("type").Value<int>();
                string message = data.GetValue("message").ToString();
                if (onGameStatusListeners != null && onGameStatusListeners.Count > 0)
                {
                    foreach (OnGameStatusListener listener in onGameStatusListeners)
                    {
                        listener?.OnGameEventMessage(type, message);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameStatus ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameStatus ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameEventHealth(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameEventHealth");
#else
                Debug.WriteLine("OnEventGameEventHealth");
#endif
                JObject data = JObject.Parse(_data);
                int num = data.GetValue("num").Value<int>();
                int health = data.GetValue("health").Value<int>();
                if (onGameEventListeners != null && onGameEventListeners.Count > 0)
                {
                    foreach (OnGameEventListener listener in onGameEventListeners)
                    {
                        listener?.OnGameEventHealth(num,health);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameEventHealth ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameEventHealth ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameEventDamage(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameEventDamage");
#else
                Debug.WriteLine("OnEventGameEventDamage");
#endif
                JObject data = JObject.Parse(_data);
                int from = data.GetValue("from").Value<int>();
                int to = data.GetValue("to").Value<int>();
                int amount = data.GetValue("amount").Value<int>();
                string option = data.GetValue("option").ToString();
                if (onGameEventListeners != null && onGameEventListeners.Count > 0)
                {
                    foreach (OnGameEventListener listener in onGameEventListeners)
                    {
                        listener?.OnGameEventDamage(@from, to, amount, option);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameEventDamage ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameEventDamage ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameEventObject(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameEventObject");
#else
                Debug.WriteLine("OnEventGameEventObject");
#endif
                JObject data = JObject.Parse(_data);
                int num = data.GetValue("num").Value<int>();
                int health = data.GetValue("health").Value<int>();
                if (onGameEventListeners != null && onGameEventListeners.Count > 0)
                {
                    foreach (OnGameEventListener listener in onGameEventListeners)
                    {
                        listener?.OnGameEventObject(num, health);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameEventObject ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameEventObject ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameEventItem(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameEventItem");
#else
                Debug.WriteLine("OnEventGameEventItem");
#endif
                JObject data = JObject.Parse(_data);
                int type = data.GetValue("type").Value<int>();
                int num = data.GetValue("num").Value<int>();
                int action = data.GetValue("action").Value<int>();
                if (onGameEventListeners != null && onGameEventListeners.Count > 0)
                {
                    foreach (OnGameEventListener listener in onGameEventListeners)
                    {
                        listener?.OnGameEventItem(type, num, action);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameEventItem ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameEventItem ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameEventKill(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameEventKill");
#else
                Debug.WriteLine("OnEventGameEventKill");
#endif
                JObject data = JObject.Parse(_data);
                int from = data.GetValue("from").Value<int>();
                int to = data.GetValue("to").Value<int>();
                string option = data.GetValue("option").ToString();
                if (onGameEventListeners != null && onGameEventListeners.Count > 0)
                {
                    foreach (OnGameEventListener listener in onGameEventListeners)
                    {
                        listener?.OnGameEventKill(@from, to, option);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameEventKill ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameEventKill ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameEventRespawn(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameEventRespawn");
#else
                Debug.WriteLine("OnEventGameEventRespawn");
#endif
                JObject data = JObject.Parse(_data);
                int num = data.GetValue("num").Value<int>();
                int time = data.GetValue("time").Value<int>();
                if (onGameEventListeners != null && onGameEventListeners.Count > 0)
                {
                    foreach (OnGameEventListener listener in onGameEventListeners)
                    {
                        listener?.OnGameEventRespawn(num, time);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameEventRespawn ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameEventRespawn ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameResult(HostID remote, RmiContext rmiContext, string _data)
        {
            try
            {
#if (!NOUNITY)
                Debug.Log("OnEventGameResult");
#else
                Debug.WriteLine("OnEventGameResult");
#endif
                JObject data = JObject.Parse(_data);
                string resultData = data.GetValue("resultData").ToString();
                if (onGameResultListeners != null && onGameResultListeners.Count > 0)
                {
                    foreach (OnGameResultListener listener in onGameResultListeners)
                    {
                        listener?.OnGameResult(resultData);
                    }
                }
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameResult ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameResult ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameUserMove(HostID remote, RmiContext rmiContext, string data)
        {
            try
            {
                if (onGameUserMoveListeners != null && onGameUserMoveListeners.Count > 0)
                {
                    foreach (OnGameUserMoveListener listener in onGameUserMoveListeners)
                    {
                        listener?.OnGameUserMove(data);
                    }
                }
#if (!NOUNITY)
                Debug.Log("OnEventGameUserMove");
#else
                Debug.WriteLine("OnEventGameUserMove");
#endif
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameUserMove ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameUserMove ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        private static bool OnEventGameUserSync(HostID remote, RmiContext rmiContext, string data)
        {
            try
            {
                if (onGameUserSyncListeners != null && onGameUserSyncListeners.Count > 0)
                {
                    foreach (OnGameUserSyncListener listener in onGameUserSyncListeners)
                    {
                        listener?.OnGameUserSync(data);
                    }
                }
#if (!NOUNITY)
                Debug.Log("OnEventGameUserSync");
#else
                Debug.WriteLine("OnEventGameUserSync");
#endif
            }
            catch (Exception e)
            {
#if (!NOUNITY)
                Debug.LogError("OnEventGameUserSync ERROR");
                Debug.LogError(e);
#else
                Debug.WriteLine("OnEventGameUserSync ERROR");
                Debug.WriteLine(e);
#endif
            }
            return true;
        }

        /// <summary>
        /// 해당 OnServerConnectListener 를 유일한 콜백으로 등록합니다
        /// </summary>
        /// <param name="_listener">OnServerConnectListener implemented class</param>
        public static void SetOnEventServerConnect(OnServerConnectListener _listener)
        {
            onServerConnectListeners.Clear();
            AddOnEventServerConnect(_listener);
        }

        /// <summary>
        /// 해당 OnServerConnectListener 를 콜백 목록에 등록합니다
        /// </summary>
        /// <param name="_listener">OnServerConnectListener implemented class</param>
        public static void AddOnEventServerConnect(OnServerConnectListener _listener)
        {
            onServerConnectListeners.Add(_listener);
        }

        /// <summary>
        /// 해당 OnLoginResultListener 를 유일한 콜백으로 등록합니다
        /// </summary>
        /// <param name="_listener">OnLoginResultListener implemented class</param>
        public static void SetOnEventUserLogin(OnLoginResultListener _listener)
        {
            onLoginResultListeners.Clear();
            AddOnEventUserLogin(_listener);
        }

        /// <summary>
        /// 해당 OnLoginResultListener 를 콜백 목록에 등록합니다
        /// </summary>
        /// <param name="_listener">OnLoginResultListener implemented class</param>
        public static void AddOnEventUserLogin(OnLoginResultListener _listener)
        {
            onLoginResultListeners.Add(_listener);
        }

        /// <summary>
        /// 해당 OnStatusResultListener 를 유일한 콜백으로 등록합니다
        /// </summary>
        /// <param name="_listener">OnStatusResultListener implemented class</param>
        public static void SetOnEventUserStatus(OnStatusResultListener _listener)
        {
            onStatusResultListeners.Clear();
            AddOnEventUserStatus(_listener);
        }

        /// <summary>
        /// 해당 OnStatusResultListener 를 콜백 목록에 등록합니다
        /// </summary>
        /// <param name="_listener">OnStatusResultListener implemented class</param>
        public static void AddOnEventUserStatus(OnStatusResultListener _listener)
        {
            onStatusResultListeners.Add(_listener);
        }
        
        /// <summary>
        /// 해당 OnSystemResultListener 를 유일한 콜백으로 등록합니다
        /// </summary>
        /// <param name="_listener">OnSystemResultListener implemented class</param>
        public static void SetOnEventSystem(OnSystemResultListener _listener)
        {
            onSystemResultListeners.Clear();
            AddOnEventSystem(_listener);
        }

        /// <summary>
        /// 해당 OnSystemResultListener 를 콜백 목록에 등록합니다
        /// </summary>
        /// <param name="_listener">OnSystemResultListener implemented class</param>
        public static void AddOnEventSystem(OnSystemResultListener _listener)
        {
            onSystemResultListeners.Add(_listener);
        }

        /// <summary>
        /// 해당 OnGameMatchedListener 를 유일한 콜백으로 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameMatchedListener implemented class</param>
        public static void SetOnEventGameQueue(OnGameMatchedListener _listener)
        {
            onGameMatchedListeners.Clear();
            AddOnEventGameQueue(_listener);
        }

        /// <summary>
        /// 해당 OnGameMatchedListener 를 콜백 목록에 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameMatchedListener implemented class</param>
        public static void AddOnEventGameQueue(OnGameMatchedListener _listener)
        {
            onGameMatchedListeners.Add(_listener);
        }

        /// <summary>
        /// 해당 OnGameUserMoveListener 를 유일한 콜백으로 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameUserMoveListener implemented class</param>
        public static void SetOnEventGameUserMove(OnGameUserMoveListener _listener)
        {
            onGameUserMoveListeners.Clear();
            AddOnEventGameUserMove(_listener);
        }

        /// <summary>
        /// 해당 OnGameUserMoveListener 를 콜백 목록에 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameUserMoveListener implemented class</param>
        public static void AddOnEventGameUserMove(OnGameUserMoveListener _listener)
        {
            onGameUserMoveListeners.Add(_listener);
        }

        /// <summary>
        /// 해당 OnGameUserSyncListener 를 유일한 콜백으로 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameUserSyncListener implemented class</param>
        public static void SetOnEventGameUserSync(OnGameUserSyncListener _listener)
        {
            onGameUserSyncListeners.Clear();
            AddOnEventGameUserSync(_listener);
        }

        /// <summary>
        /// 해당 OnGameUserSyncListener 를 콜백 목록에 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameUserSyncListener implemented class</param>
        public static void AddOnEventGameUserSync(OnGameUserSyncListener _listener)
        {
            onGameUserSyncListeners.Add(_listener);
        }

        /// <summary>
        /// 해당 OnGameInfoListener 를 유일한 콜백으로 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameInfoListener implemented class</param>
        public static void SetOnEventGameInfo(OnGameInfoListener _listener)
        {
            onGameInfoListeners.Clear();
            AddOnEventGameInfo(_listener);
        }

        /// <summary>
        /// 해당 OnGameInfoListener 를 콜백 목록에 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameInfoListener implemented class</param>
        public static void AddOnEventGameInfo(OnGameInfoListener _listener)
        {
            onGameInfoListeners.Add(_listener);
        }

        /// <summary>
        /// 해당 OnGameStatusListener 를 유일한 콜백으로 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameStatusListener implemented class</param>
        public static void SetOnEventGameStatus(OnGameStatusListener _listener)
        {
            onGameStatusListeners.Clear();
            AddOnEventGameStatus(_listener);
        }

        /// <summary>
        /// 해당 OnGameStatusListener 를 콜백 목록에 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameStatusListener implemented class</param>
        public static void AddOnEventGameStatus(OnGameStatusListener _listener)
        {
            onGameStatusListeners.Add(_listener);
        }

        /// <summary>
        /// 해당 OnGameEventListener 를 유일한 콜백으로 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameUserActionListener implemented class</param>
        public static void SetOnEventGameEvent(OnGameEventListener _listener)
        {
            onGameEventListeners.Clear();
            AddOnEventGameEvent(_listener);
        }

        /// <summary>
        /// 해당 OnGameEventListener 를 콜백 목록에 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameEventListener implemented class</param>
        public static void AddOnEventGameEvent(OnGameEventListener _listener)
        {
            onGameEventListeners.Add(_listener);
        }

        /// <summary>
        /// 해당 OnGameResultListener 를 유일한 콜백으로 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameResultListener implemented class</param>
        public static void SetOnEventGameResult(OnGameResultListener _listener)
        {
            onGameResultListeners.Clear();
            AddOnEventGameResult(_listener);
        }

        /// <summary>
        /// 해당 OnGameResultListener 를 콜백 목록에 등록합니다
        /// </summary>
        /// <param name="_listener">OnGameResultListener implemented class</param>
        public static void AddOnEventGameResult(OnGameResultListener _listener)
        {
            onGameResultListeners.Add(_listener);
        }

    }

}