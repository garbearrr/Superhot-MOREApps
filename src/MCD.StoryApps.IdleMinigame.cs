extern alias SHSharp;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

using MelonLoader;

using MCD.StoryApps.Common;
using MCD.StoryApps.Views;

using SH.SHGUI.Extended;
using UnityEngine;


namespace MCD.StoryApps.IdleMinigame
{
	public class ActionController : SHSharp::SHGUIview
	{
		// Token: 0x06002509 RID: 9481 RVA: 0x0010D0E4 File Offset: 0x0010B4E4
		public ActionController()
		{
			this.newConvertedQueue = Value.Zero;
			this.hypnosisController = new HypnosisController();
			this.vrBrainwashController = new VrBrainwashController();
			this.Active = true;
			base.AddSubView(this.hypnosisController);
			base.AddSubView(this.vrBrainwashController);
		}

		// Token: 0x1700019E RID: 414
		// (get) Token: 0x0600250A RID: 9482 RVA: 0x0010D192 File Offset: 0x0010B592
		// (set) Token: 0x0600250B RID: 9483 RVA: 0x0010D19A File Offset: 0x0010B59A
		public bool Active { get; set; }

		// Token: 0x0600250C RID: 9484 RVA: 0x0010D1A4 File Offset: 0x0010B5A4
		public override void Update()
		{
			base.Update();
			if (!this.Active)
			{
				return;
			}
			if (this.newConvertedTimer >= this.newConvertedAddTime)
			{
				this.newHotCoins += this.newConvertedSum * SharedTheSystem.General.GetHotCoinsPerConverted();
				if (this.newHotCoins > Value.Zero)
				{
					AppTheSystem.Instance.HotCoinsView.AddFloatingHotCoin(this.newHotCoins);
					this.newHotCoins = Value.Zero;
				}
				this.newConvertedSum = Value.Zero;
				this.newConvertedTimer = 0f;
			}
			if (this.newConvertedQueue > Value.Zero && this.newConvertedQueueTimer >= this.newConvertedQueueTime)
			{
				this.AddConverted(this.queueSingleAdd);
				this.newConvertedQueue -= this.queueSingleAdd;
				this.newConvertedQueueTimer = 0f;
			}
			if (this.newConvertedQueue >= this.queueSingleAddLimit)
			{
				this.queueSingleAdd = this.newConvertedQueue * new Value(0.05f);
				this.queueSingleAddLimit += this.queueStartSingleAddLimit;
			}
			if (this.newConvertedQueue < this.queueStartSingleAddLimit)
			{
				this.queueSingleAdd = new Value(0.1f);
				this.queueSingleAddLimit = this.queueStartSingleAddLimit;
			}
			this.newConvertedTimer += Time.unscaledDeltaTime;
			this.newConvertedQueueTimer += Time.unscaledDeltaTime;
		}

		// Token: 0x0600250D RID: 9485 RVA: 0x0010D337 File Offset: 0x0010B737
		public void AddConverted(Value newConverted)
		{
			SharedTheSystem.General.AddConverted(newConverted);
			this.newConvertedSum += newConverted;
		}

		// Token: 0x0600250E RID: 9486 RVA: 0x0010D356 File Offset: 0x0010B756
		public void AddConvertedToQueue(Value newConverted)
		{
			this.newConvertedQueue += newConverted;
		}

		// Token: 0x0600250F RID: 9487 RVA: 0x0010D36A File Offset: 0x0010B76A
		public void AddHotCoins(Value hotCoins)
		{
			SharedTheSystem.General.AddHotCoins(hotCoins);
			AppTheSystem.Instance.HotCoinsView.UpdateHotCoinsText();
		}

		// Token: 0x06002510 RID: 9488 RVA: 0x0010D386 File Offset: 0x0010B786
		public void SpendHotCoins(Value hotCoins)
		{
			SharedTheSystem.General.AddHotCoins(-hotCoins);
			AppTheSystem.Instance.HotCoinsView.ShowFloatingSpendText(hotCoins);
			AppTheSystem.Instance.HotCoinsView.UpdateHotCoinsText();
		}

		// Token: 0x06002511 RID: 9489 RVA: 0x0010D3B7 File Offset: 0x0010B7B7
		public void ActivateHypnosis()
		{
			this.hypnosisController.Active = true;
		}

		// Token: 0x06002512 RID: 9490 RVA: 0x0010D3C5 File Offset: 0x0010B7C5
		public void ActivateBrainwash()
		{
			this.vrBrainwashController.Active = true;
			AppTheSystem.Instance.HotCoinsView.AddAllHotCoins();
		}

		// Token: 0x06002513 RID: 9491 RVA: 0x0010D3E2 File Offset: 0x0010B7E2
		public void ResetQueues()
		{
			this.newConvertedSum = Value.Zero;
			this.newHotCoins = Value.Zero;
			this.newConvertedQueue = Value.Zero;
		}

		// Token: 0x04002736 RID: 10038
		private Value newHotCoins = Value.Zero;

		// Token: 0x04002737 RID: 10039
		private readonly HypnosisController hypnosisController;

		// Token: 0x04002738 RID: 10040
		private readonly VrBrainwashController vrBrainwashController;

		// Token: 0x04002739 RID: 10041
		private Value newConvertedSum = Value.Zero;

		// Token: 0x0400273A RID: 10042
		private float newConvertedTimer;

		// Token: 0x0400273B RID: 10043
		private float newConvertedAddTime = 1f;

		// Token: 0x0400273C RID: 10044
		private Value newConvertedQueue;

		// Token: 0x0400273D RID: 10045
		private float newConvertedQueueTimer;

		// Token: 0x0400273E RID: 10046
		private float newConvertedQueueTime = 0.01f;

		// Token: 0x0400273F RID: 10047
		private Value queueSingleAdd = Value.Zero;

		// Token: 0x04002740 RID: 10048
		private Value queueSingleAddLimit = new Value(0, 100f);

		// Token: 0x04002741 RID: 10049
		private Value queueStartSingleAddLimit = new Value(0, 100f);
	}

	public class AppTheSystem : SHSharp::SHGUIappbase
	{
        private readonly string filePath = Application.dataPath + "/Resources/StoryApps/Games/IdleMinigame/SaveData/data.hot";
        // Token: 0x06002514 RID: 9492 RVA: 0x0010D408 File Offset: 0x0010B808
        public AppTheSystem() : base("the-system-ported-from-mcd-by-garbear", true)
		{
			try
			{
				if (!File.Exists(filePath))
				{
					SharedTheSystem.ResetProgress();
				}

				NumbersEndings.ReadEndings();
				this.allowCursorDraw = true;
				AppTheSystem.Instance = this;
				this.APPINSTRUCTION.text = string.Empty;
				this.APPLABEL.Kill();
				this.ActionController = new ActionController();
				IncomeController view = new IncomeController();
				this.PopupBonusController = new PopupBonusController();
				// NOTES:
				// It seems to be the AddSubView method. Although nothing happens, using shgui.current.addviewontop does not crash.

				base.AddSubView(view); //TODO: Causes a crash
                base.AddSubView(this.ActionController); //TODO: Causes a crash
                base.AddSubView(this.PopupBonusController); //TODO: Causes a crash
				this.PrepareViews(); //TODO: Causes a crash
                this.ChangeGameState(AppTheSystem.GameState.InMessageSystem);
				
            } catch (Exception ex)
            {
				MelonLogger.Msg(ex.Message);
				MelonLogger.Msg(ex.StackTrace);
			}
		}

		// Token: 0x1700019F RID: 415
		// (get) Token: 0x06002515 RID: 9493 RVA: 0x0010D4C4 File Offset: 0x0010B8C4
		// (set) Token: 0x06002516 RID: 9494 RVA: 0x0010D4CB File Offset: 0x0010B8CB
		public static AppTheSystem Instance { get; private set; }

		// Token: 0x170001A0 RID: 416
		// (get) Token: 0x06002517 RID: 9495 RVA: 0x0010D4D3 File Offset: 0x0010B8D3
		// (set) Token: 0x06002518 RID: 9496 RVA: 0x0010D4DB File Offset: 0x0010B8DB
		public AppTheSystem.GameState CurrentGameState { get; private set; }

		// Token: 0x170001A1 RID: 417
		// (get) Token: 0x06002519 RID: 9497 RVA: 0x0010D4E4 File Offset: 0x0010B8E4
		// (set) Token: 0x0600251A RID: 9498 RVA: 0x0010D4EC File Offset: 0x0010B8EC
		public ChatView ChatView { get; set; }

		// Token: 0x170001A2 RID: 418
		// (get) Token: 0x0600251B RID: 9499 RVA: 0x0010D4F5 File Offset: 0x0010B8F5
		// (set) Token: 0x0600251C RID: 9500 RVA: 0x0010D4FD File Offset: 0x0010B8FD
		public ShopView ShopView { get; private set; }

		// Token: 0x170001A3 RID: 419
		// (get) Token: 0x0600251D RID: 9501 RVA: 0x0010D506 File Offset: 0x0010B906
		// (set) Token: 0x0600251E RID: 9502 RVA: 0x0010D50E File Offset: 0x0010B90E
		public HotCoinsView HotCoinsView { get; private set; }

		// Token: 0x170001A4 RID: 420
		// (get) Token: 0x0600251F RID: 9503 RVA: 0x0010D517 File Offset: 0x0010B917
		// (set) Token: 0x06002520 RID: 9504 RVA: 0x0010D51F File Offset: 0x0010B91F
		public ActionController ActionController { get; set; }

		// Token: 0x170001A5 RID: 421
		// (get) Token: 0x06002521 RID: 9505 RVA: 0x0010D528 File Offset: 0x0010B928
		// (set) Token: 0x06002522 RID: 9506 RVA: 0x0010D530 File Offset: 0x0010B930
		public PopupBonusController PopupBonusController { get; set; }

		// Token: 0x170001A6 RID: 422
		// (get) Token: 0x06002523 RID: 9507 RVA: 0x0010D539 File Offset: 0x0010B939
		// (set) Token: 0x06002524 RID: 9508 RVA: 0x0010D541 File Offset: 0x0010B941
		public bool InFrenzy { get; set; }

		// Token: 0x06002525 RID: 9509 RVA: 0x0010D54C File Offset: 0x0010B94C
		public override void Update()
		{
			base.Update();
			if (this.CurrentGameState == AppTheSystem.GameState.Hypnosis)
			{
				foreach (AStoryAppsView view in this.Views)
				{
					this.BlinkView(view);
				}
			}
		}

		// Token: 0x06002526 RID: 9510 RVA: 0x0010D5BC File Offset: 0x0010B9BC
		public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
		{
			if (this.CurrentGameState == AppTheSystem.GameState.InMessageSystem && Input.GetKeyDown(KeyCode.F1))
			{
				this.ChangeGameState(AppTheSystem.GameState.InShop);
				this.ShopView.CurrentState = ShopView.State.Workers;
			}
			else if (this.CurrentGameState == AppTheSystem.GameState.InMessageSystem && Input.GetKeyDown(KeyCode.F2))
			{
				this.ChangeGameState(AppTheSystem.GameState.InShop);
				this.ShopView.CurrentState = ShopView.State.Upgrades;
			}
			else if (this.CurrentGameState == AppTheSystem.GameState.InShop && this.ShopView.CurrentState == ShopView.State.Workers && Input.GetKeyDown(KeyCode.F1))
			{
				this.ChangeGameState(AppTheSystem.GameState.InMessageSystem);
			}
			else if (this.CurrentGameState == AppTheSystem.GameState.InShop && this.ShopView.CurrentState == ShopView.State.Workers && Input.GetKeyDown(KeyCode.F2))
			{
				this.ShopView.CurrentState = ShopView.State.Upgrades;
			}
			else if (this.CurrentGameState == AppTheSystem.GameState.InShop && this.ShopView.CurrentState == ShopView.State.Upgrades && Input.GetKeyDown(KeyCode.F1))
			{
				this.ShopView.CurrentState = ShopView.State.Workers;
			}
			else if (this.CurrentGameState == AppTheSystem.GameState.InShop && this.ShopView.CurrentState == ShopView.State.Upgrades && Input.GetKeyDown(KeyCode.F2))
			{
				this.ChangeGameState(AppTheSystem.GameState.InMessageSystem);
			}
			switch (key)
			{
				case SHSharp::SHGUIinput.esc:
					if (this.CurrentGameState == AppTheSystem.GameState.InShop)
					{
						this.ChangeGameState(AppTheSystem.GameState.InMessageSystem);
					}
					else if (this.CurrentGameState != AppTheSystem.GameState.Hypnosis && this.CurrentGameState != AppTheSystem.GameState.Brainwash)
					{
						this.Kill();
						this.RemoveAllViews();
					}
					break;
			}
			if (this.CurrentGameState != AppTheSystem.GameState.Hypnosis && this.CurrentGameState != AppTheSystem.GameState.Brainwash && this.CurrentGameState != AppTheSystem.GameState.BonusPopUp)
			{
				this.ChatView.ReactToInputKeyboard(key);
				this.ShopView.ReactToInputKeyboard(key);
			}
			if (this.CurrentGameState == AppTheSystem.GameState.BonusPopUp)
			{
				this.PopupBonusController.ReactToInputKeyboard(key);
			}
		}

		// Token: 0x06002527 RID: 9511 RVA: 0x0010D80C File Offset: 0x0010BC0C
		public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
		{
			foreach (AStoryAppsView astoryAppsView in this.Views)
			{
				astoryAppsView.ReactToInputMouse(x, y, clicked, scroll);
			}
		}

		// Token: 0x06002528 RID: 9512 RVA: 0x0010D86C File Offset: 0x0010BC6C
		public override void Redraw(int offx, int offy)
		{
			base.Redraw(offx, offy);
			this.ActionController.Redraw(offx, offy);
			this.PopupBonusController.Redraw(offx, offy);
		}

		// Token: 0x06002529 RID: 9513 RVA: 0x0010D890 File Offset: 0x0010BC90
		public void ChangeGameState(AppTheSystem.GameState newGameState)
		{
			switch (newGameState)
			{
				case AppTheSystem.GameState.InMessageSystem:
					this.ActionController.Active = true;
					this.ChatView.Active = true;
					this.ShopView.Active = false;
					this.ShopView.HintView.Hidden = true;
					foreach (AStoryAppsView astoryAppsView in this.Views)
					{
						astoryAppsView.SetColorRecursive('w');
					}
					break;
				case AppTheSystem.GameState.InShop:
					this.ShopView.Active = true;
					this.ChatView.Active = false;
					break;
				case AppTheSystem.GameState.Hypnosis:
				case AppTheSystem.GameState.Brainwash:
					this.ActionController.Active = false;
					this.ChatView.Active = false;
					this.APPINSTRUCTION.text = string.Empty;
					this.ShopView.HintView.Hidden = true;
					break;
				case AppTheSystem.GameState.BonusPopUp:
					break;
				default:
					throw new ArgumentOutOfRangeException("newGameState", newGameState, null);
			}
			this.previousState = this.CurrentGameState;
			this.CurrentGameState = newGameState;
		}

		// Token: 0x0600252A RID: 9514 RVA: 0x0010D9CC File Offset: 0x0010BDCC
		public void ReturnToPreviousState()
		{
			this.ChangeGameState(this.previousState);
		}

		// Token: 0x0600252B RID: 9515 RVA: 0x0010D9DC File Offset: 0x0010BDDC
		public void ResetViewsPositions()
		{
			for (int i = 0; i < this.Views.Count; i++)
			{
				AStoryAppsView astoryAppsView = this.Views.ElementAt(i);
				astoryAppsView.PositionX = (int)this.viewsPositions.ElementAt(i).x;
				astoryAppsView.PositionY = (int)this.viewsPositions.ElementAt(i).y;
			}
		}

		// Token: 0x0600252C RID: 9516 RVA: 0x0010DA48 File Offset: 0x0010BE48
		public void BlinkView(SHSharp::SHGUIview view)
		{
			if (this.blinkTimer >= this.blinkTime)
			{
				if (view.color == 'w')
				{
					view.color = '9';
				}
				int num = Convert.ToInt32(view.color.ToString());
				if (!this.blinkReturn)
				{
					if (num > 0)
					{
						view.SetColorRecursive(Convert.ToChar((num - 1).ToString()));
					}
					else if (num == 0)
					{
						this.blinkReturn = true;
					}
				}
				else if (num < 9)
				{
					view.SetColorRecursive(Convert.ToChar((num + 1).ToString()));
				}
				else
				{
					this.blinkReturn = false;
					this.blinkTime = UnityEngine.Random.Range(0.01f, 0.05f);
				}
				this.blinkTimer = 0f;
			}
			this.blinkTimer += Time.unscaledDeltaTime;
		}

		// Token: 0x0600252D RID: 9517 RVA: 0x0010DB3C File Offset: 0x0010BF3C
		private void PrepareViews()
		{
			this.ShopView = new ShopView(SharedTheSystem.Views.ShopViewPositionX, SharedTheSystem.Views.ShopViewPositionY);
			this.HotCoinsView = new HotCoinsView(SharedTheSystem.Views.HotCoinsViewPositionX, SharedTheSystem.Views.HotCoinsViewPositionY);
			this.ChatView = new ChatView(SharedTheSystem.Views.ChatViewPositionX, SharedTheSystem.Views.ChatViewPositionY, SharedTheSystem.Views.ChatViewWidth, SharedTheSystem.Views.ChatViewHeight);
			this.ShopView.Active = true;
			base.AddSubView(this.ChatView);
			base.AddSubView(this.ShopView);
			base.AddSubView(this.HotCoinsView);
			this.Views.Add(this.ChatView);
			this.Views.Add(this.ShopView);
			this.Views.Add(this.HotCoinsView);
			foreach (AStoryAppsView astoryAppsView in this.Views)
			{
				this.viewsPositions.Add(new Vector2((float)astoryAppsView.PositionX, (float)astoryAppsView.PositionY));
			}
		}

		// Token: 0x0600252E RID: 9518 RVA: 0x0010DC88 File Offset: 0x0010C088
		private void RemoveAllViews()
		{
			for (int i = 0; i < this.Views.Count; i++)
			{
				AStoryAppsView astoryAppsView = this.Views.ElementAt(i);
				HotCoinsView hotCoinsView = astoryAppsView as HotCoinsView;
				if (hotCoinsView != null)
				{
					hotCoinsView.AddAllHotCoins();
				}
				base.RemoveView(astoryAppsView);
				astoryAppsView.Kill();
				this.Views.Remove(astoryAppsView);
			}
			foreach (AStoryAppsView astoryAppsView2 in this.Views)
			{
				base.RemoveView(astoryAppsView2);
				astoryAppsView2.Kill();
			}
		}

		// Token: 0x04002747 RID: 10055
		public List<AStoryAppsView> Views = new List<AStoryAppsView>();

		// Token: 0x0400274B RID: 10059
		private AppTheSystem.GameState previousState;

		// Token: 0x0400274C RID: 10060
		private float blinkTimer;

		// Token: 0x0400274D RID: 10061
		private float blinkTime = UnityEngine.Random.Range(0.01f, 0.05f);

		// Token: 0x0400274E RID: 10062
		private bool blinkReturn;

		// Token: 0x0400274F RID: 10063
		private readonly List<Vector2> viewsPositions = new List<Vector2>();

		// Token: 0x0200066B RID: 1643
		public enum GameState
		{
			// Token: 0x04002751 RID: 10065
			InMessageSystem,
			// Token: 0x04002752 RID: 10066
			InShop,
			// Token: 0x04002753 RID: 10067
			Hypnosis,
			// Token: 0x04002754 RID: 10068
			Brainwash,
			// Token: 0x04002755 RID: 10069
			BonusPopUp
		}
	}

	public abstract class AUpgrade : ShopButton
	{
		// Token: 0x06002625 RID: 9765 RVA: 0x00111DE4 File Offset: 0x001101E4
		protected AUpgrade(string text, int startX, int startY, char color) : base(text, startX, startY, SharedTheSystem.Views.ShopButtonWidth, SharedTheSystem.Views.ShopButtonHeight, color)
		{
			base.Active = true;
			base.Button.SetOnActivate(new Action(this.ActivateUpgrade));
		}

		// Token: 0x170001EB RID: 491
		// (get) Token: 0x06002626 RID: 9766 RVA: 0x00111E30 File Offset: 0x00110230
		// (set) Token: 0x06002627 RID: 9767 RVA: 0x00111E38 File Offset: 0x00110238
		public SharedUpgrades.UpgradeType UpgradeType { get; set; }

		// Token: 0x170001EC RID: 492
		// (get) Token: 0x06002628 RID: 9768 RVA: 0x00111E41 File Offset: 0x00110241
		// (set) Token: 0x06002629 RID: 9769 RVA: 0x00111E49 File Offset: 0x00110249
		public Value Tier { get; set; }

		// Token: 0x0600262A RID: 9770
		public abstract void ActivateUpgrade();

		// Token: 0x0600262B RID: 9771
		public abstract bool CanBeUsed();

		// Token: 0x0600262C RID: 9772 RVA: 0x00111E52 File Offset: 0x00110252
		public bool CanBeBought()
		{
			return SharedTheSystem.General.GetHotCoins() >= base.Price;
		}

		// Token: 0x0600262D RID: 9773 RVA: 0x00111E69 File Offset: 0x00110269
		public void Buy()
		{
			AppTheSystem.Instance.ActionController.SpendHotCoins(base.Price);
		}

		// Token: 0x0600262E RID: 9774 RVA: 0x00111E80 File Offset: 0x00110280
		public void UpdatePriceAndTier()
		{
			base.Price = base.StartPrice * Value.Power(SharedTheSystem.Upgrades.UpgradePriceIncreaseRate, this.Tier);
			this.Tier = ++this.Tier;
		}

		// Token: 0x0600262F RID: 9775 RVA: 0x00111EBC File Offset: 0x001102BC
		public virtual Value GetRequiredWorkers()
		{
			if (this.Tier == Value.One)
			{
				return Value.One;
			}
			if (this.Tier == new Value(2f))
			{
				return new Value(10f);
			}
			if (this.Tier == new Value(3f))
			{
				return new Value(25f);
			}
			return new Value(50f) * (this.Tier - new Value(3f));
		}

		// Token: 0x06002630 RID: 9776 RVA: 0x00111F52 File Offset: 0x00110352
		public virtual void ResetPriceAndTier()
		{
			base.Price = base.StartPrice;
			this.Tier = Value.One;
		}

		// Token: 0x06002631 RID: 9777 RVA: 0x00111F6B File Offset: 0x0011036B
		public virtual void UpdateHintText()
		{
		}
	}

	public abstract class AVrElement : SHSharp::SHGUIview
	{
		// Token: 0x060024EF RID: 9455 RVA: 0x0010C4CD File Offset: 0x0010A8CD
		protected AVrElement()
		{
			//this.shaderGlobalValues = GameObject.Find("GlobalShaderValueControl").GetComponent<GlobalShaderValueControl>();
			this.vrMaterial = MCDCom.LoadMaterialFromResource("IdleGame_VrMaterial.dat"); //(Resources.Load("IdleMinigame/VrMaterial") as Material);
		}

		// Token: 0x060024F0 RID: 9456 RVA: 0x0010C4FF File Offset: 0x0010A8FF
		public override void Redraw(int offx, int offy)
		{
			base.Redraw(offx, offy);

			if (this.PersistentMenuTextureReference == null)
            {
				RenderTexture MiniMenuTexture = new RenderTexture(1216, 912, 0, RenderTextureFormat.ARGB32);

				MiniMenuTexture.name = "SHSharp::SHGUI MENU TEXTURE";
				MiniMenuTexture.wrapMode = TextureWrapMode.Clamp;
				MiniMenuTexture.filterMode = FilterMode.Bilinear;
				MiniMenuTexture.autoGenerateMips = false;

				
				this.PersistentMenuTextureReference = MiniMenuTexture;
			} 

			//this.vrMaterial.mainTexture = this.shaderGlobalValues.MiniMenuTexture;
			this.vrMaterial.mainTexture = this.PersistentMenuTextureReference;
			this.Rotate();
		}

		// Token: 0x060024F1 RID: 9457 RVA: 0x0010C525 File Offset: 0x0010A925
		public override void Kill()
		{
			base.Kill();
			UnityEngine.Object.Destroy(this.Primitive);
		}

		// Token: 0x060024F2 RID: 9458 RVA: 0x0010C538 File Offset: 0x0010A938
		protected virtual void Rotate()
		{
			float num = 0f;
			float num2 = Time.unscaledDeltaTime * 60f;
			float num3 = Time.unscaledDeltaTime * 20f;
			this.Primitive.transform.rotation = Quaternion.Euler(this.Primitive.transform.rotation.eulerAngles.x + num, this.Primitive.transform.rotation.eulerAngles.y + num2, this.Primitive.transform.rotation.eulerAngles.z + num3);
		}

		// Token: 0x060024F3 RID: 9459 RVA: 0x0010C5E4 File Offset: 0x0010A9E4
		protected void AddComponents()
		{
			Rigidbody rigidbody = this.Primitive.AddComponent<Rigidbody>();
			rigidbody.useGravity = false;
			BoxCollider boxCollider = this.Primitive.AddComponent<BoxCollider>();
			this.Primitive.GetComponent<Renderer>().material = this.vrMaterial;
		}

		// Token: 0x04002703 RID: 9987
		public GameObject Primitive;

		// Token: 0x04002704 RID: 9988
		//private GlobalShaderValueControl shaderGlobalValues;

		// Token: 0x04002705 RID: 9989
		private Material vrMaterial;

		private RenderTexture PersistentMenuTextureReference;
	}

	public class BonusPopupTimeUpgrade : AUpgrade
	{
		// Token: 0x06002632 RID: 9778 RVA: 0x00111F70 File Offset: 0x00110370
		public BonusPopupTimeUpgrade(string text, int startX, int startY, char color) : base(text, startX, startY, color)
		{
			base.UpgradeType = SharedUpgrades.UpgradeType.Popup;
			base.StartPrice = SharedTheSystem.Upgrades.PopupTimeUpgradeStartPrice;
			Value price;
			Value tier;
			SharedTheSystem.Upgrades.GetPopupTimeUpgrade(out price, out tier);
			base.Price = price;
			base.Tier = tier;
		}

		// Token: 0x06002633 RID: 9779 RVA: 0x00111FBC File Offset: 0x001103BC
		public override void ActivateUpgrade()
		{
			if (base.CanBeBought())
			{
				base.Buy();
				SharedTheSystem.Upgrades.DecreasePopupShowTime();
				base.UpdatePriceAndTier();
				SharedTheSystem.Upgrades.SavePopupTimeUpgrade(base.Price, base.Tier);
			}
		}

		// Token: 0x06002634 RID: 9780 RVA: 0x00111FF5 File Offset: 0x001103F5
		public override bool CanBeUsed()
		{
			return SharedTheSystem.Upgrades.GetPopupCount() >= this.GetRequiredWorkers();
		}

		// Token: 0x06002635 RID: 9781 RVA: 0x0011200C File Offset: 0x0011040C
		public override Value GetRequiredWorkers()
		{
			if (base.Tier == Value.One)
			{
				return new Value(20f);
			}
			if (base.Tier == new Value(2f))
			{
				return new Value(50f);
			}
			if (base.Tier == new Value(3f))
			{
				return new Value(100f);
			}
			return Value.Zero;
		}
	}

	public class Bot : SHSharp::SHGUIsprite
	{
		// Token: 0x060024F8 RID: 9464 RVA: 0x0010C7E8 File Offset: 0x0010ABE8
		public Bot()
		{
			MCDCom.AddFrameFromStr(this, MCDCom.AssetToText(MCDCom.GetAssetName("IdleGame_Bot.txt")), 4);
			//base.AddFramesFromFile(StoryAppsResources.IdleGameBot, 4, false);
			if (SharedTheSystem.Workers.GetBots() == Value.Zero)
			{
				this.currentAnimationFrame = Bot.AnimationEnum.NoBots;
				this.currentFrame = 0;
			}
			else
			{
				this.currentAnimationFrame = Bot.AnimationEnum.BotStart1;
				this.currentFrame = 1;
			}
		}

