

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace HaiFeng
{
	public class ctp_trade
	{
		#region Dll Load /UnLoad
		/// <summary>
		/// 原型是 :HMODULE LoadLibrary(LPCTSTR lpFileName);
		/// </summary>
		/// <param name="lpFileName"> DLL 文件名 </param>
		/// <returns> 函数库模块的句柄 </returns>
		[DllImport("kernel32.dll")]
		private static extern IntPtr LoadLibrary(string lpFileName);

		/// <summary>
		/// 原型是 : FARPROC GetProcAddress(HMODULE hModule, LPCWSTR lpProcName);
		/// </summary>
		/// <param name="hModule"> 包含需调用函数的函数库模块的句柄 </param>
		/// <param name="lpProcName"> 调用函数的名称 </param>
		/// <returns> 函数指针 </returns>
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		/// <summary>
		/// 原型是 : BOOL FreeLibrary(HMODULE hModule);
		/// </summary>
		/// <param name="hModule"> 需释放的函数库模块的句柄 </param>
		/// <returns> 是否已释放指定的 Dll </returns>
		[DllImport("kernel32", EntryPoint = "FreeLibrary", SetLastError = true)]
		private static extern bool FreeLibrary(IntPtr hModule);

		/// <summary>
		///
		/// </summary>
		/// <param name="pHModule"></param>
		/// <param name="lpProcName"></param>
		/// <param name="t"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		private static Delegate Invoke(IntPtr pHModule, string lpProcName, Type t)
		{
			// 若函数库模块的句柄为空，则抛出异常
			if (pHModule == IntPtr.Zero)
			{
				throw (new Exception(" 函数库模块的句柄为空 , 请确保已进行 LoadDll 操作 !"));
			}
			// 取得函数指针
			IntPtr farProc = GetProcAddress(pHModule, lpProcName);
			// 若函数指针，则抛出异常
			if (farProc == IntPtr.Zero)
			{
				throw (new Exception(" 没有找到 :" + lpProcName + " 这个函数的入口点 "));
			}
			return Marshal.GetDelegateForFunctionPointer(farProc, t);
		}
		#endregion

		IntPtr _handle = IntPtr.Zero, _api = IntPtr.Zero, _spi = IntPtr.Zero;
		delegate IntPtr Create();
		delegate IntPtr DeleRegisterSpi(IntPtr api, IntPtr pSpi);
		public ctp_trade(string pFile)
		{
			string curPath = Environment.CurrentDirectory;
			Environment.CurrentDirectory = new FileInfo(pFile).DirectoryName;
			_handle = LoadLibrary(pFile);// Environment.CurrentDirectory + "\" + pFile);
			if (_handle == IntPtr.Zero)
			{
				throw (new Exception(String.Format("没有找到:", Environment.CurrentDirectory + "\\" + pFile)));
			}
			Environment.CurrentDirectory = curPath;
			Directory.CreateDirectory("log");

			_api = (Invoke(_handle, "CreateApi", typeof(Create)) as Create)();
			_spi = (Invoke(_handle, "CreateSpi", typeof(Create)) as Create)();
			(Invoke(_handle, "RegisterSpi", typeof(DeleRegisterSpi)) as DeleRegisterSpi)(_api, _spi);
		}


		#region 声明REQ函数类型
		public delegate IntPtr DeleRelease(IntPtr api);
		public delegate IntPtr DeleInit(IntPtr api);
		public delegate IntPtr DeleJoin(IntPtr api);
		public delegate IntPtr DeleGetTradingDay(IntPtr api);
		public delegate IntPtr DeleRegisterFront(IntPtr api, string pszFrontAddress);
		public delegate IntPtr DeleRegisterNameServer(IntPtr api, string pszNsAddress);
		public delegate IntPtr DeleRegisterFensUserInfo(IntPtr api, CThostFtdcFensUserInfoField pFensUserInfo);
		public delegate IntPtr DeleSubscribePrivateTopic(IntPtr api, THOST_TE_RESUME_TYPE nResumeType);
		public delegate IntPtr DeleSubscribePublicTopic(IntPtr api, THOST_TE_RESUME_TYPE nResumeType);
		public delegate IntPtr DeleReqAuthenticate(IntPtr api, CThostFtdcReqAuthenticateField pReqAuthenticateField, int nRequestID);
		public delegate IntPtr DeleReqUserLogin(IntPtr api, CThostFtdcReqUserLoginField pReqUserLoginField, int nRequestID);
		public delegate IntPtr DeleReqUserLogout(IntPtr api, CThostFtdcUserLogoutField pUserLogout, int nRequestID);
		public delegate IntPtr DeleReqUserPasswordUpdate(IntPtr api, CThostFtdcUserPasswordUpdateField pUserPasswordUpdate, int nRequestID);
		public delegate IntPtr DeleReqTradingAccountPasswordUpdate(IntPtr api, CThostFtdcTradingAccountPasswordUpdateField pTradingAccountPasswordUpdate, int nRequestID);
		public delegate IntPtr DeleReqOrderInsert(IntPtr api, CThostFtdcInputOrderField pInputOrder, int nRequestID);
		public delegate IntPtr DeleReqParkedOrderInsert(IntPtr api, CThostFtdcParkedOrderField pParkedOrder, int nRequestID);
		public delegate IntPtr DeleReqParkedOrderAction(IntPtr api, CThostFtdcParkedOrderActionField pParkedOrderAction, int nRequestID);
		public delegate IntPtr DeleReqOrderAction(IntPtr api, CThostFtdcInputOrderActionField pInputOrderAction, int nRequestID);
		public delegate IntPtr DeleReqQueryMaxOrderVolume(IntPtr api, CThostFtdcQueryMaxOrderVolumeField pQueryMaxOrderVolume, int nRequestID);
		public delegate IntPtr DeleReqSettlementInfoConfirm(IntPtr api, CThostFtdcSettlementInfoConfirmField pSettlementInfoConfirm, int nRequestID);
		public delegate IntPtr DeleReqRemoveParkedOrder(IntPtr api, CThostFtdcRemoveParkedOrderField pRemoveParkedOrder, int nRequestID);
		public delegate IntPtr DeleReqRemoveParkedOrderAction(IntPtr api, CThostFtdcRemoveParkedOrderActionField pRemoveParkedOrderAction, int nRequestID);
		public delegate IntPtr DeleReqExecOrderInsert(IntPtr api, CThostFtdcInputExecOrderField pInputExecOrder, int nRequestID);
		public delegate IntPtr DeleReqExecOrderAction(IntPtr api, CThostFtdcInputExecOrderActionField pInputExecOrderAction, int nRequestID);
		public delegate IntPtr DeleReqForQuoteInsert(IntPtr api, CThostFtdcInputForQuoteField pInputForQuote, int nRequestID);
		public delegate IntPtr DeleReqQuoteInsert(IntPtr api, CThostFtdcInputQuoteField pInputQuote, int nRequestID);
		public delegate IntPtr DeleReqQuoteAction(IntPtr api, CThostFtdcInputQuoteActionField pInputQuoteAction, int nRequestID);
		public delegate IntPtr DeleReqBatchOrderAction(IntPtr api, CThostFtdcInputBatchOrderActionField pInputBatchOrderAction, int nRequestID);
		public delegate IntPtr DeleReqCombActionInsert(IntPtr api, CThostFtdcInputCombActionField pInputCombAction, int nRequestID);
		public delegate IntPtr DeleReqQryOrder(IntPtr api, CThostFtdcQryOrderField pQryOrder, int nRequestID);
		public delegate IntPtr DeleReqQryTrade(IntPtr api, CThostFtdcQryTradeField pQryTrade, int nRequestID);
		public delegate IntPtr DeleReqQryInvestorPosition(IntPtr api, CThostFtdcQryInvestorPositionField pQryInvestorPosition, int nRequestID);
		public delegate IntPtr DeleReqQryTradingAccount(IntPtr api, CThostFtdcQryTradingAccountField pQryTradingAccount, int nRequestID);
		public delegate IntPtr DeleReqQryInvestor(IntPtr api, CThostFtdcQryInvestorField pQryInvestor, int nRequestID);
		public delegate IntPtr DeleReqQryTradingCode(IntPtr api, CThostFtdcQryTradingCodeField pQryTradingCode, int nRequestID);
		public delegate IntPtr DeleReqQryInstrumentMarginRate(IntPtr api, CThostFtdcQryInstrumentMarginRateField pQryInstrumentMarginRate, int nRequestID);
		public delegate IntPtr DeleReqQryInstrumentCommissionRate(IntPtr api, CThostFtdcQryInstrumentCommissionRateField pQryInstrumentCommissionRate, int nRequestID);
		public delegate IntPtr DeleReqQryExchange(IntPtr api, CThostFtdcQryExchangeField pQryExchange, int nRequestID);
		public delegate IntPtr DeleReqQryProduct(IntPtr api, CThostFtdcQryProductField pQryProduct, int nRequestID);
		public delegate IntPtr DeleReqQryInstrument(IntPtr api, CThostFtdcQryInstrumentField pQryInstrument, int nRequestID);
		public delegate IntPtr DeleReqQryDepthMarketData(IntPtr api, CThostFtdcQryDepthMarketDataField pQryDepthMarketData, int nRequestID);
		public delegate IntPtr DeleReqQrySettlementInfo(IntPtr api, CThostFtdcQrySettlementInfoField pQrySettlementInfo, int nRequestID);
		public delegate IntPtr DeleReqQryTransferBank(IntPtr api, CThostFtdcQryTransferBankField pQryTransferBank, int nRequestID);
		public delegate IntPtr DeleReqQryInvestorPositionDetail(IntPtr api, CThostFtdcQryInvestorPositionDetailField pQryInvestorPositionDetail, int nRequestID);
		public delegate IntPtr DeleReqQryNotice(IntPtr api, CThostFtdcQryNoticeField pQryNotice, int nRequestID);
		public delegate IntPtr DeleReqQrySettlementInfoConfirm(IntPtr api, CThostFtdcQrySettlementInfoConfirmField pQrySettlementInfoConfirm, int nRequestID);
		public delegate IntPtr DeleReqQryInvestorPositionCombineDetail(IntPtr api, CThostFtdcQryInvestorPositionCombineDetailField pQryInvestorPositionCombineDetail, int nRequestID);
		public delegate IntPtr DeleReqQryCFMMCTradingAccountKey(IntPtr api, CThostFtdcQryCFMMCTradingAccountKeyField pQryCFMMCTradingAccountKey, int nRequestID);
		public delegate IntPtr DeleReqQryEWarrantOffset(IntPtr api, CThostFtdcQryEWarrantOffsetField pQryEWarrantOffset, int nRequestID);
		public delegate IntPtr DeleReqQryInvestorProductGroupMargin(IntPtr api, CThostFtdcQryInvestorProductGroupMarginField pQryInvestorProductGroupMargin, int nRequestID);
		public delegate IntPtr DeleReqQryExchangeMarginRate(IntPtr api, CThostFtdcQryExchangeMarginRateField pQryExchangeMarginRate, int nRequestID);
		public delegate IntPtr DeleReqQryExchangeMarginRateAdjust(IntPtr api, CThostFtdcQryExchangeMarginRateAdjustField pQryExchangeMarginRateAdjust, int nRequestID);
		public delegate IntPtr DeleReqQryExchangeRate(IntPtr api, CThostFtdcQryExchangeRateField pQryExchangeRate, int nRequestID);
		public delegate IntPtr DeleReqQrySecAgentACIDMap(IntPtr api, CThostFtdcQrySecAgentACIDMapField pQrySecAgentACIDMap, int nRequestID);
		public delegate IntPtr DeleReqQryProductExchRate(IntPtr api, CThostFtdcQryProductExchRateField pQryProductExchRate, int nRequestID);
		public delegate IntPtr DeleReqQryProductGroup(IntPtr api, CThostFtdcQryProductGroupField pQryProductGroup, int nRequestID);
		public delegate IntPtr DeleReqQryMMInstrumentCommissionRate(IntPtr api, CThostFtdcQryMMInstrumentCommissionRateField pQryMMInstrumentCommissionRate, int nRequestID);
		public delegate IntPtr DeleReqQryMMOptionInstrCommRate(IntPtr api, CThostFtdcQryMMOptionInstrCommRateField pQryMMOptionInstrCommRate, int nRequestID);
		public delegate IntPtr DeleReqQryInstrumentOrderCommRate(IntPtr api, CThostFtdcQryInstrumentOrderCommRateField pQryInstrumentOrderCommRate, int nRequestID);
		public delegate IntPtr DeleReqQryOptionInstrTradeCost(IntPtr api, CThostFtdcQryOptionInstrTradeCostField pQryOptionInstrTradeCost, int nRequestID);
		public delegate IntPtr DeleReqQryOptionInstrCommRate(IntPtr api, CThostFtdcQryOptionInstrCommRateField pQryOptionInstrCommRate, int nRequestID);
		public delegate IntPtr DeleReqQryExecOrder(IntPtr api, CThostFtdcQryExecOrderField pQryExecOrder, int nRequestID);
		public delegate IntPtr DeleReqQryForQuote(IntPtr api, CThostFtdcQryForQuoteField pQryForQuote, int nRequestID);
		public delegate IntPtr DeleReqQryQuote(IntPtr api, CThostFtdcQryQuoteField pQryQuote, int nRequestID);
		public delegate IntPtr DeleReqQryCombInstrumentGuard(IntPtr api, CThostFtdcQryCombInstrumentGuardField pQryCombInstrumentGuard, int nRequestID);
		public delegate IntPtr DeleReqQryCombAction(IntPtr api, CThostFtdcQryCombActionField pQryCombAction, int nRequestID);
		public delegate IntPtr DeleReqQryTransferSerial(IntPtr api, CThostFtdcQryTransferSerialField pQryTransferSerial, int nRequestID);
		public delegate IntPtr DeleReqQryAccountregister(IntPtr api, CThostFtdcQryAccountregisterField pQryAccountregister, int nRequestID);
		public delegate IntPtr DeleReqQryContractBank(IntPtr api, CThostFtdcQryContractBankField pQryContractBank, int nRequestID);
		public delegate IntPtr DeleReqQryParkedOrder(IntPtr api, CThostFtdcQryParkedOrderField pQryParkedOrder, int nRequestID);
		public delegate IntPtr DeleReqQryParkedOrderAction(IntPtr api, CThostFtdcQryParkedOrderActionField pQryParkedOrderAction, int nRequestID);
		public delegate IntPtr DeleReqQryTradingNotice(IntPtr api, CThostFtdcQryTradingNoticeField pQryTradingNotice, int nRequestID);
		public delegate IntPtr DeleReqQryBrokerTradingParams(IntPtr api, CThostFtdcQryBrokerTradingParamsField pQryBrokerTradingParams, int nRequestID);
		public delegate IntPtr DeleReqQryBrokerTradingAlgos(IntPtr api, CThostFtdcQryBrokerTradingAlgosField pQryBrokerTradingAlgos, int nRequestID);
		public delegate IntPtr DeleReqQueryCFMMCTradingAccountToken(IntPtr api, CThostFtdcQueryCFMMCTradingAccountTokenField pQueryCFMMCTradingAccountToken, int nRequestID);
		public delegate IntPtr DeleReqFromBankToFutureByFuture(IntPtr api, CThostFtdcReqTransferField pReqTransfer, int nRequestID);
		public delegate IntPtr DeleReqFromFutureToBankByFuture(IntPtr api, CThostFtdcReqTransferField pReqTransfer, int nRequestID);
		public delegate IntPtr DeleReqQueryBankAccountMoneyByFuture(IntPtr api, CThostFtdcReqQueryAccountField pReqQueryAccount, int nRequestID);

		#endregion

		#region REQ函数

		private int nRequestID = 0;

		public IntPtr Release()
		{
			(Invoke(_handle, "RegisterSpi", typeof(DeleRegisterSpi)) as DeleRegisterSpi)(_api, IntPtr.Zero);
			return (Invoke(_handle, "Release", typeof(DeleRelease)) as DeleRelease)(_api);
		}

		public IntPtr Init()
		{
			return (Invoke(_handle, "Init", typeof(DeleInit)) as DeleInit)(_api);
		}

		public IntPtr Join()
		{
			return (Invoke(_handle, "Join", typeof(DeleJoin)) as DeleJoin)(_api);
		}

		public IntPtr GetTradingDay()
		{
			return (Invoke(_handle, "GetTradingDay", typeof(DeleGetTradingDay)) as DeleGetTradingDay)(_api);
		}

		public IntPtr RegisterFront(string pszFrontAddress)
		{
			return (Invoke(_handle, "RegisterFront", typeof(DeleRegisterFront)) as DeleRegisterFront)(_api, pszFrontAddress);
		}

		public IntPtr RegisterNameServer(string pszNsAddress)
		{
			return (Invoke(_handle, "RegisterNameServer", typeof(DeleRegisterNameServer)) as DeleRegisterNameServer)(_api, pszNsAddress);
		}

		public IntPtr RegisterFensUserInfo(string BrokerID = "", string UserID = "", TThostFtdcLoginModeType LoginMode = TThostFtdcLoginModeType.THOST_FTDC_LM_Trade)
		{
			CThostFtdcFensUserInfoField struc = new CThostFtdcFensUserInfoField
			{
				BrokerID = BrokerID,
				UserID = UserID,
				LoginMode = LoginMode,
			};
			return (Invoke(_handle, "RegisterFensUserInfo", typeof(DeleRegisterFensUserInfo)) as DeleRegisterFensUserInfo)(_api, struc);
		}

		public IntPtr SubscribePrivateTopic(THOST_TE_RESUME_TYPE nResumeType)
		{
			return (Invoke(_handle, "SubscribePrivateTopic", typeof(DeleSubscribePrivateTopic)) as DeleSubscribePrivateTopic)(_api, nResumeType);
		}

		public IntPtr SubscribePublicTopic(THOST_TE_RESUME_TYPE nResumeType)
		{
			return (Invoke(_handle, "SubscribePublicTopic", typeof(DeleSubscribePublicTopic)) as DeleSubscribePublicTopic)(_api, nResumeType);
		}

		public IntPtr ReqAuthenticate(string BrokerID = "", string UserID = "", string UserProductInfo = "", string AuthCode = "")
		{
			CThostFtdcReqAuthenticateField struc = new CThostFtdcReqAuthenticateField
			{
				BrokerID = BrokerID,
				UserID = UserID,
				UserProductInfo = UserProductInfo,
				AuthCode = AuthCode,
			};
			return (Invoke(_handle, "ReqAuthenticate", typeof(DeleReqAuthenticate)) as DeleReqAuthenticate)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqUserLogin(string TradingDay = "", string BrokerID = "", string UserID = "", string Password = "", string UserProductInfo = "", string InterfaceProductInfo = "", string ProtocolInfo = "", string MacAddress = "", string OneTimePassword = "", string ClientIPAddress = "", string LoginRemark = "")
		{
			CThostFtdcReqUserLoginField struc = new CThostFtdcReqUserLoginField
			{
				TradingDay = TradingDay,
				BrokerID = BrokerID,
				UserID = UserID,
				Password = Password,
				UserProductInfo = UserProductInfo,
				InterfaceProductInfo = InterfaceProductInfo,
				ProtocolInfo = ProtocolInfo,
				MacAddress = MacAddress,
				OneTimePassword = OneTimePassword,
				ClientIPAddress = ClientIPAddress,
				LoginRemark = LoginRemark,
			};
			return (Invoke(_handle, "ReqUserLogin", typeof(DeleReqUserLogin)) as DeleReqUserLogin)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqUserLogout(string BrokerID = "", string UserID = "")
		{
			CThostFtdcUserLogoutField struc = new CThostFtdcUserLogoutField
			{
				BrokerID = BrokerID,
				UserID = UserID,
			};
			return (Invoke(_handle, "ReqUserLogout", typeof(DeleReqUserLogout)) as DeleReqUserLogout)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqUserPasswordUpdate(string BrokerID = "", string UserID = "", string OldPassword = "", string NewPassword = "")
		{
			CThostFtdcUserPasswordUpdateField struc = new CThostFtdcUserPasswordUpdateField
			{
				BrokerID = BrokerID,
				UserID = UserID,
				OldPassword = OldPassword,
				NewPassword = NewPassword,
			};
			return (Invoke(_handle, "ReqUserPasswordUpdate", typeof(DeleReqUserPasswordUpdate)) as DeleReqUserPasswordUpdate)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqTradingAccountPasswordUpdate(string BrokerID = "", string AccountID = "", string OldPassword = "", string NewPassword = "", string CurrencyID = "")
		{
			CThostFtdcTradingAccountPasswordUpdateField struc = new CThostFtdcTradingAccountPasswordUpdateField
			{
				BrokerID = BrokerID,
				AccountID = AccountID,
				OldPassword = OldPassword,
				NewPassword = NewPassword,
				CurrencyID = CurrencyID,
			};
			return (Invoke(_handle, "ReqTradingAccountPasswordUpdate", typeof(DeleReqTradingAccountPasswordUpdate)) as DeleReqTradingAccountPasswordUpdate)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqOrderInsert(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string OrderRef = "", string UserID = "", TThostFtdcOrderPriceTypeType OrderPriceType = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AnyPrice, TThostFtdcDirectionType Direction = TThostFtdcDirectionType.THOST_FTDC_D_Buy, string CombOffsetFlag = "", string CombHedgeFlag = "", double LimitPrice = 0, int VolumeTotalOriginal = 0, TThostFtdcTimeConditionType TimeCondition = TThostFtdcTimeConditionType.THOST_FTDC_TC_IOC, string GTDDate = "", TThostFtdcVolumeConditionType VolumeCondition = TThostFtdcVolumeConditionType.THOST_FTDC_VC_AV, int MinVolume = 0, TThostFtdcContingentConditionType ContingentCondition = TThostFtdcContingentConditionType.THOST_FTDC_CC_Immediately, double StopPrice = 0, TThostFtdcForceCloseReasonType ForceCloseReason = TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_NotForceClose, int IsAutoSuspend = 0, string BusinessUnit = "", int RequestID = 0, int UserForceClose = 0, int IsSwapOrder = 0, string ExchangeID = "", string InvestUnitID = "", string AccountID = "", string CurrencyID = "", string ClientID = "", string IPAddress = "", string MacAddress = "")
		{
			CThostFtdcInputOrderField struc = new CThostFtdcInputOrderField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				OrderRef = OrderRef,
				UserID = UserID,
				OrderPriceType = OrderPriceType,
				Direction = Direction,
				CombOffsetFlag = CombOffsetFlag,
				CombHedgeFlag = CombHedgeFlag,
				LimitPrice = LimitPrice,
				VolumeTotalOriginal = VolumeTotalOriginal,

				TimeCondition = TimeCondition,
				GTDDate = GTDDate,
				VolumeCondition = VolumeCondition,
				MinVolume = MinVolume,

				ContingentCondition = ContingentCondition,
				StopPrice = StopPrice,

				ForceCloseReason = ForceCloseReason,
				IsAutoSuspend = IsAutoSuspend,

				BusinessUnit = BusinessUnit,
				RequestID = RequestID,
				UserForceClose = UserForceClose,
				IsSwapOrder = IsSwapOrder,

				ExchangeID = ExchangeID,
				InvestUnitID = InvestUnitID,
				AccountID = AccountID,
				CurrencyID = CurrencyID,
				ClientID = ClientID,
				IPAddress = IPAddress,
				MacAddress = MacAddress,
			};
			return (Invoke(_handle, "ReqOrderInsert", typeof(DeleReqOrderInsert)) as DeleReqOrderInsert)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqParkedOrderInsert(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string OrderRef = "", string UserID = "", TThostFtdcOrderPriceTypeType OrderPriceType = TThostFtdcOrderPriceTypeType.THOST_FTDC_OPT_AnyPrice, TThostFtdcDirectionType Direction = TThostFtdcDirectionType.THOST_FTDC_D_Buy, string CombOffsetFlag = "", string CombHedgeFlag = "", double LimitPrice = 0, int VolumeTotalOriginal = 0, TThostFtdcTimeConditionType TimeCondition = TThostFtdcTimeConditionType.THOST_FTDC_TC_IOC, string GTDDate = "", TThostFtdcVolumeConditionType VolumeCondition = TThostFtdcVolumeConditionType.THOST_FTDC_VC_AV, int MinVolume = 0, TThostFtdcContingentConditionType ContingentCondition = TThostFtdcContingentConditionType.THOST_FTDC_CC_Immediately, double StopPrice = 0, TThostFtdcForceCloseReasonType ForceCloseReason = TThostFtdcForceCloseReasonType.THOST_FTDC_FCC_NotForceClose, int IsAutoSuspend = 0, string BusinessUnit = "", int RequestID = 0, int UserForceClose = 0, string ExchangeID = "", string ParkedOrderID = "", TThostFtdcUserTypeType UserType = TThostFtdcUserTypeType.THOST_FTDC_UT_Investor, TThostFtdcParkedOrderStatusType Status = TThostFtdcParkedOrderStatusType.THOST_FTDC_PAOS_NotSend, int ErrorID = 0, string ErrorMsg = "", int IsSwapOrder = 0, string AccountID = "", string CurrencyID = "", string ClientID = "", string InvestUnitID = "", string IPAddress = "", string MacAddress = "")
		{
			CThostFtdcParkedOrderField struc = new CThostFtdcParkedOrderField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				OrderRef = OrderRef,
				UserID = UserID,
				OrderPriceType = OrderPriceType,
				Direction = Direction,
				CombOffsetFlag = CombOffsetFlag,
				CombHedgeFlag = CombHedgeFlag,
				LimitPrice = LimitPrice,
				VolumeTotalOriginal = VolumeTotalOriginal,

				TimeCondition = TimeCondition,
				GTDDate = GTDDate,
				VolumeCondition = VolumeCondition,
				MinVolume = MinVolume,

				ContingentCondition = ContingentCondition,
				StopPrice = StopPrice,

				ForceCloseReason = ForceCloseReason,
				IsAutoSuspend = IsAutoSuspend,

				BusinessUnit = BusinessUnit,
				RequestID = RequestID,
				UserForceClose = UserForceClose,

				ExchangeID = ExchangeID,
				ParkedOrderID = ParkedOrderID,
				UserType = UserType,
				Status = Status,
				ErrorID = ErrorID,

				ErrorMsg = ErrorMsg,
				IsSwapOrder = IsSwapOrder,

				AccountID = AccountID,
				CurrencyID = CurrencyID,
				ClientID = ClientID,
				InvestUnitID = InvestUnitID,
				IPAddress = IPAddress,
				MacAddress = MacAddress,
			};
			return (Invoke(_handle, "ReqParkedOrderInsert", typeof(DeleReqParkedOrderInsert)) as DeleReqParkedOrderInsert)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqParkedOrderAction(string BrokerID = "", string InvestorID = "", int OrderActionRef = 0, string OrderRef = "", int RequestID = 0, int FrontID = 0, int SessionID = 0, string ExchangeID = "", string OrderSysID = "", TThostFtdcActionFlagType ActionFlag = TThostFtdcActionFlagType.THOST_FTDC_AF_Delete, double LimitPrice = 0, int VolumeChange = 0, string UserID = "", string InstrumentID = "", string ParkedOrderActionID = "", TThostFtdcUserTypeType UserType = TThostFtdcUserTypeType.THOST_FTDC_UT_Investor, TThostFtdcParkedOrderStatusType Status = TThostFtdcParkedOrderStatusType.THOST_FTDC_PAOS_NotSend, int ErrorID = 0, string ErrorMsg = "", string InvestUnitID = "", string IPAddress = "", string MacAddress = "")
		{
			CThostFtdcParkedOrderActionField struc = new CThostFtdcParkedOrderActionField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				OrderActionRef = OrderActionRef,

				OrderRef = OrderRef,
				RequestID = RequestID,
				FrontID = FrontID,
				SessionID = SessionID,

				ExchangeID = ExchangeID,
				OrderSysID = OrderSysID,
				ActionFlag = ActionFlag,
				LimitPrice = LimitPrice,
				VolumeChange = VolumeChange,

				UserID = UserID,
				InstrumentID = InstrumentID,
				ParkedOrderActionID = ParkedOrderActionID,
				UserType = UserType,
				Status = Status,
				ErrorID = ErrorID,

				ErrorMsg = ErrorMsg,
				InvestUnitID = InvestUnitID,
				IPAddress = IPAddress,
				MacAddress = MacAddress,
			};
			return (Invoke(_handle, "ReqParkedOrderAction", typeof(DeleReqParkedOrderAction)) as DeleReqParkedOrderAction)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqOrderAction(string BrokerID = "", string InvestorID = "", int OrderActionRef = 0, string OrderRef = "", int RequestID = 0, int FrontID = 0, int SessionID = 0, string ExchangeID = "", string OrderSysID = "", TThostFtdcActionFlagType ActionFlag = TThostFtdcActionFlagType.THOST_FTDC_AF_Delete, double LimitPrice = 0, int VolumeChange = 0, string UserID = "", string InstrumentID = "", string InvestUnitID = "", string IPAddress = "", string MacAddress = "")
		{
			CThostFtdcInputOrderActionField struc = new CThostFtdcInputOrderActionField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				OrderActionRef = OrderActionRef,

				OrderRef = OrderRef,
				RequestID = RequestID,
				FrontID = FrontID,
				SessionID = SessionID,

				ExchangeID = ExchangeID,
				OrderSysID = OrderSysID,
				ActionFlag = ActionFlag,
				LimitPrice = LimitPrice,
				VolumeChange = VolumeChange,

				UserID = UserID,
				InstrumentID = InstrumentID,
				InvestUnitID = InvestUnitID,
				IPAddress = IPAddress,
				MacAddress = MacAddress,
			};
			return (Invoke(_handle, "ReqOrderAction", typeof(DeleReqOrderAction)) as DeleReqOrderAction)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQueryMaxOrderVolume(string BrokerID = "", string InvestorID = "", string InstrumentID = "", TThostFtdcDirectionType Direction = TThostFtdcDirectionType.THOST_FTDC_D_Buy, TThostFtdcOffsetFlagType OffsetFlag = TThostFtdcOffsetFlagType.THOST_FTDC_OF_Open, TThostFtdcHedgeFlagType HedgeFlag = TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation, int MaxVolume = 0)
		{
			CThostFtdcQueryMaxOrderVolumeField struc = new CThostFtdcQueryMaxOrderVolumeField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				Direction = Direction,
				OffsetFlag = OffsetFlag,
				HedgeFlag = HedgeFlag,
				MaxVolume = MaxVolume,

			};
			return (Invoke(_handle, "ReqQueryMaxOrderVolume", typeof(DeleReqQueryMaxOrderVolume)) as DeleReqQueryMaxOrderVolume)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqSettlementInfoConfirm(string BrokerID = "", string InvestorID = "", string ConfirmDate = "", string ConfirmTime = "")
		{
			CThostFtdcSettlementInfoConfirmField struc = new CThostFtdcSettlementInfoConfirmField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				ConfirmDate = ConfirmDate,
				ConfirmTime = ConfirmTime,
			};
			return (Invoke(_handle, "ReqSettlementInfoConfirm", typeof(DeleReqSettlementInfoConfirm)) as DeleReqSettlementInfoConfirm)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqRemoveParkedOrder(string BrokerID = "", string InvestorID = "", string ParkedOrderID = "")
		{
			CThostFtdcRemoveParkedOrderField struc = new CThostFtdcRemoveParkedOrderField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				ParkedOrderID = ParkedOrderID,
			};
			return (Invoke(_handle, "ReqRemoveParkedOrder", typeof(DeleReqRemoveParkedOrder)) as DeleReqRemoveParkedOrder)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqRemoveParkedOrderAction(string BrokerID = "", string InvestorID = "", string ParkedOrderActionID = "")
		{
			CThostFtdcRemoveParkedOrderActionField struc = new CThostFtdcRemoveParkedOrderActionField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				ParkedOrderActionID = ParkedOrderActionID,
			};
			return (Invoke(_handle, "ReqRemoveParkedOrderAction", typeof(DeleReqRemoveParkedOrderAction)) as DeleReqRemoveParkedOrderAction)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqExecOrderInsert(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string ExecOrderRef = "", string UserID = "", int Volume = 0, int RequestID = 0, string BusinessUnit = "", TThostFtdcOffsetFlagType OffsetFlag = TThostFtdcOffsetFlagType.THOST_FTDC_OF_Open, TThostFtdcHedgeFlagType HedgeFlag = TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation, TThostFtdcActionTypeType ActionType = TThostFtdcActionTypeType.THOST_FTDC_ACTP_Exec, TThostFtdcPosiDirectionType PosiDirection = TThostFtdcPosiDirectionType.THOST_FTDC_PD_Net, TThostFtdcExecOrderPositionFlagType ReservePositionFlag = TThostFtdcExecOrderPositionFlagType.THOST_FTDC_EOPF_Reserve, TThostFtdcExecOrderCloseFlagType CloseFlag = TThostFtdcExecOrderCloseFlagType.THOST_FTDC_EOCF_AutoClose, string ExchangeID = "", string InvestUnitID = "", string AccountID = "", string CurrencyID = "", string ClientID = "", string IPAddress = "", string MacAddress = "")
		{
			CThostFtdcInputExecOrderField struc = new CThostFtdcInputExecOrderField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				ExecOrderRef = ExecOrderRef,
				UserID = UserID,
				Volume = Volume,
				RequestID = RequestID,

				BusinessUnit = BusinessUnit,
				OffsetFlag = OffsetFlag,
				HedgeFlag = HedgeFlag,
				ActionType = ActionType,
				PosiDirection = PosiDirection,
				ReservePositionFlag = ReservePositionFlag,
				CloseFlag = CloseFlag,
				ExchangeID = ExchangeID,
				InvestUnitID = InvestUnitID,
				AccountID = AccountID,
				CurrencyID = CurrencyID,
				ClientID = ClientID,
				IPAddress = IPAddress,
				MacAddress = MacAddress,
			};
			return (Invoke(_handle, "ReqExecOrderInsert", typeof(DeleReqExecOrderInsert)) as DeleReqExecOrderInsert)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqExecOrderAction(string BrokerID = "", string InvestorID = "", int ExecOrderActionRef = 0, string ExecOrderRef = "", int RequestID = 0, int FrontID = 0, int SessionID = 0, string ExchangeID = "", string ExecOrderSysID = "", TThostFtdcActionFlagType ActionFlag = TThostFtdcActionFlagType.THOST_FTDC_AF_Delete, string UserID = "", string InstrumentID = "", string InvestUnitID = "", string IPAddress = "", string MacAddress = "")
		{
			CThostFtdcInputExecOrderActionField struc = new CThostFtdcInputExecOrderActionField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				ExecOrderActionRef = ExecOrderActionRef,

				ExecOrderRef = ExecOrderRef,
				RequestID = RequestID,
				FrontID = FrontID,
				SessionID = SessionID,

				ExchangeID = ExchangeID,
				ExecOrderSysID = ExecOrderSysID,
				ActionFlag = ActionFlag,
				UserID = UserID,
				InstrumentID = InstrumentID,
				InvestUnitID = InvestUnitID,
				IPAddress = IPAddress,
				MacAddress = MacAddress,
			};
			return (Invoke(_handle, "ReqExecOrderAction", typeof(DeleReqExecOrderAction)) as DeleReqExecOrderAction)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqForQuoteInsert(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string ForQuoteRef = "", string UserID = "", string ExchangeID = "", string InvestUnitID = "", string IPAddress = "", string MacAddress = "")
		{
			CThostFtdcInputForQuoteField struc = new CThostFtdcInputForQuoteField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				ForQuoteRef = ForQuoteRef,
				UserID = UserID,
				ExchangeID = ExchangeID,
				InvestUnitID = InvestUnitID,
				IPAddress = IPAddress,
				MacAddress = MacAddress,
			};
			return (Invoke(_handle, "ReqForQuoteInsert", typeof(DeleReqForQuoteInsert)) as DeleReqForQuoteInsert)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQuoteInsert(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string QuoteRef = "", string UserID = "", double AskPrice = 0, double BidPrice = 0, int AskVolume = 0, int BidVolume = 0, int RequestID = 0, string BusinessUnit = "", TThostFtdcOffsetFlagType AskOffsetFlag = TThostFtdcOffsetFlagType.THOST_FTDC_OF_Open, TThostFtdcOffsetFlagType BidOffsetFlag = TThostFtdcOffsetFlagType.THOST_FTDC_OF_Open, TThostFtdcHedgeFlagType AskHedgeFlag = TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation, TThostFtdcHedgeFlagType BidHedgeFlag = TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation, string AskOrderRef = "", string BidOrderRef = "", string ForQuoteSysID = "", string ExchangeID = "", string InvestUnitID = "", string ClientID = "", string IPAddress = "", string MacAddress = "")
		{
			CThostFtdcInputQuoteField struc = new CThostFtdcInputQuoteField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				QuoteRef = QuoteRef,
				UserID = UserID,
				AskPrice = AskPrice,
				BidPrice = BidPrice,
				AskVolume = AskVolume,
				BidVolume = BidVolume,
				RequestID = RequestID,

				BusinessUnit = BusinessUnit,
				AskOffsetFlag = AskOffsetFlag,
				BidOffsetFlag = BidOffsetFlag,
				AskHedgeFlag = AskHedgeFlag,
				BidHedgeFlag = BidHedgeFlag,
				AskOrderRef = AskOrderRef,
				BidOrderRef = BidOrderRef,
				ForQuoteSysID = ForQuoteSysID,
				ExchangeID = ExchangeID,
				InvestUnitID = InvestUnitID,
				ClientID = ClientID,
				IPAddress = IPAddress,
				MacAddress = MacAddress,
			};
			return (Invoke(_handle, "ReqQuoteInsert", typeof(DeleReqQuoteInsert)) as DeleReqQuoteInsert)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQuoteAction(string BrokerID = "", string InvestorID = "", int QuoteActionRef = 0, string QuoteRef = "", int RequestID = 0, int FrontID = 0, int SessionID = 0, string ExchangeID = "", string QuoteSysID = "", TThostFtdcActionFlagType ActionFlag = TThostFtdcActionFlagType.THOST_FTDC_AF_Delete, string UserID = "", string InstrumentID = "", string InvestUnitID = "", string ClientID = "", string IPAddress = "", string MacAddress = "")
		{
			CThostFtdcInputQuoteActionField struc = new CThostFtdcInputQuoteActionField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				QuoteActionRef = QuoteActionRef,

				QuoteRef = QuoteRef,
				RequestID = RequestID,
				FrontID = FrontID,
				SessionID = SessionID,

				ExchangeID = ExchangeID,
				QuoteSysID = QuoteSysID,
				ActionFlag = ActionFlag,
				UserID = UserID,
				InstrumentID = InstrumentID,
				InvestUnitID = InvestUnitID,
				ClientID = ClientID,
				IPAddress = IPAddress,
				MacAddress = MacAddress,
			};
			return (Invoke(_handle, "ReqQuoteAction", typeof(DeleReqQuoteAction)) as DeleReqQuoteAction)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqBatchOrderAction(string BrokerID = "", string InvestorID = "", int OrderActionRef = 0, int RequestID = 0, int FrontID = 0, int SessionID = 0, string ExchangeID = "", string UserID = "", string InvestUnitID = "", string IPAddress = "", string MacAddress = "")
		{
			CThostFtdcInputBatchOrderActionField struc = new CThostFtdcInputBatchOrderActionField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				OrderActionRef = OrderActionRef,
				RequestID = RequestID,
				FrontID = FrontID,
				SessionID = SessionID,

				ExchangeID = ExchangeID,
				UserID = UserID,
				InvestUnitID = InvestUnitID,
				IPAddress = IPAddress,
				MacAddress = MacAddress,
			};
			return (Invoke(_handle, "ReqBatchOrderAction", typeof(DeleReqBatchOrderAction)) as DeleReqBatchOrderAction)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqCombActionInsert(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string CombActionRef = "", string UserID = "", TThostFtdcDirectionType Direction = TThostFtdcDirectionType.THOST_FTDC_D_Buy, int Volume = 0, TThostFtdcCombDirectionType CombDirection = TThostFtdcCombDirectionType.THOST_FTDC_CMDR_Comb, TThostFtdcHedgeFlagType HedgeFlag = TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation, string ExchangeID = "", string IPAddress = "", string MacAddress = "")
		{
			CThostFtdcInputCombActionField struc = new CThostFtdcInputCombActionField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				CombActionRef = CombActionRef,
				UserID = UserID,
				Direction = Direction,
				Volume = Volume,

				CombDirection = CombDirection,
				HedgeFlag = HedgeFlag,
				ExchangeID = ExchangeID,
				IPAddress = IPAddress,
				MacAddress = MacAddress,
			};
			return (Invoke(_handle, "ReqCombActionInsert", typeof(DeleReqCombActionInsert)) as DeleReqCombActionInsert)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryOrder(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string ExchangeID = "", string OrderSysID = "", string InsertTimeStart = "", string InsertTimeEnd = "")
		{
			CThostFtdcQryOrderField struc = new CThostFtdcQryOrderField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				ExchangeID = ExchangeID,
				OrderSysID = OrderSysID,
				InsertTimeStart = InsertTimeStart,
				InsertTimeEnd = InsertTimeEnd,
			};
			return (Invoke(_handle, "ReqQryOrder", typeof(DeleReqQryOrder)) as DeleReqQryOrder)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryTrade(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string ExchangeID = "", string TradeID = "", string TradeTimeStart = "", string TradeTimeEnd = "")
		{
			CThostFtdcQryTradeField struc = new CThostFtdcQryTradeField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				ExchangeID = ExchangeID,
				TradeID = TradeID,
				TradeTimeStart = TradeTimeStart,
				TradeTimeEnd = TradeTimeEnd,
			};
			return (Invoke(_handle, "ReqQryTrade", typeof(DeleReqQryTrade)) as DeleReqQryTrade)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryInvestorPosition(string BrokerID = "", string InvestorID = "", string InstrumentID = "")
		{
			CThostFtdcQryInvestorPositionField struc = new CThostFtdcQryInvestorPositionField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
			};
			return (Invoke(_handle, "ReqQryInvestorPosition", typeof(DeleReqQryInvestorPosition)) as DeleReqQryInvestorPosition)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryTradingAccount(string BrokerID = "", string InvestorID = "", string CurrencyID = "")
		{
			CThostFtdcQryTradingAccountField struc = new CThostFtdcQryTradingAccountField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				CurrencyID = CurrencyID,
			};
			return (Invoke(_handle, "ReqQryTradingAccount", typeof(DeleReqQryTradingAccount)) as DeleReqQryTradingAccount)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryInvestor(string BrokerID = "", string InvestorID = "")
		{
			CThostFtdcQryInvestorField struc = new CThostFtdcQryInvestorField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
			};
			return (Invoke(_handle, "ReqQryInvestor", typeof(DeleReqQryInvestor)) as DeleReqQryInvestor)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryTradingCode(string BrokerID = "", string InvestorID = "", string ExchangeID = "", string ClientID = "", TThostFtdcClientIDTypeType ClientIDType = TThostFtdcClientIDTypeType.THOST_FTDC_CIDT_Speculation)
		{
			CThostFtdcQryTradingCodeField struc = new CThostFtdcQryTradingCodeField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				ExchangeID = ExchangeID,
				ClientID = ClientID,
				ClientIDType = ClientIDType,
			};
			return (Invoke(_handle, "ReqQryTradingCode", typeof(DeleReqQryTradingCode)) as DeleReqQryTradingCode)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryInstrumentMarginRate(string BrokerID = "", string InvestorID = "", string InstrumentID = "", TThostFtdcHedgeFlagType HedgeFlag = TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation)
		{
			CThostFtdcQryInstrumentMarginRateField struc = new CThostFtdcQryInstrumentMarginRateField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				HedgeFlag = HedgeFlag,
			};
			return (Invoke(_handle, "ReqQryInstrumentMarginRate", typeof(DeleReqQryInstrumentMarginRate)) as DeleReqQryInstrumentMarginRate)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryInstrumentCommissionRate(string BrokerID = "", string InvestorID = "", string InstrumentID = "")
		{
			CThostFtdcQryInstrumentCommissionRateField struc = new CThostFtdcQryInstrumentCommissionRateField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
			};
			return (Invoke(_handle, "ReqQryInstrumentCommissionRate", typeof(DeleReqQryInstrumentCommissionRate)) as DeleReqQryInstrumentCommissionRate)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryExchange(string ExchangeID = "")
		{
			CThostFtdcQryExchangeField struc = new CThostFtdcQryExchangeField
			{
				ExchangeID = ExchangeID,
			};
			return (Invoke(_handle, "ReqQryExchange", typeof(DeleReqQryExchange)) as DeleReqQryExchange)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryProduct(string ProductID = "", TThostFtdcProductClassType ProductClass = TThostFtdcProductClassType.THOST_FTDC_PC_Futures)
		{
			CThostFtdcQryProductField struc = new CThostFtdcQryProductField
			{
				ProductID = ProductID,
				ProductClass = ProductClass,
			};
			return (Invoke(_handle, "ReqQryProduct", typeof(DeleReqQryProduct)) as DeleReqQryProduct)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryInstrument(string InstrumentID = "", string ExchangeID = "", string ExchangeInstID = "", string ProductID = "")
		{
			CThostFtdcQryInstrumentField struc = new CThostFtdcQryInstrumentField
			{
				InstrumentID = InstrumentID,
				ExchangeID = ExchangeID,
				ExchangeInstID = ExchangeInstID,
				ProductID = ProductID,
			};
			return (Invoke(_handle, "ReqQryInstrument", typeof(DeleReqQryInstrument)) as DeleReqQryInstrument)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryDepthMarketData(string InstrumentID = "")
		{
			CThostFtdcQryDepthMarketDataField struc = new CThostFtdcQryDepthMarketDataField
			{
				InstrumentID = InstrumentID,
			};
			return (Invoke(_handle, "ReqQryDepthMarketData", typeof(DeleReqQryDepthMarketData)) as DeleReqQryDepthMarketData)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQrySettlementInfo(string BrokerID = "", string InvestorID = "", string TradingDay = "")
		{
			CThostFtdcQrySettlementInfoField struc = new CThostFtdcQrySettlementInfoField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				TradingDay = TradingDay,
			};
			return (Invoke(_handle, "ReqQrySettlementInfo", typeof(DeleReqQrySettlementInfo)) as DeleReqQrySettlementInfo)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryTransferBank(string BankID = "", string BankBrchID = "")
		{
			CThostFtdcQryTransferBankField struc = new CThostFtdcQryTransferBankField
			{
				BankID = BankID,
				BankBrchID = BankBrchID,
			};
			return (Invoke(_handle, "ReqQryTransferBank", typeof(DeleReqQryTransferBank)) as DeleReqQryTransferBank)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryInvestorPositionDetail(string BrokerID = "", string InvestorID = "", string InstrumentID = "")
		{
			CThostFtdcQryInvestorPositionDetailField struc = new CThostFtdcQryInvestorPositionDetailField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
			};
			return (Invoke(_handle, "ReqQryInvestorPositionDetail", typeof(DeleReqQryInvestorPositionDetail)) as DeleReqQryInvestorPositionDetail)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryNotice(string BrokerID = "")
		{
			CThostFtdcQryNoticeField struc = new CThostFtdcQryNoticeField
			{
				BrokerID = BrokerID,
			};
			return (Invoke(_handle, "ReqQryNotice", typeof(DeleReqQryNotice)) as DeleReqQryNotice)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQrySettlementInfoConfirm(string BrokerID = "", string InvestorID = "")
		{
			CThostFtdcQrySettlementInfoConfirmField struc = new CThostFtdcQrySettlementInfoConfirmField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
			};
			return (Invoke(_handle, "ReqQrySettlementInfoConfirm", typeof(DeleReqQrySettlementInfoConfirm)) as DeleReqQrySettlementInfoConfirm)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryInvestorPositionCombineDetail(string BrokerID = "", string InvestorID = "", string CombInstrumentID = "")
		{
			CThostFtdcQryInvestorPositionCombineDetailField struc = new CThostFtdcQryInvestorPositionCombineDetailField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				CombInstrumentID = CombInstrumentID,
			};
			return (Invoke(_handle, "ReqQryInvestorPositionCombineDetail", typeof(DeleReqQryInvestorPositionCombineDetail)) as DeleReqQryInvestorPositionCombineDetail)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryCFMMCTradingAccountKey(string BrokerID = "", string InvestorID = "")
		{
			CThostFtdcQryCFMMCTradingAccountKeyField struc = new CThostFtdcQryCFMMCTradingAccountKeyField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
			};
			return (Invoke(_handle, "ReqQryCFMMCTradingAccountKey", typeof(DeleReqQryCFMMCTradingAccountKey)) as DeleReqQryCFMMCTradingAccountKey)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryEWarrantOffset(string BrokerID = "", string InvestorID = "", string ExchangeID = "", string InstrumentID = "")
		{
			CThostFtdcQryEWarrantOffsetField struc = new CThostFtdcQryEWarrantOffsetField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				ExchangeID = ExchangeID,
				InstrumentID = InstrumentID,
			};
			return (Invoke(_handle, "ReqQryEWarrantOffset", typeof(DeleReqQryEWarrantOffset)) as DeleReqQryEWarrantOffset)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryInvestorProductGroupMargin(string BrokerID = "", string InvestorID = "", string ProductGroupID = "", TThostFtdcHedgeFlagType HedgeFlag = TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation)
		{
			CThostFtdcQryInvestorProductGroupMarginField struc = new CThostFtdcQryInvestorProductGroupMarginField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				ProductGroupID = ProductGroupID,
				HedgeFlag = HedgeFlag,
			};
			return (Invoke(_handle, "ReqQryInvestorProductGroupMargin", typeof(DeleReqQryInvestorProductGroupMargin)) as DeleReqQryInvestorProductGroupMargin)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryExchangeMarginRate(string BrokerID = "", string InstrumentID = "", TThostFtdcHedgeFlagType HedgeFlag = TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation)
		{
			CThostFtdcQryExchangeMarginRateField struc = new CThostFtdcQryExchangeMarginRateField
			{
				BrokerID = BrokerID,
				InstrumentID = InstrumentID,
				HedgeFlag = HedgeFlag,
			};
			return (Invoke(_handle, "ReqQryExchangeMarginRate", typeof(DeleReqQryExchangeMarginRate)) as DeleReqQryExchangeMarginRate)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryExchangeMarginRateAdjust(string BrokerID = "", string InstrumentID = "", TThostFtdcHedgeFlagType HedgeFlag = TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation)
		{
			CThostFtdcQryExchangeMarginRateAdjustField struc = new CThostFtdcQryExchangeMarginRateAdjustField
			{
				BrokerID = BrokerID,
				InstrumentID = InstrumentID,
				HedgeFlag = HedgeFlag,
			};
			return (Invoke(_handle, "ReqQryExchangeMarginRateAdjust", typeof(DeleReqQryExchangeMarginRateAdjust)) as DeleReqQryExchangeMarginRateAdjust)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryExchangeRate(string BrokerID = "", string FromCurrencyID = "", string ToCurrencyID = "")
		{
			CThostFtdcQryExchangeRateField struc = new CThostFtdcQryExchangeRateField
			{
				BrokerID = BrokerID,
				FromCurrencyID = FromCurrencyID,
				ToCurrencyID = ToCurrencyID,
			};
			return (Invoke(_handle, "ReqQryExchangeRate", typeof(DeleReqQryExchangeRate)) as DeleReqQryExchangeRate)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQrySecAgentACIDMap(string BrokerID = "", string UserID = "", string AccountID = "", string CurrencyID = "")
		{
			CThostFtdcQrySecAgentACIDMapField struc = new CThostFtdcQrySecAgentACIDMapField
			{
				BrokerID = BrokerID,
				UserID = UserID,
				AccountID = AccountID,
				CurrencyID = CurrencyID,
			};
			return (Invoke(_handle, "ReqQrySecAgentACIDMap", typeof(DeleReqQrySecAgentACIDMap)) as DeleReqQrySecAgentACIDMap)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryProductExchRate(string ProductID = "")
		{
			CThostFtdcQryProductExchRateField struc = new CThostFtdcQryProductExchRateField
			{
				ProductID = ProductID,
			};
			return (Invoke(_handle, "ReqQryProductExchRate", typeof(DeleReqQryProductExchRate)) as DeleReqQryProductExchRate)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryProductGroup(string ProductID = "", string ExchangeID = "")
		{
			CThostFtdcQryProductGroupField struc = new CThostFtdcQryProductGroupField
			{
				ProductID = ProductID,
				ExchangeID = ExchangeID,
			};
			return (Invoke(_handle, "ReqQryProductGroup", typeof(DeleReqQryProductGroup)) as DeleReqQryProductGroup)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryMMInstrumentCommissionRate(string BrokerID = "", string InvestorID = "", string InstrumentID = "")
		{
			CThostFtdcQryMMInstrumentCommissionRateField struc = new CThostFtdcQryMMInstrumentCommissionRateField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
			};
			return (Invoke(_handle, "ReqQryMMInstrumentCommissionRate", typeof(DeleReqQryMMInstrumentCommissionRate)) as DeleReqQryMMInstrumentCommissionRate)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryMMOptionInstrCommRate(string BrokerID = "", string InvestorID = "", string InstrumentID = "")
		{
			CThostFtdcQryMMOptionInstrCommRateField struc = new CThostFtdcQryMMOptionInstrCommRateField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
			};
			return (Invoke(_handle, "ReqQryMMOptionInstrCommRate", typeof(DeleReqQryMMOptionInstrCommRate)) as DeleReqQryMMOptionInstrCommRate)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryInstrumentOrderCommRate(string BrokerID = "", string InvestorID = "", string InstrumentID = "")
		{
			CThostFtdcQryInstrumentOrderCommRateField struc = new CThostFtdcQryInstrumentOrderCommRateField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
			};
			return (Invoke(_handle, "ReqQryInstrumentOrderCommRate", typeof(DeleReqQryInstrumentOrderCommRate)) as DeleReqQryInstrumentOrderCommRate)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryOptionInstrTradeCost(string BrokerID = "", string InvestorID = "", string InstrumentID = "", TThostFtdcHedgeFlagType HedgeFlag = TThostFtdcHedgeFlagType.THOST_FTDC_HF_Speculation, double InputPrice = 0, double UnderlyingPrice = 0)
		{
			CThostFtdcQryOptionInstrTradeCostField struc = new CThostFtdcQryOptionInstrTradeCostField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				HedgeFlag = HedgeFlag,
				InputPrice = InputPrice,
				UnderlyingPrice = UnderlyingPrice,

			};
			return (Invoke(_handle, "ReqQryOptionInstrTradeCost", typeof(DeleReqQryOptionInstrTradeCost)) as DeleReqQryOptionInstrTradeCost)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryOptionInstrCommRate(string BrokerID = "", string InvestorID = "", string InstrumentID = "")
		{
			CThostFtdcQryOptionInstrCommRateField struc = new CThostFtdcQryOptionInstrCommRateField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
			};
			return (Invoke(_handle, "ReqQryOptionInstrCommRate", typeof(DeleReqQryOptionInstrCommRate)) as DeleReqQryOptionInstrCommRate)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryExecOrder(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string ExchangeID = "", string ExecOrderSysID = "", string InsertTimeStart = "", string InsertTimeEnd = "")
		{
			CThostFtdcQryExecOrderField struc = new CThostFtdcQryExecOrderField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				ExchangeID = ExchangeID,
				ExecOrderSysID = ExecOrderSysID,
				InsertTimeStart = InsertTimeStart,
				InsertTimeEnd = InsertTimeEnd,
			};
			return (Invoke(_handle, "ReqQryExecOrder", typeof(DeleReqQryExecOrder)) as DeleReqQryExecOrder)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryForQuote(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string ExchangeID = "", string InsertTimeStart = "", string InsertTimeEnd = "")
		{
			CThostFtdcQryForQuoteField struc = new CThostFtdcQryForQuoteField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				ExchangeID = ExchangeID,
				InsertTimeStart = InsertTimeStart,
				InsertTimeEnd = InsertTimeEnd,
			};
			return (Invoke(_handle, "ReqQryForQuote", typeof(DeleReqQryForQuote)) as DeleReqQryForQuote)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryQuote(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string ExchangeID = "", string QuoteSysID = "", string InsertTimeStart = "", string InsertTimeEnd = "")
		{
			CThostFtdcQryQuoteField struc = new CThostFtdcQryQuoteField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				ExchangeID = ExchangeID,
				QuoteSysID = QuoteSysID,
				InsertTimeStart = InsertTimeStart,
				InsertTimeEnd = InsertTimeEnd,
			};
			return (Invoke(_handle, "ReqQryQuote", typeof(DeleReqQryQuote)) as DeleReqQryQuote)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryCombInstrumentGuard(string BrokerID = "", string InstrumentID = "")
		{
			CThostFtdcQryCombInstrumentGuardField struc = new CThostFtdcQryCombInstrumentGuardField
			{
				BrokerID = BrokerID,
				InstrumentID = InstrumentID,
			};
			return (Invoke(_handle, "ReqQryCombInstrumentGuard", typeof(DeleReqQryCombInstrumentGuard)) as DeleReqQryCombInstrumentGuard)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryCombAction(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string ExchangeID = "")
		{
			CThostFtdcQryCombActionField struc = new CThostFtdcQryCombActionField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				ExchangeID = ExchangeID,
			};
			return (Invoke(_handle, "ReqQryCombAction", typeof(DeleReqQryCombAction)) as DeleReqQryCombAction)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryTransferSerial(string BrokerID = "", string AccountID = "", string BankID = "", string CurrencyID = "")
		{
			CThostFtdcQryTransferSerialField struc = new CThostFtdcQryTransferSerialField
			{
				BrokerID = BrokerID,
				AccountID = AccountID,
				BankID = BankID,
				CurrencyID = CurrencyID,
			};
			return (Invoke(_handle, "ReqQryTransferSerial", typeof(DeleReqQryTransferSerial)) as DeleReqQryTransferSerial)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryAccountregister(string BrokerID = "", string AccountID = "", string BankID = "", string BankBranchID = "", string CurrencyID = "")
		{
			CThostFtdcQryAccountregisterField struc = new CThostFtdcQryAccountregisterField
			{
				BrokerID = BrokerID,
				AccountID = AccountID,
				BankID = BankID,
				BankBranchID = BankBranchID,
				CurrencyID = CurrencyID,
			};
			return (Invoke(_handle, "ReqQryAccountregister", typeof(DeleReqQryAccountregister)) as DeleReqQryAccountregister)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryContractBank(string BrokerID = "", string BankID = "", string BankBrchID = "")
		{
			CThostFtdcQryContractBankField struc = new CThostFtdcQryContractBankField
			{
				BrokerID = BrokerID,
				BankID = BankID,
				BankBrchID = BankBrchID,
			};
			return (Invoke(_handle, "ReqQryContractBank", typeof(DeleReqQryContractBank)) as DeleReqQryContractBank)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryParkedOrder(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string ExchangeID = "")
		{
			CThostFtdcQryParkedOrderField struc = new CThostFtdcQryParkedOrderField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				ExchangeID = ExchangeID,
			};
			return (Invoke(_handle, "ReqQryParkedOrder", typeof(DeleReqQryParkedOrder)) as DeleReqQryParkedOrder)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryParkedOrderAction(string BrokerID = "", string InvestorID = "", string InstrumentID = "", string ExchangeID = "")
		{
			CThostFtdcQryParkedOrderActionField struc = new CThostFtdcQryParkedOrderActionField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				InstrumentID = InstrumentID,
				ExchangeID = ExchangeID,
			};
			return (Invoke(_handle, "ReqQryParkedOrderAction", typeof(DeleReqQryParkedOrderAction)) as DeleReqQryParkedOrderAction)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryTradingNotice(string BrokerID = "", string InvestorID = "")
		{
			CThostFtdcQryTradingNoticeField struc = new CThostFtdcQryTradingNoticeField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
			};
			return (Invoke(_handle, "ReqQryTradingNotice", typeof(DeleReqQryTradingNotice)) as DeleReqQryTradingNotice)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryBrokerTradingParams(string BrokerID = "", string InvestorID = "", string CurrencyID = "")
		{
			CThostFtdcQryBrokerTradingParamsField struc = new CThostFtdcQryBrokerTradingParamsField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
				CurrencyID = CurrencyID,
			};
			return (Invoke(_handle, "ReqQryBrokerTradingParams", typeof(DeleReqQryBrokerTradingParams)) as DeleReqQryBrokerTradingParams)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQryBrokerTradingAlgos(string BrokerID = "", string ExchangeID = "", string InstrumentID = "")
		{
			CThostFtdcQryBrokerTradingAlgosField struc = new CThostFtdcQryBrokerTradingAlgosField
			{
				BrokerID = BrokerID,
				ExchangeID = ExchangeID,
				InstrumentID = InstrumentID,
			};
			return (Invoke(_handle, "ReqQryBrokerTradingAlgos", typeof(DeleReqQryBrokerTradingAlgos)) as DeleReqQryBrokerTradingAlgos)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQueryCFMMCTradingAccountToken(string BrokerID = "", string InvestorID = "")
		{
			CThostFtdcQueryCFMMCTradingAccountTokenField struc = new CThostFtdcQueryCFMMCTradingAccountTokenField
			{
				BrokerID = BrokerID,
				InvestorID = InvestorID,
			};
			return (Invoke(_handle, "ReqQueryCFMMCTradingAccountToken", typeof(DeleReqQueryCFMMCTradingAccountToken)) as DeleReqQueryCFMMCTradingAccountToken)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqFromBankToFutureByFuture(string TradeCode = "", string BankID = "", string BankBranchID = "", string BrokerID = "", string BrokerBranchID = "", string TradeDate = "", string TradeTime = "", string BankSerial = "", string TradingDay = "", int PlateSerial = 0, TThostFtdcLastFragmentType LastFragment = TThostFtdcLastFragmentType.THOST_FTDC_LF_Yes, int SessionID = 0, string CustomerName = "", TThostFtdcIdCardTypeType IdCardType = TThostFtdcIdCardTypeType.THOST_FTDC_ICT_EID, string IdentifiedCardNo = "", TThostFtdcCustTypeType CustType = TThostFtdcCustTypeType.THOST_FTDC_CUSTT_Person, string BankAccount = "", string BankPassWord = "", string AccountID = "", string Password = "", int InstallID = 0, int FutureSerial = 0, string UserID = "", TThostFtdcYesNoIndicatorType VerifyCertNoFlag = TThostFtdcYesNoIndicatorType.THOST_FTDC_YNI_Yes, string CurrencyID = "", double TradeAmount = 0, double FutureFetchAmount = 0, TThostFtdcFeePayFlagType FeePayFlag = TThostFtdcFeePayFlagType.THOST_FTDC_FPF_BEN, double CustFee = 0, double BrokerFee = 0, string Message = "", string Digest = "", TThostFtdcBankAccTypeType BankAccType = TThostFtdcBankAccTypeType.THOST_FTDC_BAT_BankBook, string DeviceID = "", TThostFtdcBankAccTypeType BankSecuAccType = TThostFtdcBankAccTypeType.THOST_FTDC_BAT_BankBook, string BrokerIDByBank = "", string BankSecuAcc = "", TThostFtdcPwdFlagType BankPwdFlag = TThostFtdcPwdFlagType.THOST_FTDC_BPWDF_NoCheck, TThostFtdcPwdFlagType SecuPwdFlag = TThostFtdcPwdFlagType.THOST_FTDC_BPWDF_NoCheck, string OperNo = "", int RequestID = 0, int TID = 0, TThostFtdcTransferStatusType TransferStatus = TThostFtdcTransferStatusType.THOST_FTDC_TRFS_Normal, string LongCustomerName = "")
		{
			CThostFtdcReqTransferField struc = new CThostFtdcReqTransferField
			{
				TradeCode = TradeCode,
				BankID = BankID,
				BankBranchID = BankBranchID,
				BrokerID = BrokerID,
				BrokerBranchID = BrokerBranchID,
				TradeDate = TradeDate,
				TradeTime = TradeTime,
				BankSerial = BankSerial,
				TradingDay = TradingDay,
				PlateSerial = PlateSerial,

				LastFragment = LastFragment,
				SessionID = SessionID,

				CustomerName = CustomerName,
				IdCardType = IdCardType,
				IdentifiedCardNo = IdentifiedCardNo,
				CustType = CustType,
				BankAccount = BankAccount,
				BankPassWord = BankPassWord,
				AccountID = AccountID,
				Password = Password,
				InstallID = InstallID,
				FutureSerial = FutureSerial,

				UserID = UserID,
				VerifyCertNoFlag = VerifyCertNoFlag,
				CurrencyID = CurrencyID,
				TradeAmount = TradeAmount,
				FutureFetchAmount = FutureFetchAmount,

				FeePayFlag = FeePayFlag,
				CustFee = CustFee,
				BrokerFee = BrokerFee,

				Message = Message,
				Digest = Digest,
				BankAccType = BankAccType,
				DeviceID = DeviceID,
				BankSecuAccType = BankSecuAccType,
				BrokerIDByBank = BrokerIDByBank,
				BankSecuAcc = BankSecuAcc,
				BankPwdFlag = BankPwdFlag,
				SecuPwdFlag = SecuPwdFlag,
				OperNo = OperNo,
				RequestID = RequestID,
				TID = TID,

				TransferStatus = TransferStatus,
				LongCustomerName = LongCustomerName,
			};
			return (Invoke(_handle, "ReqFromBankToFutureByFuture", typeof(DeleReqFromBankToFutureByFuture)) as DeleReqFromBankToFutureByFuture)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqFromFutureToBankByFuture(string TradeCode = "", string BankID = "", string BankBranchID = "", string BrokerID = "", string BrokerBranchID = "", string TradeDate = "", string TradeTime = "", string BankSerial = "", string TradingDay = "", int PlateSerial = 0, TThostFtdcLastFragmentType LastFragment = TThostFtdcLastFragmentType.THOST_FTDC_LF_Yes, int SessionID = 0, string CustomerName = "", TThostFtdcIdCardTypeType IdCardType = TThostFtdcIdCardTypeType.THOST_FTDC_ICT_EID, string IdentifiedCardNo = "", TThostFtdcCustTypeType CustType = TThostFtdcCustTypeType.THOST_FTDC_CUSTT_Person, string BankAccount = "", string BankPassWord = "", string AccountID = "", string Password = "", int InstallID = 0, int FutureSerial = 0, string UserID = "", TThostFtdcYesNoIndicatorType VerifyCertNoFlag = TThostFtdcYesNoIndicatorType.THOST_FTDC_YNI_Yes, string CurrencyID = "", double TradeAmount = 0, double FutureFetchAmount = 0, TThostFtdcFeePayFlagType FeePayFlag = TThostFtdcFeePayFlagType.THOST_FTDC_FPF_BEN, double CustFee = 0, double BrokerFee = 0, string Message = "", string Digest = "", TThostFtdcBankAccTypeType BankAccType = TThostFtdcBankAccTypeType.THOST_FTDC_BAT_BankBook, string DeviceID = "", TThostFtdcBankAccTypeType BankSecuAccType = TThostFtdcBankAccTypeType.THOST_FTDC_BAT_BankBook, string BrokerIDByBank = "", string BankSecuAcc = "", TThostFtdcPwdFlagType BankPwdFlag = TThostFtdcPwdFlagType.THOST_FTDC_BPWDF_NoCheck, TThostFtdcPwdFlagType SecuPwdFlag = TThostFtdcPwdFlagType.THOST_FTDC_BPWDF_NoCheck, string OperNo = "", int RequestID = 0, int TID = 0, TThostFtdcTransferStatusType TransferStatus = TThostFtdcTransferStatusType.THOST_FTDC_TRFS_Normal, string LongCustomerName = "")
		{
			CThostFtdcReqTransferField struc = new CThostFtdcReqTransferField
			{
				TradeCode = TradeCode,
				BankID = BankID,
				BankBranchID = BankBranchID,
				BrokerID = BrokerID,
				BrokerBranchID = BrokerBranchID,
				TradeDate = TradeDate,
				TradeTime = TradeTime,
				BankSerial = BankSerial,
				TradingDay = TradingDay,
				PlateSerial = PlateSerial,

				LastFragment = LastFragment,
				SessionID = SessionID,

				CustomerName = CustomerName,
				IdCardType = IdCardType,
				IdentifiedCardNo = IdentifiedCardNo,
				CustType = CustType,
				BankAccount = BankAccount,
				BankPassWord = BankPassWord,
				AccountID = AccountID,
				Password = Password,
				InstallID = InstallID,
				FutureSerial = FutureSerial,

				UserID = UserID,
				VerifyCertNoFlag = VerifyCertNoFlag,
				CurrencyID = CurrencyID,
				TradeAmount = TradeAmount,
				FutureFetchAmount = FutureFetchAmount,

				FeePayFlag = FeePayFlag,
				CustFee = CustFee,
				BrokerFee = BrokerFee,

				Message = Message,
				Digest = Digest,
				BankAccType = BankAccType,
				DeviceID = DeviceID,
				BankSecuAccType = BankSecuAccType,
				BrokerIDByBank = BrokerIDByBank,
				BankSecuAcc = BankSecuAcc,
				BankPwdFlag = BankPwdFlag,
				SecuPwdFlag = SecuPwdFlag,
				OperNo = OperNo,
				RequestID = RequestID,
				TID = TID,

				TransferStatus = TransferStatus,
				LongCustomerName = LongCustomerName,
			};
			return (Invoke(_handle, "ReqFromFutureToBankByFuture", typeof(DeleReqFromFutureToBankByFuture)) as DeleReqFromFutureToBankByFuture)(_api, struc, this.nRequestID++);
		}

		public IntPtr ReqQueryBankAccountMoneyByFuture(string TradeCode = "", string BankID = "", string BankBranchID = "", string BrokerID = "", string BrokerBranchID = "", string TradeDate = "", string TradeTime = "", string BankSerial = "", string TradingDay = "", int PlateSerial = 0, TThostFtdcLastFragmentType LastFragment = TThostFtdcLastFragmentType.THOST_FTDC_LF_Yes, int SessionID = 0, string CustomerName = "", TThostFtdcIdCardTypeType IdCardType = TThostFtdcIdCardTypeType.THOST_FTDC_ICT_EID, string IdentifiedCardNo = "", TThostFtdcCustTypeType CustType = TThostFtdcCustTypeType.THOST_FTDC_CUSTT_Person, string BankAccount = "", string BankPassWord = "", string AccountID = "", string Password = "", int FutureSerial = 0, int InstallID = 0, string UserID = "", TThostFtdcYesNoIndicatorType VerifyCertNoFlag = TThostFtdcYesNoIndicatorType.THOST_FTDC_YNI_Yes, string CurrencyID = "", string Digest = "", TThostFtdcBankAccTypeType BankAccType = TThostFtdcBankAccTypeType.THOST_FTDC_BAT_BankBook, string DeviceID = "", TThostFtdcBankAccTypeType BankSecuAccType = TThostFtdcBankAccTypeType.THOST_FTDC_BAT_BankBook, string BrokerIDByBank = "", string BankSecuAcc = "", TThostFtdcPwdFlagType BankPwdFlag = TThostFtdcPwdFlagType.THOST_FTDC_BPWDF_NoCheck, TThostFtdcPwdFlagType SecuPwdFlag = TThostFtdcPwdFlagType.THOST_FTDC_BPWDF_NoCheck, string OperNo = "", int RequestID = 0, int TID = 0, string LongCustomerName = "")
		{
			CThostFtdcReqQueryAccountField struc = new CThostFtdcReqQueryAccountField
			{
				TradeCode = TradeCode,
				BankID = BankID,
				BankBranchID = BankBranchID,
				BrokerID = BrokerID,
				BrokerBranchID = BrokerBranchID,
				TradeDate = TradeDate,
				TradeTime = TradeTime,
				BankSerial = BankSerial,
				TradingDay = TradingDay,
				PlateSerial = PlateSerial,

				LastFragment = LastFragment,
				SessionID = SessionID,

				CustomerName = CustomerName,
				IdCardType = IdCardType,
				IdentifiedCardNo = IdentifiedCardNo,
				CustType = CustType,
				BankAccount = BankAccount,
				BankPassWord = BankPassWord,
				AccountID = AccountID,
				Password = Password,
				FutureSerial = FutureSerial,
				InstallID = InstallID,

				UserID = UserID,
				VerifyCertNoFlag = VerifyCertNoFlag,
				CurrencyID = CurrencyID,
				Digest = Digest,
				BankAccType = BankAccType,
				DeviceID = DeviceID,
				BankSecuAccType = BankSecuAccType,
				BrokerIDByBank = BrokerIDByBank,
				BankSecuAcc = BankSecuAcc,
				BankPwdFlag = BankPwdFlag,
				SecuPwdFlag = SecuPwdFlag,
				OperNo = OperNo,
				RequestID = RequestID,
				TID = TID,

				LongCustomerName = LongCustomerName,
			};
			return (Invoke(_handle, "ReqQueryBankAccountMoneyByFuture", typeof(DeleReqQueryBankAccountMoneyByFuture)) as DeleReqQueryBankAccountMoneyByFuture)(_api, struc, this.nRequestID++);
		}

		#endregion

		delegate void DeleSet(IntPtr spi, Delegate func);

		public delegate void DeleOnFrontConnected();
		public void SetOnFrontConnected(DeleOnFrontConnected func) { (Invoke(_handle, "SetOnFrontConnected", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnFrontDisconnected(int nReason);
		public void SetOnFrontDisconnected(DeleOnFrontDisconnected func) { (Invoke(_handle, "SetOnFrontDisconnected", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnHeartBeatWarning(int nTimeLapse);
		public void SetOnHeartBeatWarning(DeleOnHeartBeatWarning func) { (Invoke(_handle, "SetOnHeartBeatWarning", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspAuthenticate(ref CThostFtdcRspAuthenticateField pRspAuthenticateField, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspAuthenticate(DeleOnRspAuthenticate func) { (Invoke(_handle, "SetOnRspAuthenticate", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspUserLogin(ref CThostFtdcRspUserLoginField pRspUserLogin, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspUserLogin(DeleOnRspUserLogin func) { (Invoke(_handle, "SetOnRspUserLogin", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspUserLogout(ref CThostFtdcUserLogoutField pUserLogout, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspUserLogout(DeleOnRspUserLogout func) { (Invoke(_handle, "SetOnRspUserLogout", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspUserPasswordUpdate(ref CThostFtdcUserPasswordUpdateField pUserPasswordUpdate, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspUserPasswordUpdate(DeleOnRspUserPasswordUpdate func) { (Invoke(_handle, "SetOnRspUserPasswordUpdate", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspTradingAccountPasswordUpdate(ref CThostFtdcTradingAccountPasswordUpdateField pTradingAccountPasswordUpdate, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspTradingAccountPasswordUpdate(DeleOnRspTradingAccountPasswordUpdate func) { (Invoke(_handle, "SetOnRspTradingAccountPasswordUpdate", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspOrderInsert(ref CThostFtdcInputOrderField pInputOrder, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspOrderInsert(DeleOnRspOrderInsert func) { (Invoke(_handle, "SetOnRspOrderInsert", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspParkedOrderInsert(ref CThostFtdcParkedOrderField pParkedOrder, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspParkedOrderInsert(DeleOnRspParkedOrderInsert func) { (Invoke(_handle, "SetOnRspParkedOrderInsert", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspParkedOrderAction(ref CThostFtdcParkedOrderActionField pParkedOrderAction, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspParkedOrderAction(DeleOnRspParkedOrderAction func) { (Invoke(_handle, "SetOnRspParkedOrderAction", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspOrderAction(ref CThostFtdcInputOrderActionField pInputOrderAction, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspOrderAction(DeleOnRspOrderAction func) { (Invoke(_handle, "SetOnRspOrderAction", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQueryMaxOrderVolume(ref CThostFtdcQueryMaxOrderVolumeField pQueryMaxOrderVolume, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQueryMaxOrderVolume(DeleOnRspQueryMaxOrderVolume func) { (Invoke(_handle, "SetOnRspQueryMaxOrderVolume", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspSettlementInfoConfirm(ref CThostFtdcSettlementInfoConfirmField pSettlementInfoConfirm, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspSettlementInfoConfirm(DeleOnRspSettlementInfoConfirm func) { (Invoke(_handle, "SetOnRspSettlementInfoConfirm", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspRemoveParkedOrder(ref CThostFtdcRemoveParkedOrderField pRemoveParkedOrder, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspRemoveParkedOrder(DeleOnRspRemoveParkedOrder func) { (Invoke(_handle, "SetOnRspRemoveParkedOrder", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspRemoveParkedOrderAction(ref CThostFtdcRemoveParkedOrderActionField pRemoveParkedOrderAction, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspRemoveParkedOrderAction(DeleOnRspRemoveParkedOrderAction func) { (Invoke(_handle, "SetOnRspRemoveParkedOrderAction", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspExecOrderInsert(ref CThostFtdcInputExecOrderField pInputExecOrder, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspExecOrderInsert(DeleOnRspExecOrderInsert func) { (Invoke(_handle, "SetOnRspExecOrderInsert", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspExecOrderAction(ref CThostFtdcInputExecOrderActionField pInputExecOrderAction, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspExecOrderAction(DeleOnRspExecOrderAction func) { (Invoke(_handle, "SetOnRspExecOrderAction", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspForQuoteInsert(ref CThostFtdcInputForQuoteField pInputForQuote, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspForQuoteInsert(DeleOnRspForQuoteInsert func) { (Invoke(_handle, "SetOnRspForQuoteInsert", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQuoteInsert(ref CThostFtdcInputQuoteField pInputQuote, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQuoteInsert(DeleOnRspQuoteInsert func) { (Invoke(_handle, "SetOnRspQuoteInsert", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQuoteAction(ref CThostFtdcInputQuoteActionField pInputQuoteAction, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQuoteAction(DeleOnRspQuoteAction func) { (Invoke(_handle, "SetOnRspQuoteAction", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspBatchOrderAction(ref CThostFtdcInputBatchOrderActionField pInputBatchOrderAction, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspBatchOrderAction(DeleOnRspBatchOrderAction func) { (Invoke(_handle, "SetOnRspBatchOrderAction", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspCombActionInsert(ref CThostFtdcInputCombActionField pInputCombAction, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspCombActionInsert(DeleOnRspCombActionInsert func) { (Invoke(_handle, "SetOnRspCombActionInsert", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryOrder(ref CThostFtdcOrderField pOrder, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryOrder(DeleOnRspQryOrder func) { (Invoke(_handle, "SetOnRspQryOrder", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryTrade(ref CThostFtdcTradeField pTrade, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryTrade(DeleOnRspQryTrade func) { (Invoke(_handle, "SetOnRspQryTrade", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryInvestorPosition(ref CThostFtdcInvestorPositionField pInvestorPosition, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryInvestorPosition(DeleOnRspQryInvestorPosition func) { (Invoke(_handle, "SetOnRspQryInvestorPosition", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryTradingAccount(ref CThostFtdcTradingAccountField pTradingAccount, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryTradingAccount(DeleOnRspQryTradingAccount func) { (Invoke(_handle, "SetOnRspQryTradingAccount", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryInvestor(ref CThostFtdcInvestorField pInvestor, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryInvestor(DeleOnRspQryInvestor func) { (Invoke(_handle, "SetOnRspQryInvestor", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryTradingCode(ref CThostFtdcTradingCodeField pTradingCode, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryTradingCode(DeleOnRspQryTradingCode func) { (Invoke(_handle, "SetOnRspQryTradingCode", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryInstrumentMarginRate(ref CThostFtdcInstrumentMarginRateField pInstrumentMarginRate, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryInstrumentMarginRate(DeleOnRspQryInstrumentMarginRate func) { (Invoke(_handle, "SetOnRspQryInstrumentMarginRate", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryInstrumentCommissionRate(ref CThostFtdcInstrumentCommissionRateField pInstrumentCommissionRate, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryInstrumentCommissionRate(DeleOnRspQryInstrumentCommissionRate func) { (Invoke(_handle, "SetOnRspQryInstrumentCommissionRate", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryExchange(ref CThostFtdcExchangeField pExchange, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryExchange(DeleOnRspQryExchange func) { (Invoke(_handle, "SetOnRspQryExchange", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryProduct(ref CThostFtdcProductField pProduct, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryProduct(DeleOnRspQryProduct func) { (Invoke(_handle, "SetOnRspQryProduct", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryInstrument(ref CThostFtdcInstrumentField pInstrument, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryInstrument(DeleOnRspQryInstrument func) { (Invoke(_handle, "SetOnRspQryInstrument", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryDepthMarketData(ref CThostFtdcDepthMarketDataField pDepthMarketData, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryDepthMarketData(DeleOnRspQryDepthMarketData func) { (Invoke(_handle, "SetOnRspQryDepthMarketData", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQrySettlementInfo(ref CThostFtdcSettlementInfoField pSettlementInfo, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQrySettlementInfo(DeleOnRspQrySettlementInfo func) { (Invoke(_handle, "SetOnRspQrySettlementInfo", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryTransferBank(ref CThostFtdcTransferBankField pTransferBank, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryTransferBank(DeleOnRspQryTransferBank func) { (Invoke(_handle, "SetOnRspQryTransferBank", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryInvestorPositionDetail(ref CThostFtdcInvestorPositionDetailField pInvestorPositionDetail, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryInvestorPositionDetail(DeleOnRspQryInvestorPositionDetail func) { (Invoke(_handle, "SetOnRspQryInvestorPositionDetail", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryNotice(ref CThostFtdcNoticeField pNotice, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryNotice(DeleOnRspQryNotice func) { (Invoke(_handle, "SetOnRspQryNotice", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQrySettlementInfoConfirm(ref CThostFtdcSettlementInfoConfirmField pSettlementInfoConfirm, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQrySettlementInfoConfirm(DeleOnRspQrySettlementInfoConfirm func) { (Invoke(_handle, "SetOnRspQrySettlementInfoConfirm", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryInvestorPositionCombineDetail(ref CThostFtdcInvestorPositionCombineDetailField pInvestorPositionCombineDetail, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryInvestorPositionCombineDetail(DeleOnRspQryInvestorPositionCombineDetail func) { (Invoke(_handle, "SetOnRspQryInvestorPositionCombineDetail", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryCFMMCTradingAccountKey(ref CThostFtdcCFMMCTradingAccountKeyField pCFMMCTradingAccountKey, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryCFMMCTradingAccountKey(DeleOnRspQryCFMMCTradingAccountKey func) { (Invoke(_handle, "SetOnRspQryCFMMCTradingAccountKey", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryEWarrantOffset(ref CThostFtdcEWarrantOffsetField pEWarrantOffset, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryEWarrantOffset(DeleOnRspQryEWarrantOffset func) { (Invoke(_handle, "SetOnRspQryEWarrantOffset", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryInvestorProductGroupMargin(ref CThostFtdcInvestorProductGroupMarginField pInvestorProductGroupMargin, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryInvestorProductGroupMargin(DeleOnRspQryInvestorProductGroupMargin func) { (Invoke(_handle, "SetOnRspQryInvestorProductGroupMargin", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryExchangeMarginRate(ref CThostFtdcExchangeMarginRateField pExchangeMarginRate, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryExchangeMarginRate(DeleOnRspQryExchangeMarginRate func) { (Invoke(_handle, "SetOnRspQryExchangeMarginRate", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryExchangeMarginRateAdjust(ref CThostFtdcExchangeMarginRateAdjustField pExchangeMarginRateAdjust, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryExchangeMarginRateAdjust(DeleOnRspQryExchangeMarginRateAdjust func) { (Invoke(_handle, "SetOnRspQryExchangeMarginRateAdjust", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryExchangeRate(ref CThostFtdcExchangeRateField pExchangeRate, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryExchangeRate(DeleOnRspQryExchangeRate func) { (Invoke(_handle, "SetOnRspQryExchangeRate", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQrySecAgentACIDMap(ref CThostFtdcSecAgentACIDMapField pSecAgentACIDMap, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQrySecAgentACIDMap(DeleOnRspQrySecAgentACIDMap func) { (Invoke(_handle, "SetOnRspQrySecAgentACIDMap", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryProductExchRate(ref CThostFtdcProductExchRateField pProductExchRate, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryProductExchRate(DeleOnRspQryProductExchRate func) { (Invoke(_handle, "SetOnRspQryProductExchRate", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryProductGroup(ref CThostFtdcProductGroupField pProductGroup, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryProductGroup(DeleOnRspQryProductGroup func) { (Invoke(_handle, "SetOnRspQryProductGroup", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryMMInstrumentCommissionRate(ref CThostFtdcMMInstrumentCommissionRateField pMMInstrumentCommissionRate, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryMMInstrumentCommissionRate(DeleOnRspQryMMInstrumentCommissionRate func) { (Invoke(_handle, "SetOnRspQryMMInstrumentCommissionRate", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryMMOptionInstrCommRate(ref CThostFtdcMMOptionInstrCommRateField pMMOptionInstrCommRate, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryMMOptionInstrCommRate(DeleOnRspQryMMOptionInstrCommRate func) { (Invoke(_handle, "SetOnRspQryMMOptionInstrCommRate", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryInstrumentOrderCommRate(ref CThostFtdcInstrumentOrderCommRateField pInstrumentOrderCommRate, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryInstrumentOrderCommRate(DeleOnRspQryInstrumentOrderCommRate func) { (Invoke(_handle, "SetOnRspQryInstrumentOrderCommRate", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryOptionInstrTradeCost(ref CThostFtdcOptionInstrTradeCostField pOptionInstrTradeCost, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryOptionInstrTradeCost(DeleOnRspQryOptionInstrTradeCost func) { (Invoke(_handle, "SetOnRspQryOptionInstrTradeCost", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryOptionInstrCommRate(ref CThostFtdcOptionInstrCommRateField pOptionInstrCommRate, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryOptionInstrCommRate(DeleOnRspQryOptionInstrCommRate func) { (Invoke(_handle, "SetOnRspQryOptionInstrCommRate", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryExecOrder(ref CThostFtdcExecOrderField pExecOrder, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryExecOrder(DeleOnRspQryExecOrder func) { (Invoke(_handle, "SetOnRspQryExecOrder", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryForQuote(ref CThostFtdcForQuoteField pForQuote, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryForQuote(DeleOnRspQryForQuote func) { (Invoke(_handle, "SetOnRspQryForQuote", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryQuote(ref CThostFtdcQuoteField pQuote, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryQuote(DeleOnRspQryQuote func) { (Invoke(_handle, "SetOnRspQryQuote", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryCombInstrumentGuard(ref CThostFtdcCombInstrumentGuardField pCombInstrumentGuard, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryCombInstrumentGuard(DeleOnRspQryCombInstrumentGuard func) { (Invoke(_handle, "SetOnRspQryCombInstrumentGuard", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryCombAction(ref CThostFtdcCombActionField pCombAction, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryCombAction(DeleOnRspQryCombAction func) { (Invoke(_handle, "SetOnRspQryCombAction", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryTransferSerial(ref CThostFtdcTransferSerialField pTransferSerial, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryTransferSerial(DeleOnRspQryTransferSerial func) { (Invoke(_handle, "SetOnRspQryTransferSerial", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryAccountregister(ref CThostFtdcAccountregisterField pAccountregister, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryAccountregister(DeleOnRspQryAccountregister func) { (Invoke(_handle, "SetOnRspQryAccountregister", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspError(ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspError(DeleOnRspError func) { (Invoke(_handle, "SetOnRspError", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnOrder(ref CThostFtdcOrderField pOrder);
		public void SetOnRtnOrder(DeleOnRtnOrder func) { (Invoke(_handle, "SetOnRtnOrder", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnTrade(ref CThostFtdcTradeField pTrade);
		public void SetOnRtnTrade(DeleOnRtnTrade func) { (Invoke(_handle, "SetOnRtnTrade", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnOrderInsert(ref CThostFtdcInputOrderField pInputOrder, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnOrderInsert(DeleOnErrRtnOrderInsert func) { (Invoke(_handle, "SetOnErrRtnOrderInsert", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnOrderAction(ref CThostFtdcOrderActionField pOrderAction, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnOrderAction(DeleOnErrRtnOrderAction func) { (Invoke(_handle, "SetOnErrRtnOrderAction", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnInstrumentStatus(ref CThostFtdcInstrumentStatusField pInstrumentStatus);
		public void SetOnRtnInstrumentStatus(DeleOnRtnInstrumentStatus func) { (Invoke(_handle, "SetOnRtnInstrumentStatus", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnBulletin(ref CThostFtdcBulletinField pBulletin);
		public void SetOnRtnBulletin(DeleOnRtnBulletin func) { (Invoke(_handle, "SetOnRtnBulletin", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnTradingNotice(ref CThostFtdcTradingNoticeInfoField pTradingNoticeInfo);
		public void SetOnRtnTradingNotice(DeleOnRtnTradingNotice func) { (Invoke(_handle, "SetOnRtnTradingNotice", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnErrorConditionalOrder(ref CThostFtdcErrorConditionalOrderField pErrorConditionalOrder);
		public void SetOnRtnErrorConditionalOrder(DeleOnRtnErrorConditionalOrder func) { (Invoke(_handle, "SetOnRtnErrorConditionalOrder", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnExecOrder(ref CThostFtdcExecOrderField pExecOrder);
		public void SetOnRtnExecOrder(DeleOnRtnExecOrder func) { (Invoke(_handle, "SetOnRtnExecOrder", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnExecOrderInsert(ref CThostFtdcInputExecOrderField pInputExecOrder, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnExecOrderInsert(DeleOnErrRtnExecOrderInsert func) { (Invoke(_handle, "SetOnErrRtnExecOrderInsert", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnExecOrderAction(ref CThostFtdcExecOrderActionField pExecOrderAction, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnExecOrderAction(DeleOnErrRtnExecOrderAction func) { (Invoke(_handle, "SetOnErrRtnExecOrderAction", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnForQuoteInsert(ref CThostFtdcInputForQuoteField pInputForQuote, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnForQuoteInsert(DeleOnErrRtnForQuoteInsert func) { (Invoke(_handle, "SetOnErrRtnForQuoteInsert", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnQuote(ref CThostFtdcQuoteField pQuote);
		public void SetOnRtnQuote(DeleOnRtnQuote func) { (Invoke(_handle, "SetOnRtnQuote", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnQuoteInsert(ref CThostFtdcInputQuoteField pInputQuote, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnQuoteInsert(DeleOnErrRtnQuoteInsert func) { (Invoke(_handle, "SetOnErrRtnQuoteInsert", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnQuoteAction(ref CThostFtdcQuoteActionField pQuoteAction, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnQuoteAction(DeleOnErrRtnQuoteAction func) { (Invoke(_handle, "SetOnErrRtnQuoteAction", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnForQuoteRsp(ref CThostFtdcForQuoteRspField pForQuoteRsp);
		public void SetOnRtnForQuoteRsp(DeleOnRtnForQuoteRsp func) { (Invoke(_handle, "SetOnRtnForQuoteRsp", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnCFMMCTradingAccountToken(ref CThostFtdcCFMMCTradingAccountTokenField pCFMMCTradingAccountToken);
		public void SetOnRtnCFMMCTradingAccountToken(DeleOnRtnCFMMCTradingAccountToken func) { (Invoke(_handle, "SetOnRtnCFMMCTradingAccountToken", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnBatchOrderAction(ref CThostFtdcBatchOrderActionField pBatchOrderAction, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnBatchOrderAction(DeleOnErrRtnBatchOrderAction func) { (Invoke(_handle, "SetOnErrRtnBatchOrderAction", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnCombAction(ref CThostFtdcCombActionField pCombAction);
		public void SetOnRtnCombAction(DeleOnRtnCombAction func) { (Invoke(_handle, "SetOnRtnCombAction", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnCombActionInsert(ref CThostFtdcInputCombActionField pInputCombAction, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnCombActionInsert(DeleOnErrRtnCombActionInsert func) { (Invoke(_handle, "SetOnErrRtnCombActionInsert", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryContractBank(ref CThostFtdcContractBankField pContractBank, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryContractBank(DeleOnRspQryContractBank func) { (Invoke(_handle, "SetOnRspQryContractBank", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryParkedOrder(ref CThostFtdcParkedOrderField pParkedOrder, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryParkedOrder(DeleOnRspQryParkedOrder func) { (Invoke(_handle, "SetOnRspQryParkedOrder", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryParkedOrderAction(ref CThostFtdcParkedOrderActionField pParkedOrderAction, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryParkedOrderAction(DeleOnRspQryParkedOrderAction func) { (Invoke(_handle, "SetOnRspQryParkedOrderAction", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryTradingNotice(ref CThostFtdcTradingNoticeField pTradingNotice, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryTradingNotice(DeleOnRspQryTradingNotice func) { (Invoke(_handle, "SetOnRspQryTradingNotice", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryBrokerTradingParams(ref CThostFtdcBrokerTradingParamsField pBrokerTradingParams, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryBrokerTradingParams(DeleOnRspQryBrokerTradingParams func) { (Invoke(_handle, "SetOnRspQryBrokerTradingParams", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQryBrokerTradingAlgos(ref CThostFtdcBrokerTradingAlgosField pBrokerTradingAlgos, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQryBrokerTradingAlgos(DeleOnRspQryBrokerTradingAlgos func) { (Invoke(_handle, "SetOnRspQryBrokerTradingAlgos", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQueryCFMMCTradingAccountToken(ref CThostFtdcQueryCFMMCTradingAccountTokenField pQueryCFMMCTradingAccountToken, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQueryCFMMCTradingAccountToken(DeleOnRspQueryCFMMCTradingAccountToken func) { (Invoke(_handle, "SetOnRspQueryCFMMCTradingAccountToken", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnFromBankToFutureByBank(ref CThostFtdcRspTransferField pRspTransfer);
		public void SetOnRtnFromBankToFutureByBank(DeleOnRtnFromBankToFutureByBank func) { (Invoke(_handle, "SetOnRtnFromBankToFutureByBank", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnFromFutureToBankByBank(ref CThostFtdcRspTransferField pRspTransfer);
		public void SetOnRtnFromFutureToBankByBank(DeleOnRtnFromFutureToBankByBank func) { (Invoke(_handle, "SetOnRtnFromFutureToBankByBank", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnRepealFromBankToFutureByBank(ref CThostFtdcRspRepealField pRspRepeal);
		public void SetOnRtnRepealFromBankToFutureByBank(DeleOnRtnRepealFromBankToFutureByBank func) { (Invoke(_handle, "SetOnRtnRepealFromBankToFutureByBank", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnRepealFromFutureToBankByBank(ref CThostFtdcRspRepealField pRspRepeal);
		public void SetOnRtnRepealFromFutureToBankByBank(DeleOnRtnRepealFromFutureToBankByBank func) { (Invoke(_handle, "SetOnRtnRepealFromFutureToBankByBank", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnFromBankToFutureByFuture(ref CThostFtdcRspTransferField pRspTransfer);
		public void SetOnRtnFromBankToFutureByFuture(DeleOnRtnFromBankToFutureByFuture func) { (Invoke(_handle, "SetOnRtnFromBankToFutureByFuture", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnFromFutureToBankByFuture(ref CThostFtdcRspTransferField pRspTransfer);
		public void SetOnRtnFromFutureToBankByFuture(DeleOnRtnFromFutureToBankByFuture func) { (Invoke(_handle, "SetOnRtnFromFutureToBankByFuture", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnRepealFromBankToFutureByFutureManual(ref CThostFtdcRspRepealField pRspRepeal);
		public void SetOnRtnRepealFromBankToFutureByFutureManual(DeleOnRtnRepealFromBankToFutureByFutureManual func) { (Invoke(_handle, "SetOnRtnRepealFromBankToFutureByFutureManual", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnRepealFromFutureToBankByFutureManual(ref CThostFtdcRspRepealField pRspRepeal);
		public void SetOnRtnRepealFromFutureToBankByFutureManual(DeleOnRtnRepealFromFutureToBankByFutureManual func) { (Invoke(_handle, "SetOnRtnRepealFromFutureToBankByFutureManual", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnQueryBankBalanceByFuture(ref CThostFtdcNotifyQueryAccountField pNotifyQueryAccount);
		public void SetOnRtnQueryBankBalanceByFuture(DeleOnRtnQueryBankBalanceByFuture func) { (Invoke(_handle, "SetOnRtnQueryBankBalanceByFuture", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnBankToFutureByFuture(ref CThostFtdcReqTransferField pReqTransfer, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnBankToFutureByFuture(DeleOnErrRtnBankToFutureByFuture func) { (Invoke(_handle, "SetOnErrRtnBankToFutureByFuture", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnFutureToBankByFuture(ref CThostFtdcReqTransferField pReqTransfer, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnFutureToBankByFuture(DeleOnErrRtnFutureToBankByFuture func) { (Invoke(_handle, "SetOnErrRtnFutureToBankByFuture", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnRepealBankToFutureByFutureManual(ref CThostFtdcReqRepealField pReqRepeal, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnRepealBankToFutureByFutureManual(DeleOnErrRtnRepealBankToFutureByFutureManual func) { (Invoke(_handle, "SetOnErrRtnRepealBankToFutureByFutureManual", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnRepealFutureToBankByFutureManual(ref CThostFtdcReqRepealField pReqRepeal, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnRepealFutureToBankByFutureManual(DeleOnErrRtnRepealFutureToBankByFutureManual func) { (Invoke(_handle, "SetOnErrRtnRepealFutureToBankByFutureManual", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnErrRtnQueryBankBalanceByFuture(ref CThostFtdcReqQueryAccountField pReqQueryAccount, ref CThostFtdcRspInfoField pRspInfo);
		public void SetOnErrRtnQueryBankBalanceByFuture(DeleOnErrRtnQueryBankBalanceByFuture func) { (Invoke(_handle, "SetOnErrRtnQueryBankBalanceByFuture", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnRepealFromBankToFutureByFuture(ref CThostFtdcRspRepealField pRspRepeal);
		public void SetOnRtnRepealFromBankToFutureByFuture(DeleOnRtnRepealFromBankToFutureByFuture func) { (Invoke(_handle, "SetOnRtnRepealFromBankToFutureByFuture", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnRepealFromFutureToBankByFuture(ref CThostFtdcRspRepealField pRspRepeal);
		public void SetOnRtnRepealFromFutureToBankByFuture(DeleOnRtnRepealFromFutureToBankByFuture func) { (Invoke(_handle, "SetOnRtnRepealFromFutureToBankByFuture", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspFromBankToFutureByFuture(ref CThostFtdcReqTransferField pReqTransfer, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspFromBankToFutureByFuture(DeleOnRspFromBankToFutureByFuture func) { (Invoke(_handle, "SetOnRspFromBankToFutureByFuture", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspFromFutureToBankByFuture(ref CThostFtdcReqTransferField pReqTransfer, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspFromFutureToBankByFuture(DeleOnRspFromFutureToBankByFuture func) { (Invoke(_handle, "SetOnRspFromFutureToBankByFuture", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRspQueryBankAccountMoneyByFuture(ref CThostFtdcReqQueryAccountField pReqQueryAccount, ref CThostFtdcRspInfoField pRspInfo, int nRequestID, bool bIsLast);
		public void SetOnRspQueryBankAccountMoneyByFuture(DeleOnRspQueryBankAccountMoneyByFuture func) { (Invoke(_handle, "SetOnRspQueryBankAccountMoneyByFuture", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnOpenAccountByBank(ref CThostFtdcOpenAccountField pOpenAccount);
		public void SetOnRtnOpenAccountByBank(DeleOnRtnOpenAccountByBank func) { (Invoke(_handle, "SetOnRtnOpenAccountByBank", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnCancelAccountByBank(ref CThostFtdcCancelAccountField pCancelAccount);
		public void SetOnRtnCancelAccountByBank(DeleOnRtnCancelAccountByBank func) { (Invoke(_handle, "SetOnRtnCancelAccountByBank", typeof(DeleSet)) as DeleSet)(_spi, func); }
		public delegate void DeleOnRtnChangeAccountByBank(ref CThostFtdcChangeAccountField pChangeAccount);
		public void SetOnRtnChangeAccountByBank(DeleOnRtnChangeAccountByBank func) { (Invoke(_handle, "SetOnRtnChangeAccountByBank", typeof(DeleSet)) as DeleSet)(_spi, func); }
	}
}