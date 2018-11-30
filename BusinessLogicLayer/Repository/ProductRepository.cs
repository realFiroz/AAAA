﻿using System;
using System.Collections.Generic;
using Repository;
using Entities;
using DataAccessLayer;
using System.Data;
using static Enum.Enums;
using System.Data.SqlClient;
using System.Linq;

namespace Repository
{
    public class ProductRepository : IProductRepository

    {
        public ServiceRes AddProduct(ProductDetails objProduct)
        {
            ServiceRes serviceRes = new ServiceRes();
            try
            {
                if (objProduct != null)
                {
                    ICommonRepository _commonRepository = new CommonRepository();
                    string fileLocation = _commonRepository.Base64toImage(objProduct.ImagePath, "Images", "ProductImages","ProductPhoto");
                    SqlParameter[] sqlParameters = new SqlParameter[13];
                    sqlParameters[0] = new SqlParameter { ParameterName = "@Name", Value = objProduct.Name };
                    sqlParameters[1] = new SqlParameter { ParameterName = "@SKUNumber", Value = objProduct.SKUNumber };
                    sqlParameters[2] = new SqlParameter { ParameterName = "@Specification", Value = objProduct.Specification };
                    sqlParameters[3] = new SqlParameter { ParameterName = "@Price", Value = objProduct.Price };
                    sqlParameters[4] = new SqlParameter { ParameterName = "@IsPackaging", Value = objProduct.IsPackaging };
                    sqlParameters[5] = new SqlParameter { ParameterName = "@IsAvailable", Value = objProduct.IsAvailable };
                    sqlParameters[6] = new SqlParameter { ParameterName = "@Discount", Value = objProduct.Discount };
                    sqlParameters[7] = new SqlParameter { ParameterName = "@Category_Id", Value = objProduct.CategoryId };
                    sqlParameters[8] = new SqlParameter { ParameterName = "@SubCategory_Id", Value = objProduct.SubCategoryId };
                    sqlParameters[9] = new SqlParameter { ParameterName = "@Quantity", Value = objProduct.Quantity };
                    sqlParameters[10] = new SqlParameter { ParameterName = "@Photos_Url", Value = fileLocation };
                    sqlParameters[11] = new SqlParameter { ParameterName = "@Member_Id", Value = objProduct.UserId };
                    sqlParameters[12] = new SqlParameter { ParameterName = "@flag", Value = "A" };
                    int returnValue = SqlHelper.ExecuteNonQuery("Usp_Products_Test", sqlParameters);
                    if (returnValue > 0)
                    {
                        serviceRes.IsSuccess = true;
                        serviceRes.ReturnCode = "200";
                        serviceRes.ReturnMsg = "Product added successfully";
                    }
                    else
                    {
                        serviceRes.IsSuccess = false;
                        serviceRes.ReturnCode = "400";
                        serviceRes.ReturnMsg = "Something went wrong";
                    }
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(ex);
                serviceRes.IsSuccess = false;
                serviceRes.ReturnCode = "400";
                serviceRes.ReturnMsg = "Error occured in database";
            }
            return serviceRes;
        }

        public ServiceRes DeleteProduct(int productId)
        {
            throw new NotImplementedException();
        }

        public ServiceRes GetAllProduct()
        {
            ServiceRes<List<ProductDetails>> serviceRes = new ServiceRes<List<ProductDetails>>();
            List<ProductDetails> productDetails = new List<ProductDetails>();
            try
            {
                SqlParameter[] sqlParameters = new SqlParameter[1];
                sqlParameters[0] = new SqlParameter { ParameterName = "@flag", Value = "SL" };
                var dt = SqlHelper.GetTableFromSP("Usp_Products", sqlParameters);
                if (dt != null && dt.Rows.Count > 0)
                {
                    
                    serviceRes.Data = productDetails;
                    serviceRes.IsSuccess = true;
                    serviceRes.ReturnCode = "200";
                    serviceRes.ReturnMsg = "Product list ";
                }
                else
                {
                    serviceRes.Data = null;
                    serviceRes.IsSuccess = false;
                    serviceRes.ReturnCode = "202";
                    serviceRes.ReturnMsg = "Product not found";
                }
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(ex);
            }
            return serviceRes;
        }

        public ServiceRes UpdateProduct(ProductDetails objProduct)
        {
            throw new NotImplementedException();
        }

        public ServiceRes GetSubCategoryMaster(int categoryId)
        {
            ServiceRes<List<ProductSubCategory>> serviceRes = new ServiceRes<List<ProductSubCategory>>();
            try
            {
                List<ProductSubCategory> subCategories = new List<ProductSubCategory>();
                SqlParameter[] sqlParameter = new SqlParameter[1];
                sqlParameter[0] = new SqlParameter { ParameterName = "@categoryId", Value = categoryId };
                DataTable dtCities = SqlHelper.GetTableFromSP("Usp_GetSubCategory", sqlParameter);
                foreach (DataRow row in dtCities.Rows)
                {
                    ProductSubCategory businessCategory = new ProductSubCategory
                    {
                        SubCategoryId = Convert.ToInt32(row["Product_SubCategoryId"]),
                        Name = Convert.ToString(row["SubCategory_Name"])
                    };
                    subCategories.Add(businessCategory);
                }
                serviceRes.Data = subCategories;
                serviceRes.IsSuccess = true;
                serviceRes.ReturnCode = "200";
                serviceRes.ReturnMsg = "Sub Category master";
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(ex);
            }
            return serviceRes;
        }

        public ServiceRes GetCategoryMaster()
        {
            ServiceRes<List<ProductCategory>> serviceRes = new ServiceRes<List<ProductCategory>>();
            try
            {
                List<ProductCategory> productCategories = new List<ProductCategory>();
                DataTable dtCities = SqlHelper.GetTableFromSP("Usp_GetCategory");
                foreach (DataRow row in dtCities.Rows)
                {
                    ProductCategory productCategory = new ProductCategory
                    {
                        CategoryId = Convert.ToInt32(row["Product_Category_Id"]),
                        Name = Convert.ToString(row["Product_Name"])
                    };
                    productCategories.Add(productCategory);
                }
                serviceRes.Data = productCategories;
                serviceRes.IsSuccess = true;
                serviceRes.ReturnCode = "200";
                serviceRes.ReturnMsg = "Category master";
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(ex);
            }
            return serviceRes;
        }

        public ServiceRes GetProductDetailById()
        {
            throw new NotImplementedException();
        }

        public ServiceRes GetAllRecentOrders()
        {
            throw new NotImplementedException();
        }

        public ServiceRes Distributor_SalesPerformance(Distributor_User distributor_User)
        {
            ServiceRes<List<DistributorSalesReport>> serviceRes = new ServiceRes<List<DistributorSalesReport>>();
            try {
                SqlParameter[] sqlParameters = new SqlParameter[2];
                sqlParameters[0] = new SqlParameter { ParameterName = "@MemberId", Value = distributor_User.UserId };
                sqlParameters[1] = new SqlParameter { ParameterName = "@Date", Value = distributor_User.FilterDate };
                sqlParameters[2] = new SqlParameter { ParameterName = "@Flag", Value = "P" };
                var dataTable = SqlHelper.GetTableFromSP("USP_DistributorSalesReport", sqlParameters);
                if (dataTable.Rows.Count > 0)
                {
                    serviceRes.Data = dataTable.AsEnumerable().Select(x =>
                          new DistributorSalesReport
                          {
                              SalesCount = x.Field<int>("Quantity"),
                              SalesDate = x.Field<DateTime>("Orderdate")
                          }).ToList();
                    serviceRes.IsSuccess = true;
                    serviceRes.ReturnCode = "200";
                    serviceRes.ReturnMsg = "Success";
                }
                else
                {
                    serviceRes.IsSuccess = false;
                    serviceRes.ReturnCode = "400";
                    serviceRes.ReturnMsg = "Failed";
                }
            }
            catch(Exception ex)
            {
                LogManager.WriteLog(ex);
            }
            return serviceRes;
        }

        public ServiceRes Distributor_DeliveredReport(Distributor_User distributor_User)
        {
            ServiceRes<List<OrderStatus>> serviceRes = new ServiceRes<List<OrderStatus>>();
            try
            {
                SqlParameter[] sqlParameter = new SqlParameter[1];
                sqlParameter[0] = new SqlParameter { ParameterName = "@MemberId", Value = distributor_User.UserId };
                sqlParameter[1] = new SqlParameter { ParameterName = "@Flag", Value = "D" };
                var dataTable = SqlHelper.GetTableFromSP("USP_DistributorSalesReport", sqlParameter);
                if (dataTable.Rows.Count > 0)
                {
                    serviceRes.Data = dataTable.AsEnumerable().Select(x => new OrderStatus
                    {
                        OrderCount = x.Field<int>("ORDER_COUNT"),
                        Status = x.Field<string>("Order_Status")
                    }).ToList();
                    serviceRes.IsSuccess = true;
                    serviceRes.ReturnCode = "200";
                    serviceRes.ReturnMsg = "Success";
                }
                else
                {
                    serviceRes.IsSuccess = false;
                    serviceRes.ReturnCode = "400";
                    serviceRes.ReturnMsg = "Failed";
                }

;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(ex);
            }
            return serviceRes;
        }

        public ServiceRes Distributor_OrdersReport(Distributor_User distributor_User)
        {
            ServiceRes<List<OrderStatus>> serviceRes = new ServiceRes<List<OrderStatus>>();
            try {
                SqlParameter[] sqlParameter = new SqlParameter[2];
                sqlParameter[0] = new SqlParameter { ParameterName = "@MemberId", Value = distributor_User.UserId };
                sqlParameter[1] = new SqlParameter { ParameterName = "@Flag", Value = "O" };
                var dataTable = SqlHelper.GetTableFromSP("USP_DistributorSalesReport", sqlParameter);
                if (dataTable.Rows.Count > 0)
                {
                    serviceRes.Data = dataTable.AsEnumerable().Select(x =>new OrderStatus
                {
                    OrderCount = x.Field<int>("ORDER_COUNT"),
                    Status = x.Field<string>("Order_Status")
                }).ToList();
                    serviceRes.IsSuccess = true;
                    serviceRes.ReturnCode = "200";
                    serviceRes.ReturnMsg = "Success";
                }
                else
                {
                    serviceRes.IsSuccess = false;
                    serviceRes.ReturnCode = "400";
                    serviceRes.ReturnMsg = "Failed";
                }

;            }
            catch(Exception ex)
            {
                LogManager.WriteLog(ex);
            }
            return serviceRes;
        }

        public ServiceRes RecentOrderDetail(Distributor_User distributor_User)
        {
            throw new NotImplementedException();
        }
    }
}