		// Token: 0x060024F9 RID: 9465 RVA: 0x0010C860 File Offset: 0x0010AC60
		public override void Update()
		{
			base.Update();
			switch (this.currentAnimationFrame)
			{
				case Bot.AnimationEnum.NoBots:
					if (SharedTheSystem.Workers.GetBots() > Value.Zero)
					{
						this.currentAnimationFrame = Bot.AnimationEnum.BotStart1;
						this.currentFrame = 1;
						this.animationTimer = 0f;
					}
					break;
				case Bot.AnimationEnum.BotStart1:
					if (this.animationTimer >= this.startTime)
					{
						this.currentAnimationFrame = Bot.AnimationEnum.BotStart2;
						this.currentFrame = 2;
						this.animationTimer = 0f;
					}
					break;
				case Bot.AnimationEnum.BotStart2:
					if (this.animationTimer >= this.startTime)
					{
						this.currentAnimationFrame = Bot.AnimationEnum.BotStart3;
						this.currentFrame = 3;
						this.animationTimer = 0f;
					}
					break;
				case Bot.AnimationEnum.BotStart3:
					if (this.animationTimer >= this.startTime)
					{
						this.currentAnimationFrame = Bot.AnimationEnum.Writing1;
						this.currentFrame = 4;
						this.animationTimer = 0f;
					}
					break;
				case Bot.AnimationEnum.Writing1:
					if (this.animationTimer >= this.animationTime)
					{
						this.currentAnimationFrame = Bot.AnimationEnum.Writing2;
						this.currentFrame = 5;
						this.animationTimer = 0f;
					}
					break;
				case Bot.AnimationEnum.Writing2:
					if (this.animationTimer >= this.animationTime)
					{
						this.currentAnimationFrame = Bot.AnimationEnum.Writing1;
						this.currentFrame = 4;
						this.animationTimer = 0f;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			this.animationTimer += Time.unscaledDeltaTime;
		}

		// Token: 0x04002706 RID: 9990
		private Bot.AnimationEnum currentAnimationFrame;

		// Token: 0x04002707 RID: 9991
		private float animationTimer;

		// Token: 0x04002708 RID: 9992
		private readonly float animationTime = SharedTheSystem.Views.BotAnimationTime;

		// Token: 0x04002709 RID: 9993
		private float startTime = 0.3f;

		// Token: 0x0200065D RID: 1629
		private enum AnimationEnum
		{
			// Token: 0x0400270B RID: 9995
			NoBots,
			// Token: 0x0400270C RID: 9996
			BotStart1,
			// Token: 0x0400270D RID: 9997
			BotStart2,
			// Token: 0x0400270E RID: 9998
			BotStart3,
			// Token: 0x0400270F RID: 9999
			Writing1,
			// Token: 0x04002710 RID: 10000
			Writing2
		}
	}

	public class BotRateUpgrade : AUpgrade
	{
		// Token: 0x06002636 RID: 9782 RVA: 0x00112088 File Offset: 0x00110488
		public BotRateUpgrade(string text, int startX, int startY, char color) : base(text, startX, startY, color)
		{
            try
            {
				base.UpgradeType = SharedUpgrades.UpgradeType.Bot;
				base.StartPrice = SharedTheSystem.Upgrades.BotRateUpgradeStartPrice;
				Value price;
				Value tier;
				SharedTheSystem.Upgrades.GetBotRateUpgrade(out price, out tier);
				base.Price = price;
				base.Tier = tier;
			} catch (Exception e)
            {
				MelonLogger.Msg(e.ToString());
            }
		}

		// Token: 0x06002637 RID: 9783 RVA: 0x001120D3 File Offset: 0x001104D3
		public override void ActivateUpgrade()
		{
			if (base.CanBeBought())
			{
				base.Buy();
				SharedTheSystem.Upgrades.IncreaseRateMultiplier(base.UpgradeType);
				base.UpdatePriceAndTier();
				SharedTheSystem.Upgrades.SaveBotRateUpgrade(base.Price, base.Tier);
			}
		}

		// Token: 0x06002638 RID: 9784 RVA: 0x00112112 File Offset: 0x00110512
		public override bool CanBeUsed()
		{
			return SharedTheSystem.Workers.GetBots() >= this.GetRequiredWorkers();
		}

		// Token: 0x06002639 RID: 9785 RVA: 0x00112129 File Offset: 0x00110529
		public override void UpdateHintText()
		{
			base.HintText = SharedTheSystem.Upgrades.BotRateHint + SharedTheSystem.GetUpgradesHintStats(IncomeController.Instance.TotalBotIncomePerSec());
			base.UpdateHintText();
		}
	}

	public sealed class Brainwash : AUpgrade
	{
		// Token: 0x0600263A RID: 9786 RVA: 0x00112158 File Offset: 0x00110558
		public Brainwash(string text, int startX, int startY, char color) : base(text, startX, startY, color)
		{
			base.UpgradeType = SharedUpgrades.UpgradeType.Brainwash;
			base.Tier = SharedTheSystem.Upgrades.GetBrainwashTier();
			this.Frame.SetColorRecursive('r');
			base.Button.SetColorRecursive('r');
			this.RemoveView(this.PriceText);
		}

		// Token: 0x0600263B RID: 9787 RVA: 0x001121B0 File Offset: 0x001105B0
		public override void ActivateUpgrade()
		{
			AppTheSystem.Instance.ActionController.ActivateBrainwash();
			base.Tier = ++base.Tier;
			SharedTheSystem.Upgrades.SaveBrainwashTier(base.Tier);
		}

		// Token: 0x0600263C RID: 9788 RVA: 0x001121E2 File Offset: 0x001105E2
		public override bool CanBeUsed()
		{
			return SharedTheSystem.General.GetConverted() >= this.GetRequiredWorkers();
		}

		// Token: 0x0600263D RID: 9789 RVA: 0x001121FC File Offset: 0x001105FC
		public override Value GetRequiredWorkers()
		{
			if (base.Tier == Value.One)
			{
				return new Value(2, 1f);
			}
			if (base.Tier == new Value(2f))
			{
				return new Value(3, 1f);
			}
			if (base.Tier == new Value(3f))
			{
				return new Value(3, 100f);
			}
			if (base.Tier == new Value(4f))
			{
				return new Value(5, 1f);
			}
			return new Value(7, 100f);
		}

		// Token: 0x0600263E RID: 9790 RVA: 0x001122A7 File Offset: 0x001106A7
		public override void ResetPriceAndTier()
		{
		}
	}

	public class BrainwashAVrCapsule : AVrElement
	{
		// Token: 0x060024F4 RID: 9460 RVA: 0x0010C628 File Offset: 0x0010AA28
		public BrainwashAVrCapsule()
		{
			this.Primitive = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			this.Primitive.transform.position = new Vector3(80f, 80f, 110f);
			this.Primitive.transform.localScale = new Vector3(30f, 30f, 30f);
			base.AddComponents();
		}
	}

	public class BrainwashAVrCube : AVrElement
	{
		// Token: 0x060024F5 RID: 9461 RVA: 0x0010C698 File Offset: 0x0010AA98
		public BrainwashAVrCube()
		{
			this.Primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
			this.Primitive.transform.position = new Vector3(-80f, 80f, 110f);
			this.Primitive.transform.localScale = new Vector3(30f, 30f, 30f);
			base.AddComponents();
		}
	}

	public class BrainwashAVrCylinder : AVrElement
	{
		// Token: 0x060024F6 RID: 9462 RVA: 0x0010C708 File Offset: 0x0010AB08
		public BrainwashAVrCylinder()
		{
			this.Primitive = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
			this.Primitive.transform.position = new Vector3(-80f, -80f, 110f);
			this.Primitive.transform.localScale = new Vector3(30f, 30f, 30f);
			base.AddComponents();
		}
	}

	public class BrainwashAVrSphere : AVrElement
	{
		// Token: 0x060024F7 RID: 9463 RVA: 0x0010C778 File Offset: 0x0010AB78
		public BrainwashAVrSphere()
		{
			this.Primitive = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			this.Primitive.transform.position = new Vector3(80f, -80f, 110f);
			this.Primitive.transform.localScale = new Vector3(30f, 30f, 30f);
			base.AddComponents();
		}
	}

	public sealed class ChatView : AStoryAppsView
	{
		// Token: 0x06002691 RID: 9873 RVA: 0x00114A54 File Offset: 0x00112E54
		public ChatView(int positionX, int positionY, int width, int height) : base(positionX, positionY, width, height)
		{
			//TextAsset textAsset = Resources.Load("StoryApps/Games/IdleMinigame/PraiseMessages") as TextAsset;
			string textAsset = MCDCom.AssetToText(MCDCom.GetAssetName("IdleGame_PraiseMessages.txt"));
			this.praiseMessages = textAsset/*.text*/.Split(new char[]
			{
				'\n'
			}).ToList<string>();
			for (int i = 0; i < this.praiseMessages.Count; i++)
			{
				this.messageWeights.Add(1f);
			}
			for (int j = 0; j < Enum.GetNames(typeof(SharedWorkers.Workers)).Length; j++)
			{
				this.workersWeights.Add(0f);
			}
			base.PositionX = positionX;
			base.PositionY = positionY;
			this.chatWindow = new IdleGameChat(base.PositionX, base.PositionY, width, height)
			{
				desiredFrameWidth = SharedTheSystem.Views.ChatViewWidth - 4,
				maxlines = SharedTheSystem.Views.ChatViewHeight + 2,
				killOnEmptyQueue = false,
				showFadeForChatMessages = true,
				dontDisplaySender = false,
				skippable = false
			};
			this.chatWindow.ShowChatFrames();
			this.AddSubView(this.chatWindow);
			this.chatWindow.AddMySystemMessage(string.Empty, "^>^D^Fr^CrHELLO MOD. WE NEED MORE SUPERHOT PLAYERS.");
			this.AddPlayerMessage();
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x06002692 RID: 9874 RVA: 0x00114BC4 File Offset: 0x00112FC4
		// (set) Token: 0x06002693 RID: 9875 RVA: 0x00114BCC File Offset: 0x00112FCC
		public new bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
				this.chatWindow.SetActive(value);
			}
		}

		// Token: 0x06002694 RID: 9876 RVA: 0x00114BE4 File Offset: 0x00112FE4
		public override void Update()
		{
			base.Update();
			this.UpdateWorkersWeights();
			if (this.workerMessageTimer >= this.workerMessageTime)
			{
				this.AddWorkerMessage();
				this.workerMessageTimer = 0f;
			}
			this.workerMessageTimer += Time.unscaledDeltaTime;
		}

		// Token: 0x06002695 RID: 9877 RVA: 0x00114C31 File Offset: 0x00113031
		public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
		{
		}

		// Token: 0x06002696 RID: 9878 RVA: 0x00114C33 File Offset: 0x00113033
		public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
		{
			if (!this.Active)
			{
				return;
			}
			base.ReactToInputKeyboard(key);
		}

		// Token: 0x06002697 RID: 9879 RVA: 0x00114C48 File Offset: 0x00113048
		public void AddPlayerMessage()
		{
			this.chatWindow.AddPlayerMessage("MOD", this.GetRandomMessage(), new Action(this.SendPlayerMessage));
		}

		// Token: 0x06002698 RID: 9880 RVA: 0x00114C6C File Offset: 0x0011306C
		public void AddWorkerMessage()
		{
			bool flag = false;
			for (int i = 0; i < this.workersWeights.Count; i++)
			{
				if (this.workersWeights[i] != 0f)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			int randomNumber = SharedTheSystem.RandomWithWeight(this.workersWeights.ToArray());
			SharedTheSystem.CalculateWeights(this.workersWeights, randomNumber);
			switch (randomNumber)
			{
				case 0:
					this.chatWindow.AddWorkerMessage("Bot", this.GetRandomMessage());
					break;
				case 1:
					this.chatWindow.AddWorkerMessage("Fan", this.GetRandomMessage());
					break;
				case 2:
					this.chatWindow.AddWorkerMessage("Streamer", this.GetRandomMessage());
					break;
				case 3:
					this.chatWindow.AddWorkerMessage("Flamer", this.GetRandomMessage());
					break;
				case 4:
					this.chatWindow.AddWorkerMessage("Drug Dealer", this.GetRandomMessage());
					break;
				case 5:
					this.chatWindow.AddWorkerMessage("Wifi", this.GetRandomMessage());
					break;
				case 6:
					this.chatWindow.AddWorkerMessage("Indie Dev", this.GetRandomMessage());
					break;
			}
		}

		// Token: 0x06002699 RID: 9881 RVA: 0x00114DC4 File Offset: 0x001131C4
		public string GetRandomMessage()
		{
			Value one = Value.One;
			Value value;
			SharedTheSystem.Upgrades.GetMessageConvertedUpgrade(out value, out one);
			int num = 6 * (int)one.Fraction;
			if (num > this.praiseMessages.Count)
			{
				num = this.praiseMessages.Count;
			}
			int num2 = SharedTheSystem.RandomWithWeight(this.messageWeights.GetRange(0, num).ToArray());
			SharedTheSystem.CalculateWeights(this.messageWeights, num2);
			return this.praiseMessages.ElementAt(num2);
		}

		// Token: 0x0600269A RID: 9882 RVA: 0x00114E3C File Offset: 0x0011323C
		private void UpdateWorkersWeights()
		{
			if (SharedTheSystem.Workers.GetBots() > Value.Zero && this.workersWeights[0] == 0f)
			{
				this.workersWeights[0] = 1f;
			}
			if (SharedTheSystem.Workers.GetFans() > Value.Zero && this.workersWeights[1] == 0f)
			{
				this.workersWeights[1] = 1f;
			}
			if (SharedTheSystem.Workers.GetFlamers() > Value.Zero && this.workersWeights[3] == 0f)
			{
				this.workersWeights[3] = 1f;
			}
			if (SharedTheSystem.Workers.GetStreamers() > Value.Zero && this.workersWeights[2] == 0f)
			{
				this.workersWeights[2] = 1f;
			}
			if (SharedTheSystem.Workers.GetIndieDevs() > Value.Zero && this.workersWeights[6] == 0f)
			{
				this.workersWeights[6] = 1f;
			}
			if (SharedTheSystem.Workers.GetWifis() > Value.Zero && this.workersWeights[5] == 0f)
			{
				this.workersWeights[5] = 1f;
			}
			if (SharedTheSystem.Workers.GetDrugDealers() > Value.Zero && this.workersWeights[4] == 0f)
			{
				this.workersWeights[4] = 1f;
			}
		}

		// Token: 0x0600269B RID: 9883 RVA: 0x0011500C File Offset: 0x0011340C
		private void SendPlayerMessage()
		{
			AppTheSystem.Instance.ActionController.AddConvertedToQueue(SharedTheSystem.General.GetConvertedPerMessage() * SharedTheSystem.Upgrades.GetMessageConvertedMultiplier() + SharedTheSystem.Upgrades.GetMessageCpsBonus());
			this.chatWindow.FinishPlayerMessage();
			this.AddPlayerMessage();
		}

		// Token: 0x040028DE RID: 10462
		private IdleGameChat chatWindow;

		// Token: 0x040028DF RID: 10463
		private float workerMessageTimer;

		// Token: 0x040028E0 RID: 10464
		private float workerMessageTime = 5f;

		// Token: 0x040028E1 RID: 10465
		private List<string> praiseMessages = new List<string>();

		// Token: 0x040028E2 RID: 10466
		private List<float> messageWeights = new List<float>();

		// Token: 0x040028E3 RID: 10467
		private List<float> workersWeights = new List<float>();

		// Token: 0x040028E4 RID: 10468
		private bool active;
	}

	public class ConvertedMan : SHSharp::SHGUIsprite
	{
		// Token: 0x060024FA RID: 9466 RVA: 0x0010C9D7 File Offset: 0x0010ADD7
		public ConvertedMan()
		{
			MCDCom.AddFrameFromStr(this, MCDCom.AssetToText(MCDCom.GetAssetName("IdleGame_ConvertedMan.txt")), 3);
			//base.AddFramesFromFile(StoryAppsResources.IdleGameConvertedMan, 3, false);
			this.currentFrame = 0;
			this.currentAnimation = ConvertedMan.AnimationEnum.Wave1;
		}

		// Token: 0x060024FB RID: 9467 RVA: 0x0010CA0C File Offset: 0x0010AE0C
		public override void Update()
		{
			if (this.animationTimer >= this.animationTime)
			{
				ConvertedMan.AnimationEnum animationEnum = this.currentAnimation;
				if (animationEnum != ConvertedMan.AnimationEnum.Wave1)
				{
					if (animationEnum != ConvertedMan.AnimationEnum.Wave2)
					{
						throw new ArgumentOutOfRangeException();
					}
					this.currentFrame = 0;
					this.currentAnimation = ConvertedMan.AnimationEnum.Wave1;
				}
				else
				{
					this.currentFrame = 1;
					this.currentAnimation = ConvertedMan.AnimationEnum.Wave2;
				}
				this.animationTimer = 0f;
			}
			else
			{
				this.animationTimer += Time.unscaledDeltaTime;
			}
			base.Update();
		}

		// Token: 0x04002711 RID: 10001
		private float animationTimer;

		// Token: 0x04002712 RID: 10002
		private readonly float animationTime = SharedTheSystem.Views.ConvertedManAnimationTime;

		// Token: 0x04002713 RID: 10003
		private ConvertedMan.AnimationEnum currentAnimation;

		// Token: 0x0200065F RID: 1631
		private enum AnimationEnum
		{
			// Token: 0x04002715 RID: 10005
			Wave1,
			// Token: 0x04002716 RID: 10006
			Wave2
		}
	}

	public sealed class ConvertedPeopleView : AStoryAppsView
	{
		// Token: 0x0600269C RID: 9884 RVA: 0x00115064 File Offset: 0x00113464
		public ConvertedPeopleView(int positionX, int positionY) : base(positionX, positionY, 0, 0)
		{
			base.Active = true;
			base.PositionX = positionX;
			base.PositionY = positionY;
			this.convertedText = new SHSharp::SHGUItext("Converted x " + SharedTheSystem.General.GetConverted(), base.PositionX, base.PositionY, 'w', false);
			if (SharedTheSystem.General.GetConverted().Counter == 0)
			{
				this.convertedText.text = string.Format("Converted x {0:##0}", SharedTheSystem.General.GetConverted().Fraction);
			}
			this.convertedText.x = base.PositionX - this.convertedText.text.Length;
			this.AddSubView(this.convertedText);
		}

		// Token: 0x0600269D RID: 9885 RVA: 0x0011512C File Offset: 0x0011352C
		public void UpdateConvertedText()
		{
			this.convertedText.text = "Converted x " + SharedTheSystem.General.GetConverted();
			if (SharedTheSystem.General.GetConverted().Counter == 0)
			{
				this.convertedText.text = string.Format("Converted x {0:##0}", SharedTheSystem.General.GetConverted().Fraction);
			}
			this.convertedText.x = base.PositionX - this.convertedText.text.Length;
		}

		// Token: 0x040028E5 RID: 10469
		private SHSharp::SHGUItext convertedText;

		// Token: 0x040028E6 RID: 10470
		private const string CurrentlyConvertedText = "Converted x ";
	}

	[Serializable]
	public class DataSerializer
	{
		// Token: 0x0600252F RID: 9519 RVA: 0x0010DD44 File Offset: 0x0010C144
		protected DataSerializer()
		{
		}

		// Token: 0x170001A7 RID: 423
		// (get) Token: 0x06002530 RID: 9520 RVA: 0x0010DD61 File Offset: 0x0010C161
		protected static DataSerializer Instance
		{
			get
			{
				if (DataSerializer.instance != null)
				{
					return DataSerializer.instance;
				}
				DataSerializer.instance = new DataSerializer();
				DataSerializer.instance.Deserialize();
				return DataSerializer.instance;
			}
		}

		// Token: 0x170001A8 RID: 424
		// (get) Token: 0x06002531 RID: 9521 RVA: 0x0010DD8C File Offset: 0x0010C18C
		// (set) Token: 0x06002532 RID: 9522 RVA: 0x0010DD94 File Offset: 0x0010C194
		public Value FakeAccountCount { get; set; }

		// Token: 0x170001A9 RID: 425
		// (get) Token: 0x06002533 RID: 9523 RVA: 0x0010DD9D File Offset: 0x0010C19D
		// (set) Token: 0x06002534 RID: 9524 RVA: 0x0010DDA5 File Offset: 0x0010C1A5
		public Value FanCount { get; set; }

		// Token: 0x170001AA RID: 426
		// (get) Token: 0x06002535 RID: 9525 RVA: 0x0010DDAE File Offset: 0x0010C1AE
		// (set) Token: 0x06002536 RID: 9526 RVA: 0x0010DDB6 File Offset: 0x0010C1B6
		public Value BotCount { get; set; }

		// Token: 0x170001AB RID: 427
		// (get) Token: 0x06002537 RID: 9527 RVA: 0x0010DDBF File Offset: 0x0010C1BF
		// (set) Token: 0x06002538 RID: 9528 RVA: 0x0010DDC7 File Offset: 0x0010C1C7
		public Value StreamerCount { get; set; }

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06002539 RID: 9529 RVA: 0x0010DDD0 File Offset: 0x0010C1D0
		// (set) Token: 0x0600253A RID: 9530 RVA: 0x0010DDD8 File Offset: 0x0010C1D8
		public Value FlamerCount { get; set; }

		// Token: 0x170001AD RID: 429
		// (get) Token: 0x0600253B RID: 9531 RVA: 0x0010DDE1 File Offset: 0x0010C1E1
		// (set) Token: 0x0600253C RID: 9532 RVA: 0x0010DDE9 File Offset: 0x0010C1E9
		public Value HypnosisCount { get; set; }

		// Token: 0x170001AE RID: 430
		// (get) Token: 0x0600253D RID: 9533 RVA: 0x0010DDF2 File Offset: 0x0010C1F2
		// (set) Token: 0x0600253E RID: 9534 RVA: 0x0010DDFA File Offset: 0x0010C1FA
		public Value HypnosisConvertedPercent { get; set; }

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x0600253F RID: 9535 RVA: 0x0010DE03 File Offset: 0x0010C203
		// (set) Token: 0x06002540 RID: 9536 RVA: 0x0010DE0B File Offset: 0x0010C20B
		public Value FrenzyCount { get; set; }

		// Token: 0x170001B0 RID: 432
		// (get) Token: 0x06002541 RID: 9537 RVA: 0x0010DE14 File Offset: 0x0010C214
		// (set) Token: 0x06002542 RID: 9538 RVA: 0x0010DE1C File Offset: 0x0010C21C
		public Value FrenzyAccelerationPercent { get; set; }

		// Token: 0x170001B1 RID: 433
		// (get) Token: 0x06002543 RID: 9539 RVA: 0x0010DE25 File Offset: 0x0010C225
		// (set) Token: 0x06002544 RID: 9540 RVA: 0x0010DE2D File Offset: 0x0010C22D
		public float FrenzyTime { get; set; }

		// Token: 0x170001B2 RID: 434
		// (get) Token: 0x06002545 RID: 9541 RVA: 0x0010DE36 File Offset: 0x0010C236
		// (set) Token: 0x06002546 RID: 9542 RVA: 0x0010DE3E File Offset: 0x0010C23E
		public Value BrainwashTier { get; set; }

		// Token: 0x170001B3 RID: 435
		// (get) Token: 0x06002547 RID: 9543 RVA: 0x0010DE47 File Offset: 0x0010C247
		// (set) Token: 0x06002548 RID: 9544 RVA: 0x0010DE4F File Offset: 0x0010C24F
		public float BonusPopupShowTime { get; set; }

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x06002549 RID: 9545 RVA: 0x0010DE58 File Offset: 0x0010C258
		// (set) Token: 0x0600254A RID: 9546 RVA: 0x0010DE60 File Offset: 0x0010C260
		public Value BonusPopupCount { get; set; }

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x0600254B RID: 9547 RVA: 0x0010DE69 File Offset: 0x0010C269
		// (set) Token: 0x0600254C RID: 9548 RVA: 0x0010DE71 File Offset: 0x0010C271
		public Value Converted { get; set; }

		// Token: 0x170001B6 RID: 438
		// (get) Token: 0x0600254D RID: 9549 RVA: 0x0010DE7A File Offset: 0x0010C27A
		// (set) Token: 0x0600254E RID: 9550 RVA: 0x0010DE82 File Offset: 0x0010C282
		public Value HotCoins { get; set; }

		// Token: 0x170001B7 RID: 439
		// (get) Token: 0x0600254F RID: 9551 RVA: 0x0010DE8B File Offset: 0x0010C28B
		// (set) Token: 0x06002550 RID: 9552 RVA: 0x0010DE93 File Offset: 0x0010C293
		public Value TotalHotCoinsEarned { get; set; }

		// Token: 0x170001B8 RID: 440
		// (get) Token: 0x06002551 RID: 9553 RVA: 0x0010DE9C File Offset: 0x0010C29C
		// (set) Token: 0x06002552 RID: 9554 RVA: 0x0010DEA4 File Offset: 0x0010C2A4
		public Value HotCoinsPerConverted { get; set; }

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x06002553 RID: 9555 RVA: 0x0010DEAD File Offset: 0x0010C2AD
		// (set) Token: 0x06002554 RID: 9556 RVA: 0x0010DEB5 File Offset: 0x0010C2B5
		public Value ConvertedPerMessage { get; set; }

		// Token: 0x170001BA RID: 442
		// (get) Token: 0x06002555 RID: 9557 RVA: 0x0010DEBE File Offset: 0x0010C2BE
		// (set) Token: 0x06002556 RID: 9558 RVA: 0x0010DEC6 File Offset: 0x0010C2C6
		public Value WrittenMessagesCount { get; set; }

		// Token: 0x170001BB RID: 443
		// (get) Token: 0x06002557 RID: 9559 RVA: 0x0010DECF File Offset: 0x0010C2CF
		// (set) Token: 0x06002558 RID: 9560 RVA: 0x0010DED7 File Offset: 0x0010C2D7
		public Value MessagesCpsBonus { get; set; }

		// Token: 0x170001BC RID: 444
		// (get) Token: 0x06002559 RID: 9561 RVA: 0x0010DEE0 File Offset: 0x0010C2E0
		// (set) Token: 0x0600255A RID: 9562 RVA: 0x0010DEE8 File Offset: 0x0010C2E8
		public Value BotIncomeMultiplier { get; set; }

		// Token: 0x170001BD RID: 445
		// (get) Token: 0x0600255B RID: 9563 RVA: 0x0010DEF1 File Offset: 0x0010C2F1
		// (set) Token: 0x0600255C RID: 9564 RVA: 0x0010DEF9 File Offset: 0x0010C2F9
		public Value FanIncomeMultiplier { get; set; }

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x0600255D RID: 9565 RVA: 0x0010DF02 File Offset: 0x0010C302
		// (set) Token: 0x0600255E RID: 9566 RVA: 0x0010DF0A File Offset: 0x0010C30A
		public Value StreamerIncomeMultiplier { get; set; }

		// Token: 0x170001BF RID: 447
		// (get) Token: 0x0600255F RID: 9567 RVA: 0x0010DF13 File Offset: 0x0010C313
		// (set) Token: 0x06002560 RID: 9568 RVA: 0x0010DF1B File Offset: 0x0010C31B
		public Value FlamerIncomeMultiplier { get; set; }

		// Token: 0x170001C0 RID: 448
		// (get) Token: 0x06002561 RID: 9569 RVA: 0x0010DF24 File Offset: 0x0010C324
		// (set) Token: 0x06002562 RID: 9570 RVA: 0x0010DF2C File Offset: 0x0010C32C
		public Value MessageConvertedMultiplier { get; set; }

		// Token: 0x170001C1 RID: 449
		// (get) Token: 0x06002563 RID: 9571 RVA: 0x0010DF35 File Offset: 0x0010C335
		// (set) Token: 0x06002564 RID: 9572 RVA: 0x0010DF3D File Offset: 0x0010C33D
		public Value FakeAccountPrice { get; set; }

		// Token: 0x170001C2 RID: 450
		// (get) Token: 0x06002565 RID: 9573 RVA: 0x0010DF46 File Offset: 0x0010C346
		// (set) Token: 0x06002566 RID: 9574 RVA: 0x0010DF4E File Offset: 0x0010C34E
		public Value FanPrice { get; set; }

		// Token: 0x170001C3 RID: 451
		// (get) Token: 0x06002567 RID: 9575 RVA: 0x0010DF57 File Offset: 0x0010C357
		// (set) Token: 0x06002568 RID: 9576 RVA: 0x0010DF5F File Offset: 0x0010C35F
		public Value BotPrice { get; set; }

		// Token: 0x170001C4 RID: 452
		// (get) Token: 0x06002569 RID: 9577 RVA: 0x0010DF68 File Offset: 0x0010C368
		// (set) Token: 0x0600256A RID: 9578 RVA: 0x0010DF70 File Offset: 0x0010C370
		public Value StreamerPrice { get; set; }

		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x0600256B RID: 9579 RVA: 0x0010DF79 File Offset: 0x0010C379
		// (set) Token: 0x0600256C RID: 9580 RVA: 0x0010DF81 File Offset: 0x0010C381
		public Value FlamerPrice { get; set; }

		// Token: 0x170001C6 RID: 454
		// (get) Token: 0x0600256D RID: 9581 RVA: 0x0010DF8A File Offset: 0x0010C38A
		// (set) Token: 0x0600256E RID: 9582 RVA: 0x0010DF92 File Offset: 0x0010C392
		public Value BotRateUpgradePrice { get; set; }

		// Token: 0x170001C7 RID: 455
		// (get) Token: 0x0600256F RID: 9583 RVA: 0x0010DF9B File Offset: 0x0010C39B
		// (set) Token: 0x06002570 RID: 9584 RVA: 0x0010DFA3 File Offset: 0x0010C3A3
		public Value BotRateUpgradeTier { get; set; }

		// Token: 0x170001C8 RID: 456
		// (get) Token: 0x06002571 RID: 9585 RVA: 0x0010DFAC File Offset: 0x0010C3AC
		// (set) Token: 0x06002572 RID: 9586 RVA: 0x0010DFB4 File Offset: 0x0010C3B4
		public Value FanRateUpgradePrice { get; set; }

		// Token: 0x170001C9 RID: 457
		// (get) Token: 0x06002573 RID: 9587 RVA: 0x0010DFBD File Offset: 0x0010C3BD
		// (set) Token: 0x06002574 RID: 9588 RVA: 0x0010DFC5 File Offset: 0x0010C3C5
		public Value FanRateUpgradeTier { get; set; }

		// Token: 0x170001CA RID: 458
		// (get) Token: 0x06002575 RID: 9589 RVA: 0x0010DFCE File Offset: 0x0010C3CE
		// (set) Token: 0x06002576 RID: 9590 RVA: 0x0010DFD6 File Offset: 0x0010C3D6
		public Value StreamerRateUpgradePrice { get; set; }

		// Token: 0x170001CB RID: 459
		// (get) Token: 0x06002577 RID: 9591 RVA: 0x0010DFDF File Offset: 0x0010C3DF
		// (set) Token: 0x06002578 RID: 9592 RVA: 0x0010DFE7 File Offset: 0x0010C3E7
		public Value StreamerRateUpgradeTier { get; set; }

		// Token: 0x170001CC RID: 460
		// (get) Token: 0x06002579 RID: 9593 RVA: 0x0010DFF0 File Offset: 0x0010C3F0
		// (set) Token: 0x0600257A RID: 9594 RVA: 0x0010DFF8 File Offset: 0x0010C3F8
		public Value FlamerRateUpgradePrice { get; set; }

		// Token: 0x170001CD RID: 461
		// (get) Token: 0x0600257B RID: 9595 RVA: 0x0010E001 File Offset: 0x0010C401
		// (set) Token: 0x0600257C RID: 9596 RVA: 0x0010E009 File Offset: 0x0010C409
		public Value FlamerRateUpgradeTier { get; set; }

		// Token: 0x170001CE RID: 462
		// (get) Token: 0x0600257D RID: 9597 RVA: 0x0010E012 File Offset: 0x0010C412
		// (set) Token: 0x0600257E RID: 9598 RVA: 0x0010E01A File Offset: 0x0010C41A
		public Value MessageConvertedUpgradePrice { get; set; }

		// Token: 0x170001CF RID: 463
		// (get) Token: 0x0600257F RID: 9599 RVA: 0x0010E023 File Offset: 0x0010C423
		// (set) Token: 0x06002580 RID: 9600 RVA: 0x0010E02B File Offset: 0x0010C42B
		public Value MessageConvertedUpgradeTier { get; set; }

		// Token: 0x170001D0 RID: 464
		// (get) Token: 0x06002581 RID: 9601 RVA: 0x0010E034 File Offset: 0x0010C434
		// (set) Token: 0x06002582 RID: 9602 RVA: 0x0010E03C File Offset: 0x0010C43C
		public Value MessageCpsBonusUpgradePrice { get; set; }

		// Token: 0x170001D1 RID: 465
		// (get) Token: 0x06002583 RID: 9603 RVA: 0x0010E045 File Offset: 0x0010C445
		// (set) Token: 0x06002584 RID: 9604 RVA: 0x0010E04D File Offset: 0x0010C44D
		public Value MessageCpsBonusUpgradeTier { get; set; }

		// Token: 0x170001D2 RID: 466
		// (get) Token: 0x06002585 RID: 9605 RVA: 0x0010E056 File Offset: 0x0010C456
		// (set) Token: 0x06002586 RID: 9606 RVA: 0x0010E05E File Offset: 0x0010C45E
		public Value HypnosisUpgradePrice { get; set; }

		// Token: 0x170001D3 RID: 467
		// (get) Token: 0x06002587 RID: 9607 RVA: 0x0010E067 File Offset: 0x0010C467
		// (set) Token: 0x06002588 RID: 9608 RVA: 0x0010E06F File Offset: 0x0010C46F
		public Value HypnosisUpgradeTier { get; set; }

		// Token: 0x170001D4 RID: 468
		// (get) Token: 0x06002589 RID: 9609 RVA: 0x0010E078 File Offset: 0x0010C478
		// (set) Token: 0x0600258A RID: 9610 RVA: 0x0010E080 File Offset: 0x0010C480
		public Value PopupTimeUpgradePrice { get; set; }

		// Token: 0x170001D5 RID: 469
		// (get) Token: 0x0600258B RID: 9611 RVA: 0x0010E089 File Offset: 0x0010C489
		// (set) Token: 0x0600258C RID: 9612 RVA: 0x0010E091 File Offset: 0x0010C491
		public Value PopupTimeUpgradeTier { get; set; }

		// Token: 0x170001D6 RID: 470
		// (get) Token: 0x0600258D RID: 9613 RVA: 0x0010E09A File Offset: 0x0010C49A
		// (set) Token: 0x0600258E RID: 9614 RVA: 0x0010E0A2 File Offset: 0x0010C4A2
		public Value DrugDealerCount { get; set; }

		// Token: 0x170001D7 RID: 471
		// (get) Token: 0x0600258F RID: 9615 RVA: 0x0010E0AB File Offset: 0x0010C4AB
		// (set) Token: 0x06002590 RID: 9616 RVA: 0x0010E0B3 File Offset: 0x0010C4B3
		public Value DrugDealerIncomeMultiplier { get; set; }

		// Token: 0x170001D8 RID: 472
		// (get) Token: 0x06002591 RID: 9617 RVA: 0x0010E0BC File Offset: 0x0010C4BC
		// (set) Token: 0x06002592 RID: 9618 RVA: 0x0010E0C4 File Offset: 0x0010C4C4
		public Value DrugDealerPrice { get; set; }

		// Token: 0x170001D9 RID: 473
		// (get) Token: 0x06002593 RID: 9619 RVA: 0x0010E0CD File Offset: 0x0010C4CD
		// (set) Token: 0x06002594 RID: 9620 RVA: 0x0010E0D5 File Offset: 0x0010C4D5
		public Value DrugDealerRateUpgradePrice { get; set; }

		// Token: 0x170001DA RID: 474
		// (get) Token: 0x06002595 RID: 9621 RVA: 0x0010E0DE File Offset: 0x0010C4DE
		// (set) Token: 0x06002596 RID: 9622 RVA: 0x0010E0E6 File Offset: 0x0010C4E6
		public Value DrugDealerRateUpgradeTier { get; set; }

		// Token: 0x170001DB RID: 475
		// (get) Token: 0x06002597 RID: 9623 RVA: 0x0010E0EF File Offset: 0x0010C4EF
		// (set) Token: 0x06002598 RID: 9624 RVA: 0x0010E0F7 File Offset: 0x0010C4F7
		public Value WifiCount { get; set; }

		// Token: 0x170001DC RID: 476
		// (get) Token: 0x06002599 RID: 9625 RVA: 0x0010E100 File Offset: 0x0010C500
		// (set) Token: 0x0600259A RID: 9626 RVA: 0x0010E108 File Offset: 0x0010C508
		public Value WifiIncomeMultiplier { get; set; }

		// Token: 0x170001DD RID: 477
		// (get) Token: 0x0600259B RID: 9627 RVA: 0x0010E111 File Offset: 0x0010C511
		// (set) Token: 0x0600259C RID: 9628 RVA: 0x0010E119 File Offset: 0x0010C519
		public Value WifiPrice { get; set; }

		// Token: 0x170001DE RID: 478
		// (get) Token: 0x0600259D RID: 9629 RVA: 0x0010E122 File Offset: 0x0010C522
		// (set) Token: 0x0600259E RID: 9630 RVA: 0x0010E12A File Offset: 0x0010C52A
		public Value WifiRateUpgradePrice { get; set; }

		// Token: 0x170001DF RID: 479
		// (get) Token: 0x0600259F RID: 9631 RVA: 0x0010E133 File Offset: 0x0010C533
		// (set) Token: 0x060025A0 RID: 9632 RVA: 0x0010E13B File Offset: 0x0010C53B
		public Value WifiRateUpgradeTier { get; set; }

		// Token: 0x170001E0 RID: 480
		// (get) Token: 0x060025A1 RID: 9633 RVA: 0x0010E144 File Offset: 0x0010C544
		// (set) Token: 0x060025A2 RID: 9634 RVA: 0x0010E14C File Offset: 0x0010C54C
		public Value IndieDevCount { get; set; }

		// Token: 0x170001E1 RID: 481
		// (get) Token: 0x060025A3 RID: 9635 RVA: 0x0010E155 File Offset: 0x0010C555
		// (set) Token: 0x060025A4 RID: 9636 RVA: 0x0010E15D File Offset: 0x0010C55D
		public Value IndieDevIncomeMultiplier { get; set; }

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x060025A5 RID: 9637 RVA: 0x0010E166 File Offset: 0x0010C566
		// (set) Token: 0x060025A6 RID: 9638 RVA: 0x0010E16E File Offset: 0x0010C56E
		public Value IndieDevPrice { get; set; }

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x060025A7 RID: 9639 RVA: 0x0010E177 File Offset: 0x0010C577
		// (set) Token: 0x060025A8 RID: 9640 RVA: 0x0010E17F File Offset: 0x0010C57F
		public Value IndieDevRateUpgradePrice { get; set; }

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x060025A9 RID: 9641 RVA: 0x0010E188 File Offset: 0x0010C588
		// (set) Token: 0x060025AA RID: 9642 RVA: 0x0010E190 File Offset: 0x0010C590
		public Value IndieDevRateUpgradeTier { get; set; }

		// Token: 0x060025AB RID: 9643 RVA: 0x0010E19C File Offset: 0x0010C59C
		private void PrepareHashtable()
		{
			this.hashtable = new Hashtable
			{
				{
					DataSerializer.PPREF_FRENZY,
					this.FrenzyCount
				},
				{
					DataSerializer.PPREF_FRENZY_PERCENT,
					this.FrenzyAccelerationPercent
				},
				{
					DataSerializer.PPREF_FRENZY_TIME,
					this.FrenzyTime
				},
				{
					DataSerializer.PPREF_POPUP_TIME,
					this.BonusPopupShowTime
				},
				{
					DataSerializer.PPREF_POPUP_COUNT,
					this.BonusPopupCount
				},
				{
					DataSerializer.PPREF_CONVERTED,
					this.Converted
				},
				{
					DataSerializer.PPREF_HOT_COINS,
					this.HotCoins
				},
				{
					DataSerializer.PPREF_TOTAL_HOT_COINS,
					this.TotalHotCoinsEarned
				},
				{
					DataSerializer.PPREF_HOT_COINS_PER_CONVERTED,
					this.HotCoinsPerConverted
				},
				{
					DataSerializer.PPREF_CONVERTED_PER_MESSAGE,
					this.ConvertedPerMessage
				},
				{
					DataSerializer.PPREF_WRITTEN_MESSAGES_COUNT,
					this.WrittenMessagesCount
				},
				{
					DataSerializer.PPREF_MESSAGE_CONVERTED_MULT,
					this.MessageConvertedMultiplier
				},
				{
					DataSerializer.PPREF_MESSAGE_CONVERTED_PRICE,
					this.MessageConvertedUpgradePrice
				},
				{
					DataSerializer.PPREF_MESSAGE_CONVERTED_TIER,
					this.MessageConvertedUpgradeTier
				},
				{
					DataSerializer.PPREF_BRAINWASH_TIER,
					this.BrainwashTier
				},
				{
					DataSerializer.PPREF_MESSAGE_CPS_BONUS,
					this.MessagesCpsBonus
				},
				{
					DataSerializer.PPREF_MESSAGE_CPS_BONUS_PRICE,
					this.MessageCpsBonusUpgradePrice
				},
				{
					DataSerializer.PPREF_MESSAGE_CPS_BONUS_TIER,
					this.MessageCpsBonusUpgradeTier
				},
				{
					DataSerializer.PPREF_HYPNOSIS,
					this.HypnosisCount
				},
				{
					DataSerializer.PPREF_HYPNOSIS_PERCENT,
					this.HypnosisConvertedPercent
				},
				{
					DataSerializer.PPREF_HYPNOSIS_UPGRADE_PRICE,
					this.HypnosisUpgradePrice
				},
				{
					DataSerializer.PPREF_HYPNOSIS_UPGRADE_TIER,
					this.HypnosisUpgradeTier
				},
				{
					DataSerializer.PPREF_POPUP_TIME_UPGRADE_PRICE,
					this.PopupTimeUpgradePrice
				},
				{
					DataSerializer.PPREF_POPUP_TIME_UPGRADE_TIER,
					this.PopupTimeUpgradeTier
				},
				{
					DataSerializer.PPREF_FAKE_ACCOUNTS,
					this.FakeAccountCount
				},
				{
					DataSerializer.PPREF_FAKE_ACCOUNTS_PRICE,
					this.FakeAccountPrice
				},
				{
					DataSerializer.PPREF_FANS,
					this.FanCount
				},
				{
					DataSerializer.PPREF_FANS_PRICE,
					this.FanPrice
				},
				{
					DataSerializer.PPREF_FAN_INCOME_MULT,
					this.FanIncomeMultiplier
				},
				{
					DataSerializer.PPREF_FAN_RATE_PRICE,
					this.FanRateUpgradePrice
				},
				{
					DataSerializer.PPREF_FAN_RATE_TIER,
					this.FanRateUpgradeTier
				},
				{
					DataSerializer.PPREF_BOTS,
					this.BotCount
				},
				{
					DataSerializer.PPREF_BOTS_PRICE,
					this.BotPrice
				},
				{
					DataSerializer.PPREF_BOT_INCOME_MULT,
					this.BotIncomeMultiplier
				},
				{
					DataSerializer.PPREF_BOT_RATE_PRICE,
					this.BotRateUpgradePrice
				},
				{
					DataSerializer.PPREF_BOT_RATE_TIER,
					this.BotRateUpgradeTier
				},
				{
					DataSerializer.PPREF_STREAMERS,
					this.StreamerCount
				},
				{
					DataSerializer.PPREF_STREAMERS_PRICE,
					this.StreamerPrice
				},
				{
					DataSerializer.PPREF_STREAMER_INCOME_MULT,
					this.StreamerIncomeMultiplier
				},
				{
					DataSerializer.PPREF_STREAMER_RATE_PRICE,
					this.StreamerRateUpgradePrice
				},
				{
					DataSerializer.PPREF_STREAMER_RATE_TIER,
					this.StreamerRateUpgradeTier
				},
				{
					DataSerializer.PPREF_FLAMER,
					this.FlamerCount
				},
				{
					DataSerializer.PPREF_FLAMERS_PRICE,
					this.FlamerPrice
				},
				{
					DataSerializer.PPREF_FLAMER_INCOME_MULT,
					this.FlamerIncomeMultiplier
				},
				{
					DataSerializer.PPREF_FLAMER_RATE_PRICE,
					this.FlamerRateUpgradePrice
				},
				{
					DataSerializer.PPREF_FLAMER_RATE_TIER,
					this.FlamerRateUpgradeTier
				},
				{
					DataSerializer.PPREF_DRUG_DEALERS,
					this.DrugDealerCount
				},
				{
					DataSerializer.PPREF_DRUG_DEALER_INCOME_MULT,
					this.DrugDealerIncomeMultiplier
				},
				{
					DataSerializer.PPREF_DRUG_DEALERS_PRICE,
					this.DrugDealerPrice
				},
				{
					DataSerializer.PPREF_DRUG_DEALER_RATE_PRICE,
					this.DrugDealerRateUpgradePrice
				},
				{
					DataSerializer.PPREF_DRUG_DEALER_RATE_TIER,
					this.DrugDealerRateUpgradeTier
				},
				{
					DataSerializer.PPREF_WIFIS,
					this.WifiCount
				},
				{
					DataSerializer.PPREF_WIFI_INCOME_MULT,
					this.WifiIncomeMultiplier
				},
				{
					DataSerializer.PPREF_WIFIS_PRICE,
					this.WifiPrice
				},
				{
					DataSerializer.PPREF_WIFI_RATE_PRICE,
					this.WifiRateUpgradePrice
				},
				{
					DataSerializer.PPREF_WIFI_RATE_TIER,
					this.WifiRateUpgradeTier
				},
				{
					DataSerializer.PPREF_INDIE_DEVS,
					this.IndieDevCount
				},
				{
					DataSerializer.PPREF_INDIE_DEV_INCOME_MULT,
					this.IndieDevIncomeMultiplier
				},
				{
					DataSerializer.PPREF_INDIE_DEVS_PRICE,
					this.IndieDevPrice
				},
				{
					DataSerializer.PPREF_INDIE_DEV_RATE_PRICE,
					this.IndieDevRateUpgradePrice
				},
				{
					DataSerializer.PPREF_INDIE_DEV_RATE_TIER,
					this.IndieDevRateUpgradeTier
				}
			};
		}

		// Token: 0x060025AC RID: 9644 RVA: 0x0010E700 File Offset: 0x0010CB00
		private void ReadHashtable()
		{
            this.HypnosisCount = (Value)this.hashtable[DataSerializer.PPREF_HYPNOSIS];
			this.HypnosisConvertedPercent = (Value)this.hashtable[DataSerializer.PPREF_HYPNOSIS_PERCENT];
			this.FrenzyCount = (Value)this.hashtable[DataSerializer.PPREF_FRENZY];
			this.FrenzyAccelerationPercent = (Value)this.hashtable[DataSerializer.PPREF_FRENZY_PERCENT];
			this.FrenzyTime = (float)this.hashtable[DataSerializer.PPREF_FRENZY_TIME];
			this.BonusPopupShowTime = (float)this.hashtable[DataSerializer.PPREF_POPUP_TIME];
			this.BonusPopupCount = (Value)this.hashtable[DataSerializer.PPREF_POPUP_COUNT];
			this.Converted = (Value)this.hashtable[DataSerializer.PPREF_CONVERTED];
			this.HotCoins = (Value)this.hashtable[DataSerializer.PPREF_HOT_COINS];
			this.TotalHotCoinsEarned = (Value)this.hashtable[DataSerializer.PPREF_TOTAL_HOT_COINS];
			this.HotCoinsPerConverted = (Value)this.hashtable[DataSerializer.PPREF_HOT_COINS_PER_CONVERTED];
			this.ConvertedPerMessage = (Value)this.hashtable[DataSerializer.PPREF_CONVERTED_PER_MESSAGE];
			this.WrittenMessagesCount = (Value)this.hashtable[DataSerializer.PPREF_WRITTEN_MESSAGES_COUNT];
			this.BrainwashTier = (Value)this.hashtable[DataSerializer.PPREF_BRAINWASH_TIER];
			this.MessageConvertedMultiplier = (Value)this.hashtable[DataSerializer.PPREF_MESSAGE_CONVERTED_MULT];
			this.MessageConvertedUpgradePrice = (Value)this.hashtable[DataSerializer.PPREF_MESSAGE_CONVERTED_PRICE];
			this.MessageConvertedUpgradeTier = (Value)this.hashtable[DataSerializer.PPREF_MESSAGE_CONVERTED_TIER];
			this.MessagesCpsBonus = (Value)this.hashtable[DataSerializer.PPREF_MESSAGE_CPS_BONUS];
			this.MessageCpsBonusUpgradePrice = (Value)this.hashtable[DataSerializer.PPREF_MESSAGE_CPS_BONUS_PRICE];
			this.MessageCpsBonusUpgradeTier = (Value)this.hashtable[DataSerializer.PPREF_MESSAGE_CPS_BONUS_TIER];
			this.HypnosisUpgradePrice = (Value)this.hashtable[DataSerializer.PPREF_HYPNOSIS_UPGRADE_PRICE];
			this.HypnosisUpgradeTier = (Value)this.hashtable[DataSerializer.PPREF_HYPNOSIS_UPGRADE_TIER];
			this.FakeAccountCount = (Value)this.hashtable[DataSerializer.PPREF_FAKE_ACCOUNTS];
			this.FakeAccountPrice = (Value)this.hashtable[DataSerializer.PPREF_FAKE_ACCOUNTS_PRICE];
			this.FanCount = (Value)this.hashtable[DataSerializer.PPREF_FANS];
			this.FanIncomeMultiplier = (Value)this.hashtable[DataSerializer.PPREF_FAN_INCOME_MULT];
			this.FanPrice = (Value)this.hashtable[DataSerializer.PPREF_FANS_PRICE];
			this.FanRateUpgradePrice = (Value)this.hashtable[DataSerializer.PPREF_FAN_RATE_PRICE];
			this.FanRateUpgradeTier = (Value)this.hashtable[DataSerializer.PPREF_FAN_RATE_TIER];
			this.BotCount = (Value)this.hashtable[DataSerializer.PPREF_BOTS];
			this.BotIncomeMultiplier = (Value)this.hashtable[DataSerializer.PPREF_BOT_INCOME_MULT];
			this.BotPrice = (Value)this.hashtable[DataSerializer.PPREF_BOTS_PRICE];
			this.BotRateUpgradePrice = (Value)this.hashtable[DataSerializer.PPREF_BOT_RATE_PRICE];
			this.BotRateUpgradeTier = (Value)this.hashtable[DataSerializer.PPREF_BOT_RATE_TIER];
			this.StreamerCount = (Value)this.hashtable[DataSerializer.PPREF_STREAMERS];
			this.StreamerIncomeMultiplier = (Value)this.hashtable[DataSerializer.PPREF_STREAMER_INCOME_MULT];
			this.StreamerPrice = (Value)this.hashtable[DataSerializer.PPREF_STREAMERS_PRICE];
			this.StreamerRateUpgradePrice = (Value)this.hashtable[DataSerializer.PPREF_STREAMER_RATE_PRICE];
			this.StreamerRateUpgradeTier = (Value)this.hashtable[DataSerializer.PPREF_STREAMER_RATE_TIER];
			this.FlamerCount = (Value)this.hashtable[DataSerializer.PPREF_FLAMER];
			this.FlamerIncomeMultiplier = (Value)this.hashtable[DataSerializer.PPREF_FLAMER_INCOME_MULT];
			this.FlamerPrice = (Value)this.hashtable[DataSerializer.PPREF_FLAMERS_PRICE];
			this.FlamerRateUpgradePrice = (Value)this.hashtable[DataSerializer.PPREF_FLAMER_RATE_PRICE];
			this.FlamerRateUpgradeTier = (Value)this.hashtable[DataSerializer.PPREF_FLAMER_RATE_TIER];
			this.DrugDealerCount = (Value)this.hashtable[DataSerializer.PPREF_DRUG_DEALERS];
			this.DrugDealerIncomeMultiplier = (Value)this.hashtable[DataSerializer.PPREF_DRUG_DEALER_INCOME_MULT];
			this.DrugDealerPrice = (Value)this.hashtable[DataSerializer.PPREF_DRUG_DEALERS_PRICE];
			this.DrugDealerRateUpgradePrice = (Value)this.hashtable[DataSerializer.PPREF_DRUG_DEALER_RATE_PRICE];
			this.DrugDealerRateUpgradeTier = (Value)this.hashtable[DataSerializer.PPREF_DRUG_DEALER_RATE_TIER];
			this.WifiCount = (Value)this.hashtable[DataSerializer.PPREF_WIFIS];
			this.WifiIncomeMultiplier = (Value)this.hashtable[DataSerializer.PPREF_WIFI_INCOME_MULT];
			this.WifiPrice = (Value)this.hashtable[DataSerializer.PPREF_WIFIS_PRICE];
			this.WifiRateUpgradePrice = (Value)this.hashtable[DataSerializer.PPREF_WIFI_RATE_PRICE];
			this.WifiRateUpgradeTier = (Value)this.hashtable[DataSerializer.PPREF_WIFI_RATE_TIER];
			this.IndieDevCount = (Value)this.hashtable[DataSerializer.PPREF_INDIE_DEVS];
			this.IndieDevIncomeMultiplier = (Value)this.hashtable[DataSerializer.PPREF_INDIE_DEV_INCOME_MULT];
			this.IndieDevPrice = (Value)this.hashtable[DataSerializer.PPREF_INDIE_DEVS_PRICE];
			this.IndieDevRateUpgradePrice = (Value)this.hashtable[DataSerializer.PPREF_INDIE_DEV_RATE_PRICE];
			this.IndieDevRateUpgradeTier = (Value)this.hashtable[DataSerializer.PPREF_INDIE_DEV_RATE_TIER];
		}

		// Token: 0x060025AD RID: 9645 RVA: 0x0010EE70 File Offset: 0x0010D270
		public void Serialize()
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			this.PrepareHashtable();
			FileInfo fileInfo = new FileInfo(this.filePath);
			fileInfo.Directory.Create();
			using (FileStream fileStream = new FileStream(this.filePath, FileMode.OpenOrCreate))
			{
				File.SetAttributes(this.filePath, FileAttributes.Hidden);
				try
				{
					binaryFormatter.Serialize(fileStream, this.hashtable);
					this.hashtable.Clear();
				}
				catch (SerializationException ex)
				{
                    MelonLogger.Msg("Failed to serialize. Reason: " + ex.Message);
					throw;
				}
			}
		}

		// Token: 0x060025AE RID: 9646 RVA: 0x0010EF1C File Offset: 0x0010D31C
		public void Deserialize()
		{
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			this.hashtable = null;
            if (File.Exists(this.filePath))
			{
				using (FileStream fileStream = new FileStream(this.filePath, FileMode.Open))
				{
					try
					{
                        this.hashtable = (Hashtable)binaryFormatter.Deserialize(fileStream);
                    }
					catch (SerializationException ex)
					{
						MelonLogger.Msg("Failed to serialize. Reason: " + ex.Message);
						throw;
					}
				}
				this.ReadHashtable();
			}
		}

		// Token: 0x060025AF RID: 9647 RVA: 0x0010EFB8 File Offset: 0x0010D3B8
		public void ResetProgress()
		{
			this.HypnosisCount = Value.Zero;
			this.HypnosisConvertedPercent = null;
			this.FrenzyCount = null;
			this.FrenzyAccelerationPercent = null;
			this.FrenzyTime = 0f;
			this.BonusPopupShowTime = 0f;
			this.BonusPopupCount = null;
			this.Converted = null;
			this.ConvertedPerMessage = null;
			this.MessageConvertedMultiplier = null;
			this.MessageConvertedUpgradePrice = null;
			this.MessageConvertedUpgradeTier = null;
			this.HypnosisUpgradePrice = null;
			this.HypnosisUpgradeTier = null;
			this.FakeAccountCount = null;
			this.FakeAccountPrice = null;
			this.FanCount = null;
			this.FanIncomeMultiplier = null;
			this.FanPrice = null;
			this.FanRateUpgradePrice = null;
			this.FanRateUpgradeTier = null;
			this.BotCount = null;
			this.BotIncomeMultiplier = null;
			this.BotPrice = null;
			this.BotRateUpgradePrice = null;
			this.BotRateUpgradeTier = null;
			this.StreamerCount = null;
			this.StreamerIncomeMultiplier = null;
			this.StreamerPrice = null;
			this.StreamerRateUpgradePrice = null;
			this.StreamerRateUpgradeTier = null;
			this.FlamerCount = null;
			this.FlamerIncomeMultiplier = null;
			this.FlamerPrice = null;
			this.FlamerRateUpgradePrice = null;
			this.FlamerRateUpgradeTier = null;
			this.DrugDealerCount = null;
			this.DrugDealerIncomeMultiplier = null;
			this.DrugDealerPrice = null;
			this.DrugDealerRateUpgradePrice = null;
			this.DrugDealerRateUpgradeTier = null;
			this.WifiCount = null;
			this.WifiIncomeMultiplier = null;
			this.WifiPrice = null;
			this.WifiRateUpgradePrice = null;
			this.WifiRateUpgradeTier = null;
			this.IndieDevCount = null;
			this.IndieDevIncomeMultiplier = null;
			this.IndieDevPrice = null;
			this.IndieDevRateUpgradePrice = null;
			this.IndieDevRateUpgradeTier = null;
		}

		// Token: 0x04002756 RID: 10070
		private static DataSerializer instance;

		// Token: 0x04002757 RID: 10071
		private readonly string filePath = Application.dataPath + "/Resources/StoryApps/Games/IdleMinigame/SaveData/data.hot";

		// Token: 0x04002758 RID: 10072
		private Hashtable hashtable;

		// Token: 0x04002796 RID: 10134
		private static readonly int PPREF_FAKE_ACCOUNTS = "fake_accounts".GetHashCode();

		// Token: 0x04002797 RID: 10135
		private static readonly int PPREF_FANS = "fans".GetHashCode();

		// Token: 0x04002798 RID: 10136
		private static readonly int PPREF_CONVERTED = "converted".GetHashCode();

		// Token: 0x04002799 RID: 10137
		private static readonly int PPREF_BOTS = "bots".GetHashCode();

		// Token: 0x0400279A RID: 10138
		private static readonly int PPREF_STREAMERS = "streamers".GetHashCode();

		// Token: 0x0400279B RID: 10139
		private static readonly int PPREF_FLAMER = "flamers".GetHashCode();

		// Token: 0x0400279C RID: 10140
		private static readonly int PPREF_HOT_COINS = "hot_coins".GetHashCode();

		// Token: 0x0400279D RID: 10141
		private static readonly int PPREF_TOTAL_HOT_COINS = "total_hot_coins".GetHashCode();

		// Token: 0x0400279E RID: 10142
		private static readonly int PPREF_HOT_COINS_PER_CONVERTED = "hot_coins_per_converted".GetHashCode();

		// Token: 0x0400279F RID: 10143
		private static readonly int PPREF_CONVERTED_PER_MESSAGE = "converted_per_message".GetHashCode();

		// Token: 0x040027A0 RID: 10144
		private static readonly int PPREF_WRITTEN_MESSAGES_COUNT = "writtne_messages_count".GetHashCode();

		// Token: 0x040027A1 RID: 10145
		private static readonly int PPREF_MESSAGE_CPS_BONUS = "message_cps_bonus".GetHashCode();

		// Token: 0x040027A2 RID: 10146
		private static readonly int PPREF_BOT_INCOME_MULT = "bot_income_mult".GetHashCode();

		// Token: 0x040027A3 RID: 10147
		private static readonly int PPREF_FAN_INCOME_MULT = "fan_income_mult".GetHashCode();

		// Token: 0x040027A4 RID: 10148
		private static readonly int PPREF_STREAMER_INCOME_MULT = "streamer_income_mult".GetHashCode();

		// Token: 0x040027A5 RID: 10149
		private static readonly int PPREF_FLAMER_INCOME_MULT = "flamer_income_mult".GetHashCode();

		// Token: 0x040027A6 RID: 10150
		private static readonly int PPREF_MESSAGE_CONVERTED_MULT = "message_converted_mult".GetHashCode();

		// Token: 0x040027A7 RID: 10151
		private static readonly int PPREF_POPUP_TIME = "popup_time".GetHashCode();

		// Token: 0x040027A8 RID: 10152
		private static readonly int PPREF_POPUP_COUNT = "popup_count".GetHashCode();

		// Token: 0x040027A9 RID: 10153
		private static readonly int PPREF_FAKE_ACCOUNTS_PRICE = "fake_accounts_price".GetHashCode();

		// Token: 0x040027AA RID: 10154
		private static readonly int PPREF_FANS_PRICE = "fans_price".GetHashCode();

		// Token: 0x040027AB RID: 10155
		private static readonly int PPREF_BOTS_PRICE = "bots_price".GetHashCode();

		// Token: 0x040027AC RID: 10156
		private static readonly int PPREF_STREAMERS_PRICE = "streamers_price".GetHashCode();

		// Token: 0x040027AD RID: 10157
		private static readonly int PPREF_FLAMERS_PRICE = "flamers_price".GetHashCode();

		// Token: 0x040027AE RID: 10158
		private static readonly int PPREF_BOT_RATE_PRICE = "bot_rate_price".GetHashCode();

		// Token: 0x040027AF RID: 10159
		private static readonly int PPREF_BOT_RATE_TIER = "bot_rate_tier".GetHashCode();

		// Token: 0x040027B0 RID: 10160
		private static readonly int PPREF_HYPNOSIS = "hypnosis".GetHashCode();

		// Token: 0x040027B1 RID: 10161
		private static readonly int PPREF_HYPNOSIS_PERCENT = "hypnosis_percent".GetHashCode();

		// Token: 0x040027B2 RID: 10162
		private static readonly int PPREF_FRENZY = "frenzy".GetHashCode();

		// Token: 0x040027B3 RID: 10163
		private static readonly int PPREF_FRENZY_PERCENT = "frenzy_percent".GetHashCode();

		// Token: 0x040027B4 RID: 10164
		private static readonly int PPREF_FRENZY_TIME = "frenzy_time".GetHashCode();

		// Token: 0x040027B5 RID: 10165
		private static readonly int PPREF_BRAINWASH_TIER = "brainwash_tier".GetHashCode();

		// Token: 0x040027B6 RID: 10166
		private static readonly int PPREF_FAN_RATE_PRICE = "fan_rate_price".GetHashCode();

		// Token: 0x040027B7 RID: 10167
		private static readonly int PPREF_FAN_RATE_TIER = "fan_rate_tier".GetHashCode();

		// Token: 0x040027B8 RID: 10168
		private static readonly int PPREF_STREAMER_RATE_PRICE = "streamer_rate_price".GetHashCode();

		// Token: 0x040027B9 RID: 10169
		private static readonly int PPREF_STREAMER_RATE_TIER = "streamer_rate_tier".GetHashCode();

		// Token: 0x040027BA RID: 10170
		private static readonly int PPREF_FLAMER_RATE_PRICE = "flamer_rate_rice".GetHashCode();

		// Token: 0x040027BB RID: 10171
		private static readonly int PPREF_FLAMER_RATE_TIER = "flamer_rate_tier".GetHashCode();

		// Token: 0x040027BC RID: 10172
		private static readonly int PPREF_MESSAGE_CONVERTED_PRICE = "message_converted_price".GetHashCode();

		// Token: 0x040027BD RID: 10173
		private static readonly int PPREF_MESSAGE_CONVERTED_TIER = "message_converted_tier".GetHashCode();

		// Token: 0x040027BE RID: 10174
		private static readonly int PPREF_MESSAGE_CPS_BONUS_PRICE = "message_cps_bonus_price".GetHashCode();

		// Token: 0x040027BF RID: 10175
		private static readonly int PPREF_MESSAGE_CPS_BONUS_TIER = "message_cps_bonus_tier".GetHashCode();

		// Token: 0x040027C0 RID: 10176
		private static readonly int PPREF_HYPNOSIS_UPGRADE_PRICE = "hypnosis_upgrade_price".GetHashCode();

		// Token: 0x040027C1 RID: 10177
		private static readonly int PPREF_HYPNOSIS_UPGRADE_TIER = "hypnosis_upgrade_tier".GetHashCode();

		// Token: 0x040027C2 RID: 10178
		private static readonly int PPREF_POPUP_TIME_UPGRADE_PRICE = "popup_time_upgrade_price".GetHashCode();

		// Token: 0x040027C3 RID: 10179
		private static readonly int PPREF_POPUP_TIME_UPGRADE_TIER = "popup_time_upgrade_tier".GetHashCode();

		// Token: 0x040027C4 RID: 10180
		private static readonly int PPREF_DRUG_DEALERS = "drug_dealers".GetHashCode();

		// Token: 0x040027C5 RID: 10181
		private static readonly int PPREF_DRUG_DEALER_INCOME_MULT = "drug_dealer_income_mult".GetHashCode();

		// Token: 0x040027C6 RID: 10182
		private static readonly int PPREF_DRUG_DEALERS_PRICE = "drug_dealers_price".GetHashCode();

		// Token: 0x040027C7 RID: 10183
		private static readonly int PPREF_DRUG_DEALER_RATE_PRICE = "drug_dealer_rate_price".GetHashCode();

		// Token: 0x040027C8 RID: 10184
		private static readonly int PPREF_DRUG_DEALER_RATE_TIER = "drug_dealer_rate_tier".GetHashCode();

		// Token: 0x040027C9 RID: 10185
		private static readonly int PPREF_WIFIS = "wifis".GetHashCode();

		// Token: 0x040027CA RID: 10186
		private static readonly int PPREF_WIFI_INCOME_MULT = "wifi_income_mult".GetHashCode();

		// Token: 0x040027CB RID: 10187
		private static readonly int PPREF_WIFIS_PRICE = "wifis_price".GetHashCode();

		// Token: 0x040027CC RID: 10188
		private static readonly int PPREF_WIFI_RATE_PRICE = "wifi_rate_price".GetHashCode();

		// Token: 0x040027CD RID: 10189
		private static readonly int PPREF_WIFI_RATE_TIER = "wifi_rate_tier".GetHashCode();

		// Token: 0x040027CE RID: 10190
		private static readonly int PPREF_INDIE_DEVS = "indie_devs".GetHashCode();

		// Token: 0x040027CF RID: 10191
		private static readonly int PPREF_INDIE_DEV_INCOME_MULT = "indie_devs_income_mult".GetHashCode();

		// Token: 0x040027D0 RID: 10192
		private static readonly int PPREF_INDIE_DEVS_PRICE = "indie_devs_price".GetHashCode();

		// Token: 0x040027D1 RID: 10193
		private static readonly int PPREF_INDIE_DEV_RATE_PRICE = "indie_devs_rate_price".GetHashCode();

		// Token: 0x040027D2 RID: 10194
		private static readonly int PPREF_INDIE_DEV_RATE_TIER = "indie_devs_rate_tier".GetHashCode();
	}

