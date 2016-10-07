﻿using Modulo1.Infraestructure;
using Modulo1.Modelo;
using Modulo1.ViewModel;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Modulo1.Dal
{
    public class PedidoDAL
    {
        private SQLiteConnection sqlConnection;

        public PedidoDAL()
        {
            this.sqlConnection = DependencyService.Get<IDatabaseConnection>().DbConnection();
            this.sqlConnection.CreateTable<Pedido>();
            this.sqlConnection.CreateTable<ItemPedido>();
        }

        public void Add(Pedido pedido)
        {
            sqlConnection.InsertWithChildren(pedido);
        }

        public void DeleteById(long id)
        {
            sqlConnection.Delete<Pedido>(id);
        }

        public IEnumerable<Pedido> GetAllWithChildren()
        {
            return sqlConnection.GetAllWithChildren<Pedido>().OrderBy(i => i.Cliente.Nome).ToList();
        }

        public IEnumerable<Pedido> GetAbertosWithChildren()
        {
            return sqlConnection.GetAllWithChildren<Pedido>().Where(p => p.DataEHoraProducao == null).OrderBy(i => i.Cliente.Nome).ToList();
        }

        public IEnumerable<Pedido> GetEmProducaoWithChildren()
        {
            return sqlConnection.GetAllWithChildren<Pedido>().Where(
                p => p.DataEHoraProducao != null && p.DataEHoraEntrega == null).
                OrderBy(i => i.Cliente.Nome).ToList();
        }

        public IEnumerable<Pedido> GetEmEntregaWithChildren()
        {
            return sqlConnection.GetAllWithChildren<Pedido>().Where(
                p => p.DataEHoraEntrega != null && p.DataEHoraEntregue == null).
                OrderBy(i => i.Cliente.Nome).ToList();
        }

        public IEnumerable<Pedido> GetFechadosWithChildren()
        {
            return sqlConnection.GetAllWithChildren<Pedido>().Where(
                p => p.DataEHoraEntregue != null).
                OrderBy(i => i.Cliente.Nome).ToList();
        }

        public void Update(Pedido pedido)
        {
            sqlConnection.Update(pedido);
        }

        public IEnumerable<DadoParaGrafico> GetFechadosGroupedByDayWithChildren()
        {
            return sqlConnection.GetAllWithChildren<Pedido>().Where(
                p => p.DataEHoraEntregue != null).
                GroupBy(p => p.DataEHoraEntregue.Value.Day).
                Select(pedidos => new DadoParaGrafico
                {
                    DiaDoMes = pedidos.First().DataEHoraEntregue.Value.Day,
                    Total = pedidos.Sum(s => s.Total)
                }).ToList(); ;
        }
    }
}
