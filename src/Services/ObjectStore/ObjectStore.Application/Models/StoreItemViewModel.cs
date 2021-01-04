using System;
using System.Text.Json.Serialization;

namespace DrifterApps.Holefeeder.ObjectStore.Application.Models
{
    public class StoreItemViewModel
    {
        public Guid Id { get; }
        public string Code { get; }
        public string Data { get; }

        [JsonConstructor]
        public StoreItemViewModel(Guid id, string code, string data)
        {
            Id = id;
            Code = code;
            Data = data;
        }
    }
}