	public class Drug : SHSharp::SHGUIsprite
	{
		// Token: 0x060024FC RID: 9468 RVA: 0x0010CA97 File Offset: 0x0010AE97
		public Drug()
		{
			// TODO: Check IdleGame_Drug if this one looks weird - it's unclear which one was intended.
			MCDCom.AddFrameFromStr(this, MCDCom.AssetToText(MCDCom.GetAssetName("IdleGame_Drug2.txt")), 3);
			//base.AddFramesFromFile(StoryAppsResources.IdleGameDrug, 3, false);
			this.currentFrame = 0;
		}

		// Token: 0x060024FD RID: 9469 RVA: 0x0010CAC4 File Offset: 0x0010AEC4
		public override void Update()
		{
			base.Update();
			if (this.animationTimer >= this.animationTime)
			{
				if (this.currentFrame == this.frames.Count - 1)
				{
					this.currentFrame = 0;
				}
				else
				{
					this.currentFrame++;
				}
				this.animationTimer = 0f;
			}
			else
			{
				this.animationTimer += Time.unscaledDeltaTime;
			}
		}

		// Token: 0x04002717 RID: 10007
		private float animationTimer;

		// Token: 0x04002718 RID: 10008
		private readonly float animationTime = SharedTheSystem.Views.DrugAnimationTime;
	}

	public class DrugDealerRateUpgrade : AUpgrade
	{
		// Token: 0x0600263F RID: 9791 RVA: 0x001122AC File Offset: 0x001106AC
		public DrugDealerRateUpgrade(string text, int startX, int startY, char color) : base(text, startX, startY, color)
		{
			base.UpgradeType = SharedUpgrades.UpgradeType.DrugDealer;
			base.StartPrice = SharedTheSystem.Upgrades.DrugDealerRateUpgradeStartPrice;
			Value price;
			Value tier;
			SharedTheSystem.Upgrades.GetDrugDealerRateUpgrade(out price, out tier);
			base.Price = price;
			base.Tier = tier;
		}

		// Token: 0x06002640 RID: 9792 RVA: 0x001122F7 File Offset: 0x001106F7
		public override void ActivateUpgrade()
		{
			if (base.CanBeBought())
			{
				base.Buy();
				SharedTheSystem.Upgrades.IncreaseRateMultiplier(base.UpgradeType);
				base.UpdatePriceAndTier();
				SharedTheSystem.Upgrades.SaveDrugDealerRateUpgrade(base.Price, base.Tier);
			}
		}

		// Token: 0x06002641 RID: 9793 RVA: 0x00112336 File Offset: 0x00110736
		public override bool CanBeUsed()
		{
			return SharedTheSystem.Workers.GetDrugDealers() >= this.GetRequiredWorkers();
		}

		// Token: 0x06002642 RID: 9794 RVA: 0x0011234D File Offset: 0x0011074D
		public override void UpdateHintText()
		{
			base.HintText = SharedTheSystem.Upgrades.DrugDealerRateHint + SharedTheSystem.GetUpgradesHintStats(IncomeController.Instance.TotalDrugDealerIncomePerSec());
			base.UpdateHintText();
		}
	}

	public class Fan : SHSharp::SHGUIsprite
	{
		// Token: 0x060024FE RID: 9470 RVA: 0x0010CB3C File Offset: 0x0010AF3C
		public Fan()
		{
			MCDCom.AddFrameFromStr(this, MCDCom.AssetToText(MCDCom.GetAssetName("IdleGame_Fan.txt")), 3);
			//base.AddFramesFromFile(StoryAppsResources.IdleGameFan, 3, false);
			if (SharedTheSystem.Workers.GetFans() == Value.Zero)
			{
				this.currentAnimationFrame = Fan.AnimationEnum.NoFans;
				this.currentFrame = 0;
			}
			else
			{
				this.currentAnimationFrame = Fan.AnimationEnum.Idle;
				this.currentFrame = 4;
			}
		}

		// Token: 0x060024FF RID: 9471 RVA: 0x0010CBA8 File Offset: 0x0010AFA8
		public override void Update()
		{
			base.Update();
			switch (this.currentAnimationFrame)
			{
				case Fan.AnimationEnum.NoFans:
					if (SharedTheSystem.Workers.GetFans() > Value.Zero)
					{
						this.currentAnimationFrame = Fan.AnimationEnum.Idle;
						this.currentFrame = 4;
					}
					break;
				case Fan.AnimationEnum.Talk1:
					if (this.talkTimer >= this.talkTime)
					{
						this.currentAnimationFrame = Fan.AnimationEnum.Talk2;
						this.currentFrame = 2;
						this.talkTimer = 0f;
						this.animationCounter++;
					}
					this.ConvertMan();
					break;
				case Fan.AnimationEnum.Talk2:
					if (this.talkTimer >= this.talkTime)
					{
						this.currentAnimationFrame = Fan.AnimationEnum.Talk1;
						this.currentFrame = 1;
						this.talkTimer = 0f;
						this.animationCounter++;
					}
					this.ConvertMan();
					break;
				case Fan.AnimationEnum.Converted:
					if (this.talkTimer >= this.talkTime)
					{
						this.currentAnimationFrame = Fan.AnimationEnum.Idle;
						this.currentFrame = 4;
						this.talkTimer = 0f;
					}
					break;
				case Fan.AnimationEnum.Idle:
					if (this.talkTimer >= this.talkTime)
					{
						this.currentAnimationFrame = Fan.AnimationEnum.Talk1;
						this.currentFrame = 1;
						this.talkTimer = 0f;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			this.talkTimer += Time.unscaledDeltaTime;
		}

		// Token: 0x06002500 RID: 9472 RVA: 0x0010CD09 File Offset: 0x0010B109
		private void ConvertMan()
		{
			if (this.animationCounter >= SharedTheSystem.FanAnimationsBeforeConvert)
			{
				this.currentAnimationFrame = Fan.AnimationEnum.Converted;
				this.currentFrame = 3;
				this.animationCounter = 0;
				this.talkTimer = 0f;
			}
		}

		// Token: 0x04002719 RID: 10009
		private Fan.AnimationEnum currentAnimationFrame;

		// Token: 0x0400271A RID: 10010
		private float talkTimer;

		// Token: 0x0400271B RID: 10011
		private readonly float talkTime = SharedTheSystem.Views.FanAnimationTime;

		// Token: 0x0400271C RID: 10012
		private int animationCounter;

		// Token: 0x02000662 RID: 1634
		private enum AnimationEnum
		{
			// Token: 0x0400271E RID: 10014
			NoFans,
			// Token: 0x0400271F RID: 10015
			Talk1,
			// Token: 0x04002720 RID: 10016
			Talk2,
			// Token: 0x04002721 RID: 10017
			Converted,
			// Token: 0x04002722 RID: 10018
			Idle
		}
	}

	public class FanRateUpgrade : AUpgrade
	{
		// Token: 0x06002643 RID: 9795 RVA: 0x0011237C File Offset: 0x0011077C
		public FanRateUpgrade(string text, int startX, int startY, char color) : base(text, startX, startY, color)
		{
			base.UpgradeType = SharedUpgrades.UpgradeType.Fan;
			base.StartPrice = SharedTheSystem.Upgrades.FanRateUpgradeStartPrice;
			Value price;
			Value tier;
			SharedTheSystem.Upgrades.GetFanRateUpgrade(out price, out tier);
			base.Price = price;
			base.Tier = tier;
		}

		// Token: 0x06002644 RID: 9796 RVA: 0x001123C7 File Offset: 0x001107C7
		public override void ActivateUpgrade()
		{
			if (base.CanBeBought())
			{
				base.Buy();
				SharedTheSystem.Upgrades.IncreaseRateMultiplier(base.UpgradeType);
				base.UpdatePriceAndTier();
				SharedTheSystem.Upgrades.SaveFanRateUpgrade(base.Price, base.Tier);
			}
		}

		// Token: 0x06002645 RID: 9797 RVA: 0x00112406 File Offset: 0x00110806
		public override bool CanBeUsed()
		{
			return SharedTheSystem.Workers.GetFans() >= this.GetRequiredWorkers();
		}

		// Token: 0x06002646 RID: 9798 RVA: 0x0011241D File Offset: 0x0011081D
		public override void UpdateHintText()
		{
			base.HintText = SharedTheSystem.Upgrades.FanRateHint + SharedTheSystem.GetUpgradesHintStats(IncomeController.Instance.TotalFanIncomePerSec());
			base.UpdateHintText();
		}
	}

	public class Flamer : SHSharp::SHGUIsprite
	{
		// Token: 0x06002501 RID: 9473 RVA: 0x0010CD3C File Offset: 0x0010B13C
		public Flamer()
		{
			MCDCom.AddFrameFromStr(this, MCDCom.AssetToText(MCDCom.GetAssetName("IdleGame_Flamer.txt")), 3);
			//base.AddFramesFromFile(StoryAppsResources.IdleGameFlamer, 3, false);
			Value flamers = SharedTheSystem.Workers.GetFlamers();
			if (flamers == Value.Zero)
			{
				this.currentFrame = 0;
				this.currentAnimationFrame = Flamer.AnimationEnum.NoFlamers;
			}
			else
			{
				this.currentFrame = 1;
				this.currentAnimationFrame = Flamer.AnimationEnum.Type1;
			}
		}

		// Token: 0x06002502 RID: 9474 RVA: 0x0010CDAC File Offset: 0x0010B1AC
		public override void Update()
		{
			base.Update();
			if (this.animationTimer >= this.animationTime)
			{
				switch (this.currentAnimationFrame)
				{
					case Flamer.AnimationEnum.NoFlamers:
						if (SharedTheSystem.Workers.GetFlamers() > Value.Zero)
						{
							this.currentAnimationFrame = Flamer.AnimationEnum.Type1;
							this.currentFrame = 1;
						}
						break;
					case Flamer.AnimationEnum.Type1:
						this.currentFrame = 2;
						this.currentAnimationFrame = Flamer.AnimationEnum.Type2;
						break;
					case Flamer.AnimationEnum.Type2:
						this.currentFrame = 1;
						this.currentAnimationFrame = Flamer.AnimationEnum.Type1;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				this.animationTimer = 0f;
			}
			else
			{
				this.animationTimer += Time.unscaledDeltaTime;
			}
		}

		// Token: 0x04002723 RID: 10019
		private float animationTimer;

		// Token: 0x04002724 RID: 10020
		private readonly float animationTime = SharedTheSystem.Views.FlamerAnimationTime;

		// Token: 0x04002725 RID: 10021
		private Flamer.AnimationEnum currentAnimationFrame;

		// Token: 0x02000664 RID: 1636
		private enum AnimationEnum
		{
			// Token: 0x04002727 RID: 10023
			NoFlamers,
			// Token: 0x04002728 RID: 10024
			Type1,
			// Token: 0x04002729 RID: 10025
			Type2
		}
	}

	public class FlamerRateUpgrade : AUpgrade
	{
		// Token: 0x06002647 RID: 9799 RVA: 0x0011244C File Offset: 0x0011084C
		public FlamerRateUpgrade(string text, int startX, int startY, char color) : base(text, startX, startY, color)
		{
			base.UpgradeType = SharedUpgrades.UpgradeType.Flamer;
			base.StartPrice = SharedTheSystem.Upgrades.FlamerRateUpgradeStartPrice;
			Value price;
			Value tier;
			SharedTheSystem.Upgrades.GetFlamerRateUpgrade(out price, out tier);
			base.Price = price;
			base.Tier = tier;
		}

		// Token: 0x06002648 RID: 9800 RVA: 0x00112497 File Offset: 0x00110897
		public override void ActivateUpgrade()
		{
			if (base.CanBeBought())
			{
				base.Buy();
				SharedTheSystem.Upgrades.IncreaseRateMultiplier(base.UpgradeType);
				base.UpdatePriceAndTier();
				SharedTheSystem.Upgrades.SaveFlamerRateUpgrade(base.Price, base.Tier);
			}
		}

		// Token: 0x06002649 RID: 9801 RVA: 0x001124D6 File Offset: 0x001108D6
		public override bool CanBeUsed()
		{
			return SharedTheSystem.Workers.GetFlamers() >= this.GetRequiredWorkers();
		}

		// Token: 0x0600264A RID: 9802 RVA: 0x001124ED File Offset: 0x001108ED
		public override void UpdateHintText()
		{
			base.HintText = SharedTheSystem.Upgrades.FlamerRateHint + SharedTheSystem.GetUpgradesHintStats(IncomeController.Instance.TotalFlamerIncomePerSec());
			base.UpdateHintText();
		}
	}

	public class HotCoinEarnedText : SHSharp::SHGUItext
	{
		// Token: 0x060024E6 RID: 9446 RVA: 0x0010C294 File Offset: 0x0010A694
		public HotCoinEarnedText(Value earnedHotCoins, int x, int y, int targetX, int targetY, char color) : base(string.Empty, x, y, color, false)
		{
			this.EarnedHotCoins = earnedHotCoins;
			this.TargetX = targetX - this.text.Length;
			this.TargetY = targetY;
			this.text = string.Format("+ {0} HC", this.EarnedHotCoins);
			this.XChangeTime = 1f / (float)(targetX - x);
			this.YChangeTime = 1f / (float)(y - targetY);
		}

		// Token: 0x060024E7 RID: 9447 RVA: 0x0010C30C File Offset: 0x0010A70C
		public override void Update()
		{
			if (this.x != this.TargetX && this.positionTimerX >= this.XChangeTime)
			{
				if (this.x < this.TargetX)
				{
					this.x++;
				}
				else
				{
					this.x--;
				}
				this.positionTimerX = 0f;
			}
			if (this.y != this.TargetY && this.positionTimerY >= this.YChangeTime)
			{
				if (this.y < this.TargetY)
				{
					this.y++;
				}
				else
				{
					this.y--;
				}
				this.positionTimerY = 0f;
			}
			if (this.x == this.TargetX && this.y == this.TargetY)
			{
				this.AddCoins();
				base.KillInstant();
				this.Dead = true;
			}
			this.positionTimerY += Time.unscaledDeltaTime;
			this.positionTimerX += Time.unscaledDeltaTime;
			base.Update();
		}

		// Token: 0x060024E8 RID: 9448 RVA: 0x0010C436 File Offset: 0x0010A836
		public bool IsDead()
		{
			return this.Dead;
		}

		// Token: 0x060024E9 RID: 9449 RVA: 0x0010C43E File Offset: 0x0010A83E
		protected virtual void AddCoins()
		{
			AppTheSystem.Instance.ActionController.AddHotCoins(this.EarnedHotCoins);
		}

		// Token: 0x040026FB RID: 9979
		public Value EarnedHotCoins;

		// Token: 0x040026FC RID: 9980
		protected int TargetX;

		// Token: 0x040026FD RID: 9981
		protected int TargetY;

		// Token: 0x040026FE RID: 9982
		protected float YChangeTime;

		// Token: 0x040026FF RID: 9983
		protected float XChangeTime;

		// Token: 0x04002700 RID: 9984
		protected bool Dead;

		// Token: 0x04002701 RID: 9985
		private float positionTimerY;

		// Token: 0x04002702 RID: 9986
		private float positionTimerX;
	}

	public class HotCoinSpendText : HotCoinEarnedText
	{
		// Token: 0x060024EA RID: 9450 RVA: 0x0010C455 File Offset: 0x0010A855
		public HotCoinSpendText(Value spendHotCoins, int x, int y, int targetX, int targetY, char color) : base(spendHotCoins, x, y, targetX, targetY, color)
		{
			this.text = "- " + this.EarnedHotCoins;
			this.YChangeTime = 0.5f;
		}

		// Token: 0x060024EB RID: 9451 RVA: 0x0010C487 File Offset: 0x0010A887
		protected override void AddCoins()
		{
		}
	}

	public sealed class HotCoinsView : AStoryAppsView
	{
		// Token: 0x060026C9 RID: 9929 RVA: 0x00115530 File Offset: 0x00113930
		public HotCoinsView(int positionX, int positionY) : base(positionX, positionY, 0, 0)
		{
			base.PositionX = positionX;
			base.PositionY = positionY;
			this.hotCoinsText = new SHSharp::SHGUItext("Hot Coins x " + SharedTheSystem.General.GetHotCoins(), base.PositionX, base.PositionY, 'w', false);
			this.totalIncomeText = new SHSharp::SHGUItext(string.Format("{0} HC pS", IncomeController.Instance.TotalIncomePerSec() * SharedTheSystem.General.GetHotCoinsPerConverted()), SharedTheSystem.Views.TotalIncomePositionX, SharedTheSystem.Views.TotalIncomePositionY, 'w', false);
			this.hotCoinsSeparator = new SHSharp::SHGUIline(1, SHSharp::SHGUI.current.resolutionX - 2, SharedTheSystem.Views.TotalIncomePositionY + 1, true, 'w');
			this.AddSubView(this.hotCoinsText);
			this.AddSubView(this.totalIncomeText);
			this.AddSubView(this.hotCoinsSeparator);
		}

		// Token: 0x060026CA RID: 9930 RVA: 0x00115638 File Offset: 0x00113A38
		public override void Update()
		{
			base.Update();
			if (this.delayTimer >= this.startDelayTime && this.hcTextQueue.Count > 0)
			{
				HotCoinEarnedText hotCoinEarnedText = this.hcTextQueue.Dequeue();
				this.hcTexts.Add(hotCoinEarnedText);
				this.AddSubView(hotCoinEarnedText);
				this.delayTimer = 0f;
				this.startDelayTime = UnityEngine.Random.Range(0.2f, 0.4f);
			}
			for (int i = 0; i < this.hcTexts.Count; i++)
			{
				HotCoinEarnedText hotCoinEarnedText2 = this.hcTexts[i];
				if (hotCoinEarnedText2.IsDead())
				{
					this.RemoveView(hotCoinEarnedText2);
					this.hcTexts.Remove(hotCoinEarnedText2);
				}
			}
			this.delayTimer += Time.unscaledDeltaTime;
		}

		// Token: 0x060026CB RID: 9931 RVA: 0x00115708 File Offset: 0x00113B08
		public void UpdateHotCoinsText()
		{
			if (SharedTheSystem.General.GetHotCoins().Counter > 0)
			{
				this.hotCoinsText.text = string.Format("Hot Coins x {0:0}", SharedTheSystem.General.GetHotCoins());
			}
			else
			{
				this.hotCoinsText.text = string.Format("Hot Coins x {0:0.##}", SharedTheSystem.General.GetHotCoins().Fraction);
			}
		}

		// Token: 0x060026CC RID: 9932 RVA: 0x00115777 File Offset: 0x00113B77
		public void UpdateIncomePerSecondText()
		{
			this.totalIncomeText.text = string.Format("{0} HC pS", IncomeController.Instance.TotalIncomePerSec() * SharedTheSystem.General.GetHotCoinsPerConverted());
		}

		// Token: 0x060026CD RID: 9933 RVA: 0x001157A8 File Offset: 0x00113BA8
		public void AddFloatingHotCoin(Value earnedHotCoins)
		{
			HotCoinEarnedText item = new HotCoinEarnedText(earnedHotCoins, SharedTheSystem.Views.TotalIncomePositionX, SharedTheSystem.Views.TotalIncomePositionY, SharedTheSystem.Views.HotCoinsViewPositionX, SharedTheSystem.Views.HotCoinsViewPositionY, 'w');
			this.hcTextQueue.Enqueue(item);
		}

		// Token: 0x060026CE RID: 9934 RVA: 0x001157F4 File Offset: 0x00113BF4
		public void ShowFloatingSpendText(Value hotCoins)
		{
			int num = this.hotCoinsText.x + this.hotCoinsText.GetLongestLineLength() - hotCoins.ToString().Length - 1;
			HotCoinSpendText view = new HotCoinSpendText(hotCoins, num, base.PositionY, num, base.PositionY - 10, 'r');
			this.AddSubView(view);
		}

		// Token: 0x060026CF RID: 9935 RVA: 0x0011584C File Offset: 0x00113C4C
		public void AddAllHotCoins()
		{
			Value value = Value.Zero;
			for (int i = 0; i < this.hcTextQueue.Count; i++)
			{
				HotCoinEarnedText hotCoinEarnedText = this.hcTextQueue.Dequeue();
				value += hotCoinEarnedText.EarnedHotCoins;
				hotCoinEarnedText.Kill();
			}
			foreach (HotCoinEarnedText hotCoinEarnedText2 in this.hcTexts)
			{
				value += hotCoinEarnedText2.EarnedHotCoins;
				hotCoinEarnedText2.Kill();
			}
			AppTheSystem.Instance.ActionController.AddHotCoins(value);
		}

		// Token: 0x04002903 RID: 10499
		private SHSharp::SHGUItext hotCoinsText;

		// Token: 0x04002904 RID: 10500
		private const string OwnedHotCoinsText = "Hot Coins x ";

		// Token: 0x04002905 RID: 10501
		private SHSharp::SHGUItext totalIncomeText;

		// Token: 0x04002906 RID: 10502
		private SHSharp::SHGUIline hotCoinsSeparator;

		// Token: 0x04002907 RID: 10503
		private Queue<HotCoinEarnedText> hcTextQueue = new Queue<HotCoinEarnedText>();

		// Token: 0x04002908 RID: 10504
		private List<HotCoinEarnedText> hcTexts = new List<HotCoinEarnedText>();

		// Token: 0x04002909 RID: 10505
		private float startDelayTime = 0.2f;

		// Token: 0x0400290A RID: 10506
		private float delayTimer;
	}

	public class HypnosisController : SHSharp::SHGUIview
	{
		// Token: 0x06002662 RID: 9826 RVA: 0x00112B08 File Offset: 0x00110F08
		public HypnosisController()
		{
			this.newConvertedCounter = new SHSharp::SHGUItext("+ ", SHSharp::SHGUI.current.resolutionX / 2, SHSharp::SHGUI.current.resolutionY - 4, 'w', false);
			this.messageShowTime = UnityEngine.Random.Range(0.01f, 0.2f);
			this.maxTextTime = 0.3f * (float)this.messagesToShow;
		}

		// Token: 0x170001ED RID: 493
		// (get) Token: 0x06002663 RID: 9827 RVA: 0x00112B8C File Offset: 0x00110F8C
		// (set) Token: 0x06002664 RID: 9828 RVA: 0x00112B94 File Offset: 0x00110F94
		public bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
				if (this.active)
				{
					this.newConvertedNumber = SharedTheSystem.General.GetConverted() * SharedTheSystem.Upgrades.GetHypnosisPercent();
					this.currentCounter = Value.Zero;
					this.CreateTextCounter();
					this.textSingleAdd = Value.One;
					this.textTime = this.maxTextTime / this.newConvertedNumber.Fraction;
					while (this.textTime < 0.1f)
					{
						this.textSingleAdd = ++this.textSingleAdd;
						Value value2 = this.newConvertedNumber / this.textSingleAdd;
						this.textTime = this.maxTextTime / value2.Fraction;
					}
					if (this.textTime > 0.05f)
					{
						this.textTime = 0.05f;
					}
				}
			}
		}

		// Token: 0x06002665 RID: 9829 RVA: 0x00112C6C File Offset: 0x0011106C
		public override void Update()
		{
			if (!this.Active)
			{
				for (int i = 0; i < this.floatingTexts.Count; i++)
				{
					NewConvertedFloatingText newConvertedFloatingText = this.floatingTexts[i];
					if (newConvertedFloatingText.IsDead())
					{
						newConvertedFloatingText.Kill();
						base.RemoveView(newConvertedFloatingText);
						this.floatingTexts.Remove(newConvertedFloatingText);
					}
				}
				foreach (NewConvertedFloatingText newConvertedFloatingText2 in this.floatingTexts)
				{
					newConvertedFloatingText2.Update();
				}
				return;
			}
			base.Update();
			if (!this.ending)
			{
				AppTheSystem.Instance.ChangeGameState(AppTheSystem.GameState.Hypnosis);
				this.ProcessObeyPrompters();
				this.ProcessTextCounter();
			}
			else
			{
				this.ProcessTextCounter();
				for (int j = 0; j < this.obeyMessages.Count; j++)
				{
					ObeyMessage obeyMessage = this.obeyMessages[j];
					if (obeyMessage.LifeTimer >= obeyMessage.LifeTime && obeyMessage.finished)
					{
						obeyMessage.Kill();
						base.RemoveView(obeyMessage);
						this.obeyMessages.Remove(obeyMessage);
					}
				}
				if (this.obeyMessages.Count == 0)
				{
					this.ending = false;
					this.Active = false;
					base.RemoveView(this.test);
					this.test.Kill();
					NewConvertedFloatingText newConvertedFloatingText3 = new NewConvertedFloatingText(this.newConvertedNumber, this.newConvertedCounter.x, this.newConvertedCounter.y, SharedTheSystem.Views.ChatViewPositionX + SharedTheSystem.Views.ChatViewWidth - 7, SharedTheSystem.Views.ChatViewPositionY, 'w');
					base.AddSubView(newConvertedFloatingText3);
					newConvertedFloatingText3.SetPositionChangeTimes(0.1f, 0.1f);
					this.floatingTexts.Add(newConvertedFloatingText3);
					SharedTheSystem.Upgrades.IncreaseHypnosisCounter();
					AppTheSystem.Instance.ActionController.AddConvertedToQueue(this.newConvertedNumber);
					AppTheSystem.Instance.ChangeGameState(AppTheSystem.GameState.InMessageSystem);
				}
			}
			this.messageTimer += Time.unscaledDeltaTime;
		}

		// Token: 0x06002666 RID: 9830 RVA: 0x00112EA0 File Offset: 0x001112A0
		public override void Redraw(int offx, int offy)
		{
			base.Redraw(offx, offy);
			if (!this.Active)
			{
				return;
			}
			this.test.Redraw(offx, offy);
		}

		// Token: 0x06002667 RID: 9831 RVA: 0x00112EC4 File Offset: 0x001112C4
		private void ProcessObeyPrompters()
		{
			if (this.messageTimer >= this.messageShowTime)
			{
				ObeyMessage obeyMessage = new ObeyMessage
				{
					x = UnityEngine.Random.Range(2, SHSharp::SHGUI.current.resolutionX - 22),
					y = UnityEngine.Random.Range(1, SHSharp::SHGUI.current.resolutionY - 4)
				};
				this.obeyMessages.Add(obeyMessage);
				this.messageTimer = 0f;
				this.messageShowTime = UnityEngine.Random.Range(0.01f, 0.2f);
				base.AddSubView(obeyMessage);
			}
			if (this.obeyMessages.Count >= this.messagesToShow)
			{
				this.ending = true;
			}
		}

		// Token: 0x06002668 RID: 9832 RVA: 0x00112F6C File Offset: 0x0011136C
		private void ProcessTextCounter()
		{
			if (this.textCounterEnded)
			{
				return;
			}
			if (this.currentCounter <= this.newConvertedNumber)
			{
				if (this.textTimer >= this.textTime)
				{
					this.test.SetContent("+ " + this.currentCounter);
					this.test.ShowInstantPunchIn();
					this.test.GetPrompter().initDelay = 0f;
					this.test.GetPrompter().baseCharDelay = 0f;
					this.test.GetPrompter().text = "+ " + this.currentCounter;
					this.currentCounter += this.textSingleAdd;
					this.textTimer = 0f;
				}
			}
			else
			{
				this.textCounterEnded = true;
				this.test.SetContent("+ " + this.newConvertedNumber);
				this.test.ShowInstantPunchIn();
			}
			this.textTimer += Time.unscaledDeltaTime;
		}

		// Token: 0x06002669 RID: 9833 RVA: 0x00113084 File Offset: 0x00111484
		private void CreateTextCounter()
		{
			this.test = new SHSharp::SHGUIguruchatwindow(null)
			{
				x = SHSharp::SHGUI.current.resolutionX / 2,
				y = SHSharp::SHGUI.current.resolutionY - 4,
				showInstructions = false
			};
			base.AddSubView(this.test);
			this.textCounterEnded = false;
		}

		// Token: 0x04002872 RID: 10354
		private bool active;

		// Token: 0x04002873 RID: 10355
		private int messagesToShow = 20;

		// Token: 0x04002874 RID: 10356
		private float messageTimer;

		// Token: 0x04002875 RID: 10357
		private float messageShowTime;

		// Token: 0x04002876 RID: 10358
		private Value newConvertedNumber;

		// Token: 0x04002877 RID: 10359
		private bool ending;

		// Token: 0x04002878 RID: 10360
		private SHSharp::SHGUItext newConvertedCounter;

		// Token: 0x04002879 RID: 10361
		private SHSharp::SHGUIguruchatwindow test;

		// Token: 0x0400287A RID: 10362
		private Value currentCounter;

		// Token: 0x0400287B RID: 10363
		private float maxTextTime;

		// Token: 0x0400287C RID: 10364
		private float textTime;

		// Token: 0x0400287D RID: 10365
		private float textTimer;

		// Token: 0x0400287E RID: 10366
		private Value textSingleAdd;

		// Token: 0x0400287F RID: 10367
		private bool textCounterEnded;

		// Token: 0x04002880 RID: 10368
		private List<NewConvertedFloatingText> floatingTexts = new List<NewConvertedFloatingText>();

		// Token: 0x04002881 RID: 10369
		private List<ObeyMessage> obeyMessages = new List<ObeyMessage>();
	}

