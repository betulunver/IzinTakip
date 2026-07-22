// ============================================
// İzin Takip - Google Apps Script Backend
// ============================================
// Bu kodu Google Apps Script editörüne yapıştırın.
// Deploy: Deploy > New deployment > Web app > Execute as: Me, Who has access: Anyone
// Deploy ettikten sonra URL'yi ApiService.cs'deki BaseUrl'e yapıştırın.

const SPREADSHEET_ID = "GOOGLE_SHEET_ID_BURAYA";
const KULLANICILAR_SHEET = "Kullanicilar";
const IZIN_SHEET = "IzinTalepleri";
const KOD_GECERLILIK_DAKIKA = 5;

function doPost(e) {
  try {
    const data = JSON.parse(e.postData.contents);
    const action = data.action;

    switch (action) {
      case "verifyEmail":
        return jsonResponse(verifyEmail(data.email));
      case "confirmCode":
        return jsonResponse(confirmCode(data.email, data.code));
      case "loginOTP":
        return jsonResponse(loginOTP(data.email));
      case "confirmOTP":
        return jsonResponse(confirmOTP(data.email, data.otp));
      case "submitLeave":
        return jsonResponse(submitLeave(data));
      case "getLeaves":
        return jsonResponse(getLeaves(data.email));
      default:
        return jsonResponse({ success: false, message: "Geçersiz işlem." });
    }
  } catch (err) {
    return jsonResponse({ success: false, message: "Sunucu hatası: " + err.message });
  }
}

function jsonResponse(obj) {
  return ContentService
    .createTextOutput(JSON.stringify(obj))
    .setMimeType(ContentService.MimeType.JSON);
}

function generateCode() {
  return Math.floor(100000 + Math.random() * 900000).toString();
}

function getExpiry() {
  return new Date(Date.now() + KOD_GECERLILIK_DAKIKA * 60 * 1000);
}

// ---- E-Posta Doğrulama ----

function verifyEmail(email) {
  email = email.toLowerCase().trim();
  const ss = SpreadsheetApp.openById(SPREADSHEET_ID);
  const sheet = ss.getSheetByName(KULLANICILAR_SHEET);
  const data = sheet.getDataRange().getValues();

  for (let i = 1; i < data.length; i++) {
    if (data[i][0].toString().toLowerCase().trim() === email) {
      if (data[i][3] === true || data[i][3] === "TRUE") {
        return { success: false, message: "Bu e-posta zaten doğrulanmış." };
      }

      const code = generateCode();
      const expiry = getExpiry();
      sheet.getRange(i + 1, 5).setValue(code);
      sheet.getRange(i + 1, 6).setValue(expiry);

      GmailApp.sendEmail(email, "İzin Takip - Doğrulama Kodu",
        "Doğrulama kodunuz: " + code + "\nBu kod " + KOD_GECERLILIK_DAKIKA + " dakika geçerlidir.");

      return { success: true, message: "Doğrulama kodu gönderildi." };
    }
  }

  return { success: false, message: "Bu e-posta sisteme kayıtlı değil." };
}

function confirmCode(email, code) {
  email = email.toLowerCase().trim();
  const ss = SpreadsheetApp.openById(SPREADSHEET_ID);
  const sheet = ss.getSheetByName(KULLANICILAR_SHEET);
  const data = sheet.getDataRange().getValues();

  for (let i = 1; i < data.length; i++) {
    if (data[i][0].toString().toLowerCase().trim() === email) {
      const storedCode = data[i][4].toString();
      const expiry = new Date(data[i][5]);

      if (storedCode !== code) {
        return { success: false, message: "Doğrulama kodu hatalı." };
      }
      if (new Date() > expiry) {
        return { success: false, message: "Doğrulama kodunun süresi dolmuş." };
      }

      sheet.getRange(i + 1, 4).setValue(true);
      sheet.getRange(i + 1, 5).setValue("");
      sheet.getRange(i + 1, 6).setValue("");

      return { success: true, message: "E-posta doğrulaması başarılı." };
    }
  }

  return { success: false, message: "E-posta bulunamadı." };
}

// ---- Giriş (OTP) ----

