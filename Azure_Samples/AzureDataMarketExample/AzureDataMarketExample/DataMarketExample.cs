using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net;
using System.Data.Services.Client;
using WorldBank;
using AzureDataMarketExample.UnitedNationsData;

namespace AzureDataMarketExample
{
    class DataMarketExample
    {
        private Uri worldBankUri = new Uri("https://api.datamarket.azure.com/Data.ashx/WorldBank/WorldDevelopmentIndicators/");
        private Uri unitedNationsUri = new Uri("https://api.datamarket.azure.com/Data.ashx/UnitedNations/NationalAccounts/");

        private String userName = "WINDOWS_LIVE_ID";
        private String password = "ACCOUNT_KEY";

        public IEnumerable<Values> UnitedNationsValues
        {
            get
            {
                UnitedNationsNationalAccountsContainer context = new UnitedNationsNationalAccountsContainer(unitedNationsUri);
                context.Credentials = new NetworkCredential(userName, password);

                IEnumerable<Values> query = (from entity in context.Values
                                             where entity.DataSeriesId == "101"
                                             && entity.ItemCode == 21
                                             && entity.FiscalYear == 2005
                                             select entity);
                return query;
            }
        }

        public IEnumerable<CountryEntity> WorldBankData
        {
            get
            {
                WorldDevelopmentIndicatorsContainer context = new WorldDevelopmentIndicatorsContainer(worldBankUri);
                context.Credentials = new NetworkCredential(userName, password);

                DataServiceQuery<CountryEntity> dataServiceQuery = context.GetCountries("en");
                IEnumerable<CountryEntity> countryQuery = dataServiceQuery.Skip<CountryEntity>(50);

                return countryQuery;
            }
        }
    }
}
