using System.Data;
using System.Linq;
using Cadmus.Helpers;
using Cadmus.Models;
using Cadmus.ViewModels.Labels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uranus;
// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo

namespace Gigantomachy;

[TestClass]
public class CadmusTests
{
    [TestMethod]
    public void TestPrinting()
    {
        //LabelsToOneNote();
        //LabelsToLabelPrinter(2);
        //DocumentsToPrinter();
    }

    private static void LabelsToOneNote()
    {
        // Arrange
        var dt = DataConversion.RawStringToTable(REC_LABEL_TEST_STRING);

        var labelVMs = (from DataRow row in dt.Rows
                        let takeZone = row["Zone"].ToString() ?? ""
                        let takeBin = row["Bin"].ToString() ?? ""
                        let caseQty = int.Parse(row["Case"].ToString() ?? "")
                        let packQty = int.Parse(row["Pack"].ToString() ?? "")
                        let eachQty = int.Parse(row["Each"].ToString() ?? "")
                        let qpc = int.Parse(row["QPC"].ToString() ?? "")
                        let qpp = int.Parse(row["QPP"].ToString() ?? "")
                        let barcode = row["Barcode"].ToString() ?? ""
                        let item = int.Parse(row["Item"].ToString() ?? "")
                        let labelNo = int.Parse(row["LabelNo"].ToString() ?? "")
                        let labelTotal = int.Parse(row["LabelTotal"].ToString() ?? "")
                        let description = row["Description"].ToString() ?? ""
                        select new ReceivingPutAwayLabel(takeZone: takeZone, takeBin: takeBin, caseQty: caseQty, packQty: packQty,
                            eachQty: eachQty, qtyPerCase: qpc, qtyPerPack: qpp, barcode: barcode, itemNumber: item,
                            labelNumber: labelNo, labelTotal: labelTotal, description: description)
            into label
                        select new ReceivingPutAwayLabelVM(label)).ToList();

        // Act
        PrintUtility.PrintLabels("OneNote for Windows 10", labelVMs);

        // Assert
        Assert.AreEqual(1, 2 - 1, "Basic maths is wrong.");

    }

    private static void LabelsToLabelPrinter(int count)
    {
        // Arrange
        var dt = DataConversion.RawStringToTable(REC_LABEL_TEST_STRING);

        var labelVMs = (from DataRow row in dt.Rows
                        let takeZone = row["Zone"].ToString() ?? ""
                        let takeBin = row["Bin"].ToString() ?? ""
                        let caseQty = int.Parse(row["Case"].ToString() ?? "")
                        let packQty = int.Parse(row["Pack"].ToString() ?? "")
                        let eachQty = int.Parse(row["Each"].ToString() ?? "")
                        let qpc = int.Parse(row["QPC"].ToString() ?? "")
                        let qpp = int.Parse(row["QPP"].ToString() ?? "")
                        let barcode = row["Barcode"].ToString() ?? ""
                        let item = int.Parse(row["Item"].ToString() ?? "")
                        let labelNo = int.Parse(row["LabelNo"].ToString() ?? "")
                        let labelTotal = int.Parse(row["LabelTotal"].ToString() ?? "")
                        let description = row["Description"].ToString() ?? ""
                        select new ReceivingPutAwayLabel(takeZone: takeZone, takeBin: takeBin, caseQty: caseQty, packQty: packQty,
                            eachQty: eachQty, qtyPerCase: qpc, qtyPerPack: qpp, barcode: barcode, itemNumber: item,
                            labelNumber: labelNo, labelTotal: labelTotal, description: description)
            into label
                        select new ReceivingPutAwayLabelVM(label)).ToList();

        if (labelVMs.Count > count) labelVMs.RemoveRange(count, labelVMs.Count - count);

        // Act
        PrintUtility.PrintLabels("\\\\AUSEFPPS01.ebusa.com\\Intermec PM43 (203 dpi) - DP", labelVMs);

        // Assert
        Assert.AreEqual(1, 2 - 1, "Basic maths is wrong.");
    }