function loginOTP(email) {
  email = email.toLowerCase().trim();
  const ss = SpreadsheetApp.openById(SPREADSHEET_ID);
  const sheet = ss.getSheetByName(KULLANICILAR_SHEET);
  const data = sheet.getDataRange().getValues();

  for (let i = 1; i < data.length; i++) {
    if (data[i][0].toString().toLowerCase().trim() === email) {
      if (data[i][3] !== true && data[i][3] !== "TRUE") {
        return { success: false, message: "E-posta henüz doğrulanmamış. Önce doğrulama yapın." };
      }

      const otp = generateCode();
      const expiry = getExpiry();
      sheet.getRange(i + 1, 7).setValue(otp);
      sheet.getRange(i + 1, 8).setValue(expiry);

      GmailApp.sendEmail(email, "İzin Takip - Giriş Kodu",
        "Giriş kodunuz: " + otp + "\nBu kod " + KOD_GECERLILIK_DAKIKA + " dakika geçerlidir.");

      return { success: true, message: "Giriş kodu gönderildi." };
    }
  }

  return { success: false, message: "Bu e-posta sisteme kayıtlı değil." };
}

function confirmOTP(email, otp) {
  email = email.toLowerCase().trim();
  const ss = SpreadsheetApp.openById(SPREADSHEET_ID);
  const sheet = ss.getSheetByName(KULLANICILAR_SHEET);
  const data = sheet.getDataRange().getValues();

  for (let i = 1; i < data.length; i++) {
    if (data[i][0].toString().toLowerCase().trim() === email) {
      const storedOtp = data[i][6].toString();
      const expiry = new Date(data[i][7]);

      if (storedOtp !== otp) {
        return { success: false, message: "Giriş kodu hatalı." };
      }
      if (new Date() > expiry) {
        return { success: false, message: "Giriş kodunun süresi dolmuş." };
      }

      sheet.getRange(i + 1, 7).setValue("");
      sheet.getRange(i + 1, 8).setValue("");

      return {
        success: true,
        message: "Giriş başarılı.",
        data: {
          email: data[i][0],
          adSoyad: data[i][1],
          rol: data[i][2],
          dogrulandi: true
        }
      };
    }
  }

  return { success: false, message: "E-posta bulunamadı." };
}

// ---- İzin Talebi ----

function submitLeave(data) {
  const email = data.email.toLowerCase().trim();
  const ss = SpreadsheetApp.openById(SPREADSHEET_ID);
  const sheet = ss.getSheetByName(IZIN_SHEET);
  const existing = sheet.getDataRange().getValues();

  const newStart = new Date(data.baslangicTarihi);
  const newEnd = new Date(data.bitisTarihi);

  for (let i = 1; i < existing.length; i++) {
    if (existing[i][1].toString().toLowerCase().trim() === email) {
      const existStart = new Date(existing[i][5]);
      const existEnd = new Date(existing[i][6]);
      if (newStart <= existEnd && newEnd >= existStart) {
        return { success: false, message: "Bu tarih aralığında zaten bir izin talebiniz var." };
      }
    }
  }

  const id = Utilities.getUuid();
  sheet.appendRow([
    id,
    email,
    data.adSoyad,
    data.birim,
    data.izinSuresi,
    data.baslangicTarihi,
    data.bitisTarihi,
    data.iseDonus,
    data.aciklama || "",
    Utilities.formatDate(new Date(), "GMT+3", "dd.MM.yyyy HH:mm"),
    "Beklemede"
  ]);

  return { success: true, message: "İzin talebi başarıyla kaydedildi." };
}

function getLeaves(email) {
  email = email.toLowerCase().trim();
  const ss = SpreadsheetApp.openById(SPREADSHEET_ID);
  const sheet = ss.getSheetByName(IZIN_SHEET);
  const data = sheet.getDataRange().getValues();
  const leaves = [];

  for (let i = 1; i < data.length; i++) {
    if (data[i][1].toString().toLowerCase().trim() === email) {
      leaves.push({
        id: data[i][0],
        email: data[i][1],
        adSoyad: data[i][2],
        birim: data[i][3],
        izinSuresi: data[i][4],
        baslangicTarihi: data[i][5],
        bitisTarihi: data[i][6],
        iseDonus: data[i][7],
        aciklama: data[i][8],
        olusturmaTarihi: data[i][9],
        durum: data[i][10]
      });
    }
  }

  return { success: true, data: leaves, message: "İzinler getirildi." };
}

// ---- Kurulum ----
// Bu fonksiyonu Apps Script editöründen (Çalıştır > setupDurumDropdown) BİR KEZ
// çalıştırın. "Durum" sütununu (K, 11. sütun) açılır listeye çevirir ve
// mevcut/gelecek 1000 satır için geçerli kılar.
function setupDurumDropdown() {
  const ss = SpreadsheetApp.openById(SPREADSHEET_ID);
  const sheet = ss.getSheetByName(IZIN_SHEET);

  const rule = SpreadsheetApp.newDataValidation()
    .requireValueInList(["Beklemede", "Onaylandı", "Reddedildi"], true)
    .setAllowInvalid(false)
    .build();

  sheet.getRange(2, 11, 1000, 1).setDataValidation(rule);
}
