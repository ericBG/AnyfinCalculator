using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using AnyfinCalculator.Annotations;
using Hearthstone_Deck_Tracker;
using Core = Hearthstone_Deck_Tracker.API.Core;

namespace AnyfinCalculator
{
	/// <summary>
	///     Interaction logic for AnyfinDisplay.xaml
	/// </summary>
	public partial class AnyfinDisplay : INotifyPropertyChanged
	{
		private readonly AnyfinConfig _config;
		private string _damageText;

		public AnyfinDisplay(AnyfinConfig config)
		{
			InitializeComponent();
			_config = config;
			_config.PropertyChanged += ConfigSizeChanged;
			ConfigSizeChanged(this, null);
		}

		private double ScreenRatio => (4.0/3.0)/(Core.OverlayCanvas.Width/Core.OverlayCanvas.Height);

		public string DamageText
		{
			get { return _damageText; }
			set
			{
				if (value == _damageText) return;
				_damageText = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void ConfigSizeChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e?.PropertyName == "IconX" || ReferenceEquals(sender, this))
				Canvas.SetLeft(this,
					Helper.GetScaledXPos(_config.IconX/100, (int) Core.OverlayCanvas.Width, ScreenRatio));
			if (e?.PropertyName == "IconY" || ReferenceEquals(sender, this))
				Canvas.SetTop(this, Core.OverlayCanvas.Height*(_config.IconY/100));
		}

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}