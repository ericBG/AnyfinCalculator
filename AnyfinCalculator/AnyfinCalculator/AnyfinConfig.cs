using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using AnyfinCalculator.Annotations;
using Hearthstone_Deck_Tracker;

namespace AnyfinCalculator
{
	[Serializable]
	public class AnyfinConfig : INotifyPropertyChanged
	{
		[NonSerialized] public static readonly string ConfigLocation = Path.Combine(Config.Instance.DataDir,
			"anyfin.xml");

		private double _iconX;
		private double _iconY;

		public AnyfinConfig()
		{
			IconX = 83.5;
			IconY = 69.5;
		}

		[DefaultValue(false)]
		public bool ClassicMode { get; set; }

		public double IconX
		{
			get { return _iconX; }
			set
			{
				if (value == _iconX) return;
				_iconX = value;
				OnPropertyChanged();
			}
		}

		public double IconY
		{
			get { return _iconY; }
			set
			{
				if (value == _iconY) return;
				_iconY = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}