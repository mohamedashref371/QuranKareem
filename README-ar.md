# Quran Kareem v1.3
مع ٤٣ شيخ و١١ تفسير

متطلبات تشغيل البرنامج: [.NET Framework 4.8](https://go.microsoft.com/fwlink/?linkid=2088631)

لتحميل هذا البرنامج مع الكود (١٢٥مب): ([إصدار تجريبي](https://github.com/mohamedashref371/QuranKareem/archive/refs/heads/master.zip))<br>
تصفح هذا المستودع: [Quran Kareem Resources](https://www.mediafire.com/folder/w5gcahn1z9pdx)

لم أهتم بتصميم واجهة البرنامج أو إضافة نوافذ متعددة بعد، وبدلاً من ذلك استخدمت مكتبة Guna UI 2.0.

لرغبتي في إحياء فكرة من عام 2002 ([Quran-CD-Roaya-5](https://archive.org/download/QuranCDRoaya5/Quran-CD-Roaya-5.iso))، قمت ببناء نموذج قاعدة بيانات SQLite موحد في الغالب.

ولكي أضمن لنفسي بيانات سليمة، قمت بما يلي:

 • لقد استخدمت [UthmanicHafs_v2-1](https://fonts.qurancomplex.gov.sa/wp02/حفص) من [مجمع الملك فهد لطباعة المصحف الشريف](https://qurancomplex.gov.sa/) للحصول على المصحف المكتوب وتحويله إلى [sqlite database](https://github.com/mohamedashref371/QuranKareem/blob/master/data/texts/حفص%20عن%20عاصم.db).

 • لقد قمت باستخراج التفاسير مع بعض التعديلات من [KSU Ayat v1.4](https://quran.ksu.edu.sa/ayat/) و [Tanzil.net](https://tanzil.net/) إلى نموذج قاعدة البيانات الخاصة بي.

 • لقد استخدمت ملفات json التي يوفرها هذا الرابط للحصول على توقيت نهاية الآيات: https://api.qurancdn.com/api/qdc/audio/reciters/{sheikh_number}/audio_files?chapter={surah_number}&segments=true

'فكرة التلاوة مع تحديد الكلمات' أُخذت من [Quran.com](https://quran.com)<br>
بما أني اقتبست مواقيت الكلمات من موقع [Quran.com](https://quran.com)، فسوف أعتمد حسابهم لكلمات القرآن وهو 77430 كلمة ثم قمت بفصل كلمة بعدما إلى كلمتين.

يمكنك الآن إضافة أي شيخ تريده بكل سهولة وسلاسة من خلال البرنامج.<br>
قواعد البيانات قابلة للقراءة للعامة، لذا يمكنك إضافة المشايخ بأي طريقة أخرى تفضلها.

البرنامج الحالي مجرد فكرة قابلة للتحسين والتطوير.

تم إضافة ميزة الوضع الليلي.
************************************

لتسمية ملفات السور: [هنا](https://github.com/mohamedashref371/Naming-Surahs)

أحب أن أشكر [موقع codeproject](https://www.codeproject.com/) على [FastPixel Class](https://www.codeproject.com/Articles/15192/FastPixel-A-much-faster-alternative-to-Bitmap-SetP)

تحميل [قارئ قواعد البيانات](https://github.com/pawelsalawa/sqlitestudio/releases).