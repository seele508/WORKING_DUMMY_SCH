using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchTracer.Models
{
    public class Data_Map
    {
        string REF_TO_CAPTURE, PREVIOUS_REV, ORI_MT103_REF, inout, correspondent, currency, tag20, mt_103, accountnum, accountname, tanggal, rowid_ref_to_capture, rowid_previous_rev, rowid_ori_mt103_ref, loadDateCapture, loadDateOri, typeCapture, typeOri, inOutPrev, inOutOri;
        int isKerjasama;
        float amount;

        public Data_Map(String REF_TO_CAPTURE, String PREVIOUS_REV, String ORI_MT103_REF, int isKerjasama, String inout, String correspondent, String currency, String tag20, String mt_103, float amount, String accountnum, String accountname, String tanggal, String rowid_ref_to_capture, String rowid_previous_rev, String rowid_ori_mt103_ref, String dateCapture, String dateOri, String typeCapture, String typeOri, String inOutPrev, String inOutOri)
        {
            this.ORI_MT103_REF = ORI_MT103_REF;
            this.PREVIOUS_REV = PREVIOUS_REV;
            this.REF_TO_CAPTURE = REF_TO_CAPTURE;
            this.inout = inout;
            this.correspondent = correspondent;
            this.isKerjasama = isKerjasama;
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
            this.loadDateCapture = dateCapture;
            this.loadDateOri = dateOri;
            this.typeCapture = typeCapture;
            this.typeOri = typeOri;
            this.inOutPrev = inOutPrev;
            this.inOutOri = inOutOri;
        }

        public String getREF_TO_CAPTURE()
        {
            return REF_TO_CAPTURE;
        }

        public String getPREVIOUS_REV()
        {
            return PREVIOUS_REV;
        }

        public String getORI_MT103_REF()
        {
            return ORI_MT103_REF;
        }

        public String getInOut()
        {
            return inout;
        }

        public String getCorrespondent()
        {
            return correspondent;
        }

        public int getIsKerjasama()
        {
            return isKerjasama;
        }

        public String getMt_103()
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

        public String getAccountnum()
        {
            return accountnum;
        }

        public String getAccountname()
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

        public String getLoadDateToCapture()
        {
            return loadDateCapture;
        }

        public String getLoadDateOri()
        {
            return loadDateOri;
        }

        public String getTypeCapture()
        {
            return typeCapture;
        }

        public String getTypeOri()
        {
            return typeOri;
        }

        public String getInOutPrev()
        {
            return inOutPrev;
        }

        public String getInOutOri()
        {
            return inOutOri;
        }

    }
}
