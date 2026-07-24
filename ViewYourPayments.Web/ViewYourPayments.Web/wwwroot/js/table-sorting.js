// // $(function () {
// //   var $container = $('#isotope').isotope({
// //     itemSelector: '.tb-data-single',
// //     layoutMode: 'vertical',
// //     transitionDuration: 0,
// //     getSortData: {
// //       transactionDate: function (itemElem) {
// //         var transactionDate = $(itemElem).find('.transactionDate').text()
// //         return Date.parse(transactionDate)
// //       },
// //       contractNumber: '[data-contractNumber]',
// //       fundingGroup: function (itemElem) { // function
// //         var fundingGoup = $(itemElem).find('.fundingGroup').text()
// //         return fundingGoup.replace(' ', '-')
// //       },
// //       description: '[data-description]',
// //       transactionAmount: function (itemElem) { // function
// //         var amountString = $(itemElem).find('.transactionAmount').text()
// //         amountValue = Number(amountString.replace(/(\£|,)/g, ''))
// //         console.log(amountValue)
// //         return amountValue
// //       }
// //     }
// //   })
// //
// //   var filters = {}
// //
// //   $('#filters select').on('change', function () {
// //     var $this = $(this)
// //     // get group key
// //     var filterGroup = $this.attr('data-filter-group')
// //     // set filter for group
// //     // filters[filterGroup] = $this.value;
// //     filters[filterGroup] = $this.find(':selected').attr('data-filter')
// //     // combine filters
// //     var filterValue = ''
// //     for (var prop in filters) {
// //       filterValue += filters[prop]
// //     }
// //     // set filter for Isotope
// //     $container.isotope({
// //       filter: filterValue
// //     })
// //   })
// //
// //   // bind sort button click
// //   $('#sorts').on('click', 'button', function () {
// //     var sortValue = $(this).attr('data-sort-value')
// //     $container.isotope({
// //       sortBy: sortValue
// //     })
// //   })
// //
// //   // add is-checked class
// //   $('.btn-group').each(function (i, buttonGroup) {
// //     var $buttonGroup = $(buttonGroup)
// //     $buttonGroup.on('click', 'button', function () {
// //       $buttonGroup.find('.is-checked').removeClass('is-checked')
// //       $buttonGroup.find('i').removeClass()
// //       $(this).addClass('is-checked')
// //       $(this).find('i').addClass('fa')
// //
// //       if ($(this).hasClass('desc')) {
// //         $(this).removeClass('desc').addClass('asc')
// //         $(this).find('.fa').removeClass('fa-sort-amount-asc').addClass('fa-sort-amount-desc')
// //         $container.isotope({
// //           sortAscending: false
// //         })
// //       } else {
// //         $(this).removeClass('asc').addClass('desc')
// //         $(this).find('.fa').removeClass('fa-sort-amount-desc').addClass('fa-sort-amount-asc')
// //         $container.isotope({
// //           sortAscending: true
// //         })
// //       }
// //     })
// //   })
// // })
// //
// // TESTING BELOW HERE
// $(function () {
//   var $container = $('#isotope2').isotope({
//     itemSelector: '.tb-data-single',
//     layoutMode: 'vertical',
//     transitionDuration: 0,
//     // getSortData: {
//     //   transactionDate: function (itemElem) {
//     //     var transactionDate = $(itemElem).find('.transactionDate').text()
//     //     return Date.parse(transactionDate)
//     //   },
//     //   contractNumber: '[data-contractNumber]',
//     //   fundingGroup: function (itemElem) { // function
//     //     var fundingGoup = $(itemElem).find('.fundingGroup').text()
//     //     return fundingGoup.replace(' ', '-')
//     //   },
//     //   description: '[data-description]',
//     //   transactionAmount: function (itemElem) { // function
//     //     var amountString = $(itemElem).find('.transactionAmount').text()
//     //     var amountValue = Number(amountString.replace(/(\£|,)/g, ''))
//     //     return amountValue
//     //   }
//     // }
//   })
//
//   var filters = {}
//
//   $('#filters .govuk-filter-group__submenu__menu-item').on('click', function (e) {
//     e.preventDefault()
//     var $this = $(this)
//     // get group key
//     var filterGroup = $this.parent('.govuk-filter-group__submenu').attr('data-filter-group')
//
//     // set filter for group
//     // filters[filterGroup] = $this.value;
//     filters[filterGroup] = $this.data('filter')
//
//     // combine filters
//     var filterValue = ''
//     for (var prop in filters) {
//       filterValue += filters[prop]
//     }
//     console.log(filterValue)
//
//     // set filter for Isotope
//     $container.isotope({
//       filter: filterValue
//     })
//   })
//
//
//   // bind sort button click
//   $('#sorts2').on('click', 'th.govuk-table__header', function (e) {
//     e.preventDefault()
//     var sortValue = $(this).attr('data-sort-value')
//     console.log(sortValue)
//     $container.isotope({
//       sortBy: sortValue
//     })
//   })
//
//   // add is-checked class
//
//   $('.btn-group').each(function (i, buttonGroup) {
//     var $buttonGroup = $(buttonGroup)
//     $buttonGroup.on('click', 'th.govuk-table__header', function () {
//       var sortValue = $(this).attr('data-sort-value')
//       $buttonGroup.find('.is-checked').removeClass('is-checked')
//       $buttonGroup.find('i').removeClass()
//       $(this).addClass('is-checked')
//       $(this).find('i').addClass('fa')
//
//       if ($(this).hasClass('desc')) {
//         $(this).removeClass('desc').addClass('asc')
//         $(this).find('.fa').removeClass('fa-sort-amount-asc').addClass('fa-sort-amount-desc')
//         $container.isotope({
//           sortAscending: false
//         })
//       } else {
//         $(this).removeClass('asc').addClass('desc')
//         $(this).find('.fa').removeClass('fa-sort-amount-desc').addClass('fa-sort-amount-asc')
//         $container.isotope({
//           sortAscending: true
//         })
//       }
//     })
//   })
// })