	public class HypnosisPercentUpgrade : AUpgrade
	{
		// Token: 0x0600264B RID: 9803 RVA: 0x0011251C File Offset: 0x0011091C
		public HypnosisPercentUpgrade(string text, int startX, int startY, char color) : base(text, startX, startY, color)
		{
			base.UpgradeType = SharedUpgrades.UpgradeType.Hypnosis;
			base.StartPrice = SharedTheSystem.Upgrades.HypnosisUpgradeStartPrice;
			Value price;
			Value tier;
			SharedTheSystem.Upgrades.GetHypnosisUpgrade(out price, out tier);
			base.Price = price;
			base.Tier = tier;
		}

		// Token: 0x0600264C RID: 9804 RVA: 0x00112568 File Offset: 0x00110968
		public override void ActivateUpgrade()
		{
			if (base.CanBeBought())
			{
				base.Buy();
				SharedTheSystem.Upgrades.IncreaseRateMultiplier(base.UpgradeType);
				base.UpdatePriceAndTier();
				SharedTheSystem.Upgrades.SaveHypnosisUpgrade(base.Price, base.Tier);
			}
		}

		// Token: 0x0600264D RID: 9805 RVA: 0x001125A7 File Offset: 0x001109A7
		public override bool CanBeUsed()
		{
			return !(base.Tier > new Value(3f)) && SharedTheSystem.Upgrades.GetHypnosisCounter() >= this.GetRequiredWorkers();
		}

		// Token: 0x0600264E RID: 9806 RVA: 0x001125DC File Offset: 0x001109DC
		public override Value GetRequiredWorkers()
		{
			if (base.Tier == Value.One)
			{
				return new Value(5f);
			}
			if (base.Tier == new Value(2f))
			{
				return new Value(15f);
			}
			if (base.Tier == new Value(3f))
			{
				return new Value(30f);
			}
			return Value.Zero;
		}
	}

	public class IncomeController : SHSharp::SHGUIview
	{
		// Token: 0x0600266A RID: 9834 RVA: 0x001130E0 File Offset: 0x001114E0
		public IncomeController() : base()
		{
			IncomeController.Instance = this;
			SharedTheSystem.Workers.GetWorkersIncomeMultiplier(out this.botIncomeMultiplier, out this.fanIncomeMultiplier, out this.streamerIncomeMultiplier, out this.flamerIncomeMultiplier, out this.drugDealerIncomeMultiplier, out this.wifiIncomeMultiplier, out this.indieDevIncomeMultiplier);
		}

		// Token: 0x170001EE RID: 494
		// (get) Token: 0x0600266B RID: 9835 RVA: 0x0011320D File Offset: 0x0011160D
		// (set) Token: 0x0600266C RID: 9836 RVA: 0x00113214 File Offset: 0x00111614
		public static IncomeController Instance { get; private set; }

		// Token: 0x0600266D RID: 9837 RVA: 0x0011321C File Offset: 0x0011161C
		public override void Update()
		{
			base.Update();
            this.botCount = SharedTheSystem.Workers.GetBots();
            this.fanCount = SharedTheSystem.Workers.GetFans();
            this.streamerCount = SharedTheSystem.Workers.GetStreamers();
            this.flamerCount = SharedTheSystem.Workers.GetFlamers();
            this.drugDealerCount = SharedTheSystem.Workers.GetDrugDealers();
            this.wifiCount = SharedTheSystem.Workers.GetWifis();
            this.indieDevCount = SharedTheSystem.Workers.GetIndieDevs();
            SharedTheSystem.Workers.GetWorkersIncomeMultiplier(out this.botIncomeMultiplier, out this.fanIncomeMultiplier, out this.streamerIncomeMultiplier, out this.flamerIncomeMultiplier, out this.drugDealerIncomeMultiplier, out this.wifiIncomeMultiplier, out this.indieDevIncomeMultiplier);
            AppTheSystem.Instance.ActionController.AddConvertedToQueue(this.TotalIncomePerSec() * new Value(0, Time.unscaledDeltaTime));
            AppTheSystem.Instance.HotCoinsView.UpdateIncomePerSecondText();
        }

		public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
        {
			//  MCD SHGUIview has an empty ReactToInputMouse, but SH does not.
        }

        // Token: 0x0600266E RID: 9838 RVA: 0x00113307 File Offset: 0x00111707
        public Value TotalBotIncomePerSec()
		{
			return this.OneBotIncomePerSec() * this.botCount;
		}

		// Token: 0x0600266F RID: 9839 RVA: 0x0011331A File Offset: 0x0011171A
		public Value OneBotIncomePerSec()
		{
			return this.botIncomeBaseRate * this.botIncomeMultiplier * SharedTheSystem.Upgrades.GetActualFrenzyPercent();
		}

		// Token: 0x06002670 RID: 9840 RVA: 0x0011333C File Offset: 0x0011173C
		public Value TotalFanIncomePerSec()
		{
			return this.OneFanIncomePerSec() * this.fanCount;
		}

		// Token: 0x06002671 RID: 9841 RVA: 0x0011334F File Offset: 0x0011174F
		public Value OneFanIncomePerSec()
		{
			return this.fanIncomeBaseRate * this.fanIncomeMultiplier * SharedTheSystem.Upgrades.GetActualFrenzyPercent();
		}

		// Token: 0x06002672 RID: 9842 RVA: 0x00113371 File Offset: 0x00111771
		public Value TotalStreamerIncomePerSec()
		{
			return this.OneStreamerIncomePerSec() * this.streamerCount;
		}

		// Token: 0x06002673 RID: 9843 RVA: 0x00113384 File Offset: 0x00111784
		public Value OneStreamerIncomePerSec()
		{
			return this.streamerIncomeBaseRate * this.streamerIncomeMultiplier * SharedTheSystem.Upgrades.GetActualFrenzyPercent();
		}

		// Token: 0x06002674 RID: 9844 RVA: 0x001133A6 File Offset: 0x001117A6
		public Value TotalFlamerIncomePerSec()
		{
			return this.OneFlamerIncomePerSec() * this.flamerCount;
		}

		// Token: 0x06002675 RID: 9845 RVA: 0x001133B9 File Offset: 0x001117B9
		public Value OneFlamerIncomePerSec()
		{
			return this.flamerIncomeBaseRate * this.flamerIncomeMultiplier * SharedTheSystem.Upgrades.GetActualFrenzyPercent();
		}

		// Token: 0x06002676 RID: 9846 RVA: 0x001133DC File Offset: 0x001117DC
		public Value TotalIncomePerSec()
		{
			Value v = Value.Zero;
			v += this.TotalBotIncomePerSec();
			v += this.TotalFanIncomePerSec();
			v += this.TotalStreamerIncomePerSec();
			v += this.TotalFlamerIncomePerSec();
			v += this.TotalDrugDealerIncomePerSec();
			v += this.TotalWifiIncomePerSec();
			return v + this.TotalIndieDevIncomePerSec();
		}

		// Token: 0x06002677 RID: 9847 RVA: 0x0011344B File Offset: 0x0011184B
		public Value TotalDrugDealerIncomePerSec()
		{
			return this.OneDrugDealerIncomePerSec() * this.drugDealerCount;
		}

		// Token: 0x06002678 RID: 9848 RVA: 0x0011345E File Offset: 0x0011185E
		public Value OneDrugDealerIncomePerSec()
		{
			return this.drugDealerIncomeBaseRate * this.drugDealerIncomeMultiplier * SharedTheSystem.Upgrades.GetActualFrenzyPercent();
		}

		// Token: 0x06002679 RID: 9849 RVA: 0x00113480 File Offset: 0x00111880
		public Value TotalWifiIncomePerSec()
		{
			return this.OneWifiIncomePerSec() * this.wifiCount;
		}

		// Token: 0x0600267A RID: 9850 RVA: 0x00113493 File Offset: 0x00111893
		public Value OneWifiIncomePerSec()
		{
			return this.wifiIncomeBaseRate * this.wifiIncomeMultiplier * SharedTheSystem.Upgrades.GetActualFrenzyPercent();
		}

		// Token: 0x0600267B RID: 9851 RVA: 0x001134B5 File Offset: 0x001118B5
		public Value TotalIndieDevIncomePerSec()
		{
			return this.indieDevIncomeBaseRate * this.indieDevCount * this.indieDevIncomeMultiplier;
		}

		// Token: 0x0600267C RID: 9852 RVA: 0x001134D3 File Offset: 0x001118D3
		public Value OneIndieDevIncomePerSec()
		{
			return this.indieDevIncomeBaseRate * this.indieDevIncomeMultiplier;
		}

		// Token: 0x04002883 RID: 10371
		private Value botIncomeBaseRate = SharedTheSystem.Workers.BotStartRate;

		// Token: 0x04002884 RID: 10372
		private Value fanIncomeBaseRate = SharedTheSystem.Workers.FanStartRate;

		// Token: 0x04002885 RID: 10373
		private Value streamerIncomeBaseRate = SharedTheSystem.Workers.StreamerStartRate;

		// Token: 0x04002886 RID: 10374
		private Value flamerIncomeBaseRate = SharedTheSystem.Workers.FlamerStartRate;

		// Token: 0x04002887 RID: 10375
		private Value botIncomeMultiplier;

		// Token: 0x04002888 RID: 10376
		private Value fanIncomeMultiplier;

		// Token: 0x04002889 RID: 10377
		private Value streamerIncomeMultiplier;

		// Token: 0x0400288A RID: 10378
		private Value flamerIncomeMultiplier;

		// Token: 0x0400288B RID: 10379
		private Value botCount = SharedTheSystem.Workers.GetBots();

		// Token: 0x0400288C RID: 10380
		private Value fanCount = SharedTheSystem.Workers.GetFans();

		// Token: 0x0400288D RID: 10381
		private Value streamerCount = SharedTheSystem.Workers.GetStreamers();

		// Token: 0x0400288E RID: 10382
		private Value flamerCount = SharedTheSystem.Workers.GetFlamers();

		// Token: 0x0400288F RID: 10383
		private Value drugDealerIncomeBaseRate = SharedTheSystem.Workers.DrugDealerStartRate;

		// Token: 0x04002890 RID: 10384
		private Value drugDealerIncomeMultiplier;

		// Token: 0x04002891 RID: 10385
		private Value drugDealerCount = SharedTheSystem.Workers.GetDrugDealers();

		// Token: 0x04002892 RID: 10386
		private Value wifiIncomeBaseRate = SharedTheSystem.Workers.WifiStartRate;

		// Token: 0x04002893 RID: 10387
		private Value wifiIncomeMultiplier;

		// Token: 0x04002894 RID: 10388
		private Value wifiCount = SharedTheSystem.Workers.GetWifis();

		// Token: 0x04002895 RID: 10389
		private Value indieDevIncomeBaseRate = SharedTheSystem.Workers.IndieDevStartRate;

		// Token: 0x04002896 RID: 10390
		private Value indieDevIncomeMultiplier;

		// Token: 0x04002897 RID: 10391
		private Value indieDevCount = SharedTheSystem.Workers.GetIndieDevs();
	}

	public class IndieDev : SHSharp::SHGUIsprite
	{
		// Token: 0x06002503 RID: 9475 RVA: 0x0010CE68 File Offset: 0x0010B268
		public IndieDev()
		{
			MCDCom.AddFrameFromStr(this, MCDCom.AssetToText(MCDCom.GetAssetName("IdleGame_IndieDev.txt")), 3);
			//base.AddFramesFromFile(StoryAppsResources.IdleGameIndieDev, 3, false);
			this.currentFrame = 0;
		}

		// Token: 0x06002504 RID: 9476 RVA: 0x0010CE98 File Offset: 0x0010B298
		public override void Update()
		{
			base.Update();
			if (this.animationTimer >= this.animationTime)
			{
				if (this.currentFrame == this.frames.Count - 1)
				{
					this.currentFrame = 0;
				}
				else
				{
					this.currentFrame++;
				}
				this.animationTimer = 0f;
			}
			else
			{
				this.animationTimer += Time.unscaledDeltaTime;
			}
		}

		// Token: 0x0400272A RID: 10026
		private float animationTimer;

		// Token: 0x0400272B RID: 10027
		private readonly float animationTime = SharedTheSystem.Views.IndieDevAnimationTime;
	}

	public class IndieDevRateUpgrade : AUpgrade
	{
		// Token: 0x0600264F RID: 9807 RVA: 0x00112658 File Offset: 0x00110A58
		public IndieDevRateUpgrade(string text, int startX, int startY, char color) : base(text, startX, startY, color)
		{
			base.UpgradeType = SharedUpgrades.UpgradeType.IndieDev;
			base.StartPrice = SharedTheSystem.Upgrades.IndieDevRateUpgradeStartPrice;
			Value price;
			Value tier;
			SharedTheSystem.Upgrades.GetIndieDevRateUpgrade(out price, out tier);
			base.Price = price;
			base.Tier = tier;
		}

		// Token: 0x06002650 RID: 9808 RVA: 0x001126A3 File Offset: 0x00110AA3
		public override void ActivateUpgrade()
		{
			if (base.CanBeBought())
			{
				base.Buy();
				SharedTheSystem.Upgrades.IncreaseRateMultiplier(SharedUpgrades.UpgradeType.IndieDev);
				base.UpdatePriceAndTier();
				SharedTheSystem.Upgrades.SaveIndieDevRateUpgrade(base.Price, base.Tier);
			}
		}

		// Token: 0x06002651 RID: 9809 RVA: 0x001126DD File Offset: 0x00110ADD
		public override bool CanBeUsed()
		{
			return SharedTheSystem.Workers.GetIndieDevs() >= this.GetRequiredWorkers();
		}

		// Token: 0x06002652 RID: 9810 RVA: 0x001126F4 File Offset: 0x00110AF4
		public override void UpdateHintText()
		{
			base.HintText = SharedTheSystem.Upgrades.IndieDevRateHint + SharedTheSystem.GetUpgradesHintStats(IncomeController.Instance.TotalIndieDevIncomePerSec());
			base.UpdateHintText();
		}
	}

	public class IdleGameChat : SHSharp::APPscrollconsole
	{
		// Token: 0x0600269E RID: 9886 RVA: 0x001151B7 File Offset: 0x001135B7
		public IdleGameChat(int positionX, int positionY, int width, int height)
		{
			this.width = width;
			this.height = height;
			this.x = positionX;
			this.y = positionY;
		}

		// Token: 0x0600269F RID: 9887 RVA: 0x001151E8 File Offset: 0x001135E8
		public override void Update()
		{
			base.Update();
			ClearInstantMessagesAbove(0);
			this.convertedView.UpdateConvertedText();
			if (this.playerMessage != null && this.messageWindow.GetPrompter().IsAlmostFinished())
			{
				this.playerMessageCallback();
			}
		}

		public void ClearInstantMessagesAbove(int y)
		{
			for (int i = 0; i < this.messages.Count; i++)
			{
				if (this.messages[i].y < y)
				{
					this.messages[i].KillInstant();
				}
			}
		}

		// Token: 0x060026A0 RID: 9888 RVA: 0x00115238 File Offset: 0x00113638
		public new void ShowChatFrames()
		{
			this.frame = base.AddSubView(new SHSharp::SHGUIframe(0, 0, this.x + this.width, this.y + this.height, 'w'));
			this.appname = base.AddSubView(new SHSharp::SHGUItext("idleCHAT", 3, 0, 'w', false));
			this.convertedView = new ConvertedPeopleView(this.x + this.width, 0);
			this.clock = base.AddSubView(this.convertedView);
			this.frameOffset = 1;
			this.lines = 1;
		}

		protected SHSharp::scrollmessage AddChatMessage(string sender, string message, bool leftright, bool interactive, bool poor, bool overrideLast = false, bool showInstant = false)
		{
			if (this.dontDisplaySender)
			{
				sender = string.Empty;
			}
			message = message.Replace("\r", string.Empty);
			SHSharp::SHGUIguruchatwindow shguiguruchatwindow = new SHSharp::SHGUIguruchatwindow(null);
			shguiguruchatwindow.SetAlign((!leftright) ? SHSharp::SHAlign.Right : SHSharp::SHAlign.Left);
			if (interactive)
			{
				shguiguruchatwindow.SetInteractive();
			}
			if (poor)
			{
				shguiguruchatwindow.poorMode = true;
			}
			shguiguruchatwindow.SetWidth(this.desiredFrameWidth);
			shguiguruchatwindow.SetContent(message);
			shguiguruchatwindow.SetLabel(sender);
			shguiguruchatwindow.x = this.chatMargin;
			if (!leftright)
			{
				shguiguruchatwindow.x = SHSharp::SHGUI.current.resolutionX - this.chatMargin - shguiguruchatwindow.width;
			}
			int heightOfCompleteTextWithFrameVERYSLOWandMOODY = shguiguruchatwindow.GetHeightOfCompleteTextWithFrameVERYSLOWandMOODY();
			SHSharp::scrollmessage scrollmessage = new SHSharp::scrollmessage(shguiguruchatwindow, heightOfCompleteTextWithFrameVERYSLOWandMOODY, 0f, false, 0f, string.Empty);
			scrollmessage.overrideLast = overrideLast;
			this.queue.Add(scrollmessage);
			if (overrideLast)
			{
				shguiguruchatwindow.showInstructions = false;
			}
			if (this.defaultConsoleCallback != null)
			{
				shguiguruchatwindow.SetCallback(this.defaultConsoleCallback);
			}
			if (showInstant)
			{
				shguiguruchatwindow.ShowInstantPunchIn();
			}
			if (showInstant && !interactive && leftright)
			{
				shguiguruchatwindow.showInstructions = false;
			}
			return scrollmessage;
		}


		// Token: 0x060026A1 RID: 9889 RVA: 0x001152D0 File Offset: 0x001136D0
		public void AddPlayerMessage(string sender, string message, Action callback)
		{
			this.playerMessageCallback = callback;
			this.playerMessage = AddChatMessage(sender, message, true, true, false, false, false);
			this.messageWindow = (this.playerMessage.view as SHSharp::SHGUIguruchatwindow);
			base.DisplayNextMessage();
		}

		// Token: 0x060026A2 RID: 9890 RVA: 0x00115314 File Offset: 0x00113714
		public void AddWorkerMessage(string sender, string message)
		{
			SHSharp::scrollmessage scrollmessage = AddChatMessage(sender, message, true, false, true, false, false);
			base.DisplayNextMessage();
			if (scrollmessage.view.y > this.playerMessage.view.y)
			{
				int y = this.playerMessage.view.y;
				this.playerMessage.view.y = scrollmessage.view.y;
				scrollmessage.view.y = y;
			}
			if (AppTheSystem.Instance.CurrentGameState == AppTheSystem.GameState.InShop)
			{
				scrollmessage.view.SetColorRecursive('1');
			}
		}

		// Token: 0x060026A3 RID: 9891 RVA: 0x001153AB File Offset: 0x001137AB
		public void FinishPlayerMessage()
		{
			this.messageWindow.ReactToInputKeyboard(SHSharp::SHGUIinput.enter);
		}

		// Token: 0x060026A4 RID: 9892 RVA: 0x001153BC File Offset: 0x001137BC
		public void SetMessagesColor(bool active)
		{
			foreach (SHSharp::SHGUIview SHGUIview in this.messages)
			{
				SHGUIview.SetColorRecursive((!active) ? '1' : 'w');
			}
			if (this.messageWindow != null)
			{
				if (!active)
				{
					this.messageWindow.GetPrompter().drawCarriage = false;
				}
				else
				{
					this.messageWindow.GetPrompter().drawCarriage = true;
					this.messageWindow.SetInteractive();
				}
			}
		}

		// Token: 0x060026A5 RID: 9893 RVA: 0x0011546C File Offset: 0x0011386C
		public void SetActive(bool active)
		{
			base.SetColorRecursive((!active) ? '1' : 'w');
			this.SetMessagesColor(active);
		}

		// Token: 0x040028E7 RID: 10471
		private new int width;

		// Token: 0x040028E8 RID: 10472
		private int height;

		// Token: 0x040028E9 RID: 10473
		private SHSharp::scrollmessage playerMessage;

		// Token: 0x040028EA RID: 10474
		private SHSharp::SHGUIguruchatwindow messageWindow;

		// Token: 0x040028EB RID: 10475
		private Action playerMessageCallback;

		// Token: 0x040028EC RID: 10476
		private SHSharp::scrollmessage workerMessage;

		// Token: 0x040028ED RID: 10477
		private SHSharp::SHGUIguruchatwindow workerMessageWindow;

		// Token: 0x040028EE RID: 10478
		private Queue<Action> workersMessageCallbacksQueue = new Queue<Action>();

		// Token: 0x040028EF RID: 10479
		private ConvertedPeopleView convertedView;

		private int chatMargin = 3;
	}

	public class MessageConvertedUpgrade : AUpgrade
	{
		// Token: 0x06002653 RID: 9811 RVA: 0x00112720 File Offset: 0x00110B20
		public MessageConvertedUpgrade(string text, int startX, int startY, char color) : base(text, startX, startY, color)
		{
			base.UpgradeType = SharedUpgrades.UpgradeType.ConvertedPerMessage;
			base.StartPrice = SharedTheSystem.Upgrades.MessageConvertedStartPrice;
			Value price;
			Value tier;
			SharedTheSystem.Upgrades.GetMessageConvertedUpgrade(out price, out tier);
			base.Price = price;
			base.Tier = tier;
		}

		// Token: 0x06002654 RID: 9812 RVA: 0x0011276B File Offset: 0x00110B6B
		public override void ActivateUpgrade()
		{
			if (base.CanBeBought())
			{
				base.Buy();
				SharedTheSystem.Upgrades.IncreaseRateMultiplier(base.UpgradeType);
				base.UpdatePriceAndTier();
				SharedTheSystem.Upgrades.SaveMessageConvertedUpgrade(base.Price, base.Tier);
			}
		}

		// Token: 0x06002655 RID: 9813 RVA: 0x001127AA File Offset: 0x00110BAA
		public override bool CanBeUsed()
		{
			return SharedTheSystem.Workers.GetFakeAccounts() >= this.GetRequiredWorkers();
		}
	}

	public class MessageCpsBonusUpgrade : AUpgrade
	{
		// Token: 0x06002656 RID: 9814 RVA: 0x001127C4 File Offset: 0x00110BC4
		public MessageCpsBonusUpgrade(string text, int startX, int startY, char color) : base(text, startX, startY, color)
		{
			base.UpgradeType = SharedUpgrades.UpgradeType.MessageCps;
			base.StartPrice = SharedTheSystem.Upgrades.MessageCpsBonusStartPrice;
			Value price;
			Value tier;
			SharedTheSystem.Upgrades.GetMessageCpsBonusUpgrade(out price, out tier);
			base.Price = price;
			base.Tier = tier;
		}

		// Token: 0x06002657 RID: 9815 RVA: 0x00112810 File Offset: 0x00110C10
		public override void ActivateUpgrade()
		{
			if (base.CanBeBought())
			{
				base.Buy();
				SharedTheSystem.Upgrades.IncreaseRateMultiplier(base.UpgradeType);
				base.HintText = SharedTheSystem.Upgrades.GetNewMessageCpsBonusHint();
				base.UpdatePriceAndTier();
				SharedTheSystem.Upgrades.SaveMessageCpsBonusUpgrade(base.Price, base.Tier);
			}
		}

		// Token: 0x06002658 RID: 9816 RVA: 0x0011286A File Offset: 0x00110C6A
		public override bool CanBeUsed()
		{
			return !(base.Tier > new Value(5f)) && SharedTheSystem.General.GetWrittenMessagesCount() >= this.GetRequiredWorkers();
		}

		// Token: 0x06002659 RID: 9817 RVA: 0x001128A0 File Offset: 0x00110CA0
		public override Value GetRequiredWorkers()
		{
			if (base.Tier == Value.One)
			{
				return new Value(100f);
			}
			if (base.Tier == new Value(2f))
			{
				return new Value(500f);
			}
			if (base.Tier == new Value(3f))
			{
				return new Value(1500f);
			}
			if (base.Tier == new Value(4f))
			{
				return new Value(5000f);
			}
			if (base.Tier == new Value(5f))
			{
				return new Value(10000f);
			}
			return Value.Zero;
		}
	}

	public class NewConvertedFloatingText : HotCoinEarnedText
	{
		// Token: 0x060024EC RID: 9452 RVA: 0x0010C489 File Offset: 0x0010A889
		public NewConvertedFloatingText(Value newConverted, int x, int y, int targetX, int targetY, char color) : base(newConverted, x, y, targetX, targetY, color)
		{
			this.text = "+ " + newConverted + " @";
			this.YChangeTime = 0.3f;
		}

		// Token: 0x060024ED RID: 9453 RVA: 0x0010C4BB File Offset: 0x0010A8BB
		public void SetPositionChangeTimes(float xChangeTime, float yChangeTime)
		{
			this.XChangeTime = xChangeTime;
			this.YChangeTime = yChangeTime;
		}

		// Token: 0x060024EE RID: 9454 RVA: 0x0010C4CB File Offset: 0x0010A8CB
		protected override void AddCoins()
		{
		}
	}

	[XmlRoot("root")]
	public class NumbersEndings
	{
		// Token: 0x060025B2 RID: 9650 RVA: 0x0010F4DC File Offset: 0x0010D8DC
		public static void ReadEndings()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(NumbersEndings));
			if (!File.Exists(NumbersEndings.FilePath))
			{
				NumbersEndings.SaveEndings();
			}
			using (FileStream fileStream = new FileStream(NumbersEndings.FilePath, FileMode.OpenOrCreate))
			{
				NumbersEndings.Instance = (NumbersEndings)xmlSerializer.Deserialize(fileStream);
			}
		}

		// Token: 0x060025B3 RID: 9651 RVA: 0x0010F54C File Offset: 0x0010D94C
		public static void AddEnding()
		{
			string text = NumbersEndings.Instance.Endings[NumbersEndings.Instance.Endings.Count - 1];
			string text2 = string.Empty;
			if (text[text.Length - 1] < 'z')
			{
				char c = text[text.Length - 1];
				c += '\u0001';
				text2 = text2 + text[0].ToString() + c.ToString();
			}
			else
			{
				char c2 = text[0];
				text2 = text2 + (c2 + '\u0001').ToString() + "a";
			}
			NumbersEndings.Instance.Endings.Add(text2);
			NumbersEndings.SaveEndings();
		}

