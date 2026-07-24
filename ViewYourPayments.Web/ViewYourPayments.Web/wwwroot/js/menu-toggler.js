function toggleMenus (targetElems) {
  $('.menuOpen').toggleClass('hidden menuOpen')
  targetElems.forEach(function (element) {
    var $targetElem = $(element)
    // if (element === 'closeOthers') {
    //   $('.menuOpen').toggleClass('hidden menuOpen')
    // }
    $targetElem.toggleClass('hidden menuOpen')
  })
}

$('.toggle-menu').on('click', function (e) {
  e.preventDefault()
  e.stopPropagation()
  var $this = $(this)
  var $body = $('body')
  let targetElems = $this.data('targetElement').split(', ')

  toggleMenus(targetElems)
  if (!$body.hasClass('close-menus')) {
    $body.addClass('close-menus')
  }
})

$(document).on('click', '.close-menus', function (event) {
  let $target = $(event.target)
  const $menuOpen = $('.menuOpen')

  if(!$target.closest('#filters').length && $menuOpen.is(":visible")) {
    $menuOpen.toggleClass('hidden menuOpen')
    $('body').removeClass('close-menus')
  }
})
