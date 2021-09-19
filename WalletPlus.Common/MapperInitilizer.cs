using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletPlus.Common.Entities;
using WalletPlus.Common.Models;

namespace WalletPlus.Common
{
    public class MapperInitilizer : Profile
    {
        public MapperInitilizer()
        {
            CreateMap<Customer, CustomerModel>().ReverseMap();
            CreateMap<Customer, CreateCustomerModel>().ReverseMap();
            CreateMap<Customer, UpdateCustomerModel>().ReverseMap();

            CreateMap<Point, PointModel>().ReverseMap();

            CreateMap<PointSetting, PointSettingModel>().ReverseMap();

            CreateMap<Transaction, TransactionModel>().ReverseMap();
            CreateMap<Transaction, FundWalletModel>().ReverseMap();
            CreateMap<Transaction, MakeTransferModel>().ReverseMap();

            CreateMap<Wallet, WalletModel>().ReverseMap();
        }
    }
}