		// Token: 0x060025B4 RID: 9652 RVA: 0x0010F618 File Offset: 0x0010DA18
		public static void SaveEndings()
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(NumbersEndings));
			if (!File.Exists(NumbersEndings.FilePath))
			{
				FileInfo fileInfo = new FileInfo(NumbersEndings.FilePath);
				fileInfo.Directory.Create();
				NumbersEndings.Instance = new NumbersEndings
				{
					Endings = new List<string>
					{
						"k",
						"M",
						"G",
						"T",
						"P",
						"E",
						"Z",
						"Y",
						"Aa"
					}
				};
			}
			using (TextWriter textWriter = new StreamWriter(NumbersEndings.FilePath))
			{
				xmlSerializer.Serialize(textWriter, NumbersEndings.Instance);
			}
		}

		// Token: 0x040027D3 RID: 10195
		public static NumbersEndings Instance;

		// Token: 0x040027D4 RID: 10196
		[XmlArray("endings")]
		[XmlArrayItem("item")]
		public List<string> Endings;

		// Token: 0x040027D5 RID: 10197
		private static readonly string FilePath = Application.dataPath + "/Resources/StoryApps/Games/IdleMinigame/HotCoinEndings.hot";
	}

	public class ObeyMessage : SHSharp::SHGUIguruchatwindow
	{
		// Token: 0x060026A6 RID: 9894 RVA: 0x0011548C File Offset: 0x0011388C
		public ObeyMessage() : base(null)
		{
			base.SetContent("OBEY. THE. SYSTEM.");
			base.SetColorRecursive('r');
			this.showInstructions = false;
			this.dontDrawViewsBelow = true;
			this.LifeTime = UnityEngine.Random.Range(1f, 1.5f);
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x060026A7 RID: 9895 RVA: 0x001154D8 File Offset: 0x001138D8
		// (set) Token: 0x060026A8 RID: 9896 RVA: 0x001154E0 File Offset: 0x001138E0
		public bool Ending { get; set; }

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x060026A9 RID: 9897 RVA: 0x001154E9 File Offset: 0x001138E9
		// (set) Token: 0x060026AA RID: 9898 RVA: 0x001154F1 File Offset: 0x001138F1
		public float LifeTime { get; set; }

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x060026AB RID: 9899 RVA: 0x001154FA File Offset: 0x001138FA
		// (set) Token: 0x060026AC RID: 9900 RVA: 0x00115502 File Offset: 0x00113902
		public float LifeTimer { get; set; }

		// Token: 0x060026AD RID: 9901 RVA: 0x0011550B File Offset: 0x0011390B
		public override void Update()
		{
			base.Update();
			if (this.finished)
			{
				this.LifeTimer += Time.unscaledDeltaTime;
			}
		}
	}

	public class PopupBonusController : SHSharp::SHGUIview
	{
		// Token: 0x0600267D RID: 9853 RVA: 0x001134E8 File Offset: 0x001118E8
		public PopupBonusController()
		{
			this.startX = (int)((double)(SHSharp::SHGUI.current.resolutionX / 2) - 0.5 * (double)this.length);
			this.startY = (int)((double)(SHSharp::SHGUI.current.resolutionY / 2) - 0.5 * (double)this.height);
			this.gainFull = string.Format(this.gain1 + "{0}%" + this.gain2, SharedTheSystem.Upgrades.GetHypnosisPercent().Fraction * 100f);
			this.popupFrame = new SHSharp::SHGUIframe(this.startX, this.startY, this.startX + this.length, this.startY + this.height, 'w');
			this.rect = new SHSharp::SHGUIrect(this.startX, this.startY, this.startX + this.length, this.startY + this.height, '0', ' ', 2)
			{
				dontDrawViewsBelow = true
			};
			this.titleText = new SHSharp::SHGUItext(this.title, (int)((double)(this.startX + this.length / 2) - 0.5 * (double)this.title.Length), this.startY + 1, 'w', false);
			this.descriptionText = new SHSharp::SHGUItext(this.description, this.startX + this.description.Length / 4, this.startY + 3, 'w', false);
			this.descriptionText.BreakTextForLineLength(this.length - 2);
			this.gainText = new SHSharp::SHGUItext(this.gainFull, (int)((double)(this.startX + this.length / 2) - 0.5 * (double)this.gainFull.Length), this.startY + 7, 'w', false);
			this.confirmButton = new SHSharp::SHGUIButton(this.accept, this.startX + this.length / 2 - this.accept.Length / 2, this.startY + this.height - 2, 'r');
			this.timeLeft = (int)this.durationTime + 1;
			this.clock = new SHSharp::SHGUItext(this.clockPhrase + this.timeLeft, this.startX + this.length / 2 - this.clockPhrase.Length / 2, this.confirmButton.y - 1, 'r', false);
			this.popupFrame.AddSubView(this.titleText);
			this.popupFrame.AddSubView(this.descriptionText);
			this.popupFrame.AddSubView(this.gainText);
			this.popupFrame.AddSubView(this.confirmButton);
			this.popupFrame.AddSubView(this.clock);
			this.ChooseNextBonus();
			this.PrepareNextBonus();
		}

		// Token: 0x0600267E RID: 9854 RVA: 0x0011385C File Offset: 0x00111C5C
		public override void Update()
		{
			base.Update();
			this.showTime = SharedTheSystem.Upgrades.GetPopupShowTime();
			PopupBonusController.BonusType bonusType = this.nextBonusType;
			if (bonusType != PopupBonusController.BonusType.Hypnosis)
			{
				if (bonusType == PopupBonusController.BonusType.Frenzy)
				{
					this.gainFull = string.Format(this.gain1 + "{0}%" + this.gain2, SharedTheSystem.Upgrades.GetFrenzyPercent().Fraction * 100f);
					this.gainText.text = this.gainFull;
				}
			}
			else
			{
				this.gainFull = string.Format(this.gain1 + "{0}%" + this.gain2, SharedTheSystem.Upgrades.GetHypnosisPercent().Fraction * 100f);
				this.gainText.text = this.gainFull;
			}
			if (this.inFrenzyMode)
			{
				if (this.frenzyTimer >= SharedTheSystem.Upgrades.GetFrenzyTime())
				{
					this.inFrenzyMode = false;
					AppTheSystem.Instance.InFrenzy = false;
					this.frenzyTimer = 0f;
				}
				this.frenzyTimer += Time.unscaledDeltaTime;
				return;
			}
			if (AppTheSystem.Instance.CurrentGameState == AppTheSystem.GameState.Hypnosis)
			{
				return;
			}
			if (AppTheSystem.Instance.CurrentGameState == AppTheSystem.GameState.Brainwash)
			{
				this.brainwashDone = true;
				return;
			}
			if (this.brainwashDone)
			{
				this.showTimer = 0f;
				this.brainwashDone = false;
			}
			if (this.showTimer >= this.showTime)
			{
				base.AddSubView(this.rect);
				base.AddSubView(this.popupFrame);
				AppTheSystem.Instance.ChangeGameState(AppTheSystem.GameState.BonusPopUp);
				SharedTheSystem.Upgrades.IncreasePopupCount();
				this.popupInbound = true;
				this.showTimer = 0f;
			}
			if (this.popupInbound)
			{
				if (this.durationTimer >= this.durationTime)
				{
					base.RemoveView(this.popupFrame);
					base.RemoveView(this.rect);
					AppTheSystem.Instance.ChangeGameState(AppTheSystem.GameState.InMessageSystem);
					this.popupInbound = false;
					this.durationTimer = 0f;
					this.timeLeft = (int)this.durationTime + 1;
				}
				this.durationTimer += Time.unscaledDeltaTime;
				this.timeLeft = (int)(this.durationTime - this.durationTimer) + 1;
				this.clock.text = this.clockPhrase + this.timeLeft;
			}
			else
			{
				this.showTimer += Time.unscaledDeltaTime;
			}
		}

		// Token: 0x0600267F RID: 9855 RVA: 0x00113AE8 File Offset: 0x00111EE8
		public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
		{
			base.ReactToInputKeyboard(key);
			if (key == SHSharp::SHGUIinput.enter)
			{
				PopupBonusController.BonusType bonusType = this.nextBonusType;
				if (bonusType != PopupBonusController.BonusType.Hypnosis)
				{
					if (bonusType == PopupBonusController.BonusType.Frenzy)
					{
						this.inFrenzyMode = true;
						AppTheSystem.Instance.InFrenzy = true;
						AppTheSystem.Instance.ChangeGameState(AppTheSystem.GameState.InMessageSystem);
					}
				}
				else
				{
					AppTheSystem.Instance.ActionController.ActivateHypnosis();
				}
				this.durationTimer = 0f;
				this.showTimer = 0f;
				this.popupInbound = false;
				this.timeLeft = (int)this.durationTime;
				base.RemoveView(this.popupFrame);
				base.RemoveView(this.rect);
				this.ChooseNextBonus();
				this.PrepareNextBonus();
			}
		}

		// Token: 0x06002680 RID: 9856 RVA: 0x00113BA8 File Offset: 0x00111FA8
		private void ChooseNextBonus()
		{
			int num = SharedTheSystem.RandomWithWeight(this.popupWeights.ToArray());
			if (num != 0)
			{
				if (num == 1)
				{
					this.nextBonusType = PopupBonusController.BonusType.Frenzy;
				}
			}
			else
			{
				this.nextBonusType = PopupBonusController.BonusType.Hypnosis;
			}
			SharedTheSystem.CalculateWeights(this.popupWeights, num);
		}

		// Token: 0x06002681 RID: 9857 RVA: 0x00113C04 File Offset: 0x00112004
		private void PrepareNextBonus()
		{
			PopupBonusController.BonusType bonusType = this.nextBonusType;
			if (bonusType != PopupBonusController.BonusType.Hypnosis)
			{
				if (bonusType == PopupBonusController.BonusType.Frenzy)
				{
					this.title = "SUPERHOT FRENZY";
					this.description = "Bodies are disposable. Your workers \n enter the frenzy mode!";
					this.gain2 = " converted per second";
				}
			}
			else
			{
				this.title = "SUPERHOT HYPNOSIS";
				this.description = "Mind is software. Hypnotize them \n with digital hypnosis!";
				this.gain2 = " of converted people";
			}
			this.titleText.text = this.title;
			this.titleText.x = (int)((double)(this.startX + this.length / 2) - 0.5 * (double)this.title.Length);
			this.descriptionText.text = this.description;
			string text = Regex.Split(this.description, "\n").FirstOrDefault<string>();
			if (text != null)
			{
				this.descriptionText.x = (int)((double)(this.startX + this.length / 2) - 0.5 * (double)text.Length);
			}
			this.descriptionText.BreakTextForLineLength(this.length - 2);
		}

		// Token: 0x04002898 RID: 10392
		private SHSharp::SHGUIframe popupFrame;

		// Token: 0x04002899 RID: 10393
		private SHSharp::SHGUIrect rect;

		// Token: 0x0400289A RID: 10394
		private SHSharp::SHGUItext titleText;

		// Token: 0x0400289B RID: 10395
		private SHSharp::SHGUItext descriptionText;

		// Token: 0x0400289C RID: 10396
		private SHSharp::SHGUItext gainText;

		// Token: 0x0400289D RID: 10397
		private SHSharp::SHGUIButton confirmButton;

		// Token: 0x0400289E RID: 10398
		private SHSharp::SHGUItext clock;

		// Token: 0x0400289F RID: 10399
		private string title = "SUPERHOT HYPNOSIS";

		// Token: 0x040028A0 RID: 10400
		private string description = "Hypnotize massive number of people \n with digital hypnosis!";

		// Token: 0x040028A1 RID: 10401
		private string gain1 = "Gain: ";

		// Token: 0x040028A2 RID: 10402
		private string gain2 = " of converted people";

		// Token: 0x040028A3 RID: 10403
		private string gainFull;

		// Token: 0x040028A4 RID: 10404
		private string accept = "Enter to Accept";

		// Token: 0x040028A5 RID: 10405
		private string clockPhrase = "00:0";

		// Token: 0x040028A6 RID: 10406
		private int length = 40;

		// Token: 0x040028A7 RID: 10407
		private int height = 12;

		// Token: 0x040028A8 RID: 10408
		private int startX;

		// Token: 0x040028A9 RID: 10409
		private int startY;

		// Token: 0x040028AA RID: 10410
		private float showTimer;

		// Token: 0x040028AB RID: 10411
		private float showTime = SharedTheSystem.Upgrades.GetPopupShowTime();

		// Token: 0x040028AC RID: 10412
		private float frenzyTimer;

		// Token: 0x040028AD RID: 10413
		private float durationTime = 5f;

		// Token: 0x040028AE RID: 10414
		private float durationTimer;

		// Token: 0x040028AF RID: 10415
		private int timeLeft;

		// Token: 0x040028B0 RID: 10416
		private bool popupInbound;

		// Token: 0x040028B1 RID: 10417
		private bool inFrenzyMode;

		// Token: 0x040028B2 RID: 10418
		private bool brainwashDone;

		// Token: 0x040028B3 RID: 10419
		private PopupBonusController.BonusType nextBonusType;

		// Token: 0x040028B4 RID: 10420
		private List<float> popupWeights = new List<float>
		{
			1f,
			1f
		};

		// Token: 0x02000686 RID: 1670
		public enum BonusType
		{
			// Token: 0x040028B6 RID: 10422
			Hypnosis,
			// Token: 0x040028B7 RID: 10423
			Frenzy
		}
	}

	public class ScrollView<T> : SHSharp::SHGUIview where T : ShopButton
	{
		// Token: 0x060026D0 RID: 9936 RVA: 0x00115908 File Offset: 0x00113D08
		public ScrollView(int positionX, int positionY)
		{
			this.PositionX = positionX;
			this.PositionY = positionY;
			this.TopIndex = 0;
			this.CurrentPos = 0;
			this.MaxElements = SharedTheSystem.Views.ScrollViewMaxElements;
			this.scrollbar = new SHSharp::SHGUIview();
			this.size = SharedTheSystem.Views.ShopButtonHeight * this.MaxElements;
			this.sliderArrowUpView = new SHSharp::SHGUItext("▲", positionX - 1, positionY, 'w', false);
			this.sliderArrowDownView = new SHSharp::SHGUItext("▼", positionX - 1, positionY + this.size - 1, 'w', false);
			this.sliderView = new SHSharp::SHGUItext(string.Empty, positionX - 1, positionY + 1, 'w', false);
			this.scrollbar.AddSubView(this.sliderArrowUpView);
			this.scrollbar.AddSubView(this.sliderView);
			this.scrollbar.AddSubView(this.sliderArrowDownView);
			base.AddSubView(this.scrollbar);
		}

		// Token: 0x17000200 RID: 512
		// (get) Token: 0x060026D1 RID: 9937 RVA: 0x001159FC File Offset: 0x00113DFC
		// (set) Token: 0x060026D2 RID: 9938 RVA: 0x00115A04 File Offset: 0x00113E04
		public List<T> Items { get; set; }

		// Token: 0x17000201 RID: 513
		// (get) Token: 0x060026D3 RID: 9939 RVA: 0x00115A0D File Offset: 0x00113E0D
		// (set) Token: 0x060026D4 RID: 9940 RVA: 0x00115A15 File Offset: 0x00113E15
		public int CurrentPos { get; set; }

		// Token: 0x17000202 RID: 514
		// (get) Token: 0x060026D5 RID: 9941 RVA: 0x00115A1E File Offset: 0x00113E1E
		// (set) Token: 0x060026D6 RID: 9942 RVA: 0x00115A26 File Offset: 0x00113E26
		public int TopIndex { get; private set; }

		// Token: 0x060026D7 RID: 9943 RVA: 0x00115A30 File Offset: 0x00113E30
		public override void Redraw(int offx, int offy)
		{
			base.Redraw(offx, offy);
			if (this.CurrentPos - this.TopIndex >= this.MaxElements)
			{
				for (int i = 0; i < this.Items.Count; i++)
				{
					T t = this.Items[i];
					t.PositionY -= SharedTheSystem.Views.ShopButtonHeight;
				}
				this.TopIndex++;
			}
			if (this.CurrentPos < this.TopIndex)
			{
				for (int j = 0; j < this.Items.Count; j++)
				{
					T t2 = this.Items[j];
					t2.PositionY += SharedTheSystem.Views.ShopButtonHeight;
				}
				this.TopIndex--;
			}
			if (this.CurrentPos == this.TopIndex && this.Items.Count > 0)
			{
				AppTheSystem.Instance.ShopView.HintView.Hidden = false;
				AStoryAppsView hintView = AppTheSystem.Instance.ShopView.HintView;
				T t3 = this.Items[this.CurrentPos];
				hintView.PositionY = t3.PositionY - 2;
			}
			else if (this.Items.Count > 0)
			{
				AppTheSystem.Instance.ShopView.HintView.Hidden = false;
				AStoryAppsView hintView2 = AppTheSystem.Instance.ShopView.HintView;
				T t4 = this.Items[this.CurrentPos];
				hintView2.PositionY = t4.PositionY - 3;
			}
			else
			{
				AppTheSystem.Instance.ShopView.HintView.Hidden = true;
			}
			for (int k = 0; k < this.Items.Count; k++)
			{
				if (k >= this.TopIndex && k < this.TopIndex + this.MaxElements)
				{
					this.Items[k].hidden = false;
				}
				else
				{
					this.Items[k].hidden = true;
				}
			}
			if (this.MaxElements < this.Items.Count)
			{
				if (this.scrollbar.hidden)
				{
					this.scrollbar.hidden = false;
				}
				string text = string.Empty;
				int num = 1;
				while ((float)num < Mathf.Clamp01((float)this.size / (float)(SharedTheSystem.Views.ShopButtonHeight * this.Items.Count)) * (float)(this.size - 1))
				{
					text += "█\n";
					num++;
				}
				this.sliderView.text = text;
				this.sliderView.y = this.PositionY + 1 + Mathf.CeilToInt((float)this.TopIndex * (float)this.size / ((float)text.Length / 2f));
			}
			else if (!this.scrollbar.hidden)
			{
				this.scrollbar.hidden = true;
			}
		}

		// Token: 0x060026D8 RID: 9944 RVA: 0x00115D74 File Offset: 0x00114174
		public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
		{
			if (key == SHSharp::SHGUIinput.down)
			{
				if (this.Items.Count == 0)
				{
					return;
				}
				if (this.CurrentPos >= this.Items.Count - 1)
				{
					return;
				}
				this.CurrentPos++;
			}
			if (key == SHSharp::SHGUIinput.up)
			{
				if (this.Items.Count == 0)
				{
					return;
				}
				if (this.CurrentPos <= 0)
				{
					return;
				}
				this.CurrentPos--;
			}
		}

		// Token: 0x0400290B RID: 10507
		protected int MaxElements;

		// Token: 0x0400290C RID: 10508
		protected int PositionX;

		// Token: 0x0400290D RID: 10509
		protected int PositionY;

		// Token: 0x04002911 RID: 10513
		private SHSharp::SHGUIview scrollbar;

		// Token: 0x04002912 RID: 10514
		private SHSharp::SHGUItext sliderView;

		// Token: 0x04002913 RID: 10515
		private SHSharp::SHGUItext sliderArrowUpView;

		// Token: 0x04002914 RID: 10516
		private SHSharp::SHGUItext sliderArrowDownView;

		// Token: 0x04002915 RID: 10517
		private int size;
	}

	public class SharedGeneral : DataSerializer
	{
		// Token: 0x060025B7 RID: 9655 RVA: 0x0010F754 File Offset: 0x0010DB54
		public Value GetConverted()
		{
			Value value;
			if (DataSerializer.Instance.Converted != null)
			{
				value = DataSerializer.Instance.Converted;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.Converted = value;
			}
			return value;
		}

		// Token: 0x060025B8 RID: 9656 RVA: 0x0010F798 File Offset: 0x0010DB98
		public Value GetHotCoins()
		{
			Value value;
			if (DataSerializer.Instance.HotCoins != null)
			{
				value = DataSerializer.Instance.HotCoins;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.HotCoins = value;
			}
			return value;
		}

		// Token: 0x060025B9 RID: 9657 RVA: 0x0010F7DC File Offset: 0x0010DBDC
		public Value GetTotalHotCoinsEarned()
		{
			Value value;
			if (DataSerializer.Instance.TotalHotCoinsEarned != null)
			{
				value = DataSerializer.Instance.TotalHotCoinsEarned;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.TotalHotCoinsEarned = value;
			}
			return value;
		}

		// Token: 0x060025BA RID: 9658 RVA: 0x0010F820 File Offset: 0x0010DC20
		public Value GetHotCoinsPerConverted()
		{
			Value value;
			if (DataSerializer.Instance.HotCoinsPerConverted != null)
			{
				value = DataSerializer.Instance.HotCoinsPerConverted;
			}
			else
			{
				value = this.StartHotCoinsPerConverted;
				DataSerializer.Instance.HotCoinsPerConverted = value;
			}
			return value;
		}

		// Token: 0x060025BB RID: 9659 RVA: 0x0010F868 File Offset: 0x0010DC68
		public Value GetConvertedPerMessage()
		{
			Value value;
			if (DataSerializer.Instance.ConvertedPerMessage != null)
			{
				value = DataSerializer.Instance.ConvertedPerMessage;
			}
			else
			{
				value = this.StartConvertedPerMessage;
				DataSerializer.Instance.ConvertedPerMessage = value;
			}
			return value;
		}

		// Token: 0x060025BC RID: 9660 RVA: 0x0010F8B0 File Offset: 0x0010DCB0
		public Value GetWrittenMessagesCount()
		{
			Value value;
			if (DataSerializer.Instance.WrittenMessagesCount != null)
			{
				value = DataSerializer.Instance.WrittenMessagesCount;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.WrittenMessagesCount = value;
			}
			return value;
		}

		// Token: 0x060025BD RID: 9661 RVA: 0x0010F8F4 File Offset: 0x0010DCF4
		public void AddConverted(Value value)
		{
			if (DataSerializer.Instance.Converted == null)
			{
				return;
			}
			DataSerializer.Instance.Converted += value;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025BE RID: 9662 RVA: 0x0010F92C File Offset: 0x0010DD2C
		public void IncreaseWrittenMessagesCount()
		{
			if (DataSerializer.Instance.WrittenMessagesCount == null)
			{
				DataSerializer.Instance.WrittenMessagesCount = Value.Zero;
			}
			DataSerializer.Instance.WrittenMessagesCount += Value.One;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025BF RID: 9663 RVA: 0x0010F984 File Offset: 0x0010DD84
		public void AddHotCoins(Value value)
		{
			DataSerializer.Instance.HotCoins += value;
			if (DataSerializer.Instance.TotalHotCoinsEarned == null)
			{
				DataSerializer.Instance.TotalHotCoinsEarned = Value.Zero;
			}
			if (value.Counter >= 0 && value.Fraction >= 0f)
			{
				DataSerializer.Instance.TotalHotCoinsEarned += value;
			}
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025C0 RID: 9664 RVA: 0x0010FA07 File Offset: 0x0010DE07
		public void IncreaseHotCoinsIncome()
		{
			DataSerializer.Instance.HotCoinsPerConverted += SharedTheSystem.Upgrades.HotCoinIncomeBonusPercent * DataSerializer.Instance.HotCoinsPerConverted;
		}

		// Token: 0x040027D6 RID: 10198
		public Value StartHotCoinsPerConverted = new Value(1f);

		// Token: 0x040027D7 RID: 10199
		public Value StartConvertedPerMessage = new Value(1f);
	}

	public class SharedTheSystem : DataSerializer
	{
		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x060025C2 RID: 9666 RVA: 0x0010FA3F File Offset: 0x0010DE3F
		public static SharedGeneral General
		{
			get
			{
				if (SharedTheSystem.general != null)
				{
					return SharedTheSystem.general;
				}
				SharedTheSystem.general = new SharedGeneral();
				return SharedTheSystem.general;
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x060025C3 RID: 9667 RVA: 0x0010FA60 File Offset: 0x0010DE60
		public static SharedViews Views
		{
			get
			{
				if (SharedTheSystem.views != null)
				{
					return SharedTheSystem.views;
				}
				SharedTheSystem.views = new SharedViews();
				return SharedTheSystem.views;
			}
		}

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x060025C4 RID: 9668 RVA: 0x0010FA81 File Offset: 0x0010DE81
		public static SharedUpgrades Upgrades
		{
			get
			{
				if (SharedTheSystem.upgrades != null)
				{
					return SharedTheSystem.upgrades;
				}
				SharedTheSystem.upgrades = new SharedUpgrades();
				return SharedTheSystem.upgrades;
			}
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x060025C5 RID: 9669 RVA: 0x0010FAA2 File Offset: 0x0010DEA2
		public static SharedWorkers Workers
		{
			get
			{
				if (SharedTheSystem.workers != null)
				{
					return SharedTheSystem.workers;
				}
				SharedTheSystem.workers = new SharedWorkers();
                return SharedTheSystem.workers;
			}
		}

		// Token: 0x060025C6 RID: 9670 RVA: 0x0010FAC4 File Offset: 0x0010DEC4
		public static string GetHintStats(Value incomePerOne, Value totalIncomePerSec)
		{
			string text = "\nIncome ";
			if (incomePerOne.Counter > 0 && totalIncomePerSec.Counter > 0)
			{
				text += string.Format("One: {0:0}, All: {1:0}", incomePerOne, totalIncomePerSec);
			}
			else if (totalIncomePerSec.Counter > 0)
			{
				text += string.Format("One: {0:0.##}, All: {1:0}", incomePerOne.Fraction, totalIncomePerSec);
			}
			else
			{
				text += string.Format("One: {0:0.##}, All: {1:0.##}", incomePerOne.Fraction, totalIncomePerSec.Fraction);
			}
			return text;
		}

		// Token: 0x060025C7 RID: 9671 RVA: 0x0010FB60 File Offset: 0x0010DF60
		public static string GetUpgradesHintStats(Value totalIncomePerSec)
		{
			string str = string.Format("\nCurrent income: {0:0}", totalIncomePerSec);
			return str + string.Format("\nAfter upgrade: {0:0}", totalIncomePerSec * SharedTheSystem.Upgrades.BaseIncomeMultiplier);
		}

		// Token: 0x060025C8 RID: 9672 RVA: 0x0010FB9B File Offset: 0x0010DF9B
		public static void ResetProgress()
		{
			DataSerializer.Instance.ResetProgress();
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025C9 RID: 9673 RVA: 0x0010FBB4 File Offset: 0x0010DFB4
		public static int RandomWithWeight(float[] probs)
		{
			float num = 0f;
			foreach (float num2 in probs)
			{
				num += num2;
			}
			float num3 = UnityEngine.Random.value * num;
			for (int j = 0; j < probs.Length; j++)
			{
				if (num3 < probs[j])
				{
					return j;
				}
				num3 -= probs[j];
			}
			return probs.Length - 1;
		}

		// Token: 0x060025CA RID: 9674 RVA: 0x0010FC24 File Offset: 0x0010E024
		public static void CalculateWeights(List<float> weights, int randomNumber)
		{
			if (weights.Count > 1)
			{
				for (int i = 0; i < weights.Count; i++)
				{
					if (i == randomNumber)
					{
						weights[i] = 0f;
					}
					else if (weights[i] < 1f)
					{
						weights[i] = Mathf.Clamp01(weights[i] + 1f / ((float)weights.Count - 1f));
					}
				}
			}
		}

		// Token: 0x040027D8 RID: 10200
		private static SharedGeneral general;

		// Token: 0x040027D9 RID: 10201
		private static SharedViews views;

		// Token: 0x040027DA RID: 10202
		private static SharedUpgrades upgrades;

		// Token: 0x040027DB RID: 10203
		private static SharedWorkers workers;

		// Token: 0x040027DC RID: 10204
		public static float BotMessageSendTime = 5f;

		// Token: 0x040027DD RID: 10205
		public static float StreamerActionMinTime = 2f;

		// Token: 0x040027DE RID: 10206
		public static float StreamerActionMaxTime = 6f;

		// Token: 0x040027DF RID: 10207
		public static int FanAnimationsBeforeConvert = 3;

		// Token: 0x040027E0 RID: 10208
		public const string DRUG_DEALER = "Drug dealer";

		// Token: 0x040027E1 RID: 10209
		public const string DrugDealerHint = "Sells SUPERHOT drugs on street";

		// Token: 0x040027E2 RID: 10210
		public const string WIFI = "Wifi Control";

		// Token: 0x040027E3 RID: 10211
		public const string WifiHint = "Control minds via wifi signal";

		// Token: 0x040027E4 RID: 10212
		public const string INDIE_DEV = "Indie Dev";

		// Token: 0x040027E5 RID: 10213
		public const string IndieDevHint = "Hire indies to create SUPERHOT clones";

		// Token: 0x040027E6 RID: 10214
		public const string FakeAccountHint = "Extra message for every one you send";

		// Token: 0x040027E7 RID: 10215
		public const string BotHint = "Types messages for you";

		// Token: 0x040027E8 RID: 10216
		public const string FanHint = "Talks to people and convert them";

		// Token: 0x040027E9 RID: 10217
		public const string StreamerHint = "Stream SUPERHOT online";

		// Token: 0x040027EA RID: 10218
		public const string FlamerHint = "Argue on social media";

		// Token: 0x040027EB RID: 10219
		public const string FAKE_ACCOUNTS = "Fake accounts";

		// Token: 0x040027EC RID: 10220
		public const string BETTER_MESSAGES = "Better messages";

		// Token: 0x040027ED RID: 10221
		public const string FANS = "Fans";

		// Token: 0x040027EE RID: 10222
		public const string CONVERTED = "Converted";

		// Token: 0x040027EF RID: 10223
		public const string BOTS = "Bots";

		// Token: 0x040027F0 RID: 10224
		public const string FLAMERS = "Flamers";

		// Token: 0x040027F1 RID: 10225
		public const string STREAMERS = "Streamers";

		// Token: 0x040027F2 RID: 10226
		public const string HOT_COINS = "Hot Coins";

		// Token: 0x040027F3 RID: 10227
		public const string HYPNOSIS = "Hypnosis";

		// Token: 0x040027F4 RID: 10228
		public const string STAR = "☼";

		// Token: 0x040027F5 RID: 10229
		public const string MULTIPLY_SIGN = " x ";

		// Token: 0x040027F6 RID: 10230
		//public const string PRAISES_PATH = "StoryApps/Games/IdleMinigame/PraiseMessages";

		// Token: 0x040027F7 RID: 10231
		//public const string NUMBER_ENDINGS_PATH = "/Resources/StoryApps/Games/IdleMinigame/HotCoinEndings.hot";

		// Token: 0x040027F8 RID: 10232
		//public const string DATA_PATH = "/Resources/StoryApps/Games/IdleMinigame/SaveData/data.hot";

		// Token: 0x040027F9 RID: 10233
		public const string HypnosisTitle = "SUPERHOT HYPNOSIS";

		// Token: 0x040027FA RID: 10234
		public const string HypnosisDescription = "Mind is software. Hypnotize them \n with digital hypnosis!";

		// Token: 0x040027FB RID: 10235
		public const string FrenzyTitle = "SUPERHOT FRENZY";

		// Token: 0x040027FC RID: 10236
		public const string FrenzyDescription = "Bodies are disposable. Your workers \n enter the frenzy mode!";

		// Token: 0x040027FD RID: 10237
		public const string ConvertedGain = " of converted people";

		// Token: 0x040027FE RID: 10238
		public const string ConvertedPerSecondGain = " converted per second";
	}

	public class SharedUpgrades : DataSerializer
	{
		// Token: 0x060025CC RID: 9676 RVA: 0x0010FCCC File Offset: 0x0010E0CC
		public SharedUpgrades()
		{
			this.HypnosisUpgradeHint = string.Format("Hypnosis adds {0}% converted more", this.HypnosisConvertedStartPercent.Fraction * 100f);
			this.PopupTimeUpgradeHint = string.Format("Popups appears {0}% more often", this.PopupShowStartTimeDecreasePercent * 100f);
			this.BrainwashHint = string.Format("Sacrifice your progress. Get bonus of {0}% more hot coin income", this.HotCoinIncomeBonusPercent.Fraction * 100f);
			float num;
			if (DataSerializer.Instance.MessagesCpsBonus == null)
			{
				num = this.MessagesCpsBonusIncreasePercent;
			}
			else
			{
				num = DataSerializer.Instance.MessagesCpsBonus.Fraction + this.MessagesCpsBonusIncreasePercent;
			}
			this.MessageCpsBonusHint = string.Format("Each message give +{0}% of your @pS", num * 100f);
		}

		// Token: 0x060025CD RID: 9677 RVA: 0x0010FFCD File Offset: 0x0010E3CD
		public void SaveWifiRateUpgrade(Value newPrice, Value newTier)
		{
			DataSerializer.Instance.WifiRateUpgradePrice = newPrice;
			DataSerializer.Instance.WifiRateUpgradeTier = newTier;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025CE RID: 9678 RVA: 0x0010FFEF File Offset: 0x0010E3EF
		public void GetWifiRateUpgrade(out Value price, out Value tier)
		{
			price = (DataSerializer.Instance.WifiRateUpgradePrice ?? this.WifiRateUpgradeStartPrice);
			tier = (DataSerializer.Instance.WifiRateUpgradeTier ?? Value.One);
		}

		// Token: 0x060025CF RID: 9679 RVA: 0x00110022 File Offset: 0x0010E422
		public void SaveIndieDevRateUpgrade(Value newPrice, Value newTier)
		{
			DataSerializer.Instance.IndieDevRateUpgradePrice = newPrice;
			DataSerializer.Instance.IndieDevRateUpgradeTier = newTier;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025D0 RID: 9680 RVA: 0x00110044 File Offset: 0x0010E444
		public void GetIndieDevRateUpgrade(out Value price, out Value tier)
		{
			price = (DataSerializer.Instance.IndieDevRateUpgradePrice ?? this.IndieDevRateUpgradeStartPrice);
			tier = (DataSerializer.Instance.IndieDevRateUpgradeTier ?? Value.One);
		}

		// Token: 0x060025D1 RID: 9681 RVA: 0x00110078 File Offset: 0x0010E478
		public Value GetPopupCount()
		{
			Value value;
			if (DataSerializer.Instance.BonusPopupCount != null)
			{
				value = DataSerializer.Instance.BonusPopupCount;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.BonusPopupCount = value;
			}
			return value;
		}

		// Token: 0x060025D2 RID: 9682 RVA: 0x001100BC File Offset: 0x0010E4BC
		public Value GetHypnosisCounter()
		{
			Value value;
			if (DataSerializer.Instance.HypnosisCount != null)
			{
				value = DataSerializer.Instance.HypnosisCount;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.HypnosisCount = value;
			}
			return value;
		}

		// Token: 0x060025D3 RID: 9683 RVA: 0x00110100 File Offset: 0x0010E500
		public Value GetHypnosisPercent()
		{
			Value value;
			if (DataSerializer.Instance.HypnosisConvertedPercent != null)
			{
				value = DataSerializer.Instance.HypnosisConvertedPercent;
			}
			else
			{
				value = this.HypnosisConvertedStartPercent;
				DataSerializer.Instance.HypnosisConvertedPercent = value;
			}
			return value;
		}

		// Token: 0x060025D4 RID: 9684 RVA: 0x00110148 File Offset: 0x0010E548
		public Value GetFrenzyCount()
		{
			Value value;
			if (DataSerializer.Instance.FrenzyCount != null)
			{
				value = DataSerializer.Instance.FrenzyCount;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.FrenzyCount = value;
			}
			return value;
		}

		// Token: 0x060025D5 RID: 9685 RVA: 0x0011018C File Offset: 0x0010E58C
		public Value GetFrenzyPercent()
		{
			Value value;
			if (DataSerializer.Instance.FrenzyAccelerationPercent != null)
			{
				value = DataSerializer.Instance.FrenzyAccelerationPercent;
			}
			else
			{
				value = this.FrenzyAccelerationStartPercent;
				DataSerializer.Instance.FrenzyAccelerationPercent = value;
			}
			return value;
		}

		// Token: 0x060025D6 RID: 9686 RVA: 0x001101D4 File Offset: 0x0010E5D4
		public Value GetActualFrenzyPercent()
		{
			if (!AppTheSystem.Instance.InFrenzy)
			{
				return Value.One;
			}
			Value value;
			if (DataSerializer.Instance.FrenzyAccelerationPercent != null)
			{
				value = DataSerializer.Instance.FrenzyAccelerationPercent;
			}
			else
			{
				value = this.FrenzyAccelerationStartPercent;
				DataSerializer.Instance.FrenzyAccelerationPercent = value;
			}
			return value;
		}

		// Token: 0x060025D7 RID: 9687 RVA: 0x00110230 File Offset: 0x0010E630
		public float GetFrenzyTime()
		{
			float num;
			if (DataSerializer.Instance.FrenzyTime != 0f)
			{
				num = DataSerializer.Instance.FrenzyTime;
			}
			else
			{
				num = this.FrenzyStartTime;
				DataSerializer.Instance.FrenzyTime = num;
			}
			return num;
		}

		// Token: 0x060025D8 RID: 9688 RVA: 0x00110274 File Offset: 0x0010E674
		public float GetPopupShowTime()
		{
			float num;
			if (DataSerializer.Instance.BonusPopupShowTime != 0f)
			{
				num = DataSerializer.Instance.BonusPopupShowTime;
			}
			else
			{
				num = this.PopupShowStartTime;
				DataSerializer.Instance.BonusPopupShowTime = num;
			}
			return num;
		}

		// Token: 0x060025D9 RID: 9689 RVA: 0x001102B8 File Offset: 0x0010E6B8
		public Value GetBrainwashTier()
		{
			return DataSerializer.Instance.BrainwashTier ?? Value.One;
		}

		// Token: 0x060025DA RID: 9690 RVA: 0x001102D0 File Offset: 0x0010E6D0
		public Value GetMessageConvertedMultiplier()
		{
			Value value;
			if (DataSerializer.Instance.MessageConvertedMultiplier != null)
			{
				value = DataSerializer.Instance.MessageConvertedMultiplier;
			}
			else
			{
				value = Value.One;
				DataSerializer.Instance.MessageConvertedMultiplier = value;
			}
			return value;
		}

		// Token: 0x060025DB RID: 9691 RVA: 0x00110314 File Offset: 0x0010E714
		public Value GetMessageCpsBonus()
		{
			Value result;
			if (DataSerializer.Instance.MessagesCpsBonus != null)
			{
				result = IncomeController.Instance.TotalIncomePerSec() * DataSerializer.Instance.MessagesCpsBonus;
			}
			else
			{
				result = IncomeController.Instance.TotalIncomePerSec() * this.MessagesCpsBonusStartPercent;
				DataSerializer.Instance.MessagesCpsBonus = this.MessagesCpsBonusStartPercent;
			}
			return result;
		}

		// Token: 0x060025DC RID: 9692 RVA: 0x0011037C File Offset: 0x0010E77C
		public void IncreaseRateMultiplier(SharedUpgrades.UpgradeType upgradeType)
		{
			switch (upgradeType)
			{
				case SharedUpgrades.UpgradeType.Bot:
					DataSerializer.Instance.BotIncomeMultiplier *= this.BaseIncomeMultiplier;
					break;
				case SharedUpgrades.UpgradeType.Fan:
					DataSerializer.Instance.FanIncomeMultiplier *= this.BaseIncomeMultiplier;
					break;
				case SharedUpgrades.UpgradeType.Streamer:
					DataSerializer.Instance.StreamerIncomeMultiplier *= this.BaseIncomeMultiplier;
					break;
				case SharedUpgrades.UpgradeType.Flamer:
					DataSerializer.Instance.FlamerIncomeMultiplier *= this.BaseIncomeMultiplier;
					break;
				case SharedUpgrades.UpgradeType.DrugDealer:
					DataSerializer.Instance.DrugDealerIncomeMultiplier *= this.BaseIncomeMultiplier;
					break;
				case SharedUpgrades.UpgradeType.WifiRate:
					DataSerializer.Instance.WifiIncomeMultiplier *= this.BaseIncomeMultiplier;
					break;
				case SharedUpgrades.UpgradeType.IndieDev:
					DataSerializer.Instance.IndieDevIncomeMultiplier *= this.BaseIncomeMultiplier;
					break;
				case SharedUpgrades.UpgradeType.ConvertedPerMessage:
					DataSerializer.Instance.MessageConvertedMultiplier = this.GetMessageConvertedMultiplier() * this.BaseIncomeMultiplier;
					break;
				case SharedUpgrades.UpgradeType.MessageCps:
					DataSerializer.Instance.MessagesCpsBonus += new Value(this.MessagesCpsBonusIncreasePercent);
					break;
				case SharedUpgrades.UpgradeType.Hypnosis:
					DataSerializer.Instance.HypnosisConvertedPercent = this.GetHypnosisPercent() + this.HypnosisConvertedStartPercent;
					break;
			}
		}

		// Token: 0x060025DD RID: 9693 RVA: 0x00110505 File Offset: 0x0010E905
		public void IncreaseHypnosisCounter()
		{
			DataSerializer.Instance.HypnosisCount += Value.One;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025DE RID: 9694 RVA: 0x0011052B File Offset: 0x0010E92B
		public void IncreaseFrenzyCounter()
		{
			DataSerializer.Instance.HypnosisCount += Value.One;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025DF RID: 9695 RVA: 0x00110551 File Offset: 0x0010E951
		public void IncreasePopupCount()
		{
			DataSerializer.Instance.BonusPopupCount += Value.One;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025E0 RID: 9696 RVA: 0x00110577 File Offset: 0x0010E977
		public void DecreasePopupShowTime()
		{
			DataSerializer.Instance.BonusPopupShowTime -= this.PopupShowStartTimeDecreasePercent * DataSerializer.Instance.BonusPopupShowTime;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025E1 RID: 9697 RVA: 0x001105A5 File Offset: 0x0010E9A5
		public void SaveBrainwashTier(Value newTier)
		{
			DataSerializer.Instance.BrainwashTier = newTier;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025E2 RID: 9698 RVA: 0x001105BC File Offset: 0x0010E9BC
		public string GetNewMessageCpsBonusHint()
		{
			return string.Format("Each message give +{0}% of your @pS", (DataSerializer.Instance.MessagesCpsBonus.Fraction + this.MessagesCpsBonusIncreasePercent) * 100f);
		}

		// Token: 0x060025E3 RID: 9699 RVA: 0x001105E9 File Offset: 0x0010E9E9
		public void SaveBotRateUpgrade(Value newPrice, Value newTier)
		{
			DataSerializer.Instance.BotRateUpgradePrice = newPrice;
			DataSerializer.Instance.BotRateUpgradeTier = newTier;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025E4 RID: 9700 RVA: 0x0011060B File Offset: 0x0010EA0B
		public void GetBotRateUpgrade(out Value price, out Value tier)
		{
			price = (DataSerializer.Instance.BotRateUpgradePrice ?? this.BotRateUpgradeStartPrice);
			tier = (DataSerializer.Instance.BotRateUpgradeTier ?? Value.One);
		}

		// Token: 0x060025E5 RID: 9701 RVA: 0x0011063E File Offset: 0x0010EA3E
		public void SaveFanRateUpgrade(Value newPrice, Value newTier)
		{
			DataSerializer.Instance.FanRateUpgradePrice = newPrice;
			DataSerializer.Instance.FanRateUpgradeTier = newTier;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025E6 RID: 9702 RVA: 0x00110660 File Offset: 0x0010EA60
		public void GetFanRateUpgrade(out Value price, out Value tier)
		{
			price = (DataSerializer.Instance.FanRateUpgradePrice ?? this.FanRateUpgradeStartPrice);
			tier = (DataSerializer.Instance.FanRateUpgradeTier ?? Value.One);
		}

		// Token: 0x060025E7 RID: 9703 RVA: 0x00110693 File Offset: 0x0010EA93
		public void SaveStreamerRateUpgrade(Value newPrice, Value newTier)
		{
			DataSerializer.Instance.StreamerRateUpgradePrice = newPrice;
			DataSerializer.Instance.StreamerRateUpgradeTier = newTier;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025E8 RID: 9704 RVA: 0x001106B5 File Offset: 0x0010EAB5
		public void GetStreamerRateUpgrade(out Value price, out Value tier)
		{
			price = (DataSerializer.Instance.StreamerRateUpgradePrice ?? this.StreamerRateUpgradeStartPrice);
			tier = (DataSerializer.Instance.StreamerRateUpgradeTier ?? Value.One);
		}

		// Token: 0x060025E9 RID: 9705 RVA: 0x001106E8 File Offset: 0x0010EAE8
		public void SaveFlamerRateUpgrade(Value newPrice, Value newTier)
		{
			DataSerializer.Instance.FlamerRateUpgradePrice = newPrice;
			DataSerializer.Instance.FlamerRateUpgradeTier = newTier;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025EA RID: 9706 RVA: 0x0011070A File Offset: 0x0010EB0A
		public void GetFlamerRateUpgrade(out Value price, out Value tier)
		{
			price = (DataSerializer.Instance.FlamerRateUpgradePrice ?? this.FlamerRateUpgradeStartPrice);
			tier = (DataSerializer.Instance.FlamerRateUpgradeTier ?? Value.One);
		}

		// Token: 0x060025EB RID: 9707 RVA: 0x0011073D File Offset: 0x0010EB3D
		public void SaveDrugDealerRateUpgrade(Value newPrice, Value newTier)
		{
			DataSerializer.Instance.DrugDealerRateUpgradePrice = newPrice;
			DataSerializer.Instance.DrugDealerRateUpgradeTier = newTier;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025EC RID: 9708 RVA: 0x0011075F File Offset: 0x0010EB5F
		public void GetDrugDealerRateUpgrade(out Value price, out Value tier)
		{
			price = (DataSerializer.Instance.DrugDealerRateUpgradePrice ?? this.DrugDealerRateUpgradeStartPrice);
			tier = (DataSerializer.Instance.DrugDealerRateUpgradeTier ?? Value.One);
		}

		// Token: 0x060025ED RID: 9709 RVA: 0x00110792 File Offset: 0x0010EB92
		public void SaveMessageConvertedUpgrade(Value newPrice, Value newTier)
		{
			DataSerializer.Instance.MessageConvertedUpgradePrice = newPrice;
			DataSerializer.Instance.MessageConvertedUpgradeTier = newTier;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025EE RID: 9710 RVA: 0x001107B4 File Offset: 0x0010EBB4
		public void GetMessageConvertedUpgrade(out Value price, out Value tier)
		{
			price = (DataSerializer.Instance.MessageConvertedUpgradePrice ?? this.MessageConvertedStartPrice);
			tier = (DataSerializer.Instance.MessageConvertedUpgradeTier ?? Value.One);
		}

		// Token: 0x060025EF RID: 9711 RVA: 0x001107E7 File Offset: 0x0010EBE7
		public void SaveMessageCpsBonusUpgrade(Value newPrice, Value newTier)
		{
			DataSerializer.Instance.MessageCpsBonusUpgradePrice = newPrice;
			DataSerializer.Instance.MessageCpsBonusUpgradeTier = newTier;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025F0 RID: 9712 RVA: 0x00110809 File Offset: 0x0010EC09
		public void GetMessageCpsBonusUpgrade(out Value price, out Value tier)
		{
			price = (DataSerializer.Instance.MessageCpsBonusUpgradePrice ?? this.MessageCpsBonusStartPrice);
			tier = (DataSerializer.Instance.MessageCpsBonusUpgradeTier ?? Value.One);
		}

		// Token: 0x060025F1 RID: 9713 RVA: 0x0011083C File Offset: 0x0010EC3C
		public void GetHypnosisUpgrade(out Value price, out Value tier)
		{
			price = (DataSerializer.Instance.HypnosisUpgradePrice ?? this.HypnosisUpgradeStartPrice);
			tier = (DataSerializer.Instance.HypnosisUpgradeTier ?? Value.One);
		}

		// Token: 0x060025F2 RID: 9714 RVA: 0x0011086F File Offset: 0x0010EC6F
		public void SaveHypnosisUpgrade(Value newPrice, Value newTier)
		{
			DataSerializer.Instance.HypnosisUpgradePrice = newPrice;
			DataSerializer.Instance.HypnosisUpgradeTier = newTier;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025F3 RID: 9715 RVA: 0x00110891 File Offset: 0x0010EC91
		public void GetPopupTimeUpgrade(out Value price, out Value tier)
		{
			price = (DataSerializer.Instance.PopupTimeUpgradePrice ?? this.PopupTimeUpgradeStartPrice);
			tier = (DataSerializer.Instance.PopupTimeUpgradeTier ?? Value.One);
		}

		// Token: 0x060025F4 RID: 9716 RVA: 0x001108C4 File Offset: 0x0010ECC4
		public void SavePopupTimeUpgrade(Value newPrice, Value newTier)
		{
			DataSerializer.Instance.PopupTimeUpgradePrice = newPrice;
			DataSerializer.Instance.PopupTimeUpgradeTier = newTier;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x040027FF RID: 10239
		public Value BaseIncomeMultiplier = new Value(2f);

		// Token: 0x04002800 RID: 10240
		public Value MessagesCpsBonusStartPercent = new Value(0f);

		// Token: 0x04002801 RID: 10241
		public float MessagesCpsBonusIncreasePercent = 0.01f;

		// Token: 0x04002802 RID: 10242
		public Value HypnosisConvertedStartPercent = new Value(0.2f);

		// Token: 0x04002803 RID: 10243
		public Value FrenzyAccelerationStartPercent = new Value(10f);

		// Token: 0x04002804 RID: 10244
		public float FrenzyStartTime = 30f;

		// Token: 0x04002805 RID: 10245
		public float PopupShowStartTime = 180f;

		// Token: 0x04002806 RID: 10246
		public float PopupShowStartTimeDecreasePercent = 0.2f;

		// Token: 0x04002807 RID: 10247
		public Value HotCoinIncomeBonusPercent = new Value(0.2f);

		// Token: 0x04002808 RID: 10248
		public readonly Value UpgradePriceIncreaseRate = new Value(10f);

		// Token: 0x04002809 RID: 10249
		public string BOT_RATE = "Bot rate";

		// Token: 0x0400280A RID: 10250
		public string BotRateHint = "Bots generates TWICE as much converted";

		// Token: 0x0400280B RID: 10251
		public Value BotRateUpgradeStartPrice = new Value(100f);

		// Token: 0x0400280C RID: 10252
		public string FAN_RATE = "Fan rate";

		// Token: 0x0400280D RID: 10253
		public string FanRateHint = "Fans generates TWICE as much converted";

		// Token: 0x0400280E RID: 10254
		public Value FanRateUpgradeStartPrice = new Value(1, 5f);

		// Token: 0x0400280F RID: 10255
		public string STREAMER_RATE = "Streamer rate";

		// Token: 0x04002810 RID: 10256
		public string StreamerRateHint = "Streamers generates TWICE as much converted";

		// Token: 0x04002811 RID: 10257
		public Value StreamerRateUpgradeStartPrice = new Value(1, 20f);

		// Token: 0x04002812 RID: 10258
		public string FLAMER_RATE = "Flamer rate";

		// Token: 0x04002813 RID: 10259
		public string FlamerRateHint = "Flamers generates TWICE as much converted";

		// Token: 0x04002814 RID: 10260
		public Value FlamerRateUpgradeStartPrice = new Value(1, 500f);

		// Token: 0x04002815 RID: 10261
		public string DRUG_DEALER_RATE = "Dealer rate";

		// Token: 0x04002816 RID: 10262
		public string DrugDealerRateHint = "Drug Dealers generates TWICE as much converted";

		// Token: 0x04002817 RID: 10263
		public Value DrugDealerRateUpgradeStartPrice = new Value(2, 10f);

		// Token: 0x04002818 RID: 10264
		public string WIFI_RATE = "Wifi rate";

		// Token: 0x04002819 RID: 10265
		public string WifiRateHint = "Wifis generates TWICE as much converted";

		// Token: 0x0400281A RID: 10266
		public Value WifiRateUpgradeStartPrice = new Value(2, 200f);

		// Token: 0x0400281B RID: 10267
		public string INDIE_DEV_RATE = "IndieDev rate";

		// Token: 0x0400281C RID: 10268
		public string IndieDevRateHint = "IndieDevs generates TWICE as much converted";

		// Token: 0x0400281D RID: 10269
		public Value IndieDevRateUpgradeStartPrice = new Value(3, 5f);

		// Token: 0x0400281E RID: 10270
		public string MESSAGE_CONVERTED = "Better messages";

		// Token: 0x0400281F RID: 10271
		public string MessageConvertedHint = "Messages generates TWICE as much converted";

		// Token: 0x04002820 RID: 10272
		public Value MessageConvertedStartPrice = new Value(1, 1f);

		// Token: 0x04002821 RID: 10273
		public string MESSAGE_CPS_BONUS = "Message @pS";

		// Token: 0x04002822 RID: 10274
		public string MessageCpsBonusHint = string.Empty;

		// Token: 0x04002823 RID: 10275
		public Value MessageCpsBonusStartPrice = new Value(1, 5f);

		// Token: 0x04002824 RID: 10276
		public string HYPNOSIS_UPGRADE = "Better hypnosis";

		// Token: 0x04002825 RID: 10277
		public string HypnosisUpgradeHint;

		// Token: 0x04002826 RID: 10278
		public Value HypnosisUpgradeStartPrice = new Value(1, 100f);

		// Token: 0x04002827 RID: 10279
		public string POPUP_TIME = "Faster popups";

		// Token: 0x04002828 RID: 10280
		public string PopupTimeUpgradeHint;

		// Token: 0x04002829 RID: 10281
		public Value PopupTimeUpgradeStartPrice = new Value(1, 500f);

		// Token: 0x0400282A RID: 10282
		public string BRAINWASH = "VR Brainwash";

		// Token: 0x0400282B RID: 10283
		public string BrainwashHint;

		// Token: 0x02000671 RID: 1649
		public enum UpgradeType
		{
			// Token: 0x0400282D RID: 10285
			Bot,
			// Token: 0x0400282E RID: 10286
			FakeAccount,
			// Token: 0x0400282F RID: 10287
			Fan,
			// Token: 0x04002830 RID: 10288
			Streamer,
			// Token: 0x04002831 RID: 10289
			Flamer,
			// Token: 0x04002832 RID: 10290
			DrugDealer,
			// Token: 0x04002833 RID: 10291
			WifiRate,
			// Token: 0x04002834 RID: 10292
			IndieDev,
			// Token: 0x04002835 RID: 10293
			ConvertedPerMessage,
			// Token: 0x04002836 RID: 10294
			MessageCps,
			// Token: 0x04002837 RID: 10295
			Hypnosis,
			// Token: 0x04002838 RID: 10296
			Brainwash,
			// Token: 0x04002839 RID: 10297
			Popup
		}
	}

	public class SharedViews
	{
		// Token: 0x0400283A RID: 10298
		public int ChatViewPositionX = 2;

		// Token: 0x0400283B RID: 10299
		public int ChatViewPositionY = 3;

		// Token: 0x0400283C RID: 10300
		public int ChatViewHeight = 16;

		// Token: 0x0400283D RID: 10301
		public int ChatViewWidth = 38;

		// Token: 0x0400283E RID: 10302
		public int ShopViewPositionX = 45;

		// Token: 0x0400283F RID: 10303
		public int ShopViewPositionY = 15;

		// Token: 0x04002840 RID: 10304
		public int ShopButtonHeight = 3;

		// Token: 0x04002841 RID: 10305
		public int ShopButtonWidth = 17;

		// Token: 0x04002842 RID: 10306
		public int ShopHintViewHeight = 5;

		// Token: 0x04002843 RID: 10307
		public int ShopHintViewWidth = 40;

		// Token: 0x04002844 RID: 10308
		public float ConvertedManAnimationTime = 1f;

		// Token: 0x04002845 RID: 10309
		public float StreamerAnimationTime = 1f;

		// Token: 0x04002846 RID: 10310
		public float FlamerAnimationTime = 0.7f;

		// Token: 0x04002847 RID: 10311
		public float DrugAnimationTime = 0.3f;

		// Token: 0x04002848 RID: 10312
		public float FanAnimationTime = 0.5f;

		// Token: 0x04002849 RID: 10313
		public float WifiAnimationTime = 0.5f;

		// Token: 0x0400284A RID: 10314
		public float IndieDevAnimationTime = 0.5f;

		// Token: 0x0400284B RID: 10315
		public float BotAnimationTime = 1f;

		// Token: 0x0400284C RID: 10316
		public int HotCoinsViewPositionX = 44;

		// Token: 0x0400284D RID: 10317
		public int HotCoinsViewPositionY = 1;

		// Token: 0x0400284E RID: 10318
		public int StarViewPositionX = 2;

		// Token: 0x0400284F RID: 10319
		public int StarViewPositionY = 1;

		// Token: 0x04002850 RID: 10320
		public char StarViewColor = 'r';

		// Token: 0x04002851 RID: 10321
		public int TotalIncomePositionX = 3;

		// Token: 0x04002852 RID: 10322
		public int TotalIncomePositionY = 1;

		// Token: 0x04002853 RID: 10323
		public int ScrollViewMaxElements = 6;
	}

	public class SharedWorkers : DataSerializer
	{
		// Token: 0x060025F7 RID: 9719 RVA: 0x00110AF8 File Offset: 0x0010EEF8
		public Value GetFakeAccounts()
		{
			Value value;
			if (DataSerializer.Instance.FakeAccountCount != null)
			{
				value = DataSerializer.Instance.FakeAccountCount;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.FakeAccountCount = value;
			}
			return value;
		}

		// Token: 0x060025F8 RID: 9720 RVA: 0x00110B3C File Offset: 0x0010EF3C
		public Value GetFans()
		{
			Value value;
			if (DataSerializer.Instance.FanCount != null)
			{
				value = DataSerializer.Instance.FanCount;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.FanCount = value;
			}
			return value;
		}

		// Token: 0x060025F9 RID: 9721 RVA: 0x00110B80 File Offset: 0x0010EF80
		public Value GetBots()
		{
			Value value;
            if (DataSerializer.Instance.BotCount != null)
			{
                value = DataSerializer.Instance.BotCount;
            }
			else
			{
                value = Value.Zero;
				DataSerializer.Instance.BotCount = value;
            }
			return value;
		}

		// Token: 0x060025FA RID: 9722 RVA: 0x00110BC4 File Offset: 0x0010EFC4
		public Value GetStreamers()
		{
			Value value;
			if (DataSerializer.Instance.StreamerCount != null)
			{
				value = DataSerializer.Instance.StreamerCount;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.StreamerCount = value;
			}
			return value;
		}

		// Token: 0x060025FB RID: 9723 RVA: 0x00110C08 File Offset: 0x0010F008
		public Value GetFlamers()
		{
			Value value;
			if (DataSerializer.Instance.FlamerCount != null)
			{
				value = DataSerializer.Instance.FlamerCount;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.FlamerCount = value;
			}
			return value;
		}

		// Token: 0x060025FC RID: 9724 RVA: 0x00110C4C File Offset: 0x0010F04C
		public void GetWorkersIncomeMultiplier(out Value botMultiplier, out Value fanMultiplier, out Value streamerMultiplier, out Value flamerMultiplier, out Value drugDealerMultiplier, out Value wifiMultiplier, out Value indieDevMultiplier)
		{
			Value value;
			if (DataSerializer.Instance.BotIncomeMultiplier != null)
			{
				value = DataSerializer.Instance.BotIncomeMultiplier;
			}
			else
			{
				value = Value.One;
				DataSerializer.Instance.BotIncomeMultiplier = value;
			}
			botMultiplier = value;
			if (DataSerializer.Instance.FanIncomeMultiplier != null)
			{
				value = DataSerializer.Instance.FanIncomeMultiplier;
			}
			else
			{
				value = Value.One;
				DataSerializer.Instance.FanIncomeMultiplier = value;
			}
			fanMultiplier = value;
			if (DataSerializer.Instance.StreamerIncomeMultiplier != null)
			{
				value = DataSerializer.Instance.StreamerIncomeMultiplier;
			}
			else
			{
				value = Value.One;
				DataSerializer.Instance.StreamerIncomeMultiplier = value;
			}
			streamerMultiplier = value;
			if (DataSerializer.Instance.FlamerIncomeMultiplier != null)
			{
				value = DataSerializer.Instance.FlamerIncomeMultiplier;
			}
			else
			{
				value = Value.One;
				DataSerializer.Instance.FlamerIncomeMultiplier = value;
			}
			flamerMultiplier = value;
			if (DataSerializer.Instance.DrugDealerIncomeMultiplier != null)
			{
				value = DataSerializer.Instance.DrugDealerIncomeMultiplier;
			}
			else
			{
				value = Value.One;
				DataSerializer.Instance.DrugDealerIncomeMultiplier = value;
			}
			drugDealerMultiplier = value;
			if (DataSerializer.Instance.WifiIncomeMultiplier != null)
			{
				value = DataSerializer.Instance.WifiIncomeMultiplier;
			}
			else
			{
				value = Value.One;
				DataSerializer.Instance.WifiIncomeMultiplier = value;
			}
			wifiMultiplier = value;
			if (DataSerializer.Instance.IndieDevIncomeMultiplier != null)
			{
				value = DataSerializer.Instance.IndieDevIncomeMultiplier;
			}
			else
			{
				value = Value.One;
				DataSerializer.Instance.IndieDevIncomeMultiplier = value;
			}
			indieDevMultiplier = value;
		}

		// Token: 0x060025FD RID: 9725 RVA: 0x00110DEC File Offset: 0x0010F1EC
		public void AddFakeAccounts(Value value)
		{
			DataSerializer.Instance.FakeAccountCount += value;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025FE RID: 9726 RVA: 0x00110E0E File Offset: 0x0010F20E
		public void AddFans(Value value)
		{
			DataSerializer.Instance.FanCount += value;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x060025FF RID: 9727 RVA: 0x00110E30 File Offset: 0x0010F230
		public void AddBots(Value value)
		{
			DataSerializer.Instance.BotCount += value;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x06002600 RID: 9728 RVA: 0x00110E52 File Offset: 0x0010F252
		public void AddStreamers(Value value)
		{
			DataSerializer.Instance.StreamerCount += value;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x06002601 RID: 9729 RVA: 0x00110E74 File Offset: 0x0010F274
		public void AddFlamer(Value value)
		{
			DataSerializer.Instance.FlamerCount += value;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x06002602 RID: 9730 RVA: 0x00110E98 File Offset: 0x0010F298
		public Value GetDrugDealers()
		{
			Value value;
			if (DataSerializer.Instance.DrugDealerCount != null)
			{
				value = DataSerializer.Instance.DrugDealerCount;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.DrugDealerCount = value;
			}
			return value;
		}

		// Token: 0x06002603 RID: 9731 RVA: 0x00110EDC File Offset: 0x0010F2DC
		public void AddDrugDealers(Value value)
		{
			DataSerializer.Instance.DrugDealerCount += value;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x06002604 RID: 9732 RVA: 0x00110F00 File Offset: 0x0010F300
		public Value GetWifis()
		{
			Value value;
			if (DataSerializer.Instance.WifiCount != null)
			{
				value = DataSerializer.Instance.WifiCount;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.WifiCount = value;
			}
			return value;
		}

		// Token: 0x06002605 RID: 9733 RVA: 0x00110F44 File Offset: 0x0010F344
		public void AddWifis(Value value)
		{
			DataSerializer.Instance.WifiCount += value;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x06002606 RID: 9734 RVA: 0x00110F68 File Offset: 0x0010F368
		public Value GetIndieDevs()
		{
			Value value;
			if (DataSerializer.Instance.IndieDevCount != null)
			{
				value = DataSerializer.Instance.IndieDevCount;
			}
			else
			{
				value = Value.Zero;
				DataSerializer.Instance.IndieDevCount = value;
			}
			return value;
		}

		// Token: 0x06002607 RID: 9735 RVA: 0x00110FAC File Offset: 0x0010F3AC
		public void AddIndieDevs(Value value)
		{
			DataSerializer.Instance.IndieDevCount += value;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x06002608 RID: 9736 RVA: 0x00110FD0 File Offset: 0x0010F3D0
		public void GetShopPrices(out Value accountPrice, out Value botPrice, out Value fanPrice, out Value streamerPrice, out Value flamerPrice, out Value drugDealerPrice, out Value wifiPrice, out Value indieDevPrice)
		{
			accountPrice = (DataSerializer.Instance.FakeAccountPrice ?? this.FakeAccountStartPrice);
			botPrice = (DataSerializer.Instance.BotPrice ?? this.BotStartPrice);
			fanPrice = (DataSerializer.Instance.FanPrice ?? this.FanStartPrice);
			streamerPrice = (DataSerializer.Instance.StreamerPrice ?? this.StreamerStartPrice);
			flamerPrice = (DataSerializer.Instance.FlamerPrice ?? this.FlamerStartPrice);
			drugDealerPrice = (DataSerializer.Instance.DrugDealerPrice ?? this.DrugDealerStartPrice);
			wifiPrice = (DataSerializer.Instance.WifiPrice ?? this.WifiStartPrice);
			indieDevPrice = (DataSerializer.Instance.IndieDevPrice ?? this.IndieDevStartPrice);
		}

		// Token: 0x06002609 RID: 9737 RVA: 0x001110AC File Offset: 0x0010F4AC
		public void SaveShopPrices(Value accountPrice, Value botPrice, Value fanPrice, Value streamerPrice, Value flamerPrice, Value drugDealerPrice, Value wifiPrice, Value indieDevPrice)
		{
			DataSerializer.Instance.FakeAccountPrice = accountPrice;
			DataSerializer.Instance.FanPrice = fanPrice;
			DataSerializer.Instance.BotPrice = botPrice;
			DataSerializer.Instance.StreamerPrice = streamerPrice;
			DataSerializer.Instance.FlamerPrice = flamerPrice;
			DataSerializer.Instance.DrugDealerPrice = drugDealerPrice;
			DataSerializer.Instance.WifiPrice = wifiPrice;
			DataSerializer.Instance.IndieDevPrice = indieDevPrice;
			DataSerializer.Instance.Serialize();
		}

		// Token: 0x0600260A RID: 9738 RVA: 0x00111120 File Offset: 0x0010F520
		public Value CalculatePrice(Value basePrice, Value priceIncreaseRate, Value owned)
		{
			return basePrice * Value.Power(priceIncreaseRate, owned);
		}

		// Token: 0x04002854 RID: 10324
		public readonly Value PriceIncreaseRate = new Value(1.15f);

		// Token: 0x04002855 RID: 10325
		public readonly Value BotStartPrice = new Value(10f);

		// Token: 0x04002856 RID: 10326
		public readonly Value FakeAccountStartPrice = new Value(100f);

		// Token: 0x04002857 RID: 10327
		public readonly Value FanStartPrice = new Value(800f);

		// Token: 0x04002858 RID: 10328
		public readonly Value StreamerStartPrice = new Value(7000f);

		// Token: 0x04002859 RID: 10329
		public readonly Value FlamerStartPrice = new Value(1, 65f);

		// Token: 0x0400285A RID: 10330
		public readonly Value DrugDealerStartPrice = new Value(2, 1f);

		// Token: 0x0400285B RID: 10331
		public readonly Value WifiStartPrice = new Value(2, 10f);

		// Token: 0x0400285C RID: 10332
		public readonly Value IndieDevStartPrice = new Value(3, 1f);

		// Token: 0x0400285D RID: 10333
		public readonly Value BotStartRate = new Value(0.25f);

		// Token: 0x0400285E RID: 10334
		public readonly Value FanStartRate = new Value(8f);

		// Token: 0x0400285F RID: 10335
		public readonly Value StreamerStartRate = new Value(40f);

		// Token: 0x04002860 RID: 10336
		public readonly Value FlamerStartRate = new Value(400f);

		// Token: 0x04002861 RID: 10337
		public readonly Value DrugDealerStartRate = new Value(1500f);

		// Token: 0x04002862 RID: 10338
		public readonly Value WifiStartRate = new Value(1, 10f);

		// Token: 0x04002863 RID: 10339
		public readonly Value IndieDevStartRate = new Value(2, 1f);

		// Token: 0x02000674 RID: 1652
		public enum Workers
		{
			// Token: 0x04002865 RID: 10341
			Bot,
			// Token: 0x04002866 RID: 10342
			Fan,
			// Token: 0x04002867 RID: 10343
			Streamer,
			// Token: 0x04002868 RID: 10344
			Flamer,
			// Token: 0x04002869 RID: 10345
			DrugDealer,
			// Token: 0x0400286A RID: 10346
			Wifi,
			// Token: 0x0400286B RID: 10347
			IndieDev
		}
	}

	public class ShopButton : AStoryAppsView
	{
		// Token: 0x060026AE RID: 9902 RVA: 0x00111AA4 File Offset: 0x0010FEA4
		public ShopButton(string text, int startX, int startY, int width, int height, char col) : base(startX, startY, width, height)
		{
			base.PositionX = startX;
			base.PositionY = startY;
			this.Button = new SHSharp::SHGUIButton(text, startX + 1, startY + 1, col);
			this.Frame = new SHSharp::SHGUIframe(startX, startY, startX + width, startY + height - 1, col);
			this.PriceText = new SHSharp::SHGUItext(this.price.ToString(), startX + width, startY + height - 1, col, false);
			this.CountText = new SHSharp::SHGUItext(this.count.ToString(), startX + 1, startY, 'w', false);
			this.text = text;
			this.Active = false;
			this.AddSubView(this.Frame);
			this.AddSubView(this.PriceText);
			this.AddSubView(this.Button);
		}

		// Token: 0x060026AF RID: 9903 RVA: 0x00111B8B File Offset: 0x0010FF8B
		public void SetFrameColor(char color)
		{
			this.Frame.SetColorRecursive(color);
		}

		// Token: 0x060026B0 RID: 9904 RVA: 0x00111B9A File Offset: 0x0010FF9A
		public void CheckPriceColor()
		{
			this.PriceCol = ((!(this.Price > SharedTheSystem.General.GetHotCoins())) ? 'w' : 'r');
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x060026B1 RID: 9905 RVA: 0x00111BC5 File Offset: 0x0010FFC5
		// (set) Token: 0x060026B2 RID: 9906 RVA: 0x00111BCD File Offset: 0x0010FFCD
		public string HintText { get; set; }

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x060026B3 RID: 9907 RVA: 0x00111BD6 File Offset: 0x0010FFD6
		// (set) Token: 0x060026B4 RID: 9908 RVA: 0x00111BDE File Offset: 0x0010FFDE
		public SHSharp::SHGUIButton Button { get; private set; }

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x060026B5 RID: 9909 RVA: 0x00111BE7 File Offset: 0x0010FFE7
		// (set) Token: 0x060026B6 RID: 9910 RVA: 0x00111BEF File Offset: 0x0010FFEF
		public SHSharp::SHGUIsprite Anim { get; set; }

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x060026B7 RID: 9911 RVA: 0x00111BF8 File Offset: 0x0010FFF8
		// (set) Token: 0x060026B8 RID: 9912 RVA: 0x00111C00 File Offset: 0x00110000
		public bool ShowAnim { get; set; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x060026B9 RID: 9913 RVA: 0x00111C09 File Offset: 0x00110009
		// (set) Token: 0x060026BA RID: 9914 RVA: 0x00111C11 File Offset: 0x00110011
		public Value StartPrice { get; set; }

		// Token: 0x170001F9 RID: 505
		// (get) Token: 0x060026BB RID: 9915 RVA: 0x00111C1A File Offset: 0x0011001A
		// (set) Token: 0x060026BC RID: 9916 RVA: 0x00111C22 File Offset: 0x00110022
		public new bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
				this.Button.text = ((!this.active) ? "???" : this.text);
			}
		}

		// Token: 0x170001FA RID: 506
		// (set) Token: 0x060026BD RID: 9917 RVA: 0x00111C51 File Offset: 0x00110051
		public bool ShowCount
		{
			set
			{
				if (!value)
				{
					this.RemoveView(this.CountText);
				}
				else
				{
					this.AddSubView(this.CountText);
				}
			}
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x060026BE RID: 9918 RVA: 0x00111C77 File Offset: 0x00110077
		// (set) Token: 0x060026BF RID: 9919 RVA: 0x00111C80 File Offset: 0x00110080
		public Value Price
		{
			get
			{
				return this.price;
			}
			set
			{
				this.price = value;
                if (this.price != null)
				{
                    this.PriceText.text = string.Format("{0}HC", value);
                    this.PriceText.text = ((value.Counter <= 0) ? string.Format("{0:0.##}HC", value.Fraction) : string.Format("{0:0}HC", value));
                    this.PriceText.x = SharedTheSystem.Views.ShopViewPositionX + SharedTheSystem.Views.ShopButtonWidth - this.PriceText.text.Length;
                }
			}
		}

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x060026C0 RID: 9920 RVA: 0x00111D28 File Offset: 0x00110128
		// (set) Token: 0x060026C1 RID: 9921 RVA: 0x00111D30 File Offset: 0x00110130
		public Value Count
		{
			get
			{
				return this.count;
			}
			set
			{
				this.count = value;
				if (this.CountText != null)
				{
					this.CountText.text = string.Format("x{0}", value);
				}
			}
		}

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x060026C2 RID: 9922 RVA: 0x00111D5A File Offset: 0x0011015A
		// (set) Token: 0x060026C3 RID: 9923 RVA: 0x00111D62 File Offset: 0x00110162
		public char PriceCol
		{
			get
			{
				return this.priceCol;
			}
			set
			{
				this.priceCol = value;
				if (this.PriceText != null)
				{
					this.PriceText.color = value;
				}
			}
		}

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x060026C4 RID: 9924 RVA: 0x00111D82 File Offset: 0x00110182
		// (set) Token: 0x060026C5 RID: 9925 RVA: 0x00111D8A File Offset: 0x0011018A
		public char BackColor
		{
			get
			{
				return this.backColor;
			}
			set
			{
				this.backColor = value;
				if (this.Button != null)
				{
					this.Button.backColor = value;
				}
			}
		}

		// Token: 0x170001FF RID: 511
		// (get) Token: 0x060026C6 RID: 9926 RVA: 0x00111DAA File Offset: 0x001101AA
		// (set) Token: 0x060026C7 RID: 9927 RVA: 0x00111DB2 File Offset: 0x001101B2
		public char Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.color = value;
				base.SetColorRecursive(value);
				this.CountText.color = 'w';
				this.PriceText.color = this.PriceCol;
			}
		}

		// Token: 0x060026C8 RID: 9928 RVA: 0x00111DE1 File Offset: 0x001101E1
		public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
		{
		}

		// Token: 0x040028F3 RID: 10483
		protected SHSharp::SHGUIframe Frame;

		// Token: 0x040028F4 RID: 10484
		protected SHSharp::SHGUIrect Rect;

		// Token: 0x040028F5 RID: 10485
		protected SHSharp::SHGUItext PriceText;

		// Token: 0x040028F6 RID: 10486
		protected SHSharp::SHGUItext CountText;

		// Token: 0x040028F7 RID: 10487
		protected SHSharp::SHGUItext OneIncomeText;

		// Token: 0x040028F8 RID: 10488
		private readonly string text;

		// Token: 0x040028FE RID: 10494
		private Value price = Value.Zero;

		// Token: 0x040028FF RID: 10495
		private Value count = Value.Zero;

		// Token: 0x04002900 RID: 10496
		private char priceCol;

		// Token: 0x04002901 RID: 10497
		private char backColor;

		// Token: 0x04002902 RID: 10498
		private bool active;
	}

	public class ShopHeaderView : SHSharp::SHGUIview
	{
		// Token: 0x060026D9 RID: 9945 RVA: 0x00115DF4 File Offset: 0x001141F4
		public ShopHeaderView(int posX, int posY)
		{
			this.hintOne = new SHSharp::SHGUIButton(string.Empty, posX, posY, 'w');
			this.hintTwo = new SHSharp::SHGUIButton(string.Empty, posX, posY + 1, 'w');
			base.AddSubView(this.hintOne);
			base.AddSubView(this.hintTwo);
			this.timerColor = 0f;
		}

		// Token: 0x060026DA RID: 9946 RVA: 0x00115E84 File Offset: 0x00114284
		public override void Update()
		{
			if (this.NewUpgrade || this.NewWorker)
			{
				if (this.timerColor >= this.changeColorDelay)
				{
					if (this.NewUpgrade && this.NewWorker && !this.inWorkers && !this.inUpgrades)
					{
						this.hintOne.color = ((this.hintOne.color != 'w') ? 'w' : 'r');
						this.hintTwo.color = ((this.hintTwo.color != 'w') ? 'w' : 'r');
					}
					else if ((this.inWorkers && this.NewUpgrade) || (this.inUpgrades && this.NewWorker))
					{
						this.hintOne.color = 'w';
						this.hintTwo.color = ((this.hintTwo.color != 'w') ? 'w' : 'r');
					}
					else if (this.NewWorker && !this.inWorkers && !this.inUpgrades)
					{
						this.hintOne.color = ((this.hintOne.color != 'w') ? 'w' : 'r');
						this.hintTwo.color = 'w';
					}
					else if (this.NewUpgrade && !this.inWorkers && !this.inUpgrades)
					{
						this.hintOne.color = 'w';
						this.hintTwo.color = ((this.hintTwo.color != 'w') ? 'w' : 'r');
					}
					this.timerColor = 0f;
				}
				this.timerColor += Time.unscaledDeltaTime;
			}
			else
			{
				this.hintOne.color = (this.hintOne.color = 'w');
				this.hintTwo.color = (this.hintTwo.color = 'w');
			}
			base.Update();
		}

		// Token: 0x060026DB RID: 9947 RVA: 0x001160A4 File Offset: 0x001144A4
		public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
		{
			if (x >= this.hintOne.x && x <= this.hintOne.x + this.hintOne.text.Length && y == this.hintOne.y)
			{
				this.hintOne.backColor = 'r';
				if (clicked)
				{
					if (AppTheSystem.Instance.CurrentGameState == AppTheSystem.GameState.InMessageSystem)
					{
						AppTheSystem.Instance.ChangeGameState(AppTheSystem.GameState.InShop);
						AppTheSystem.Instance.ShopView.CurrentState = ShopView.State.Workers;
					}
					else if (AppTheSystem.Instance.CurrentGameState == AppTheSystem.GameState.InShop)
					{
						AppTheSystem.Instance.ChangeGameState(AppTheSystem.GameState.InMessageSystem);
					}
				}
			}
			else
			{
				this.hintOne.backColor = '0';
			}
			if (x >= this.hintTwo.x && x <= this.hintTwo.x + this.hintTwo.text.Length && y == this.hintTwo.y)
			{
				this.hintTwo.backColor = 'r';
				if (clicked)
				{
					if (AppTheSystem.Instance.CurrentGameState == AppTheSystem.GameState.InMessageSystem)
					{
						AppTheSystem.Instance.ChangeGameState(AppTheSystem.GameState.InShop);
						AppTheSystem.Instance.ShopView.CurrentState = ShopView.State.Upgrades;
					}
					else if (AppTheSystem.Instance.CurrentGameState == AppTheSystem.GameState.InShop)
					{
						if (AppTheSystem.Instance.ShopView.CurrentState == ShopView.State.Workers)
						{
							AppTheSystem.Instance.ShopView.CurrentState = ShopView.State.Upgrades;
						}
						else
						{
							AppTheSystem.Instance.ShopView.CurrentState = ShopView.State.Workers;
						}
					}
				}
			}
			else
			{
				this.hintTwo.backColor = '0';
			}
		}

		// Token: 0x060026DC RID: 9948 RVA: 0x00116248 File Offset: 0x00114648
		public void ChangeHeader(ShopView.State currentState, bool isActive)
		{
			if (!isActive)
			{
				this.hintOne.text = this.workersHintText;
				this.hintTwo.text = this.upgradesHintText;
				this.inWorkers = false;
				this.inUpgrades = false;
			}
			else if (currentState != ShopView.State.Workers)
			{
				if (currentState == ShopView.State.Upgrades)
				{
					this.hintOne.text = this.closeHintText;
					this.hintTwo.text = this.workersHintText;
					this.inUpgrades = true;
				}
			}
			else
			{
				this.hintOne.text = this.closeHintText;
				this.hintTwo.text = this.upgradesHintText;
				this.inWorkers = true;
			}
		}

		// Token: 0x04002916 RID: 10518
		public bool NewUpgrade;

		// Token: 0x04002917 RID: 10519
		public bool NewWorker;

		// Token: 0x04002918 RID: 10520
		private SHSharp::SHGUIButton hintOne;

		// Token: 0x04002919 RID: 10521
		private SHSharp::SHGUItext hintTwo;

		// Token: 0x0400291A RID: 10522
		private string workersHintText = "[F1] Workers";

		// Token: 0x0400291B RID: 10523
		private string upgradesHintText = "[F2] Upgrades";

		// Token: 0x0400291C RID: 10524
		private string closeHintText = "[Esc] Close Shop";

		// Token: 0x0400291D RID: 10525
		private bool inUpgrades;

		// Token: 0x0400291E RID: 10526
		private bool inWorkers;

		// Token: 0x0400291F RID: 10527
		private float timerColor;

		// Token: 0x04002920 RID: 10528
		private float changeColorDelay = 0.5f;
	}

	public sealed class ShopHintView : AStoryAppsView
	{
		// Token: 0x060026DD RID: 9949 RVA: 0x00116304 File Offset: 0x00114704
		public ShopHintView(int positionX, int positionY, int width, int height) : base(positionX, positionY, width, height)
		{
			base.PositionX = positionX;
			base.PositionY = positionY;
			this.hintFrame = new SHSharp::SHGUIframe(positionX, positionY, positionX + width, positionY + height, 'w');
			this.hintRect = new SHSharp::SHGUIrect(positionX, positionY, positionX + width, positionY + height, '0', ' ', 2);
			this.hintText = new SHGUItextEx(string.Empty, positionX + 1, positionY + 1, 'w', false);
			this.Anim = new SHSharp::SHGUIsprite();
			this.AddSubView(this.hintRect);
			this.AddSubView(this.hintFrame);
			this.AddSubView(this.hintText);
			this.AddSubView(this.Anim);
		}

		// Token: 0x17000203 RID: 515
		// (get) Token: 0x060026DE RID: 9950 RVA: 0x001163B9 File Offset: 0x001147B9
		// (set) Token: 0x060026DF RID: 9951 RVA: 0x001163C1 File Offset: 0x001147C1
		public SHSharp::SHGUIsprite Anim { get; private set; }

		// Token: 0x17000204 RID: 516
		// (get) Token: 0x060026E0 RID: 9952 RVA: 0x001163CA File Offset: 0x001147CA
		// (set) Token: 0x060026E1 RID: 9953 RVA: 0x001163D2 File Offset: 0x001147D2
		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				this.text = value;
				if (this.hintText != null)
				{
					this.hintText.text = this.text;
					this.hintText.SmartBreakTextForLineLength(SharedTheSystem.Views.ShopHintViewWidth - 2);
				}
			}
		}

		// Token: 0x17000205 RID: 517
		// (get) Token: 0x060026E2 RID: 9954 RVA: 0x0011640F File Offset: 0x0011480F
		// (set) Token: 0x060026E3 RID: 9955 RVA: 0x00116417 File Offset: 0x00114817
		public bool Hidden
		{
			get
			{
				return this.hidden;
			}
			set
			{
				this.hidden = value;
				this.hintFrame.hidden = value;
				this.hintRect.hidden = value;
				this.hintText.hidden = value;
			}
		}

		// Token: 0x060026E4 RID: 9956 RVA: 0x00116444 File Offset: 0x00114844
		public override void Update()
		{
			this.hintRect.startx = base.PositionX;
			this.hintRect.starty = base.PositionY;
			this.hintRect.endx = base.PositionX + SharedTheSystem.Views.ShopHintViewWidth;
			this.hintRect.endy = base.PositionY + SharedTheSystem.Views.ShopHintViewHeight;
			base.Update();
		}

		// Token: 0x060026E5 RID: 9957 RVA: 0x001164B4 File Offset: 0x001148B4
		public void SetAnim(SHSharp::SHGUIsprite anim)
		{
			this.RemoveView(this.Anim);
			this.Anim = anim;
			this.Anim.x = base.PositionX + SharedTheSystem.Views.ShopHintViewWidth - 8;
			this.Anim.y = base.PositionY + 1;
			this.AddSubView(this.Anim);
			this.hintText.SmartBreakTextForLineLength(SharedTheSystem.Views.ShopHintViewWidth - 10);
		}

		// Token: 0x060026E6 RID: 9958 RVA: 0x0011652B File Offset: 0x0011492B
		public void RemoveAnim()
		{
			this.RemoveView(this.Anim);
		}

		// Token: 0x04002921 RID: 10529
		private SHSharp::SHGUIframe hintFrame;

		// Token: 0x04002922 RID: 10530
		private SHSharp::SHGUIrect hintRect;

		// Token: 0x04002923 RID: 10531
		private SHGUItextEx hintText;

		// Token: 0x04002925 RID: 10533
		private bool showAnim;

		// Token: 0x04002926 RID: 10534
		private string text;
	}

	public class ShopUpgradesView : ScrollView<AUpgrade>
	{
		// Token: 0x060026E7 RID: 9959 RVA: 0x0011653C File Offset: 0x0011493C
		public ShopUpgradesView(int positionX, int positionY) : base(positionX, positionY)
		{
			BotRateUpgrade item = new BotRateUpgrade(SharedTheSystem.Upgrades.BOT_RATE, positionX, positionY, '1')
			{
				BackColor = '1',
				ShowCount = false,
				HintText = SharedTheSystem.Upgrades.BotRateHint
			};
			FanRateUpgrade item2 = new FanRateUpgrade(SharedTheSystem.Upgrades.FAN_RATE, positionX, positionY, '1')
			{
				BackColor = '1',
				ShowCount = false,
				HintText = SharedTheSystem.Upgrades.FanRateHint
			};
			StreamerRateUpgrade item3 = new StreamerRateUpgrade(SharedTheSystem.Upgrades.STREAMER_RATE, positionX, positionY, '1')
			{
				BackColor = '1',
				ShowCount = false,
				HintText = SharedTheSystem.Upgrades.StreamerRateHint
			};
			FlamerRateUpgrade item4 = new FlamerRateUpgrade(SharedTheSystem.Upgrades.FLAMER_RATE, positionX, positionY, '1')
			{
				BackColor = '1',
				ShowCount = false,
				HintText = SharedTheSystem.Upgrades.FlamerRateHint
			};
			DrugDealerRateUpgrade item5 = new DrugDealerRateUpgrade(SharedTheSystem.Upgrades.DRUG_DEALER_RATE, positionX, positionY, '1')
			{
				BackColor = '1',
				ShowCount = false,
				HintText = SharedTheSystem.Upgrades.DrugDealerRateHint
			};
			WifiRateUpgrade item6 = new WifiRateUpgrade(SharedTheSystem.Upgrades.WIFI_RATE, positionX, positionY, '1')
			{
				BackColor = '1',
				ShowCount = false,
				HintText = SharedTheSystem.Upgrades.WifiRateHint
			};
			IndieDevRateUpgrade item7 = new IndieDevRateUpgrade(SharedTheSystem.Upgrades.INDIE_DEV_RATE, positionX, positionY, '1')
			{
				BackColor = '1',
				ShowCount = false,
				HintText = SharedTheSystem.Upgrades.IndieDevRateHint
			};
			MessageConvertedUpgrade item8 = new MessageConvertedUpgrade(SharedTheSystem.Upgrades.MESSAGE_CONVERTED, positionX, positionY, '1')
			{
				BackColor = '1',
				ShowCount = false,
				HintText = SharedTheSystem.Upgrades.MessageConvertedHint
			};
			MessageCpsBonusUpgrade item9 = new MessageCpsBonusUpgrade(SharedTheSystem.Upgrades.MESSAGE_CPS_BONUS, positionX, positionY, '1')
			{
				BackColor = '1',
				ShowCount = false,
				HintText = SharedTheSystem.Upgrades.MessageCpsBonusHint
			};
			HypnosisPercentUpgrade item10 = new HypnosisPercentUpgrade(SharedTheSystem.Upgrades.HYPNOSIS_UPGRADE, positionX, positionY, '1')
			{
				BackColor = '1',
				ShowCount = false,
				HintText = SharedTheSystem.Upgrades.HypnosisUpgradeHint
			};
			BonusPopupTimeUpgrade item11 = new BonusPopupTimeUpgrade(SharedTheSystem.Upgrades.POPUP_TIME, positionX, positionY, '1')
			{
				BackColor = '1',
				ShowCount = false,
				HintText = SharedTheSystem.Upgrades.PopupTimeUpgradeHint
			};
			Brainwash item12 = new Brainwash(SharedTheSystem.Upgrades.BRAINWASH, positionX, positionY, '1')
			{
				BackColor = '1',
				ShowCount = false,
				HintText = SharedTheSystem.Upgrades.BrainwashHint
			};
			base.Items = new List<AUpgrade>();
			this.buttonsToAdd = new List<AUpgrade>
			{
				item,
				item2,
				item3,
				item4,
				item5,
				item6,
				item7,
				item8,
				item9,
				item10,
				item11,
				item12
			};
			this.CheckButtonsInQueue();
		}

		// Token: 0x060026E8 RID: 9960 RVA: 0x001168A4 File Offset: 0x00114CA4
		public override void Update()
		{
			foreach (AUpgrade aupgrade in base.Items)
			{
				aupgrade.CheckPriceColor();
				aupgrade.UpdateHintText();
			}
		}

		// Token: 0x17000206 RID: 518
		// (get) Token: 0x060026E9 RID: 9961 RVA: 0x00116908 File Offset: 0x00114D08
		// (set) Token: 0x060026EA RID: 9962 RVA: 0x00116910 File Offset: 0x00114D10
		public bool NewItems { get; set; }

		// Token: 0x060026EB RID: 9963 RVA: 0x0011691C File Offset: 0x00114D1C
		public bool CheckButtonsInQueue()
		{
			for (int i = 0; i < this.buttonsToAdd.Count; i++)
			{
				AUpgrade aupgrade = this.buttonsToAdd[i];
				if (aupgrade.CanBeUsed())
				{
					base.Items.Add(aupgrade);
					this.UpdateButtonsPosition();
					base.AddSubView(aupgrade);
					if (base.Items.Count > this.MaxElements)
					{
						aupgrade.hidden = true;
					}
					this.buttonsToAdd.Remove(aupgrade);
					this.NewItems = true;
				}
			}
			for (int j = 0; j < base.Items.Count; j++)
			{
				AUpgrade aupgrade2 = base.Items[j];
				if (aupgrade2 is Brainwash && j != base.Items.Count - 1)
				{
					base.Items.Remove(aupgrade2);
					base.Items.Add(aupgrade2);
					this.UpdateButtonsPosition();
				}
				if (!aupgrade2.CanBeUsed())
				{
					this.buttonsToAdd.Add(aupgrade2);
					base.RemoveView(aupgrade2);
					base.Items.Remove(aupgrade2);
					this.UpdateButtonsPosition();
				}
			}
			return this.NewItems;
		}

		// Token: 0x060026EC RID: 9964 RVA: 0x00116A48 File Offset: 0x00114E48
		private void UpdateButtonsPosition()
		{
			for (int i = 0; i < base.Items.Count; i++)
			{
				base.Items.ElementAt(i).PositionY = this.PositionY + this.buttonWidth * i;
			}
		}

		// Token: 0x04002927 RID: 10535
		private List<AUpgrade> buttonsToAdd;

		// Token: 0x04002928 RID: 10536
		private readonly int buttonWidth = SharedTheSystem.Views.ShopButtonWidth;
	}

	public sealed class ShopView : AStoryAppsView
	{
		// Token: 0x060026ED RID: 9965 RVA: 0x00116A94 File Offset: 0x00114E94
		public ShopView(int positionX, int positionY) : base(positionX, positionY, 0, 0)
		{
			base.PositionX = positionX;
			base.PositionY = positionY;
			// Error happens below here.
			this.upgradesView = new ShopUpgradesView(base.PositionX, base.PositionY + 2);
			this.workersView = new ShopWorkersView(base.PositionX, base.PositionY + 2);
			this.viewsHint = new ShopHeaderView(base.PositionX + 1, base.PositionY);
			this.hintView = new ShopHintView(2, 7, SharedTheSystem.Views.ShopHintViewWidth, SharedTheSystem.Views.ShopHintViewHeight)
			{
				Hidden = true
			};
			this.AddSubView(this.viewsHint);
			this.AddSubView(this.hintView);
			this.separator = new SHSharp::SHGUIline(1, SHSharp::SHGUI.current.resolutionY - 2, base.PositionX - 1, false, 'w');
			this.separator.SetStyle("+|+");
			this.AddSubView(this.separator);
			this.CurrentState = ShopView.State.Workers;
			this.starting = true;
		}

		// Token: 0x17000207 RID: 519
		// (get) Token: 0x060026EE RID: 9966 RVA: 0x00116B97 File Offset: 0x00114F97
		// (set) Token: 0x060026EF RID: 9967 RVA: 0x00116B9F File Offset: 0x00114F9F
		public new bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
				if (!value)
				{
					this.RemoveView(this.upgradesView);
					this.RemoveView(this.workersView);
					this.viewsHint.ChangeHeader(this.currentState, this.active);
				}
			}
		}

		// Token: 0x17000208 RID: 520
		// (get) Token: 0x060026F0 RID: 9968 RVA: 0x00116BDD File Offset: 0x00114FDD
		// (set) Token: 0x060026F1 RID: 9969 RVA: 0x00116BE8 File Offset: 0x00114FE8
		public ShopView.State CurrentState
		{
			get
			{
				return this.currentState;
			}
			set
			{
				this.currentState = value;
				if (value == ShopView.State.Workers)
				{
					bool flag = false;
					foreach (ShopButton shopButton in this.workersView.Items)
					{
						if (!flag && shopButton.Active)
						{
							this.activeButtonIndex = this.workersView.Items.IndexOf(shopButton);
							this.workersView.CurrentPos = this.activeButtonIndex;
							flag = true;
						}
					}
					this.RemoveView(this.upgradesView);
					this.AddSubView(this.workersView);
					if (flag)
					{
						this.ActivateButton(this.workersView.Items.ElementAt(this.activeButtonIndex));
					}
					this.viewsHint.ChangeHeader(this.currentState, this.active);
				}
				else if (value == ShopView.State.Upgrades)
				{
					bool flag2 = false;
					foreach (AUpgrade aupgrade in this.upgradesView.Items)
					{
						if (!flag2 && aupgrade.Active)
						{
							this.activeButtonIndex = this.upgradesView.Items.IndexOf(aupgrade);
							this.upgradesView.CurrentPos = this.activeButtonIndex;
							flag2 = true;
						}
					}
					this.RemoveView(this.workersView);
					this.AddSubView(this.upgradesView);
					if (flag2)
					{
						this.ActivateButton(this.upgradesView.Items.ElementAt(this.activeButtonIndex));
					}
					this.viewsHint.ChangeHeader(this.currentState, this.active);
				}
				this.UpdateInstructions();
			}
		}

		// Token: 0x17000209 RID: 521
		// (get) Token: 0x060026F2 RID: 9970 RVA: 0x00116DCC File Offset: 0x001151CC
		public ShopHintView HintView
		{
			get
			{
				return this.hintView;
			}
		}

		// Token: 0x060026F3 RID: 9971 RVA: 0x00116DD4 File Offset: 0x001151D4
		public override void Update()
		{
			base.Update();
			if (!this.starting)
			{
				this.viewsHint.NewUpgrade = this.upgradesView.CheckButtonsInQueue();
				this.viewsHint.NewWorker = this.workersView.CheckButtonsInQueue();
			}
			else
			{
				this.workersView.NewItems = false;
				this.upgradesView.NewItems = false;
				this.starting = false;
			}
			ShopView.State state = this.CurrentState;
			if (state != ShopView.State.Workers)
			{
				if (state == ShopView.State.Upgrades)
				{
					if (this.activeButtonIndex > this.upgradesView.Items.Count - 1 && this.upgradesView.Items.Count > 0)
					{
						this.activeButtonIndex = this.upgradesView.Items.Count - 1;
						this.upgradesView.CurrentPos = this.activeButtonIndex;
					}
					if (this.activeButtonIndex < 0)
					{
						this.activeButtonIndex = 0;
					}
					foreach (AUpgrade button in this.upgradesView.Items)
					{
						this.ResetButtonActivation(button);
					}
				}
			}
			else
			{
				foreach (ShopButton button2 in this.workersView.Items)
				{
					this.ResetButtonActivation(button2);
				}
			}
			if (!this.Active)
			{
				AppTheSystem.Instance.APPINSTRUCTION.text = string.Empty;
				return;
			}
			ShopView.State state2 = this.CurrentState;
			if (state2 != ShopView.State.Workers)
			{
				if (state2 == ShopView.State.Upgrades)
				{
					if (this.upgradesView.Items.Count > 0 && this.upgradesView.Items.ElementAt(this.activeButtonIndex).Active)
					{
						this.ActivateButton(this.upgradesView.Items.ElementAt(this.activeButtonIndex));
					}
				}
			}
			else
			{
				this.ActivateButton(this.workersView.Items.ElementAt(this.activeButtonIndex));
			}
		}

		// Token: 0x060026F4 RID: 9972 RVA: 0x00117030 File Offset: 0x00115430
		public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
		{
			ShopView.State state = this.CurrentState;
			if (state != ShopView.State.Workers)
			{
				if (state == ShopView.State.Upgrades)
				{
					for (int i = 0; i < this.upgradesView.Items.Count; i++)
					{
						if (x >= this.upgradesView.Items[i].PositionX && x <= this.upgradesView.Items[i].PositionX + SharedTheSystem.Views.ShopButtonWidth && y >= this.upgradesView.Items[i].PositionY && y <= this.upgradesView.Items[i].PositionY + SharedTheSystem.Views.ShopButtonHeight - 1)
						{
							if (this.activeButtonIndex != i)
							{
								this.activeButtonIndex = i;
								this.upgradesView.CurrentPos = i;
							}
							if (clicked && this.upgradesView.Items[i].Button.OnActivate != null)
							{
								this.upgradesView.Items[i].Button.OnActivate();
							}
						}
					}
				}
			}
			else
			{
				for (int j = 0; j < this.workersView.Items.Count; j++)
				{
					if (x >= this.workersView.Items[j].PositionX && x <= this.workersView.Items[j].PositionX + SharedTheSystem.Views.ShopButtonWidth && y >= this.workersView.Items[j].PositionY && y <= this.workersView.Items[j].PositionY + SharedTheSystem.Views.ShopButtonHeight - 1)
					{
						if (this.activeButtonIndex != j)
						{
							this.activeButtonIndex = j;
							this.workersView.CurrentPos = j;
						}
						if (clicked && this.workersView.Items[j].Button.OnActivate != null)
						{
							this.workersView.Items[j].Button.OnActivate();
						}
					}
				}
			}
			for (int k = 0; k < this.children.Count; k++)
			{
				this.children[k].ReactToInputMouse(x, y, clicked, scroll);
			}
		}

		// Token: 0x060026F5 RID: 9973 RVA: 0x001172B0 File Offset: 0x001156B0
		public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
		{
			if (!this.Active)
			{
				return;
			}
			ShopView.State state = this.CurrentState;
			if (state != ShopView.State.Workers)
			{
				if (state == ShopView.State.Upgrades)
				{
					this.upgradesView.ReactToInputKeyboard(key);
					this.activeButtonIndex = this.upgradesView.CurrentPos;
					if (key == SHSharp::SHGUIinput.down)
					{
						this.UpdateInstructions();
					}
					if (key == SHSharp::SHGUIinput.up)
					{
						this.UpdateInstructions();
					}
					if (this.upgradesView.Items.Count == 0)
					{
						return;
					}
					if (this.upgradesView.Items.ElementAt(this.activeButtonIndex).Active)
					{
						this.upgradesView.Items.ElementAt(this.activeButtonIndex).ReactToInputKeyboard(key);
					}
					this.upgradesView.NewItems = false;
				}
			}
			else
			{
				this.workersView.ReactToInputKeyboard(key);
				this.activeButtonIndex = this.workersView.CurrentPos;
				if (key == SHSharp::SHGUIinput.down)
				{
					this.UpdateInstructions();
				}
				if (key == SHSharp::SHGUIinput.up)
				{
					this.UpdateInstructions();
				}
				if (this.workersView.Items.ElementAt(this.activeButtonIndex).Active)
				{
					this.workersView.Items.ElementAt(this.activeButtonIndex).ReactToInputKeyboard(key);
				}
				this.workersView.NewItems = false;
			}
		}

		// Token: 0x060026F6 RID: 9974 RVA: 0x00117400 File Offset: 0x00115800
		private void ActivateButton(ShopButton button)
		{
			button.BackColor = '1';
			button.Color = 'w';
			button.SetFrameColor('w');
			if (button is Brainwash)
			{
				button.Color = 'r';
				button.SetFrameColor('r');
				button.SetColorRecursive('r');
			}
			this.UpdateInstructions();
			button.Button.SetActionKey(SHSharp::SHGUIinput.enter);
		}

		// Token: 0x060026F7 RID: 9975 RVA: 0x0011745C File Offset: 0x0011585C
		private void ResetButtonActivation(ShopButton button)
		{
			button.BackColor = '0';
			button.Color = '5';
			button.SetFrameColor('1');
			button.Button.SetActionKey(SHSharp::SHGUIinput.none);
		}

		// Token: 0x060026F8 RID: 9976 RVA: 0x00117484 File Offset: 0x00115884
		private void UpdateInstructions()
		{
			ShopView.State state = this.CurrentState;
			if (state != ShopView.State.Workers)
			{
				if (state == ShopView.State.Upgrades)
				{
					if (this.upgradesView.Items.Count == 0)
					{
						this.hintView.Text = string.Empty;
						return;
					}
					this.hintView.RemoveAnim();
					this.hintView.Text = this.upgradesView.Items.ElementAt(this.activeButtonIndex).HintText;
				}
			}
			else if (!this.workersView.Items.ElementAt(this.activeButtonIndex).Active)
			{
				this.hintView.Text = "???";
				this.hintView.RemoveAnim();
			}
			else
			{
				this.hintView.Text = this.workersView.Items.ElementAt(this.activeButtonIndex).HintText;
				if (this.workersView.Items.ElementAt(this.activeButtonIndex).ShowAnim && this.workersView.Items.ElementAt(this.activeButtonIndex).Count > Value.Zero)
				{
					this.hintView.SetAnim(this.workersView.Items.ElementAt(this.activeButtonIndex).Anim);
				}
				else
				{
					this.hintView.RemoveAnim();
				}
			}
		}

		// Token: 0x060026F9 RID: 9977 RVA: 0x001175F4 File Offset: 0x001159F4
		public void ResetShopButtons()
		{
			this.RemoveView(this.workersView);
			this.RemoveView(this.upgradesView);
			this.upgradesView = new ShopUpgradesView(base.PositionX, base.PositionY + 2);
			this.workersView = new ShopWorkersView(base.PositionX, base.PositionY + 2);
		}

		// Token: 0x0400292A RID: 10538
		private bool active;

		// Token: 0x0400292B RID: 10539
		private ShopView.State currentState;

		// Token: 0x0400292C RID: 10540
		private SHSharp::SHGUIline separator;

		// Token: 0x0400292D RID: 10541
		private ShopUpgradesView upgradesView;

		// Token: 0x0400292E RID: 10542
		private ShopWorkersView workersView;

		// Token: 0x0400292F RID: 10543
		private ShopHeaderView viewsHint;

		// Token: 0x04002930 RID: 10544
		private ShopHintView hintView;

		// Token: 0x04002931 RID: 10545
		private int activeButtonIndex;

		// Token: 0x04002932 RID: 10546
		private bool starting;

		// Token: 0x02000694 RID: 1684
		public enum State
		{
			// Token: 0x04002934 RID: 10548
			Workers,
			// Token: 0x04002935 RID: 10549
			Upgrades
		}
	}

	public class ShopWorkersView : ScrollView<ShopButton>
	{
		// Token: 0x060026FA RID: 9978 RVA: 0x0011764C File Offset: 0x00115A4C
		public ShopWorkersView(int positionX, int positionY) : base(positionX, positionY)
		{
			this.buttonsToAdd = new Queue<ShopButton>();
			this.botsButton = new ShopButton("Bots", positionX, positionY, SharedTheSystem.Views.ShopButtonWidth, SharedTheSystem.Views.ShopButtonHeight, '1')
			{
				BackColor = '1',
				ShowCount = true,
				HintText = "Types messages for you",
				StartPrice = SharedTheSystem.Workers.BotStartPrice,
				Anim = new Bot(),
				ShowAnim = true
			};
			this.botsButton.Button.SetOnActivate(new Action(this.BotsButtonActivate));
			this.botsButton.Count = SharedTheSystem.Workers.GetBots();
			this.fakeAccountsButton = new ShopButton("Fake accounts", positionX, positionY + this.buttonWidth, SharedTheSystem.Views.ShopButtonWidth, SharedTheSystem.Views.ShopButtonHeight, '1')
			{
				BackColor = '1',
				ShowCount = true,
				HintText = "Extra message for every one you send",
				StartPrice = SharedTheSystem.Workers.FakeAccountStartPrice,
				ShowAnim = false
			};
			this.fakeAccountsButton.Button.SetOnActivate(new Action(this.FakeAccountsButtonActivate));
			this.fakeAccountsButton.Count = SharedTheSystem.Workers.GetFakeAccounts();
			this.buttonsToAdd.Enqueue(this.fakeAccountsButton);
			this.fansButton = new ShopButton("Fans", positionX, positionY + (this.buttonsToAdd.Count + 1) * this.buttonWidth, SharedTheSystem.Views.ShopButtonWidth, SharedTheSystem.Views.ShopButtonHeight, '1')
			{
				BackColor = '1',
				ShowCount = true,
				HintText = "Talks to people and convert them",
				StartPrice = SharedTheSystem.Workers.FanStartPrice,
				Anim = new Fan(),
				ShowAnim = true
			};
			this.fansButton.Button.SetOnActivate(new Action(this.FansButtonActivate));
			this.fansButton.Count = SharedTheSystem.Workers.GetFans();
			this.buttonsToAdd.Enqueue(this.fansButton);
			this.streamersButton = new ShopButton("Streamers", positionX, positionY + (this.buttonsToAdd.Count + 1) * this.buttonWidth, SharedTheSystem.Views.ShopButtonWidth, SharedTheSystem.Views.ShopButtonHeight, '1')
			{
				BackColor = '1',
				ShowCount = true,
				HintText = "Stream SUPERHOT online",
				StartPrice = SharedTheSystem.Workers.StreamerStartPrice,
				Anim = new Streamer(),
				ShowAnim = true
			};
			this.streamersButton.Button.SetOnActivate(new Action(this.StreamersButtonActivate));
			this.streamersButton.Count = SharedTheSystem.Workers.GetStreamers();
			this.buttonsToAdd.Enqueue(this.streamersButton);
			this.flamersButton = new ShopButton("Flamers", positionX, positionY + (this.buttonsToAdd.Count + 1) * this.buttonWidth, SharedTheSystem.Views.ShopButtonWidth, SharedTheSystem.Views.ShopButtonHeight, '1')
			{
				BackColor = '1',
				ShowCount = true,
				HintText = "Argue on social media",
				StartPrice = SharedTheSystem.Workers.FlamerStartPrice,
				Anim = new Flamer(),
				ShowAnim = true
			};
			this.flamersButton.Button.SetOnActivate(new Action(this.FlamersButtonActivate));
			this.flamersButton.Count = SharedTheSystem.Workers.GetFlamers();
			this.buttonsToAdd.Enqueue(this.flamersButton);
			this.drugDealerButton = new ShopButton("Drug dealer", positionX, positionY + (this.buttonsToAdd.Count + 1) * this.buttonWidth, SharedTheSystem.Views.ShopButtonWidth, SharedTheSystem.Views.ShopButtonHeight, '1')
			{
				BackColor = '1',
				ShowCount = true,
				HintText = "Sells SUPERHOT drugs on street",
				StartPrice = SharedTheSystem.Workers.DrugDealerStartPrice,
				Anim = new Drug(),
				ShowAnim = true
			};
			this.drugDealerButton.Button.SetOnActivate(new Action(this.DrugDealersButtonActivate));
			this.drugDealerButton.Count = SharedTheSystem.Workers.GetDrugDealers();
			this.buttonsToAdd.Enqueue(this.drugDealerButton);
			this.wifisButton = new ShopButton("Wifi Control", positionX, positionY + (this.buttonsToAdd.Count + 1) * this.buttonWidth, SharedTheSystem.Views.ShopButtonWidth, SharedTheSystem.Views.ShopButtonHeight, '1')
			{
				BackColor = '1',
				ShowCount = true,
				HintText = "Control minds via wifi signal",
				StartPrice = SharedTheSystem.Workers.WifiStartPrice,
				Anim = new Wifi(),
				ShowAnim = true
			};
			this.wifisButton.Button.SetOnActivate(new Action(this.WifisButtonActivate));
			this.wifisButton.Count = SharedTheSystem.Workers.GetWifis();
			this.buttonsToAdd.Enqueue(this.wifisButton);
			this.indieDevsButton = new ShopButton("Indie Dev", positionX, positionY + (this.buttonsToAdd.Count + 1) * this.buttonWidth, SharedTheSystem.Views.ShopButtonWidth, SharedTheSystem.Views.ShopButtonHeight, '1')
			{
				BackColor = '1',
				ShowCount = true,
				HintText = "Hire indies to create SUPERHOT clones",
				StartPrice = SharedTheSystem.Workers.IndieDevStartPrice,
				Anim = new IndieDev(),
				ShowAnim = true
			};
			this.indieDevsButton.Button.SetOnActivate(new Action(this.IndieDevsButtonActivate));
			this.indieDevsButton.Count = SharedTheSystem.Workers.GetIndieDevs();
			this.buttonsToAdd.Enqueue(this.indieDevsButton);
			base.Items = new List<ShopButton>
			{
				this.botsButton
			};
			base.AddSubView(this.botsButton);
			this.CheckButtonsInQueue();
			this.PrepareButtonPrices();
		}

		// Token: 0x060026FB RID: 9979 RVA: 0x00117C60 File Offset: 0x00116060
		public override void Update()
		{
			base.Update();
			this.fakeAccountsButton.PriceCol = ((!(this.accountPrice > SharedTheSystem.General.GetHotCoins())) ? 'w' : 'r');
			this.fakeAccountsButton.Count = SharedTheSystem.Workers.GetFakeAccounts();
			this.botsButton.PriceCol = ((!(this.botPrice > SharedTheSystem.General.GetHotCoins())) ? 'w' : 'r');
			this.botsButton.Count = SharedTheSystem.Workers.GetBots();
			if (this.botsButton.Count > Value.Zero)
			{
				this.botsButton.HintText = "Types messages for you" + SharedTheSystem.GetHintStats(IncomeController.Instance.OneBotIncomePerSec(), IncomeController.Instance.TotalBotIncomePerSec());
			}
			this.fansButton.PriceCol = ((!(this.fanPrice > SharedTheSystem.General.GetHotCoins())) ? 'w' : 'r');
			this.fansButton.Count = SharedTheSystem.Workers.GetFans();
			if (this.fansButton.Count > Value.Zero)
			{
				this.fansButton.HintText = "Talks to people and convert them" + SharedTheSystem.GetHintStats(IncomeController.Instance.OneFanIncomePerSec(), IncomeController.Instance.TotalFanIncomePerSec());
			}
			this.streamersButton.PriceCol = ((!(this.streamerPrice > SharedTheSystem.General.GetHotCoins())) ? 'w' : 'r');
			this.streamersButton.Count = SharedTheSystem.Workers.GetStreamers();
			if (this.streamersButton.Count > Value.Zero)
			{
				this.streamersButton.HintText = "Stream SUPERHOT online" + SharedTheSystem.GetHintStats(IncomeController.Instance.OneStreamerIncomePerSec(), IncomeController.Instance.TotalStreamerIncomePerSec());
			}
			this.flamersButton.PriceCol = ((!(this.flamerPrice > SharedTheSystem.General.GetHotCoins())) ? 'w' : 'r');
			this.flamersButton.Count = SharedTheSystem.Workers.GetFlamers();
			if (this.flamersButton.Count > Value.Zero)
			{
				this.flamersButton.HintText = "Argue on social media" + SharedTheSystem.GetHintStats(IncomeController.Instance.OneFlamerIncomePerSec(), IncomeController.Instance.TotalFlamerIncomePerSec());
			}
			this.drugDealerButton.PriceCol = ((!(this.drugDealerPrice > SharedTheSystem.General.GetHotCoins())) ? 'w' : 'r');
			this.drugDealerButton.Count = SharedTheSystem.Workers.GetDrugDealers();
			if (this.drugDealerButton.Count > Value.Zero)
			{
				this.drugDealerButton.HintText = "Sells SUPERHOT drugs on street" + SharedTheSystem.GetHintStats(IncomeController.Instance.OneDrugDealerIncomePerSec(), IncomeController.Instance.TotalDrugDealerIncomePerSec());
			}
			this.wifisButton.PriceCol = ((!(this.wifiPrice > SharedTheSystem.General.GetHotCoins())) ? 'w' : 'r');
			this.wifisButton.Count = SharedTheSystem.Workers.GetWifis();
			if (this.wifisButton.Count > Value.Zero)
			{
				this.wifisButton.HintText = "Control minds via wifi signal" + SharedTheSystem.GetHintStats(IncomeController.Instance.OneWifiIncomePerSec(), IncomeController.Instance.TotalWifiIncomePerSec());
			}
			this.indieDevsButton.PriceCol = ((!(this.indieDevPrice > SharedTheSystem.General.GetHotCoins())) ? 'w' : 'r');
			this.indieDevsButton.Count = SharedTheSystem.Workers.GetIndieDevs();
			if (this.indieDevsButton.Count > Value.Zero)
			{
				this.indieDevsButton.HintText = "Hire indies to create SUPERHOT clones" + SharedTheSystem.GetHintStats(IncomeController.Instance.OneIndieDevIncomePerSec(), IncomeController.Instance.TotalIndieDevIncomePerSec());
			}
			SharedTheSystem.Workers.SaveShopPrices(this.accountPrice, this.botPrice, this.fanPrice, this.streamerPrice, this.flamerPrice, this.drugDealerPrice, this.wifiPrice, this.indieDevPrice);
		}

		// Token: 0x060026FC RID: 9980 RVA: 0x001180C0 File Offset: 0x001164C0
		public void PrepareButtonPrices()
		{
			SharedTheSystem.Workers.GetShopPrices(out this.accountPrice, out this.botPrice, out this.fanPrice, out this.streamerPrice, out this.flamerPrice, out this.drugDealerPrice, out this.wifiPrice, out this.indieDevPrice);
			this.fakeAccountsButton.Price = this.accountPrice;
			this.botsButton.Price = this.botPrice;
			this.fansButton.Price = this.fanPrice;
			this.streamersButton.Price = this.streamerPrice;
			this.flamersButton.Price = this.flamerPrice;
			this.drugDealerButton.Price = this.drugDealerPrice;
			this.wifisButton.Price = this.wifiPrice;
			this.indieDevsButton.Price = this.indieDevPrice;
		}

		// Token: 0x060026FD RID: 9981 RVA: 0x00118190 File Offset: 0x00116590
		private void FakeAccountsButtonActivate()
		{
			if (this.accountPrice > SharedTheSystem.General.GetHotCoins())
			{
				return;
			}
			SharedTheSystem.Workers.AddFakeAccounts(Value.One);
			AppTheSystem.Instance.ActionController.SpendHotCoins(this.accountPrice);
			this.UpdateAccountPrice();
		}

		// Token: 0x060026FE RID: 9982 RVA: 0x001181E4 File Offset: 0x001165E4
		private void UpdateAccountPrice()
		{
			this.accountPrice = SharedTheSystem.Workers.CalculatePrice(SharedTheSystem.Workers.FakeAccountStartPrice, SharedTheSystem.Workers.PriceIncreaseRate, SharedTheSystem.Workers.GetFakeAccounts() + Value.One);
			this.fakeAccountsButton.Price = this.accountPrice;
		}

		// Token: 0x060026FF RID: 9983 RVA: 0x0011823C File Offset: 0x0011663C
		private void BotsButtonActivate()
		{
			if (this.botPrice > SharedTheSystem.General.GetHotCoins())
			{
				return;
			}
			SharedTheSystem.Workers.AddBots(Value.One);
			AppTheSystem.Instance.ActionController.SpendHotCoins(this.botPrice);
			this.UpdateBotPrice();
		}

		// Token: 0x06002700 RID: 9984 RVA: 0x00118290 File Offset: 0x00116690
		private void UpdateBotPrice()
		{
			this.botPrice = SharedTheSystem.Workers.CalculatePrice(SharedTheSystem.Workers.BotStartPrice, SharedTheSystem.Workers.PriceIncreaseRate, SharedTheSystem.Workers.GetBots() + Value.One);
			this.botsButton.Price = this.botPrice;
		}

		// Token: 0x06002701 RID: 9985 RVA: 0x001182E8 File Offset: 0x001166E8
		private void FansButtonActivate()
		{
			if (this.fanPrice > SharedTheSystem.General.GetHotCoins())
			{
				return;
			}
			SharedTheSystem.Workers.AddFans(Value.One);
			AppTheSystem.Instance.ActionController.SpendHotCoins(this.fanPrice);
			this.UpdateFanPrice();
		}

		// Token: 0x06002702 RID: 9986 RVA: 0x0011833C File Offset: 0x0011673C
		private void UpdateFanPrice()
		{
			this.fanPrice = SharedTheSystem.Workers.CalculatePrice(SharedTheSystem.Workers.FanStartPrice, SharedTheSystem.Workers.PriceIncreaseRate, SharedTheSystem.Workers.GetFans() + Value.One);
			this.fansButton.Price = this.fanPrice;
		}

		// Token: 0x06002703 RID: 9987 RVA: 0x00118394 File Offset: 0x00116794
		private void StreamersButtonActivate()
		{
			if (this.streamerPrice > SharedTheSystem.General.GetHotCoins())
			{
				return;
			}
			SharedTheSystem.Workers.AddStreamers(Value.One);
			AppTheSystem.Instance.ActionController.SpendHotCoins(this.streamerPrice);
			this.UpdateStreamerPrice();
		}

		// Token: 0x06002704 RID: 9988 RVA: 0x001183E8 File Offset: 0x001167E8
		private void UpdateStreamerPrice()
		{
			this.streamerPrice = SharedTheSystem.Workers.CalculatePrice(SharedTheSystem.Workers.StreamerStartPrice, SharedTheSystem.Workers.PriceIncreaseRate, SharedTheSystem.Workers.GetStreamers() + Value.One);
			this.streamersButton.Price = this.streamerPrice;
		}

		// Token: 0x06002705 RID: 9989 RVA: 0x00118440 File Offset: 0x00116840
		private void FlamersButtonActivate()
		{
			if (this.flamerPrice > SharedTheSystem.General.GetHotCoins())
			{
				return;
			}
			SharedTheSystem.Workers.AddFlamer(Value.One);
			AppTheSystem.Instance.ActionController.SpendHotCoins(this.flamerPrice);
			this.UpdateFlamerPrice();
		}

		// Token: 0x06002706 RID: 9990 RVA: 0x00118494 File Offset: 0x00116894
		private void UpdateFlamerPrice()
		{
			this.flamerPrice = SharedTheSystem.Workers.CalculatePrice(SharedTheSystem.Workers.FlamerStartPrice, SharedTheSystem.Workers.PriceIncreaseRate, SharedTheSystem.Workers.GetFlamers() + Value.One);
			this.flamersButton.Price = this.flamerPrice;
		}

		// Token: 0x06002707 RID: 9991 RVA: 0x001184EC File Offset: 0x001168EC
		private void DrugDealersButtonActivate()
		{
			if (this.drugDealerPrice > SharedTheSystem.General.GetHotCoins())
			{
				return;
			}
			SharedTheSystem.Workers.AddDrugDealers(Value.One);
			AppTheSystem.Instance.ActionController.SpendHotCoins(this.drugDealerPrice);
			this.UpdateDrugDealerPrice();
		}

		// Token: 0x06002708 RID: 9992 RVA: 0x00118540 File Offset: 0x00116940
		private void UpdateDrugDealerPrice()
		{
			this.drugDealerPrice = SharedTheSystem.Workers.CalculatePrice(SharedTheSystem.Workers.DrugDealerStartPrice, SharedTheSystem.Workers.PriceIncreaseRate, SharedTheSystem.Workers.GetDrugDealers() + Value.One);
			this.drugDealerButton.Price = this.drugDealerPrice;
		}

		// Token: 0x06002709 RID: 9993 RVA: 0x00118598 File Offset: 0x00116998
		private void WifisButtonActivate()
		{
			if (this.wifiPrice > SharedTheSystem.General.GetHotCoins())
			{
				return;
			}
			SharedTheSystem.Workers.AddWifis(Value.One);
			AppTheSystem.Instance.ActionController.SpendHotCoins(this.wifiPrice);
			this.UpdateWifiPrice();
		}

		// Token: 0x0600270A RID: 9994 RVA: 0x001185EC File Offset: 0x001169EC
		private void UpdateWifiPrice()
		{
			this.wifiPrice = SharedTheSystem.Workers.CalculatePrice(SharedTheSystem.Workers.WifiStartPrice, SharedTheSystem.Workers.PriceIncreaseRate, SharedTheSystem.Workers.GetWifis() + Value.One);
			this.wifisButton.Price = this.wifiPrice;
		}

		// Token: 0x0600270B RID: 9995 RVA: 0x00118644 File Offset: 0x00116A44
		private void IndieDevsButtonActivate()
		{
			if (this.indieDevPrice > SharedTheSystem.General.GetHotCoins())
			{
				return;
			}
			SharedTheSystem.Workers.AddIndieDevs(Value.One);
			AppTheSystem.Instance.ActionController.SpendHotCoins(this.indieDevPrice);
			this.UpdateIndieDevPrice();
		}

		// Token: 0x0600270C RID: 9996 RVA: 0x00118698 File Offset: 0x00116A98
		private void UpdateIndieDevPrice()
		{
			this.indieDevPrice = SharedTheSystem.Workers.CalculatePrice(SharedTheSystem.Workers.IndieDevStartPrice, SharedTheSystem.Workers.PriceIncreaseRate, SharedTheSystem.Workers.GetIndieDevs() + Value.One);
			this.indieDevsButton.Price = this.indieDevPrice;
		}

		// Token: 0x1700020A RID: 522
		// (get) Token: 0x0600270D RID: 9997 RVA: 0x001186EE File Offset: 0x00116AEE
		// (set) Token: 0x0600270E RID: 9998 RVA: 0x001186F6 File Offset: 0x00116AF6
		public bool NewItems { get; set; }

		// Token: 0x0600270F RID: 9999 RVA: 0x00118700 File Offset: 0x00116B00
		public bool CheckButtonsInQueue()
		{
			for (int i = 0; i < base.Items.Count; i++)
			{
				ShopButton shopButton = base.Items[i];
				if (!shopButton.Active)
				{
					if (shopButton.StartPrice <= SharedTheSystem.General.GetTotalHotCoinsEarned())
					{
						shopButton.Active = true;
						if (this.buttonsToAdd.Count > 0)
						{
							ShopButton shopButton2 = this.buttonsToAdd.Dequeue();
							base.Items.Add(shopButton2);
							base.AddSubView(shopButton2);
							if (base.Items.Count > this.MaxElements)
							{
								shopButton2.hidden = true;
							}
							this.NewItems = true;
						}
					}
				}
			}
			return this.NewItems;
		}

		// Token: 0x04002936 RID: 10550
		private Queue<ShopButton> buttonsToAdd;

		// Token: 0x04002937 RID: 10551
		private readonly int buttonWidth = SharedTheSystem.Views.ShopButtonHeight;

		// Token: 0x04002938 RID: 10552
		private ShopButton fakeAccountsButton;

		// Token: 0x04002939 RID: 10553
		private Value accountPrice;

		// Token: 0x0400293A RID: 10554
		private ShopButton botsButton;

		// Token: 0x0400293B RID: 10555
		private Value botPrice;

		// Token: 0x0400293C RID: 10556
		private ShopButton fansButton;

		// Token: 0x0400293D RID: 10557
		private Value fanPrice;

		// Token: 0x0400293E RID: 10558
		private ShopButton streamersButton;

		// Token: 0x0400293F RID: 10559
		private Value streamerPrice;

		// Token: 0x04002940 RID: 10560
		private ShopButton flamersButton;

		// Token: 0x04002941 RID: 10561
		private Value flamerPrice;

		// Token: 0x04002942 RID: 10562
		private ShopButton drugDealerButton;

		// Token: 0x04002943 RID: 10563
		private Value drugDealerPrice;

		// Token: 0x04002944 RID: 10564
		private ShopButton wifisButton;

		// Token: 0x04002945 RID: 10565
		private Value wifiPrice;

		// Token: 0x04002946 RID: 10566
		private ShopButton indieDevsButton;

		// Token: 0x04002947 RID: 10567
		private Value indieDevPrice;
	}

	public class Streamer : SHSharp::SHGUIsprite
	{
		// Token: 0x06002505 RID: 9477 RVA: 0x0010CF10 File Offset: 0x0010B310
		public Streamer()
		{
			MCDCom.AddFrameFromStr(this, MCDCom.AssetToText(MCDCom.GetAssetName("IdleGame_Streamer.txt")), 3);
			//base.AddFramesFromFile(StoryAppsResources.IdleGameStreamer, 3, false);
			Value streamers = SharedTheSystem.Workers.GetStreamers();
			if (streamers == Value.Zero)
			{
				this.currentFrame = 0;
				this.currentAnimationFrame = Streamer.AnimationEnum.NoStreamers;
			}
			else
			{
				this.currentFrame = 1;
				this.currentAnimationFrame = Streamer.AnimationEnum.Type1;
			}
		}

		// Token: 0x06002506 RID: 9478 RVA: 0x0010CF80 File Offset: 0x0010B380
		public override void Update()
		{
			base.Update();
			if (this.animationTimer >= this.animationTime)
			{
				switch (this.currentAnimationFrame)
				{
					case Streamer.AnimationEnum.NoStreamers:
						if (SharedTheSystem.Workers.GetStreamers() > Value.Zero)
						{
							this.currentAnimationFrame = Streamer.AnimationEnum.Type1;
							this.currentFrame = 1;
						}
						break;
					case Streamer.AnimationEnum.Type1:
						this.currentFrame = 2;
						this.currentAnimationFrame = Streamer.AnimationEnum.Type2;
						break;
					case Streamer.AnimationEnum.Type2:
						this.currentFrame = 1;
						this.currentAnimationFrame = Streamer.AnimationEnum.Type1;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				this.animationTimer = 0f;
			}
			else
			{
				this.animationTimer += Time.unscaledDeltaTime;
			}
		}

		// Token: 0x0400272C RID: 10028
		private float animationTimer;

		// Token: 0x0400272D RID: 10029
		private readonly float animationTime = SharedTheSystem.Views.StreamerAnimationTime;

		// Token: 0x0400272E RID: 10030
		private Streamer.AnimationEnum currentAnimationFrame;

		// Token: 0x02000667 RID: 1639
		private enum AnimationEnum
		{
			// Token: 0x04002730 RID: 10032
			NoStreamers,
			// Token: 0x04002731 RID: 10033
			Type1,
			// Token: 0x04002732 RID: 10034
			Type2
		}
	}

	public class StreamerRateUpgrade : AUpgrade
	{
		// Token: 0x0600265A RID: 9818 RVA: 0x00112968 File Offset: 0x00110D68
		public StreamerRateUpgrade(string text, int startX, int startY, char color) : base(text, startX, startY, color)
		{
			base.UpgradeType = SharedUpgrades.UpgradeType.Streamer;
			base.StartPrice = SharedTheSystem.Upgrades.StreamerRateUpgradeStartPrice;
			Value price;
			Value tier;
			SharedTheSystem.Upgrades.GetStreamerRateUpgrade(out price, out tier);
			base.Price = price;
			base.Tier = tier;
		}

		// Token: 0x0600265B RID: 9819 RVA: 0x001129B3 File Offset: 0x00110DB3
		public override void ActivateUpgrade()
		{
			if (base.CanBeBought())
			{
				base.Buy();
				SharedTheSystem.Upgrades.IncreaseRateMultiplier(base.UpgradeType);
				base.UpdatePriceAndTier();
				SharedTheSystem.Upgrades.SaveStreamerRateUpgrade(base.Price, base.Tier);
			}
		}

		// Token: 0x0600265C RID: 9820 RVA: 0x001129F2 File Offset: 0x00110DF2
		public override bool CanBeUsed()
		{
			return SharedTheSystem.Workers.GetStreamers() >= this.GetRequiredWorkers();
		}

		// Token: 0x0600265D RID: 9821 RVA: 0x00112A09 File Offset: 0x00110E09
		public override void UpdateHintText()
		{
			base.HintText = SharedTheSystem.Upgrades.StreamerRateHint + SharedTheSystem.GetUpgradesHintStats(IncomeController.Instance.TotalStreamerIncomePerSec());
			base.UpdateHintText();
		}
	}

	[Serializable]
	public class Value
	{
		// Token: 0x0600260B RID: 9739 RVA: 0x0011112F File Offset: 0x0010F52F
		public Value(int counter, float fraction)
		{
			this.Counter = counter;
			this.Fraction = fraction;
		}

		// Token: 0x0600260C RID: 9740 RVA: 0x00111145 File Offset: 0x0010F545
		public Value(float fraction)
		{
			this.Fraction = fraction;
		}

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x0600260D RID: 9741 RVA: 0x00111154 File Offset: 0x0010F554
		// (set) Token: 0x0600260E RID: 9742 RVA: 0x0011115C File Offset: 0x0010F55C
		public int Counter
		{
			get
			{
				return this.counter;
			}
			set
			{
				this.counter = Mathf.Clamp(value, 0, int.MaxValue);
			}
		}

		// Token: 0x170001EA RID: 490
		// (get) Token: 0x0600260F RID: 9743 RVA: 0x00111170 File Offset: 0x0010F570
		// (set) Token: 0x06002610 RID: 9744 RVA: 0x00111178 File Offset: 0x0010F578
		public float Fraction
		{
			get
			{
				return this.fraction;
			}
			set
			{
				float num = value;
				while (Mathf.Abs(num) >= 1000f)
				{
					this.Counter++;
					num /= 1000f;
				}
				while (Mathf.Abs(num) < 1f && this.Counter > 0)
				{
					this.Counter--;
					num *= 1000f;
				}
				this.fraction = num;
			}
		}

		// Token: 0x06002611 RID: 9745 RVA: 0x001111F0 File Offset: 0x0010F5F0
		public static void CheckOperators(Value v1, Value v2)
		{
			/*\
			 * SHSharp::SHDebug.Log(string.Format("{0} + {1} = {2}", v1, v2, v1 + v2), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);
			SHSharp::SHDebug.Log(string.Format("{0} - {1} = {2}", v1, v2, v1 - v2), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);
			SHSharp::SHDebug.Log(string.Format("{0} * {1} = {2}", v1, v2, v1 * v2), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);
			SHSharp::SHDebug.Log(string.Format("{0} / {1} = {2}", v1, v2, v1 / v2), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);
			SHSharp::SHDebug.Log(string.Format("{0} == {1} : {2}", v1, v2, v1 == v2), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);
			SHSharp::SHDebug.Log(string.Format("{0} != {1} : {2}", v1, v2, v1 != v2), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);
			SHSharp::SHDebug.Log(string.Format("{0} < {1} : {2}", v1, v2, v1 < v2), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);
			SHSharp::SHDebug.Log(string.Format("{0} > {1} : {2}", v1, v2, v1 > v2), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);
			SHSharp::SHDebug.Log(string.Format("{0} <= {1} : {2}", v1, v2, v1 <= v2), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);
			SHSharp::SHDebug.Log(string.Format("{0} >= {1} : {2}", v1, v2, v1 >= v2), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);
			string format = "{0}++";
			Value value = v1;
			v1 = ++value;
			SHSharp::SHDebug.Log(string.Format(format, value), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);
			SHSharp::SHDebug.Log(string.Format("++{0}", v1 = ++v1), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);
			string format2 = "{0}--";
			Value value2 = v2;
			v2 = --value2;
			SHSharp::SHDebug.Log(string.Format(format2, value2), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);
			SHSharp::SHDebug.Log(string.Format("--{0}", v2 = --v2), SHSharp::LogPriority.Zero, SHSharp::LogCategory.Other);*/
		}

		// Token: 0x06002612 RID: 9746 RVA: 0x00111398 File Offset: 0x0010F798
		public static Value operator +(Value v1, Value v2)
		{
			int num = Mathf.Abs(v1.Counter - v2.Counter);
			int num2;
			float num3;
			if (v1.Counter == v2.Counter)
			{
				num2 = v1.Counter;
				num3 = v1.Fraction + v2.Fraction;
			}
			else if (v1.Counter > v2.Counter)
			{
				num2 = v1.Counter;
				float num4 = v1.Fraction * Mathf.Pow(1000f, (float)num);
				num3 = num4 + v2.Fraction;
				num3 /= Mathf.Pow(1000f, (float)num);
			}
			else
			{
				num2 = v2.Counter;
				float num4 = v2.Fraction * Mathf.Pow(1000f, (float)num);
				num3 = num4 + v1.Fraction;
				num3 /= Mathf.Pow(1000f, (float)num);
			}
			return new Value(num2, num3);
		}

		// Token: 0x06002613 RID: 9747 RVA: 0x00111468 File Offset: 0x0010F868
		public static Value operator -(Value v1, Value v2)
		{
			int num = Mathf.Abs(v1.Counter - v2.Counter);
			int num2;
			float num3;
			if (v1.Counter == v2.Counter)
			{
				num2 = v1.Counter;
				num3 = v1.Fraction - v2.Fraction;
			}
			else if (v1.Counter > v2.Counter)
			{
				num2 = v1.Counter;
				float num4 = v1.Fraction * Mathf.Pow(1000f, (float)num);
				num3 = num4 - v2.Fraction;
				num3 /= Mathf.Pow(1000f, (float)num);
			}
			else
			{
				num2 = 0;
				num3 = 0f;
			}
			return new Value(num2, num3);
		}

		// Token: 0x06002614 RID: 9748 RVA: 0x0011150B File Offset: 0x0010F90B
		public static Value operator -(Value v1)
		{
			return new Value(v1.Counter, -v1.Fraction);
		}

		// Token: 0x06002615 RID: 9749 RVA: 0x00111520 File Offset: 0x0010F920
		public static Value operator *(Value v1, Value v2)
		{
			int num = v1.Counter + v2.Counter;
			float num2 = v1.Fraction * v2.Fraction;
			return new Value(num, num2);
		}

		// Token: 0x06002616 RID: 9750 RVA: 0x00111550 File Offset: 0x0010F950
		public static Value operator /(Value v1, Value v2)
		{
			int num = v1.Counter - v2.Counter;
			float num2 = v1.Fraction / v2.Fraction;
			return new Value(num, num2);
		}

		// Token: 0x06002617 RID: 9751 RVA: 0x00111580 File Offset: 0x0010F980
		public static Value Power(Value baseValue, Value exponentValue)
		{
			if (exponentValue.Counter != 0)
			{
				throw new ArgumentException("the Value counter must = 0");
			}
			int num = (int)((float)baseValue.Counter * exponentValue.Fraction);
			float num2 = Mathf.Pow(baseValue.Fraction, exponentValue.Fraction);
			return new Value(num, num2);
		}

		// Token: 0x06002618 RID: 9752 RVA: 0x001115CC File Offset: 0x0010F9CC
		public static Value operator ++(Value v1)
		{
			v1 += Value.One;
			return v1;
		}

		// Token: 0x06002619 RID: 9753 RVA: 0x001115DC File Offset: 0x0010F9DC
		public static Value operator --(Value v1)
		{
			v1 -= Value.One;
			return v1;
		}

        // Token: 0x0600261A RID: 9754 RVA: 0x001115EC File Offset: 0x0010F9EC
        public static bool operator !=(Value v1, Value v2)
        {
            return !(v1 == v2);
        }

        public static bool operator ==(Value v1, Value v2)
        {
            // Check if both are null
            if (object.ReferenceEquals(v1, v2))
            {
                return true;
            }

            // Check if one of them is null
            if (v1 is null || v2 is null)
            {
                return false;
            }

            // Check the equality of properties if both are not null
            return v1.Counter == v2.Counter && v1.Fraction == v2.Fraction;
        }

        // Token: 0x0600261C RID: 9756 RVA: 0x00111638 File Offset: 0x0010FA38
        public static bool operator <(Value v1, Value v2)
		{
			return v1.Counter < v2.Counter || (v1.Counter <= v2.Counter && v1.Fraction < v2.Fraction);
		}

		// Token: 0x0600261D RID: 9757 RVA: 0x0011166E File Offset: 0x0010FA6E
		public static bool operator >(Value v1, Value v2)
		{
			return v1.Counter > v2.Counter || (v1.Counter >= v2.Counter && v1.Fraction > v2.Fraction);
		}

		// Token: 0x0600261E RID: 9758 RVA: 0x001116A4 File Offset: 0x0010FAA4
		public static bool operator <=(Value v1, Value v2)
		{
			return v1 < v2 || v1 == v2;
		}

		// Token: 0x0600261F RID: 9759 RVA: 0x001116BC File Offset: 0x0010FABC
		public static bool operator >=(Value v1, Value v2)
		{
			return v1 > v2 || v1 == v2;
		}

		// Token: 0x06002620 RID: 9760 RVA: 0x001116D4 File Offset: 0x0010FAD4
		public override string ToString()
		{
			if (this.Counter > 0)
			{
				if (this.Counter > NumbersEndings.Instance.Endings.Count - 1)
				{
					NumbersEndings.AddEnding();
				}
				return string.Format("{0:##0.##}{1}", this.Fraction, NumbersEndings.Instance.Endings[this.Counter - 1]);
			}
			return string.Format("{0:0.##}", this.Fraction);
		}

		// Token: 0x06002621 RID: 9761 RVA: 0x00111750 File Offset: 0x0010FB50
		public override int GetHashCode()
		{
			return this.Counter * 397 ^ this.Fraction.GetHashCode();
		}

		// Token: 0x06002622 RID: 9762 RVA: 0x0011177E File Offset: 0x0010FB7E
		public override bool Equals(object obj)
		{
			return this == (Value)obj;
		}

		// Token: 0x06002623 RID: 9763 RVA: 0x0011178C File Offset: 0x0010FB8C
		protected bool Equals(Value other)
		{
			return this.counter == other.counter && this.fraction.Equals(other.fraction);
		}

		// Token: 0x0400286C RID: 10348
		[SerializeField]
		private int counter;

		// Token: 0x0400286D RID: 10349
		[SerializeField]
		private float fraction;

		// Token: 0x0400286E RID: 10350
		public static readonly Value Zero = new Value(0, 0f);

		// Token: 0x0400286F RID: 10351
		public static readonly Value One = new Value(0, 1f);
	}

	public class VrBrainwashController : SHSharp::SHGUIview
	{
		// Token: 0x06002682 RID: 9858 RVA: 0x00113D30 File Offset: 0x00112130
		public VrBrainwashController()
		{
			this.currentState = VrBrainwashController.CurrentState.Prepare;
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x06002683 RID: 9859 RVA: 0x00113E05 File Offset: 0x00112205
		// (set) Token: 0x06002684 RID: 9860 RVA: 0x00113E0D File Offset: 0x0011220D
		public bool Active { get; set; }

		// Token: 0x06002685 RID: 9861 RVA: 0x00113E18 File Offset: 0x00112218
		public override void Update()
		{
			if (!this.Active)
			{
				return;
			}
			base.Update();
			switch (this.currentState)
			{
				case VrBrainwashController.CurrentState.Prepare:
					this.targets.Clear();
					this.GenerateTargets();
					AppTheSystem.Instance.ChangeGameState(AppTheSystem.GameState.Brainwash);
					this.currentState = VrBrainwashController.CurrentState.ViewsGroup;
					break;
				case VrBrainwashController.CurrentState.ViewsGroup:
					if (this.viewMoveTimer >= this.viewMoveTime)
					{
						this.currentState = VrBrainwashController.CurrentState.TransitionPrepare;
						this.viewMoveTimer = 0f;
					}
					else
					{
						this.MoveViews();
						this.BlinkViews();
						this.ChangeTargets();
						this.viewMoveTimer += Time.unscaledDeltaTime;
					}
					break;
				case VrBrainwashController.CurrentState.TransitionPrepare:
					{
						this.transitionArray = new int[SHSharp::SHGUI.current.resolutionX, SHSharp::SHGUI.current.resolutionY];
						this.transitionArray[SHSharp::SHGUI.current.resolutionX / 2, SHSharp::SHGUI.current.resolutionY / 2] = 1;
						this.transitionSymbols.Clear();
						SHSharp::SHGUItext SHGUItext = new SHSharp::SHGUItext(this.transitionSymbol, SHSharp::SHGUI.current.resolutionX / 2, SHSharp::SHGUI.current.resolutionY / 2, 'w', false);
						this.transitionSymbols.Add(SHGUItext);
						base.AddSubView(SHGUItext);
						this.transitionTimer = 0f;
						this.transitionTime = UnityEngine.Random.Range(this.transitionTimeMin, this.transitionTimeMax);
						this.currentState = VrBrainwashController.CurrentState.Transition;
						break;
					}
				case VrBrainwashController.CurrentState.Transition:
					{
						bool flag = true;
						if (!this.ending)
						{
							this.MoveViews();
							this.ChangeTargets();
						}
						this.BlinkViews();
						for (int i = 0; i < this.transitionArray.GetLength(0); i++)
						{
							for (int j = 0; j < this.transitionArray.GetLength(1); j++)
							{
								if (this.transitionArray[i, j] != 1)
								{
									flag = false;
								}
							}
						}
						if (flag)
						{
							if (this.transitionEndTimer >= this.transitionEndTime)
							{
								this.currentState = (this.ending ? VrBrainwashController.CurrentState.Return : VrBrainwashController.CurrentState.PrimitivesPrepare);
								this.transitionEndTimer = 0f;
							}
							this.transitionEndTimer += Time.unscaledDeltaTime;
						}
						else if (this.transitionTimer >= this.transitionTime)
						{
							for (int k = 0; k < this.transitionArray.GetLength(0); k++)
							{
								for (int l = 0; l < this.transitionArray.GetLength(1); l++)
								{
									if (this.transitionArray[k, l] == 1)
									{
										this.CalculateSymbolPlacement(k, l);
									}
								}
							}
							this.transitionTimer = 0f;
							this.transitionTime = UnityEngine.Random.Range(this.transitionTimeMin, this.transitionTimeMax);
						}
						this.transitionTimer += Time.unscaledDeltaTime;
						break;
					}
				case VrBrainwashController.CurrentState.PrimitivesPrepare:
					AppTheSystem.Instance.ResetViewsPositions();
					this.vrElements.Clear();
					this.GenerateVrElements();
					this.GenerateOuterColliders();
					this.currentState = VrBrainwashController.CurrentState.Primitives;
					this.RemoveTransitionSymbols();
					break;
				case VrBrainwashController.CurrentState.Primitives:
					if (this.elementsGenerateTimer >= this.elementsGenerateTime)
					{
						this.GenerateVrElements();
						this.elementsGenerateTimer = 0f;
					}
					foreach (AVrElement avrElement in this.vrElements)
					{
						avrElement.Primitive.GetComponent<Rigidbody>().AddForce(UnityEngine.Random.Range(-40f, 40f), UnityEngine.Random.Range(-40f, 40f), UnityEngine.Random.Range(-10f, 10f));
					}
					if (this.brainwashTimer >= this.brainwashTime)
					{
						this.currentState = VrBrainwashController.CurrentState.TransitionPrepare;
						this.ending = true;
					}
					this.brainwashTimer += Time.unscaledDeltaTime;
					this.elementsGenerateTimer += Time.unscaledDeltaTime;
					break;
				case VrBrainwashController.CurrentState.Return:
					this.Active = false;
					this.ending = false;
					this.currentState = VrBrainwashController.CurrentState.Prepare;
					this.brainwashTimer = 0f;
					this.RemoveVrElements();
					this.RemoveOuterColliders();
					this.RemoveTransitionSymbols();
					SharedTheSystem.General.IncreaseHotCoinsIncome();
					AppTheSystem.Instance.ActionController.ResetQueues();
					SharedTheSystem.ResetProgress();
					AppTheSystem.Instance.ActionController.ResetQueues();
					AppTheSystem.Instance.ShopView.ResetShopButtons();
					AppTheSystem.Instance.ChangeGameState(AppTheSystem.GameState.InMessageSystem);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06002686 RID: 9862 RVA: 0x001142B0 File Offset: 0x001126B0
		private void MoveViews()
		{
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < AppTheSystem.Instance.Views.Count; i++)
			{
				AStoryAppsView astoryAppsView = AppTheSystem.Instance.Views.ElementAt(i);
				Vector2 vector = this.targets.ElementAt(i);
				if ((float)astoryAppsView.PositionX != vector.x && this.positionTimerX >= this.xChangeTime)
				{
					if ((float)astoryAppsView.PositionX < vector.x)
					{
						astoryAppsView.PositionX++;
					}
					else
					{
						astoryAppsView.PositionX--;
					}
					flag = true;
				}
				if ((float)astoryAppsView.PositionY != vector.y && this.positionTimerY >= this.yChangeTime)
				{
					if ((float)astoryAppsView.PositionY < vector.y)
					{
						astoryAppsView.PositionY++;
					}
					else
					{
						astoryAppsView.PositionY--;
					}
					flag2 = true;
				}
			}
			if (flag)
			{
				this.positionTimerX = 0f;
			}
			if (flag2)
			{
				this.positionTimerY = 0f;
			}
			this.positionTimerY += Time.unscaledDeltaTime;
			this.positionTimerX += Time.unscaledDeltaTime;
		}

		// Token: 0x06002687 RID: 9863 RVA: 0x001143FC File Offset: 0x001127FC
		private void BlinkViews()
		{
			foreach (AStoryAppsView view in AppTheSystem.Instance.Views)
			{
				AppTheSystem.Instance.BlinkView(view);
			}
		}

		// Token: 0x06002688 RID: 9864 RVA: 0x00114460 File Offset: 0x00112860
		private void GenerateTargets()
		{
			for (int i = 0; i < AppTheSystem.Instance.Views.Count; i++)
			{
				this.targets.Add(new Vector2((float)UnityEngine.Random.Range(1, 30), (float)UnityEngine.Random.Range(1, 15)));
			}
		}

		// Token: 0x06002689 RID: 9865 RVA: 0x001144B0 File Offset: 0x001128B0
		private void ChangeTargets()
		{
			if (this.targetChangeTimer >= this.targetChangeTime)
			{
				this.targets.Clear();
				this.GenerateTargets();
				this.targetChangeTimer = 0f;
			}
			this.targetChangeTimer += Time.unscaledDeltaTime;
		}

		// Token: 0x0600268A RID: 9866 RVA: 0x001144FC File Offset: 0x001128FC
		private bool AreViewsInTarget()
		{
			bool flag = false;
			foreach (AStoryAppsView astoryAppsView in AppTheSystem.Instance.Views)
			{
				if (astoryAppsView.PositionX != this.targetX && astoryAppsView.PositionY != this.targetY)
				{
					flag = true;
				}
			}
			return !flag;
		}

		// Token: 0x0600268B RID: 9867 RVA: 0x00114580 File Offset: 0x00112980
		private void CalculateSymbolPlacement(int i, int j)
		{
			int k = 0;
			while (k <= 20)
			{
				int num = i + UnityEngine.Random.Range(-4, 2);
				int num2 = j + UnityEngine.Random.Range(-4, 2);
				k++;
				if (num >= 0 && num2 >= 0 && num < this.transitionArray.GetLength(0) && num2 < this.transitionArray.GetLength(1) && this.transitionArray[num, num2] != 1)
				{
					if (this.transitionArray[num, num2] != 1)
					{
						this.transitionArray[num, num2] = 1;
						SHSharp::SHGUItext SHGUItext = new SHSharp::SHGUItext(this.transitionSymbol, num, num2, 'w', false);
						this.transitionSymbols.Add(SHGUItext);
						base.AddSubView(SHGUItext);
					}
					return;
				}
			}
		}

		// Token: 0x0600268C RID: 9868 RVA: 0x00114640 File Offset: 0x00112A40
		private void RemoveTransitionSymbols()
		{
			foreach (SHSharp::SHGUItext SHGUItext in this.transitionSymbols)
			{
				SHGUItext.Kill();
				base.RemoveView(SHGUItext);
			}
			this.transitionSymbols.Clear();
		}

		// Token: 0x0600268D RID: 9869 RVA: 0x001146B0 File Offset: 0x00112AB0
		private void GenerateOuterColliders()
		{
			this.outerColliders.Clear();
			GameObject gameObject = new GameObject();
			gameObject.transform.position = new Vector3(0f, 96f, 110f);
			GameObject gameObject2 = new GameObject();
			gameObject2.transform.position = new Vector3(0f, -122f, 110f);
			GameObject gameObject3 = new GameObject();
			gameObject3.transform.position = new Vector3(-154f, 1f, 110f);
			gameObject3.transform.rotation = Quaternion.AngleAxis(90f, new Vector3(0f, 0f, 1f));
			GameObject gameObject4 = new GameObject();
			gameObject4.transform.position = new Vector3(155f, 1f, 110f);
			gameObject4.transform.rotation = Quaternion.AngleAxis(90f, new Vector3(0f, 0f, 1f));
			GameObject gameObject5 = new GameObject();
			gameObject5.transform.position = new Vector3(0f, 0f, 25f);
			gameObject5.transform.rotation = Quaternion.AngleAxis(90f, new Vector3(1f, 0f, 0f));
			this.outerColliders.Add(gameObject);
			this.outerColliders.Add(gameObject2);
			this.outerColliders.Add(gameObject3);
			this.outerColliders.Add(gameObject4);
			this.outerColliders.Add(gameObject5);
			foreach (GameObject gameObject6 in this.outerColliders)
			{
				gameObject6.transform.localScale = new Vector3(300f, 10f, 300f);
				gameObject6.AddComponent<BoxCollider>();
			}
		}

		// Token: 0x0600268E RID: 9870 RVA: 0x001148B0 File Offset: 0x00112CB0
		private void RemoveOuterColliders()
		{
			foreach (GameObject obj in this.outerColliders)
			{
				UnityEngine.Object.Destroy(obj);
			}
			this.outerColliders.Clear();
		}

		// Token: 0x0600268F RID: 9871 RVA: 0x00114918 File Offset: 0x00112D18
		private void GenerateVrElements()
		{
			for (int i = 0; i < this.elementsInUpdate; i++)
			{
				switch (UnityEngine.Random.Range(1, 5))
				{
					case 1:
						{
							BrainwashAVrCube brainwashAVrCube = new BrainwashAVrCube();
							this.vrElements.Add(brainwashAVrCube);
							base.AddSubView(brainwashAVrCube);
							break;
						}
					case 2:
						{
							BrainwashAVrCapsule brainwashAVrCapsule = new BrainwashAVrCapsule();
							this.vrElements.Add(brainwashAVrCapsule);
							base.AddSubView(brainwashAVrCapsule);
							break;
						}
					case 3:
						{
							BrainwashAVrCylinder brainwashAVrCylinder = new BrainwashAVrCylinder();
							this.vrElements.Add(brainwashAVrCylinder);
							base.AddSubView(brainwashAVrCylinder);
							break;
						}
					case 4:
						{
							BrainwashAVrSphere brainwashAVrSphere = new BrainwashAVrSphere();
							this.vrElements.Add(brainwashAVrSphere);
							base.AddSubView(brainwashAVrSphere);
							break;
						}
				}
			}
		}

		// Token: 0x06002690 RID: 9872 RVA: 0x001149E4 File Offset: 0x00112DE4
		private void RemoveVrElements()
		{
			foreach (AVrElement avrElement in this.vrElements)
			{
				avrElement.Kill();
				base.RemoveView(avrElement);
			}
			this.vrElements.Clear();
		}

		// Token: 0x040028B9 RID: 10425
		private float brainwashTimer;

		// Token: 0x040028BA RID: 10426
		private float brainwashTime = 5f;

		// Token: 0x040028BB RID: 10427
		private int targetX = 20;

		// Token: 0x040028BC RID: 10428
		private int targetY = 5;

		// Token: 0x040028BD RID: 10429
		private float viewMoveTimer;

		// Token: 0x040028BE RID: 10430
		private float viewMoveTime = 1f;

		// Token: 0x040028BF RID: 10431
		private float targetChangeTimer;

		// Token: 0x040028C0 RID: 10432
		private float targetChangeTime = 0.3f;

		// Token: 0x040028C1 RID: 10433
		private float positionTimerX;

		// Token: 0x040028C2 RID: 10434
		private float positionTimerY;

		// Token: 0x040028C3 RID: 10435
		private float xChangeTime = 0.03f;

		// Token: 0x040028C4 RID: 10436
		private float yChangeTime = 0.03f;

		// Token: 0x040028C5 RID: 10437
		private VrBrainwashController.CurrentState currentState;

		// Token: 0x040028C6 RID: 10438
		private int elementsInUpdate = 2;

		// Token: 0x040028C7 RID: 10439
		private float elementsGenerateTimer;

		// Token: 0x040028C8 RID: 10440
		private float elementsGenerateTime = 0.2f;

		// Token: 0x040028C9 RID: 10441
		private int[,] transitionArray;

		// Token: 0x040028CA RID: 10442
		private string transitionSymbol = "█";

		// Token: 0x040028CB RID: 10443
		private float transitionTimer;

		// Token: 0x040028CC RID: 10444
		private float transitionTime = 0.2f;

		// Token: 0x040028CD RID: 10445
		private float transitionTimeMax = 0.3f;

		// Token: 0x040028CE RID: 10446
		private float transitionTimeMin = 0.1f;

		// Token: 0x040028CF RID: 10447
		private float transitionEndTimer;

		// Token: 0x040028D0 RID: 10448
		private float transitionEndTime = 0.5f;

		// Token: 0x040028D1 RID: 10449
		private bool ending;

		// Token: 0x040028D2 RID: 10450
		private List<AVrElement> vrElements = new List<AVrElement>();

		// Token: 0x040028D3 RID: 10451
		private List<GameObject> outerColliders = new List<GameObject>();

		// Token: 0x040028D4 RID: 10452
		private List<SHSharp::SHGUItext> transitionSymbols = new List<SHSharp::SHGUItext>();

		// Token: 0x040028D5 RID: 10453
		private List<Vector2> targets = new List<Vector2>();

		// Token: 0x02000688 RID: 1672
		private enum CurrentState
		{
			// Token: 0x040028D7 RID: 10455
			Prepare,
			// Token: 0x040028D8 RID: 10456
			ViewsGroup,
			// Token: 0x040028D9 RID: 10457
			TransitionPrepare,
			// Token: 0x040028DA RID: 10458
			Transition,
			// Token: 0x040028DB RID: 10459
			PrimitivesPrepare,
			// Token: 0x040028DC RID: 10460
			Primitives,
			// Token: 0x040028DD RID: 10461
			Return
		}
	}

	public class Wifi : SHSharp::SHGUIsprite
	{
		// Token: 0x06002507 RID: 9479 RVA: 0x0010D03C File Offset: 0x0010B43C
		public Wifi()
		{
			MCDCom.AddFrameFromStr(this, MCDCom.AssetToText(MCDCom.GetAssetName("IdleGame_Wifi.txt")), 3);
			//base.AddFramesFromFile(StoryAppsResources.IdleGameWifi, 3, false);
			this.currentFrame = 0;
		}

		// Token: 0x06002508 RID: 9480 RVA: 0x0010D06C File Offset: 0x0010B46C
		public override void Update()
		{
			base.Update();
			if (this.animationTimer >= this.animationTime)
			{
				if (this.currentFrame == this.frames.Count - 1)
				{
					this.currentFrame = 0;
				}
				else
				{
					this.currentFrame++;
				}
				this.animationTimer = 0f;
			}
			else
			{
				this.animationTimer += Time.unscaledDeltaTime;
			}
		}

		// Token: 0x04002733 RID: 10035
		private float animationTimer;

		// Token: 0x04002734 RID: 10036
		private readonly float animationTime = SharedTheSystem.Views.WifiAnimationTime;
	}

	public class WifiRateUpgrade : AUpgrade
	{
		// Token: 0x0600265E RID: 9822 RVA: 0x00112A38 File Offset: 0x00110E38
		public WifiRateUpgrade(string text, int startX, int startY, char color) : base(text, startX, startY, color)
		{
			base.UpgradeType = SharedUpgrades.UpgradeType.WifiRate;
			base.StartPrice = SharedTheSystem.Upgrades.WifiRateUpgradeStartPrice;
			Value price;
			Value tier;
			SharedTheSystem.Upgrades.GetWifiRateUpgrade(out price, out tier);
			base.Price = price;
			base.Tier = tier;
		}

		// Token: 0x0600265F RID: 9823 RVA: 0x00112A83 File Offset: 0x00110E83
		public override void ActivateUpgrade()
		{
			if (base.CanBeBought())
			{
				base.Buy();
				SharedTheSystem.Upgrades.IncreaseRateMultiplier(base.UpgradeType);
				base.UpdatePriceAndTier();
				SharedTheSystem.Upgrades.SaveWifiRateUpgrade(base.Price, base.Tier);
			}
		}

		// Token: 0x06002660 RID: 9824 RVA: 0x00112AC2 File Offset: 0x00110EC2
		public override bool CanBeUsed()
		{
			return SharedTheSystem.Workers.GetWifis() >= this.GetRequiredWorkers();
		}

		// Token: 0x06002661 RID: 9825 RVA: 0x00112AD9 File Offset: 0x00110ED9
		public override void UpdateHintText()
		{
			base.HintText = SharedTheSystem.Upgrades.WifiRateHint + SharedTheSystem.GetUpgradesHintStats(IncomeController.Instance.TotalWifiIncomePerSec());
			base.UpdateHintText();
		}
	}
}
