using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OfficeOpenXml;
using System.IO;
using System;
//频繁的IO操作降低了帧率，怎么办？
public class SaveData : MonoBehaviour
{
    public string name;
    private int index;//写入数据在Excel中的列数索引
    public StringButtonController stringbutton;
    public string time;

    //public string data = System.DateTime.Now;
    // Start is called before the first frame update
    void Start()
    {
        DateTime NowTime = DateTime.Now.ToLocalTime();
        time = NowTime.ToString("yyyy-MM-dd HH;mm;ss");

        index = 1;
        //创建一个新Excel文件用于保存实验数据
        string filePath = "C:/Research/Data/ADHD/0514/" + name + stringbutton.w + "N " + stringbutton.reactTime + "S " + "ForceCurve " + time + ".xlsx"; 
        // string filePath = "E:/毕业相关-冯万玮/shiyanPrecion/test/1.xlsx";
       
        FileInfo fileInfo = new FileInfo(filePath);
        //excelPackage = new ExcelPackage(fileInfo);
        //打开Excel文件
        using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
        {
            //创建一张表
            excelPackage.Workbook.Worksheets.Add("sheet1");
            excelPackage.Workbook.Worksheets.Add("sheet2");
            //获取表对象
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[1];
            //保存文件
            excelPackage.Save();
        }



    }
    public void writeExcel(float success, float reactTime, ArrayList force)
    {

        //记录数据
        //打开Excel文件用于保存实验数据
        //string filePath = "E:/毕业相关-冯万玮/shiyanPrecion/test/1.xlsx";
        string filePath = "C:/Research/Data/ADHD/0514/" + name + stringbutton.w + "N " + stringbutton.reactTime + "S " + "ForceCurve " + time + ".xlsx";
        FileInfo fileInfo = new FileInfo(filePath);

        //打开Excel文件
        using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
        {

            //获取表对象
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets[1];
            //写入表格
            worksheet.Cells[1, index].Value = success;//写入游戏难度
            worksheet.Cells[2, index].Value = reactTime;//写入反应时间

            //获取表对象
            ExcelWorksheet worksheet2 = excelPackage.Workbook.Worksheets[2];
            //写入表格
            for (int i = 0; i < force.Count ; i++)
            {
                worksheet2.Cells[i+1,index].Value = force[i];
            }

            //
            index++;

            //保存文件
            excelPackage.Save();
        }
        //Debug.Log("被试数据存储完毕...");
    }


}
