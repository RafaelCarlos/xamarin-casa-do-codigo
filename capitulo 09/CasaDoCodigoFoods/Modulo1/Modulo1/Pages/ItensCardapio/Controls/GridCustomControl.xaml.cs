﻿using Modulo1.Dal;
using Modulo1.Modelo;
using Modulo1.Pages.TiposItensCardapio;
using Plugin.Media;
using System;
using System.IO;
using Xamarin.Forms;

namespace Modulo1.Pages.ItensCardapio.Controls
{
    public partial class GridCustomControl : Grid
    {
        private byte[] bytesFoto;
        private ItemCardapio itemCardapio = null;

        public GridCustomControl()
        {
            InitializeComponent();
        }

        private async void OnTapLookForTipos(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new TiposItensCardapioSearchPage(idTipo, nomeTipo));
        }
        
        public async void OnAlbumClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await App.Current.MainPage.DisplayAlert("Álbum não suportado", "Não existe permissão para acessar o álbum de fotos", "OK");
                return;
            }

            var file = await CrossMedia.Current.PickPhotoAsync();
            if (file == null)
                return;
            var stream = file.GetStream();
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            fotoAlbum.Source = ImageSource.FromStream(() =>
            {
                var s = file.GetStream();
                file.Dispose();
                return s;
            });
            bytesFoto = memoryStream.ToArray();
        }

        private async void OnTappedSaveItem(object sender, EventArgs args)
        {
            var dal = new ItemCardapioDAL();
            if (this.itemCardapio == null)
            {
                this.itemCardapio = new ItemCardapio();
            }
            this.itemCardapio.Nome = nome.Text;
            this.itemCardapio.Descricao = descricao.Text;
            this.itemCardapio.Preco = Convert.ToDouble(preco.Text);
            this.itemCardapio.TipoItemCardapioId = Convert.ToInt32(idTipo.Text);
            this.itemCardapio.Foto = bytesFoto;

            if (this.itemCardapio.ItemCardapioId == null)
            {
                dal.Add(this.itemCardapio);
                await App.Current.MainPage.DisplayAlert("Inserção de item", "Item inserido com sucesso", "Ok");
            }
            else
            {
                dal.Update(this.itemCardapio);
                await App.Current.MainPage.DisplayAlert("Atualização de item", "Item atualizado com sucesso", "Ok");
                await Navigation.PopAsync();
            }

            ClearControls();
        }

        private void ClearControls()
        {
            nome.Text = string.Empty;
            descricao.Text = string.Empty;
            preco.Text = string.Empty;
            idTipo.Text = string.Empty;
            nomeTipo.Text = "Selecione o Tipo do Item";
            bytesFoto = null;
            fotoAlbum.Source = null;
        }

        public void PopularControles(ItemCardapio itemCardapio)
        {
            this.itemCardapio = itemCardapio;
            nome.Text = this.itemCardapio.Nome;
            descricao.Text = this.itemCardapio.Descricao;
            preco.Text = this.itemCardapio.Preco.ToString("N");
            if (this.itemCardapio.Foto != null)
            {
                fotoAlbum.Source = ImageSource.FromStream(() => new MemoryStream(this.itemCardapio.Foto));
                bytesFoto = this.itemCardapio.Foto;
            }
            nomeTipo.Text = this.itemCardapio.TipoItemCardapio.Nome;
            idTipo.Text = this.itemCardapio.TipoItemCardapioId.ToString();
        }
    }
}
