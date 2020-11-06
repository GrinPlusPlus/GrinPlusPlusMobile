using GrinPlusPlus.Models;
using System.Collections.Generic;

namespace GrinPlusPlus.Services
{
    class MockDataProvider : IDataProvider
    {
        public List<Transaction> GetAllData()
        {
            return new List<Transaction>()
            {
                new Transaction()
                {
                    Id = 1,
                    Status = "Receiving",
                    Amount = 25369,
                    Decimals = 234584236,
                    Fee = 0.008000,
                    Date = new System.DateTime(2020,7,15,0,0,0),
                    Address = "grin1p4fuklglxqsgg602hu4c4jl4aunu5tynyf4lkg96ezh3jefzpy6swshp5x",
                    Slate = "096888ecbc7b04f0c7c5d1c3b80c9930bac4025cdbbf1eafe6b824a4b20cc62223",
                    Message = "Testing",
                    Kernels = "d96e3937988027ccf03443e4161c74d7d76aa4cdf9cae34bb8e6f225e492fe65",
                    Outputs = new List<Output>() {new Output(){ Commitment = "00017fea774817563a81461106bf59ae0e49dd37bcd3102a7ba90d7006de8055", Amount= 200.0 } },
                },
                new Transaction()
                {
                    Id = 2,
                    Status = "Sending",
                    Amount = 10,
                    Decimals = 819835236,
                    Fee = 0.008000,
                    Date = new System.DateTime(2020,7,15,1,0,0),
                    Address = "grin1p4fuklglxqsgg602hu4c4jl4aunu5tynyf4lkg96ezh3jefzpy6swshp5x",
                    Slate = "096888ecbc7b04f0c7c5d1c3b80c9930bac4025cdbbf1eafe6b824a4b20cc62223",
                    Message = "Testing",
                    Kernels = "d96e3937988027ccf03443e4161c74d7d76aa4cdf9cae34bb8e6f225e492fe65",
                    Outputs = new List<Output>() {new Output(){ Commitment = "00017fea774817563a81461106bf59ae0e49dd37bcd3102a7ba90d7006de8055", Amount= 200.0 } },
                },
                new Transaction()
                {
                    Id = 3,
                    Status = "Received",
                    Amount = 1,
                    Decimals = 350900221,
                    Fee = 0.008000,
                    Date = new System.DateTime(2020,7,15,1,0,0),
                    Address = "grin1p4fuklglxqsgg602hu4c4jl4aunu5tynyf4lkg96ezh3jefzpy6swshp5x",
                    Slate = "096888ecbc7b04f0c7c5d1c3b80c9930bac4025cdbbf1eafe6b824a4b20cc62223",
                    Message = "Testing",
                    Kernels = "d96e3937988027ccf03443e4161c74d7d76aa4cdf9cae34bb8e6f225e492fe65",
                    Outputs = new List<Output>() {new Output(){ Commitment = "00017fea774817563a81461106bf59ae0e49dd37bcd3102a7ba90d7006de8055", Amount= 200.0 } },
                },
                new Transaction()
                {
                    Id = 4,
                    Status = "Sent",
                    Amount = 6,
                    Decimals = 131123123,
                    Fee = 0.008000,
                    Date = new System.DateTime(2020,7,15,2,0,0),
                    Address = "grin1p4fuklglxqsgg602hu4c4jl4aunu5tynyf4lkg96ezh3jefzpy6swshp5x",
                    Slate = "096888ecbc7b04f0c7c5d1c3b80c9930bac4025cdbbf1eafe6b824a4b20cc62223",
                    Message = "Testing",
                    Kernels = "d96e3937988027ccf03443e4161c74d7d76aa4cdf9cae34bb8e6f225e492fe65",
                    Outputs = new List<Output>() {new Output(){ Commitment = "00017fea774817563a81461106bf59ae0e49dd37bcd3102a7ba90d7006de8055", Amount= 200.0 } },
                },
                new Transaction()
                {
                    Id = 5,
                    Status = "Sent",
                    Amount = 100,
                    Decimals = 090000001,
                    Fee = 0.008000,
                    Date = new System.DateTime(2020,7,15,1,0,0),
                    Address = "grin1p4fuklglxqsgg602hu4c4jl4aunu5tynyf4lkg96ezh3jefzpy6swshp5x",
                    Slate = "096888ecbc7b04f0c7c5d1c3b80c9930bac4025cdbbf1eafe6b824a4b20cc62223",
                    Message = "Testing",
                    Kernels = "d96e3937988027ccf03443e4161c74d7d76aa4cdf9cae34bb8e6f225e492fe65",
                    Outputs = new List<Output>() {new Output(){ Commitment = "00017fea774817563a81461106bf59ae0e49dd37bcd3102a7ba90d7006de8055", Amount= 200.0 } },
                },
                new Transaction()
                {
                    Id = 6,
                    Status = "Received",
                    Amount = 1,
                    Decimals = 350900221,
                    Fee = 0.008000,
                    Date = new System.DateTime(2020,7,15,1,0,0),
                    Address = "grin1p4fuklglxqsgg602hu4c4jl4aunu5tynyf4lkg96ezh3jefzpy6swshp5x",
                    Slate = "096888ecbc7b04f0c7c5d1c3b80c9930bac4025cdbbf1eafe6b824a4b20cc62223",
                    Message = "Testing",
                    Kernels = "d96e3937988027ccf03443e4161c74d7d76aa4cdf9cae34bb8e6f225e492fe65",
                    Outputs = new List<Output>() {new Output(){ Commitment = "00017fea774817563a81461106bf59ae0e49dd37bcd3102a7ba90d7006de8055", Amount= 200.0 } },
                },
                new Transaction()
                {
                    Id = 7,
                    Status = "Sent",
                    Amount = 6,
                    Decimals = 131123123,
                    Fee = 0.008000,
                    Date = new System.DateTime(2020,7,15,2,0,0),
                    Address = "grin1p4fuklglxqsgg602hu4c4jl4aunu5tynyf4lkg96ezh3jefzpy6swshp5x",
                    Slate = "096888ecbc7b04f0c7c5d1c3b80c9930bac4025cdbbf1eafe6b824a4b20cc62223",
                    Message = "Testing",
                    Kernels = "d96e3937988027ccf03443e4161c74d7d76aa4cdf9cae34bb8e6f225e492fe65",
                    Outputs = new List<Output>() {new Output(){ Commitment = "00017fea774817563a81461106bf59ae0e49dd37bcd3102a7ba90d7006de8055", Amount= 200.0 } },
                },
                new Transaction()
                {
                    Id = 8,
                    Status = "Sent",
                    Amount = 100,
                    Decimals = 090000001,
                    Fee = 0.008000,
                    Date = new System.DateTime(2020,7,15,1,0,0),
                    Address = "grin1p4fuklglxqsgg602hu4c4jl4aunu5tynyf4lkg96ezh3jefzpy6swshp5x",
                    Slate = "096888ecbc7b04f0c7c5d1c3b80c9930bac4025cdbbf1eafe6b824a4b20cc62223",
                    Message = "Testing",
                    Kernels = "d96e3937988027ccf03443e4161c74d7d76aa4cdf9cae34bb8e6f225e492fe65",
                    Outputs = new List<Output>() {new Output(){ Commitment = "00017fea774817563a81461106bf59ae0e49dd37bcd3102a7ba90d7006de8055", Amount= 200.0 } },
                },
                new Transaction()
                {
                    Id = 9,
                    Status = "Sent",
                    Amount = 6,
                    Decimals = 131123123,
                    Fee = 0.008000,
                    Date = new System.DateTime(2020,7,15,2,0,0),
                    Address = "grin1p4fuklglxqsgg602hu4c4jl4aunu5tynyf4lkg96ezh3jefzpy6swshp5x",
                    Slate = "096888ecbc7b04f0c7c5d1c3b80c9930bac4025cdbbf1eafe6b824a4b20cc62223",
                    Message = "Testing",
                    Kernels = "d96e3937988027ccf03443e4161c74d7d76aa4cdf9cae34bb8e6f225e492fe65",
                    Outputs = new List<Output>() {new Output(){ Commitment = "00017fea774817563a81461106bf59ae0e49dd37bcd3102a7ba90d7006de8055", Amount= 200.0 } },
                },
                new Transaction()
                {
                    Id = 10,
                    Status = "Sent",
                    Amount = 100,
                    Decimals = 090000001,
                    Fee = 0.008000,
                    Date = new System.DateTime(2020,7,15,1,0,0),
                    Address = "grin1p4fuklglxqsgg602hu4c4jl4aunu5tynyf4lkg96ezh3jefzpy6swshp5x",
                    Slate = "096888ecbc7b04f0c7c5d1c3b80c9930bac4025cdbbf1eafe6b824a4b20cc62223",
                    Message = "Testing",
                    Kernels = "d96e3937988027ccf03443e4161c74d7d76aa4cdf9cae34bb8e6f225e492fe65",
                    Outputs = new List<Output>() {new Output(){ Commitment = "00017fea774817563a81461106bf59ae0e49dd37bcd3102a7ba90d7006de8055", Amount= 200.0 } },
                },
            };
        }
    }
}
