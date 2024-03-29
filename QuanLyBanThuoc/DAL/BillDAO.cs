﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using QuanLyBanThuoc.DTO;
using System.Data.SqlClient;

namespace QuanLyBanThuoc.DAL
{
   public class BillDAO
    {
        private static BillDAO instance;

        public static BillDAO Instance {
            get { if (instance == null) instance = new BillDAO();
                return BillDAO.instance;
            }
          private set
            {
                BillDAO.instance = value;
            }
        }
        private BillDAO() { }
        public int GetUncheckBillIDByTableID(int id)
        {
            DataTable data = DataProvider.Instance.ExcuteQuery("SELECT * FROM dbo.Bill WHERE IdTable = " + id + "AND Status=0");
            if (data.Rows.Count > 0)
            {
                Bill bill = new Bill(data.Rows[0]);
                return bill.Id;
            }
            return -1;
        }
        public void InsertBill(int id,string userName)
        {
            DataProvider.Instance.ExcuteNonQuery("exec USP_InsertBill @idTable , @UserName", new object[] {id,userName});

        }
        public int GetMaxIDBill()
        {
            try
            {
                return (int)DataProvider.Instance.ExcuteScalar("SELECT MAX(id) FROM dbo.Bill");
            }
            catch
            {
                return 1;
            }
        }
        public void CheckOut(int id,int discount,float totalPrice)
        {
            string query = "UPDATE dbo.Bill SET dateCheckOut = GETDATE(), Status = 1, " + "Discount = " + discount + ", TotalPrice = " + totalPrice + " WHERE Id = " + id;
            DataProvider.Instance.ExcuteNonQuery(query);
        }
        public DataTable GetBillListByDate(DateTime checkIn, DateTime checkOut)
        {
            return DataProvider.Instance.ExcuteQuery("exec USP_GetListBillByDate @checkIn , @checkOut", new object[] { checkIn, checkOut });
        }
        public DataTable GetBillListByDateForReport() 
        {
            string connectionSTR = @"Server=DESKTOP-367SKLK\SQLEXPRESS;Database=QuanLyBanThuoc;User Id=sa;pwd=tienhung091";
            DataTable ds = DataProvider.Instance.ExcuteQuery("SELECT t.Id, b.TotalPrice, DateCheckIn, DateCheckOut, discount FROM dbo.Bill AS b, dbo.TableMedicine AS t WHERE  b.status = 1 AND t.id = b.idTable");
            return ds;
        }
        public void DeleteBillByTableId(int id)
        {
            DataProvider.Instance.ExcuteNonQuery("Delete from dbo.BillInfo where idBill in (select Id from dbo.Bill where IdTable=" + id+")");
            DataProvider.Instance.ExcuteNonQuery("Delete from dbo.Bill Where IdTable=" + id);
        }
        public void DeleteBillByTableIdStatus(int id)
        {
            DataProvider.Instance.ExcuteNonQuery("Delete from dbo.BillInfo where idBill in (select Id from dbo.Bill where IdTable=" + id +"AND Status = 0" + ")");
            DataProvider.Instance.ExcuteNonQuery("Delete from dbo.Bill Where IdTable=" + id + "AND Status = 0");
        }
        public string getDateCheckOutByIdBill(int idBill)
        {
            string query = string.Format("select DateCheckOut from dbo.Bill Where Id = {0}", idBill);

            DataTable table = DataProvider.Instance.ExcuteQuery(query);
            DataRow row = table.Rows[0];
            string result = row["DateCheckOut"].ToString();
           string[] result1 = result.Split(' ');
            return result1[0];
            
        }

    }
}
