
function CheckTabsIfValid(e, activeTab, showToast) {
    switch (activeTab) {
        case basicInfoTab:
            var carProceyType = $("#modal #PROCEDNB").val();
            if (carProceyType == null || carProceyType == "") {
                $("#modal select#PROCEDNB").trigger("chosen:activate");
                toastr.clear();
                if (showToast) toastr.error("يجب تحديد نوع المعاملة", "حقل إجباري");
                if (e != null) e.preventDefault();
                if (e != null) e.stopPropagation();
                $("#modal button.save-btn").attr("disabled", true);
                IsFormValid = false;
                return false;
            }
            else {
                $("#modal button.save-btn").removeAttr("disabled");
            }
            break;
        case questionsInfoTab:

            break;
        case carInfoTab:
            var carProceyType = $("#modal #PROCEDNB").val();
            isValidQ = true;
            if (carProceyType == "@ProcedBase.CarProcedTypeIds.RegisterNewCar") {//تسجيل سيارة جديدة
                if ($("#modal select#NewCarCategoryId").val() == "") {
                    $("#modal select#NewCarCategoryId").trigger("chosen:activate");
                    toastr.clear();
                    if (showToast) toastr.error("يجب تحديد نوع المركبة.", "خطأ");
                    isValidQ = false;
                    if (e != null) e.preventDefault();
                    if (e != null) e.stopPropagation();
                    $("#modal button.save-btn").attr("disabled", true);
                    return false;
                }
                else if ($("#modal select#ZREGSId").val() == "") {
                    $("#modal select#ZREGSId").trigger("chosen:activate");
                    toastr.clear();
                    if (showToast) toastr.error("يجب تحديد الفئة التي ستسجل بها المركبة.", "خطأ");
                    isValidQ = false;
                    if (e != null) e.preventDefault();
                    if (e != null) e.stopPropagation();
                    $("#modal button.save-btn").attr("disabled", true);
                    return false;
                }
                else if ($("#modal select#CUSTTYP").val() == "") {
                    $("#modal select#CUSTTYP").trigger("chosen:activate");
                    toastr.clear();
                    if (showToast) toastr.error("يجب تحديد نوع البيان الجمركي.", "خطأ");
                    isValidQ = false;
                    if (e != null) e.preventDefault();
                    if (e != null) e.stopPropagation();
                    $("#modal button.save-btn").attr("disabled", true);
                    return false;
                }
                else if ($("#modal select#TABTYP").val() == "") {
                    $("#modal select#TABTYP").trigger("chosen:activate");
                    toastr.clear();
                    if (showToast) toastr.error("يجب تحديد نموذج اللوحات.", "خطأ");
                    isValidQ = false;
                    if (e != null) e.preventDefault();
                    if (e != null) e.stopPropagation();
                    $("#modal button.save-btn").attr("disabled", true);
                    return false;
                } else if ($("#modal select#BaseRegId").val() == "") {
                    $("#modal select#BaseRegId").trigger("chosen:activate");

                    toastr.clear();
                    if (showToast) toastr.error("يجب تحديد اساس التسجيل.", "خطأ");
                    isValidQ = false;
                    if (e != null) e.preventDefault();
                    if (e != null) e.stopPropagation();
                    $("#modal button.save-btn").attr("disabled", true);
                    return false;
                }
            }
            var qRows = $("#modal .questionselect");
            $.each(qRows, function (index, element) {
                var zQuestionNB = $(element).closest("tr").find(".zQuestionNB").val();
                if ($(element).is(":checked")) {
                    //---------------------------------------------------------------------------------------
                    if (zQuestionNB + "" == '@ProcedBase.ZQuestionsIds.Question11')//هل تحمل المركبة لوحة مرور أو تجربة
                    {
                        if ($("#modal #TMPTABNB").val() == "") {
                            toastr.clear();
                            if (showToast) toastr.error("يجب تحديد رقم لوحة المرور أو التجربة.", "خطأ");

                            isValidQ = false;
                            if (e != null) e.preventDefault();
                            if (e != null) e.stopPropagation();
                            $("#modal button.save-btn").attr("disabled", true);
                            return false;
                        }
                        else if ($("#modal select#TMPCITY").val() == "") {
                            toastr.clear();
                            if (showToast) toastr.error("يجب تحديد المحافظة المانحة للوحة المرور أو التجربة.", "خطأ");

                            isValidQ = false;
                            if (e != null) e.preventDefault();
                            if (e != null) e.stopPropagation();
                            $("#modal button.save-btn").attr("disabled", true);
                            return false;
                        }
                    }
                    else if (zQuestionNB + "" == '@ProcedBase.ZQuestionsIds.Question3')//هل يوجد وضع إشارة أو رهن
                    {
                        var signs = $("#modal #carSignsTbl > tr");
                        if (signs.length == 0) {
                            toastr.clear();
                            if (showToast) toastr.error("يجب إدخال بيانات الإشارات التي يجب وضعها.", "خطأ");
                            isValidQ = false;
                            if (e != null) e.preventDefault();
                            if (e != null) e.stopPropagation();
                            $("#modal button.save-btn").attr("disabled", true);
                            return false;
                        }
                        else {
                            var signSIEZERVerified = true;
                            $.each(signs, function (index, element) {
                                var signSIEZERNB = $(element).find("input.signSIEZERNB");
                                if (signSIEZERNB.val() == "") {
                                    signSIEZERVerified = false;
                                }
                            });
                            if (!signSIEZERVerified) {
                                toastr.clear();
                                if (showToast) toastr.error("يجب تحديد الراهن أو الحاجز للإشارة المطلوبة.", "خطأ");

                                isValidQ = false;
                                if (e != null) e.preventDefault();
                                if (e != null) e.stopPropagation();
                                $("#modal button.save-btn").attr("disabled", true);
                                return false;
                            }
                        }
                    }
                    else if (zQuestionNB + "" == '@ProcedBase.ZQuestionsIds.Question5') { //هل يوجد تبدّل فني جزئي
                        var carChangeAttribsTblVerified = true;
                        var carChangeAttribs = $("#modal #carChangeAttribsTbl > tr");
                        if (carChangeAttribs.length == 0) {
                            toastr.clear();
                            if (showToast) toastr.error("يجب تحديد نوع التبدّل الفنّي الذي سيطرأ على المركبة في هذه المعاملة.", "خطأ");
                            isValidQ = false;
                            if (e != null) e.preventDefault();
                            if (e != null) e.stopPropagation();
                            $("#modal button.save-btn").attr("disabled", true);
                            return false;
                        } else {
                            $.each(carChangeAttribs, function (index, element) {
                                var TEKNBX = $(element).find(".TEKNBX");
                                if (TEKNBX.val() == "") {
                                    carChangeAttribsTblVerified = false;
                                }
                            });
                            if (!carChangeAttribsTblVerified) {
                                toastr.clear();
                                if (showToast) toastr.error("يجب تحديد نوع التبدّل الفنّي الذي سيطرأ على المركبة في هذه المعاملة.", "خطأ");

                                isValidQ = false;
                                if (e != null) e.preventDefault();
                                if (e != null) e.stopPropagation();
                                $("#modal button.save-btn").attr("disabled", true);
                                return false;
                            }
                        }
                    }
                    //---------------------------------------------------------------------------------------
                    if (zQuestionNB == 1) {
                        if (carProceyType == "@ProcedBase.CarProcedTypeIds.RegisterNewCar") {
                            var carOwnersTbl = $("#modal #carOwnersTbl > tr");
                            var buyersInProcedTbl = $("#modal #buyersInProcedTbl > tr");
                            var carBuyerTbl = $("#modal #carBuyerTbl > tr");
                            try {
                                checkAddedQutasForBuyer();
                            } catch (xx) { }
                            try {
                                checkBaughtQutas();
                            } catch (e) { }
                            if (carOwnersTbl.length == 0) {
                                toastr.clear();
                                if (showToast) toastr.error("<h4>يجب تحديد البائع.</h4>", "<h4><b>خطأ</b></h4>");

                                isValidQ = false;
                                if (e != null) e.preventDefault();
                                if (e != null) e.stopPropagation();
                                $("#modal button.save-btn").attr("disabled", true);
                                return false;
                            }
                            else if (carBuyerTbl.length == 0) {
                                toastr.clear();
                                if (showToast) toastr.error("<h4>يجب تحديد المشتري.</h4>", "<h4><b>خطأ</b></h4>");
                                isValidQ = false;
                                if (e != null) e.preventDefault();
                                if (e != null) e.stopPropagation();
                                $("#modal button.save-btn").attr("disabled", true);
                                return false;
                            }

                            else if (QUATAS != QUATASForBuyer) {

                                toastr.clear();
                                if (showToast) toastr.error("<h4>يجب أن تكون الحصص المشتراة تساوي حصص المالكين .</h4>", "<h4><b>خطأ</b></h4>");

                                isValidQ = false;
                                if (e != null) e.preventDefault();
                                if (e != null) e.stopPropagation();
                                $("#modal button.save-btn").attr("disabled", true);
                                return false;
                            }
                            else {
                                isValidQ = true;
                            }
                            //return false;

                        }
                        else {

                            var carOwnersTbl = $("#modal #carOwnersTbl > tr");
                            var buyersInProcedTbl = $("#modal #buyersInProcedTbl > tr");
                            var carBuyerTbl = $("#modal #carBuyerTbl > tr");
                            try {
                                checkAddedQutasForBuyer();
                            } catch (xx) { }
                            try {
                                checkBaughtQutas();
                            } catch (e) { }
                            if (carOwnersTbl.length == 0) {
                                toastr.clear();
                                if (showToast) toastr.error("<h4>يجب تحديد البائع.</h4>", "<h4><b>خطأ</b></h4>");

                                isValidQ = false;
                                if (e != null) e.preventDefault();
                                if (e != null) e.stopPropagation();
                                $("#modal button.save-btn").attr("disabled", true);
                                return false;
                            }
                            else if (carBuyerTbl.length == 0) {
                                toastr.clear();
                                if (showToast) toastr.error("<h4>يجب تحديد المشتري.</h4>", "<h4><b>خطأ</b></h4>");

                                isValidQ = false;
                                if (e != null) e.preventDefault();
                                if (e != null) e.stopPropagation();
                                $("#modal button.save-btn").attr("disabled", true);
                                return false;
                            }

                            else if (BaughtQutas != QUATASForBuyer) {

                                toastr.clear();
                                if (showToast) toastr.error("<h4>يجب أن تكون الحصص المشتراة تساوي الحصص المباعة.</h4>", "<h4><b>خطأ</b></h4>");

                                isValidQ = false;
                                $("#modal button.save-btn").attr("disabled", true);
                                if (e != null) e.preventDefault();
                                if (e != null) e.stopPropagation();
                                return false;
                            }
                            else { isValidQ = true; }

                        }
                    }
                }
            });
            if (carProceyType == "@ProcedBase.CarProcedTypeIds.RegisterNewCar") {//تسجيل سيارة جديدة
                checkAddedQutas();
                if (QUATAS != 2400) {
                    toastr.clear();
                    if (showToast) toastr.error("يجب أن يكون مجموع حصص مالكي هذه المركبة يساوي 2400 سهماً تماماً.", "خطأ");
                    isValidQ = false;
                    $("#modal button.save-btn").attr("disabled", true);
                    //if(e) e.preventDefault();
                    //if(e) e.stopPropagation();
                    //return false;
                }
            } else {
                var carNb = $("#modal #CARNB").val();
                if (!carNb) {
                    toastr.clear();
                    $("#modal button.save-btn").attr("disabled", true);
                    if (showToast) toastr.error("يجب تحديد المركبة", "خطأ");
                    isValidQ = false;
                    if (e != null) e.preventDefault();
                    if (e != null) e.stopPropagation();
                    //return false;
                }
            }
            if (isValidQ) {
                $("#modal button.save-btn").removeAttr("disabled");
            }
            else {
                $("#modal button.save-btn").attr("disabled", true);
                if (e != null) e.preventDefault();
                if (e != null) e.stopPropagation();
                return false;
            }
            break;
        case demandInfoTab:
            var selectedOwnerNb = $("#modal #OWNERNB").val();
            if (selectedOwnerNb == "" || selectedOwnerNb == null) {
                toastr.clear();
                if (showToast) toastr.error("يجب تحديد مقدّم الطلب لهذه المعاملة.", "خطأ");
                if (e != null) e.preventDefault();
                if (e != null) e.stopPropagation();
                $("#modal button.save-btn").attr("disabled", true);
                return false;
            } else {
                $("#modal button.save-btn").removeAttr("disabled");
            }
            break;
        case docsInfoTab:
            var docs = $("#modal #xDocsBody > tr");
            var docsIsValid = true;
            if (docs.length != 0) {
                $.each(docs, function (index, element) {
                    var docNo = $(element).find('.DOCNO').val();
                    var docSource = $(element).find('.DOCSOURCE').val();
                    if (docNo == "" || docSource == "") {
                        docsIsValid = false;
                    }
                });
                if (!docsIsValid) {
                    $("#modal button.save-btn").attr("disabled", true);
                    toastr.clear();
                    if (showToast) toastr.error("يجب تحديد الرقم والجهة المصدرة لجميع الوثائق الإجبارية.", "خطأ");
                    if(e) e.preventDefault();
                    if (e != null) e.stopPropagation();
                    return false;
                } else {
                    $("#modal button.save-btn").removeAttr("disabled");
                }
            }
            break;
        case proxyInfoTab:

            break;
    }
}