    private static void DocumentsToPrinter()
    {
        // Arrange
        const string s = @"Zone	Bin	Case	Pack	Each	QPC	QPP	Barcode	Item	LabelNo	LabelTotal	Description
OZ	I AH 3	30	0	0	6	0	Í;[4!Î	275920	1	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AH 3	30	0	0	6	0	Í;[4!Î	275920	2	2	TC POKE CELEBRATIONS PRE FIG";

        var dt = DataConversion.RawStringToTable(s);

        var labelVMs = (from DataRow row in dt.Rows
                        let takeZone = row["Zone"].ToString() ?? ""
                        let takeBin = row["Bin"].ToString() ?? ""
                        let caseQty = int.Parse(row["Case"].ToString() ?? "")
                        let packQty = int.Parse(row["Pack"].ToString() ?? "")
                        let eachQty = int.Parse(row["Each"].ToString() ?? "")
                        let qpc = int.Parse(row["QPC"].ToString() ?? "")
                        let qpp = int.Parse(row["QPP"].ToString() ?? "")
                        let barcode = row["Barcode"].ToString() ?? ""
                        let item = int.Parse(row["Item"].ToString() ?? "")
                        let labelNo = int.Parse(row["LabelNo"].ToString() ?? "")
                        let labelTotal = int.Parse(row["LabelTotal"].ToString() ?? "")
                        let description = row["Description"].ToString() ?? ""
                        select new ReceivingPutAwayLabel(takeZone: takeZone, takeBin: takeBin, caseQty: caseQty, packQty: packQty,
                            eachQty: eachQty, qtyPerCase: qpc, qtyPerPack: qpp, barcode: barcode, itemNumber: item,
                            labelNumber: labelNo, labelTotal: labelTotal, description: description)
            into label
                        select new ReceivingPutAwayLabelVM(label)).ToList();

        // Act
        //PrintUtility.PrintDocuments("\\\\aubrsisprint\\AUBRPDC002", labelVMs);

        // Assert
        Assert.AreEqual(1, 2 - 1, "Basic maths is wrong.");

    }

    private const string REC_LABEL_TEST_STRING = @"Zone	Bin	Case	Pack	Each	QPC	QPP	Barcode	Item	LabelNo	LabelTotal	Description
OZ	I AG 3	12	0	0	54	0	Í4Å9uÎ	209725	1	1	PLUSH MINEC OCELOT 14IN
OZ	I AH 3	30	0	0	6	0	Í;[4!Î	275920	1	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AH 3	30	0	0	6	0	Í;[4!Î	275920	2	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AK 2	30	0	0	6	0	Í;[4!Î	275920	1	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AK 2	30	0	0	6	0	Í;[4!Î	275920	2	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AL 2	30	0	0	6	0	Í;[4!Î	275920	1	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AL 2	30	0	0	6	0	Í;[4!Î	275920	2	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AM 2	30	0	0	6	0	Í;[4!Î	275920	1	2	TC POKE CELEBRATIONS PRE FIG
OZ	I AM 2	30	0	0	6	0	Í;[4!Î	275920	2	2	TC POKE CELEBRATIONS PRE FIG
OZ	G AB 3	24	0	0	24	0	Í4Å;{Î	209727	1	2	PLUSH MINEC WOLF 15IN
OZ	G AB 3	24	0	0	24	0	Í4Å;{Î	209727	2	2	PLUSH MINEC WOLF 15IN
OZ	F BF 3	25	0	0	6	0	Í;Ya=Î	275765	1	2	STAT HALO MASTER CHIEF W/GRAP
OZ	F BF 3	25	0	0	6	0	Í;Ya=Î	275765	2	2	STAT HALO MASTER CHIEF W/GRAP
OZ	G AA 2	0	36	0	0	0	Í:FuRÎ	263885	1	2	REP MARV THOR STORMBREAKER
OZ	G AA 2	0	36	0	0	0	Í:FuRÎ	263885	2	2	REP MARV THOR STORMBREAKER
OZ	G AB 2	0	36	0	0	0	Í:FuRÎ	263885	1	2	REP MARV THOR STORMBREAKER
OZ	H AF 3	16	0	0	20	0	Í7?crÎ	233167	2	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AG 3	16	0	0	20	0	Í7?crÎ	233167	1	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AG 3	16	0	0	20	0	Í7?crÎ	233167	2	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AH 3	16	0	0	20	0	Í7?crÎ	233167	1	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AH 3	16	0	0	20	0	Í7?crÎ	233167	2	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AI 3	16	0	0	20	0	Í7?crÎ	233167	1	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AI 3	16	0	0	20	0	Í7?crÎ	233167	2	2	COS MINE/C DIAMOND/S ENDER/P
OZ	H AA 2	23	0	0	36	6	Í;.%hÎ	271405	1	1	POP SW MANDO CHILD BUTTERFLY
OZ	H AV 4	63	2	0	2	0	Í:zH3Î	269040	1	2	REP SW MANDALORIAN HELMET
OZ	H AV 4	63	2	0	2	0	Í:zH3Î	269040	2	2	REP SW MANDALORIAN HELMET
OZ	H AV 5	64	0	0	2	0	Í:zH3Î	269040	1	2	REP SW MANDALORIAN HELMET
OZ	H AV 5	64	0	0	2	0	Í:zH3Î	269040	2	2	REP SW MANDALORIAN HELMET
OZ	H AW 2	64	0	0	2	0	Í:zH3Î	269040	1	2	REP SW MANDALORIAN HELMET
OZ	H AW 2	64	0	0	2	0	Í:zH3Î	269040	2	2	REP SW MANDALORIAN HELMET
OZ	F AR 4	16	0	0	2	0	Í:'jZÎ	260774	1	1	PLUSH POKE GENGAR 24IN
OZ	F AR 5	16	0	0	2	0	Í:'jZÎ	260774	1	1	PLUSH POKE GENGAR 24IN
OZ	F AS 4	16	0	0	2	0	Í:'jZÎ	260774	1	1	PLUSH POKE GENGAR 24IN
OZ	F AS 5	16	0	0	2	0	Í:'jZÎ	260774	1	1	PLUSH POKE GENGAR 24IN
OZ	F AT 5	16	0	0	2	0	Í:'jZÎ	260774	1	1	PLUSH POKE GENGAR 24IN
OZ	F AS 1	24	0	0	2	0	Í:'jZÎ	260774	1	2	PLUSH POKE GENGAR 24IN
OZ	F AS 1	24	0	0	2	0	Í:'jZÎ	260774	2	2	PLUSH POKE GENGAR 24IN
OZ	F AS 2	24	0	0	2	0	Í:'jZÎ	260774	1	2	PLUSH POKE GENGAR 24IN
OZ	F AS 2	24	0	0	2	0	Í:'jZÎ	260774	2	2	PLUSH POKE GENGAR 24IN
OZ	F AT 1	24	0	0	2	0	Í:'jZÎ	260774	1	2	PLUSH POKE GENGAR 24IN
OZ	F AT 1	24	0	0	2	0	Í:'jZÎ	260774	2	2	PLUSH POKE GENGAR 24IN
OZ	G AE 1	24	0	0	2	0	Í:'jZÎ	260774	1	2	PLUSH POKE GENGAR 24IN
OZ	G AE 1	24	0	0	2	0	Í:'jZÎ	260774	2	2	PLUSH POKE GENGAR 24IN
OZ	G AG 3	29	0	0	8	0	Í9<_bÎ	252863	1	1	RING FIT ADVENTURE NS
OZ	F BL 1	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BO 1	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BO 2	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BP 1	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BP 2	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BQ 1	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BQ 2	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BR 1	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	F BR 2	24	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
OZ	H AC 1	26	0	0	4	2	Í:}F3Î	269338	1	1	GLASS POKE 25 KANTO 4PK
OZ	ASPLEY1	180	0	0	6	0	Í9""KYÎ	250243	1	1	LAMP ZELDA MASTER SWORD
OZ	ASPLEY7	450	0	0	4	2	Í;""3zÎ	270219	1	1	GLASS SW VADER'S FREE TIME 4PK
OZ	ASPLEY6	528	0	0	2	0	Í;BX[Î	273456	1	1	PLUSH POKE EEVEE HOLIDAY 24IN
";

}