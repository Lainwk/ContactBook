// 联系人管理系统 - 客户端脚本

// 页面加载完成后执行
$(document).ready(function () {
    // 自动隐藏提示消息
    setTimeout(function () {
        $('.alert').fadeOut('slow');
    }, 5000);

    // 确认删除对话框
    $('.btn-danger[href*="Delete"]').click(function (e) {
        if (!$(this).closest('form').length) {
            if (!confirm('确定要删除吗？此操作无法撤销！')) {
                e.preventDefault();
            }
        }
    });

    // 表单提交前验证
    $('form').submit(function () {
        var submitBtn = $(this).find('button[type="submit"]');
        submitBtn.prop('disabled', true);
        submitBtn.html('<span class="loading"></span> 处理中...');
    });

    // 搜索框自动聚焦
    if ($('input[name="searchTerm"]').length) {
        $('input[name="searchTerm"]').focus();
    }

    // 工具提示初始化
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
});

// 收藏功能
function toggleFavorite(contactId) {
    $.ajax({
        url: '/Contact/ToggleFavorite',
        type: 'POST',
        data: {
            id: contactId,
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (response) {
            if (response.success) {
                location.reload();
            } else {
                alert('操作失败：' + response.message);
            }
        },
        error: function () {
            alert('操作失败，请稍后重试。');
        }
    });
}

// 显示加载动画
function showLoading() {
    $('body').append('<div class="loading-overlay"><div class="loading"></div></div>');
}

// 隐藏加载动画
function hideLoading() {
    $('.loading-overlay').remove();
}

// 格式化日期
function formatDate(date) {
    var d = new Date(date);
    var year = d.getFullYear();
    var month = ('0' + (d.getMonth() + 1)).slice(-2);
    var day = ('0' + d.getDate()).slice(-2);
    var hour = ('0' + d.getHours()).slice(-2);
    var minute = ('0' + d.getMinutes()).slice(-2);
    return year + '-' + month + '-' + day + ' ' + hour + ':' + minute;
}

// 复制到剪贴板
function copyToClipboard(text) {
    var temp = $('<input>');
    $('body').append(temp);
    temp.val(text).select();
    document.execCommand('copy');
    temp.remove();
    alert('已复制到剪贴板：' + text);
}
