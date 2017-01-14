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
		private bool _yFromBottom;
		private bool _xFromRight;

		public AnyfinConfig()
		{
			IconX = 5;
			IconY = 0;
			YFromBottom = true;
			XFromRight = false;
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

		public bool YFromBottom
		{
			get { return _yFromBottom; }
			set
			{
				if (value == _yFromBottom) return;
				_yFromBottom = value;
				OnPropertyChanged();
			}
		}

		public bool XFromRight
		{
			get { return _xFromRight; }
			set
			{
				if (value == _xFromRight) return;
				_xFromRight = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}
}