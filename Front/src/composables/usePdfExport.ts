import { jsPDF } from 'jspdf'
import autoTable from 'jspdf-autotable'

// Type augmentation for jsPDF
declare module 'jspdf' {
  interface jsPDF {
    autoTable: (options: any) => jsPDF
  }
}

// Register autoTable plugin properly - bind the doc as first argument
;(jsPDF as any).API.autoTable = function(this: jsPDF, options: any) {
  return autoTable(this, options)
}

export interface PdfColumn {
  header: string
  dataKey: string
  width?: number
}

export interface PdfExportOptions {
  title: string
  subtitle?: string
  columns: PdfColumn[]
  data: Record<string, unknown>[]
  fileName?: string
  orientation?: 'portrait' | 'landscape'
  margins?: { top: number; right: number; bottom: number; left: number }
  styles?: {
    fontSize?: number
    cellPadding?: number
    headerColor?: [number, number, number]
    alternateRowColor?: [number, number, number]
  }
}

const DEFAULT_OPTIONS = {
  orientation: 'landscape' as const,
  margins: { top: 20, right: 10, bottom: 20, left: 10 },
  styles: {
    fontSize: 8,
    cellPadding: 2,
    headerColor: [41, 98, 255] as [number, number, number],
    alternateRowColor: [245, 247, 250] as [number, number, number],
  },
}

export function usePdfExport() {
  const exportToPdf = (options: PdfExportOptions) => {
    const opts = { ...DEFAULT_OPTIONS, ...options }
    const { title, subtitle, columns, data, fileName, orientation, margins, styles } = opts

    const doc = new jsPDF({
      orientation,
      unit: 'mm',
      format: 'a4',
    })

    // Title
    doc.setFontSize(16)
    doc.setTextColor(30, 30, 30)
    doc.text(title, margins.left, 12)

    // Subtitle
    let yPos = 18
    if (subtitle) {
      doc.setFontSize(10)
      doc.setTextColor(100, 100, 100)
      doc.text(subtitle, margins.left, yPos)
      yPos += 6
    }

    // Date
    doc.setFontSize(8)
    doc.setTextColor(130, 130, 130)
    doc.text(`Generado: ${new Date().toLocaleString('es-HN')}`, margins.left, yPos)
    yPos += 8

    // Table
    doc.autoTable({
      startY: yPos,
      head: [columns.map(c => c.header)],
      body: data.map(row => columns.map(c => row[c.dataKey] ?? '')),
      theme: 'striped',
      styles: {
        fontSize: styles?.fontSize ?? 8,
        cellPadding: styles?.cellPadding ?? 2,
        overflow: 'linebreak',
        halign: 'left',
      },
      headStyles: {
        fillColor: styles?.headerColor ?? [41, 98, 255],
        textColor: 255,
        fontStyle: 'bold',
        halign: 'center',
      },
      alternateRowStyles: {
        fillColor: styles?.alternateRowColor ?? [245, 247, 250],
      },
      columnStyles: Object.fromEntries(
        columns
          .map((c, i) => (c.width ? [i, { cellWidth: c.width }] : null))
          .filter(Boolean) as [number, { cellWidth: number }][]
      ),
      margin: margins,
      didDrawPage: (data: any) => {
        // Footer with page number
        const pageCount = doc.getNumberOfPages()
        doc.setFontSize(7)
        doc.setTextColor(150, 150, 150)
        doc.text(
          `Página ${data.pageNumber} de ${pageCount}`,
          doc.internal.pageSize.width - margins.right,
          doc.internal.pageSize.height - 5,
          { align: 'right' }
        )
      },
    })

    doc.save(fileName ?? `${title.toLowerCase().replace(/\s+/g, '-')}.pdf`)
  }

  return { exportToPdf }
}

// Helper to format currency for PDF
export function formatCurrencyPdf(amount: number): string {
  return new Intl.NumberFormat('es-HN', { style: 'currency', currency: 'HNL' }).format(amount)
}

// Helper to format date for PDF
export function formatDatePdf(dateStr: string): string {
  return new Intl.DateTimeFormat('es-HN', { day: '2-digit', month: 'short', year: 'numeric' }).format(new Date(dateStr))
}

// Helper to format date time for PDF
export function formatDateTimePdf(dateStr: string): string {
  return new Intl.DateTimeFormat('es-HN', { day: '2-digit', month: 'short', hour: '2-digit', minute: '2-digit' }).format(new Date(dateStr))
}