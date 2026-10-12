/**
 * Generates HTML for printing a receipt
 * Separated to avoid Vue SFC parser issues with HTML strings in script sections
 */
export function generatePrintReceiptHTML(receipt: string, saleId: number): string {
  return '<!DOCTYPE html>' +
    '<html>' +
    '<head>' +
    '<meta charset="utf-8">' +
    '<title>Ticket Venta #' + saleId + '</title>' +
    '<style>' +
    '@media print { @page { margin: 0; size: 80mm auto; } body { margin: 0; padding: 8px; } .no-print { display: none; } }' +
    'body { font-family: \'Courier New\', \'Consolas\', \'Monospace\', monospace; font-size: 11px; line-height: 1.3; white-space: pre-wrap; padding: 8px; max-width: 320px; margin: 0 auto; }' +
    '.print-btn { display: block; margin: 16px auto 0; padding: 12px 24px; background: #2563eb; color: white; border: none; border-radius: 6px; font-size: 14px; cursor: pointer; }' +
    '.print-btn:hover { background: #1d4ed8; }' +
    '</style>' +
    '</head>' +
    '<body onload="window.print()">' +
    receipt +
    '<button class="print-btn no-print" onclick="window.print()">Imprimir / Guardar PDF</button>' +
    '<script>setTimeout(() => window.close(), 10000)<\/script>' +
    '</body>' +
    '</html>'
}

/**
 * Prints receipt using iframe to avoid popup blockers
 */
export function printReceiptViaIframe(receipt: string, saleId: number): void {
  const html = generatePrintReceiptHTML(receipt, saleId)
  
  // Create iframe
  const iframe = document.createElement('iframe')
  iframe.style.position = 'fixed'
  iframe.style.left = '-9999px'
  iframe.style.top = '-9999px'
  iframe.style.width = '0'
  iframe.style.height = '0'
  iframe.style.border = 'none'
  document.body.appendChild(iframe)
  
  // Write content to iframe
  const doc = iframe.contentDocument || iframe.contentWindow?.document
  if (doc) {
    doc.open()
    doc.write(html)
    doc.close()
    
    // Trigger print when loaded
    iframe.onload = () => {
      try {
        iframe.contentWindow?.focus()
        iframe.contentWindow?.print()
      } catch (e) {
        console.warn('Print failed:', e)
      }
      // Cleanup after delay
      setTimeout(() => {
        document.body.removeChild(iframe)
      }, 1000)
    }
  }
}