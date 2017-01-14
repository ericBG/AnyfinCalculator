using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Utility.Logging;
using MahApps.Metro.Controls.Dialogs;
using Card = Hearthstone_Deck_Tracker.Hearthstone.Card;
using Core = Hearthstone_Deck_Tracker.API.Core;
using System.Threading.Tasks;

namespace AnyfinCalculator
{
	public class AnyfinPlugin : IPlugin
	{
		private DamageCalculator _calculator;
		private AnyfinConfig _config;
		private HearthstoneTextBlock _displayBlock;
		private AnyfinDisplay _display;
		//todo: this
		private CardToolTip _toolTip;
		private StackPanel _toolTipsPanel;
		private bool _inAnyfinGame;

		protected MenuItem mainMenuItem { get; set; }

		public void OnLoad()
		{
			_displayBlock = new HearthstoneTextBlock { FontSize = 36, Visibility = Visibility.Collapsed };
			_calculator = new DamageCalculator();
			ConfigHandler();
			_display = new AnyfinDisplay(_config) { Visibility = Visibility.Collapsed };
			_toolTip = new CardToolTip();
			_toolTipsPanel = new StackPanel();

			// Create main menu item
			mainMenuItem = new MenuItem();
			mainMenuItem.Header = "Calculate Board";
			mainMenuItem.Click += new RoutedEventHandler(ForceUpdateClick);

			GameEvents.OnPlayerHandMouseOver.Add(OnMouseOver);
			GameEvents.OnMouseOverOff.Add(OnMouseOff);
			GameEvents.OnGameStart.Add(OnGameStart);
			GameEvents.OnGameEnd.Add(OnGameEnd);
			GameEvents.OnOpponentPlayToGraveyard.Add(UpdateDisplay);
			GameEvents.OnPlayerPlayToGraveyard.Add(UpdateDisplay);
			GameEvents.OnPlayerPlay.Add(UpdateDisplay);
			GameEvents.OnOpponentPlay.Add(UpdateDisplay);
			DeckManagerEvents.OnDeckSelected.Add(OnGameStart);
			GameEvents.OnTurnStart.Add(OnTurnStart);
			Core.OverlayCanvas.Children.Add(_display);
			if (!Core.Game.IsInMenu)
			OnGameStart();
		}

		#region New Mode

		private void OnGameEnd()
		{
			_inAnyfinGame = false;
			_display.Visibility = Visibility.Collapsed;
		}

		private void OnGameStart(Deck obj) => OnGameStart();

		private void OnGameStart()
		{
			if (!(DeckList.Instance?.ActiveDeck.Cards.Contains(Murlocs.AnyfinCanHappen) ?? false)) return;
			_inAnyfinGame = true;
			UpdateDisplay(null);
		}

		private void ForceUpdateClick(object o, RoutedEventArgs a)
		{
			UpdateDisplay(null);
		}

		private void OnTurnStart(ActivePlayer player)
		{
			UpdateDisplay(null);
		}

		private async void UpdateDisplay(Card c)
		{
			if (!_inAnyfinGame) return;

			// Temporary fix for race condition where the GameTags on the played card haven't been updated yet.
			await Task.Delay(200);

			Range<int> range = _calculator.CalculateDamageDealt();
			_display.DamageText = $"{range.Minimum}\n{range.Maximum}";
			if (_display.Visibility == Visibility.Collapsed)
				_display.Visibility = _calculator.DeadMurlocs.Any() ? Visibility.Visible : Visibility.Collapsed;
		}

		#endregion

		public async void OnButtonPress()
			=> await Core.MainWindow.ShowMessageAsync("Warning", "There is currently no options for this plugin.");

		public void OnUnload()
		{
		}

		public void OnUpdate()
		{
		}

		public string Name => "Anyfin Can Happen Calculator";

		public string Description
			=>
				"Anyfin Can Happen Calculator is a plugin for Hearthstone Deck Tracker which allows you to quickly and easily figure out the damage " +
				"(or damage range) that playing Anyfin Can Happen will have on your current board. \n For any questions or issues look at github.com/ericBG/AnyfinCalculator";

		public MenuItem MenuItem
		{
			get { return mainMenuItem; }
		}

		public string ButtonText => "Options";
		public string Author => "ericBG";
		public Version Version => new Version(1, 0, 6);

		private void ConfigHandler()
		{
			if (File.Exists(AnyfinConfig.ConfigLocation))
			{
				using (var fs = File.OpenRead(AnyfinConfig.ConfigLocation))
				using (var r = XmlReader.Create(fs))
				{
					var x = new XmlSerializer(typeof(AnyfinConfig));
					if (!x.CanDeserialize(r)) _config = new AnyfinConfig();
					else _config = (AnyfinConfig)x.Deserialize(r);
				}
			}
			else
			{
				using (var fs = File.OpenWrite(AnyfinConfig.ConfigLocation))
				{
					var x = new XmlSerializer(typeof(AnyfinConfig));
					x.Serialize(fs, (_config = new AnyfinConfig()));
				}
			}
		}

		#region Classic Mode

		private void OnMouseOff()
		{
			_displayBlock.Visibility = Visibility.Collapsed;
			Core.OverlayCanvas.Children.Remove(_displayBlock);
		}

		private void PlaceTextboxWithText(string text)
		{
			_displayBlock.Text = text;
			_displayBlock.Visibility = Visibility.Visible;
			if (!Core.OverlayCanvas.Children.Contains(_displayBlock))
				Core.OverlayCanvas.Children.Add(_displayBlock);
			Log.Debug($"Textbox has been placed to display:\n '{text}'.");
		}

		private void OnMouseOver(Card card)
		{
			if (!card.IsAnyfin() || !_config.ClassicMode) return;
			Log.Debug("Anyfin hover detected");
			var damageDealt = _calculator.CalculateDamageDealt();
			var friendlyText = damageDealt.Minimum == damageDealt.Maximum ? "" : "between ";
			PlaceTextboxWithText($"Anyfin can deal {friendlyText}{damageDealt}");
		}

		#endregion
	}
}
