using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchTracer.Models
{
    public class Data_MT
    {
        String type_mt, mt, reff, inout, correspondent, 
            currency, tag20, mt_103, accountnum, accountname, 
            tanggal, rowid_ref_to_capture, rowid_previous_rev, rowid_ori_mt103_ref, loadDate, 
            type, source_base;
        int isKerjasama;
        float amount;

        public Data_MT(String type_mt, String mt, String reff, int isKerjasama, String inout, String correspondent, String currency, String tag20, String mt_103, float amount, String accountnum, String accountname, String tanggal)
        {
            this.type_mt = type_mt;
            this.mt = mt;
            this.reff = reff;
            this.inout = inout;
            this.isKerjasama = isKerjasama;
            this.correspondent = correspondent;
            this.currency = currency;
            this.tag20 = tag20;
            this.mt_103 = mt_103;
            this.amount = amount;
            this.accountnum = accountnum;
            this.accountname = accountname;
            this.tanggal = tanggal;
            this.rowid_ref_to_capture = "0";
            this.rowid_previous_rev = "0";
            this.rowid_ori_mt103_ref = "0";
        }

        public Data_MT(String type_mt, String mt, String reff, int isKerjasama, String inout, String correspondent, String currency, String tag20, String mt_103, float amount, String accountnum, String accountname, String tanggal, String rowid_ref_to_capture, String rowid_previous_rev, String rowid_ori_mt103_ref, String loadDate, String type)
        {
            this.type_mt = type_mt;
            this.mt = mt;
            this.reff = reff;
            this.inout = inout;
            this.isKerjasama = isKerjasama;
            this.correspondent = correspondent;
            this.currency = currency;
            this.tag20 = tag20;
            this.mt_103 = mt_103;
            this.amount = amount;
            this.accountnum = accountnum;
            this.accountname = accountname;
            this.tanggal = tanggal;
            this.rowid_ref_to_capture = rowid_ref_to_capture;
            this.rowid_previous_rev = rowid_previous_rev;
            this.rowid_ori_mt103_ref = rowid_ori_mt103_ref;
            this.loadDate = loadDate;
            this.type = type;
        }

        public Data_MT(String type_mt, String mt, String reff, int isKerjasama, String inout, String correspondent, String currency, String tag20, String mt_103, float amount, String accountnum, String accountname, String tanggal, String rowid_ref_to_capture, String rowid_previous_rev, String rowid_ori_mt103_ref, String loadDate, String type, String source_base)
        {
            this.type_mt = type_mt;
            this.mt = mt;
            this.reff = reff;
            this.inout = inout;
            this.isKerjasama = isKerjasama;
            this.correspondent = correspondent;
            this.currency = currency;
            this.tag20 = tag20;
            this.mt_103 = mt_103;
            this.amount = amount;
            this.accountnum = accountnum;
            this.accountname = accountname;
            this.tanggal = tanggal;
            this.rowid_ref_to_capture = rowid_ref_to_capture;
            this.rowid_previous_rev = rowid_previous_rev;
            this.rowid_ori_mt103_ref = rowid_ori_mt103_ref;
            this.loadDate = loadDate;
            this.type = type;
            this.source_base = source_base;
        }


        public String getType_MT()
        {
            return type_mt;
        }

        public String getType()
        {
            return type;
        }

        public String getMT()
        {
            return mt;
        }

        public String getReff()
        {
            return reff;
        }

        public String getInOut()
        {
            return inout;
        }

        public int getIsKerjasama()
        {
            return isKerjasama;
        }

        public String getCorrespondent()
        {
            return correspondent;
        }

        public String getMT_103()
        {
            return mt_103;
        }

        public String getCurrency()
        {
            return currency;
        }

        public String getTag20()
        {
            return tag20;
        }

        public float getAmount()
        {
            return amount;
        }

        public String getAccountNum()
        {
            return accountnum;
        }

        public String getAccountName()
        {
            return accountname;
        }

        public String getTanggal()
        {
            return tanggal;
        }

        public String getRowid_ref_to_capture()
        {
            return rowid_ref_to_capture;
        }

        public String getRowid_previous_rev()
        {
            return rowid_previous_rev;
        }

        public String getRowid_ori_mt103_ref()
        {
            return rowid_ori_mt103_ref;
        }

        public String getLoad_date()
        {
            return loadDate;
        }

        public String getSource_base()
        {
            return source_base;
        }
    }
}
