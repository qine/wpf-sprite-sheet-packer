﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.CommandWpf;
using SpriteSheetPacker.Extensions;
using SpriteSheetPacker.Model;
using SpriteSheetPacker.Service;
using SpriteSheetPacker.Util;

namespace SpriteSheetPacker
{
	public sealed class MainWindowViewModel : INotifyPropertyChanged
	{
		private BitmapSource _packedImagePreview;
		private readonly IPacker _packer;
		private readonly SaveFileDialogService _saveFileDialogService;
		private bool _isPacking;

		public MainWindowViewModel()
		{
			ImagesPath = new ObservableCollection<string>();

			_saveFileDialogService = new SaveFileDialogService();

			var foreignFunctionCaller = new ForeignFunctionCaller();
			_packer = new ForeignPacker(foreignFunctionCaller);

			PackCommand = new RelayCommand(OnPackCommand);

			Padding = 20;
			AlphaTreshold = 100;
			OutputTextureSizes = new List<Size>
			{
				new Size(512, 512),
				new Size(1024, 1024),
				new Size(2048, 2048),
				new Size(4096, 4096)
			};
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ICommand PackCommand { get; set; }

		public string TilesX { get; set; }

		public string TilesY { get; set; }

		public int AlphaTreshold { get; set; }

		public int Padding { get; set; }

		public bool IsBlackTrimmingEnabled { get; set; }

		public bool IsLivePreviewEnabed { get; set; }

		public List<Size> OutputTextureSizes { get; }

		public BitmapSource PackedImagePreview
		{
			get => _packedImagePreview;
			set
			{
				if (Equals(value, _packedImagePreview))
					return;
				_packedImagePreview = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<string> ImagesPath { get; }

		public bool IsPacking
		{
			get => _isPacking;
			set
			{
				if (value == _isPacking)
					return;
				_isPacking = value;
				OnPropertyChanged();
			}
		}

		private async void OnPackCommand()
		{
			IsPacking = true;

			var packerImage = await _packer.PackAsync(new PackParameters(2, 2, 50, 20, new Size(1024, 768), ImagesPath.ToList()));

			PackedImagePreview = packerImage.ToBitmapSource();

			IsPacking = false;

			string saveFilePath;
			if (_saveFileDialogService.SaveFileDialog(out saveFilePath))
			{
				var bitmapWriter = new BitmapStreamWriter();
				bitmapWriter.Write(saveFilePath, PackedImagePreview);
			}
		}

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
