
<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import { useRoute } from 'vue-router'
import PostalMime, { type Email } from 'postal-mime'
import { toast } from 'vue-sonner'
import DOMPurify from 'dompurify'
import type { Address, Mailbox } from 'postal-mime'

const { t } = useI18n({ useScope: 'local' })
const route = useRoute()
const { downloadEmlFile } = useAliases()

const mailboxId = computed(() => route.params.mailbox as string)
const aliasId = computed(() => route.params.alias as string)
const messageId = computed(() => {
  const rawMessageParam = route.params.message as string
  const dateIdSegment = rawMessageParam
    .split('|')
    .find((segment) => segment.startsWith('da:'))

  return dateIdSegment ? dateIdSegment.slice(3) : rawMessageParam
})

const loading = ref(false)
const loadError = ref<string | null>(null)
const emlData = ref<Email | null>(null)
const rawEmlBlob = ref<Blob | null>(null)
const filename = ref<string | null>(null)
const contentType = ref<string | null>(null)

async function fetchMessage() {
  loading.value = true
  loadError.value = null
  emlData.value = null
  rawEmlBlob.value = null

  try {
    const result = await downloadEmlFile(
      'v1',
      mailboxId.value,
      aliasId.value,
      messageId.value
    )

    // result = { content: Blob, contentType: string, filename: string }
    rawEmlBlob.value = result.content
    filename.value = result.filename
    contentType.value = result.contentType
    emlData.value = await PostalMime.parse(result, {
      attachmentEncoding: 'base64',
      maxNestingDepth: 100,
      maxHeadersSize: 1024 * 1024
    })
  } catch (e: any) {
    loadError.value = e?.data?.detail || e?.statusMessage || t('submit.error')
    toast.error(loadError.value)
  } finally {
    loading.value = false
  }
  
}

function isMailbox(addr: Address): addr is Mailbox {
  return !('group' in addr) || addr.group === undefined
}

function formatAddress(addr?: Address | null) {
  if (!addr) return ''
  if (isMailbox(addr)) {
    return addr.name ? `${addr.name} <${addr.address}>` : addr.address
  }
  return addr.name || 'Group'
}

function formatAddressList(list?: Address[] | null) {
  return (list || []).map(formatAddress).filter(Boolean).join(', ')
}

const safeHtml = computed(() =>
  emlData.value?.html ? DOMPurify.sanitize(emlData.value.html) : ''
  
)

onMounted(fetchMessage)
watch([mailboxId, aliasId, messageId], fetchMessage)
</script>
<template>
  <div v-if="loading">Loading…</div>
  <div v-else-if="loadError">{{ loadError }}</div>
  <div v-else-if="emlData">
    <h2>{{ emlData.subject || '(no subject)' }}</h2>

    <p><strong>From:</strong> {{ formatAddress(emlData.from) }}</p>
    <p><strong>To:</strong> {{ formatAddressList(emlData.to) }}</p>
    <p v-if="emlData.cc?.length">
      <strong>Cc:</strong> {{ formatAddressList(emlData.cc) }}
    </p>
    <p><strong>Date:</strong> {{ emlData.date }}</p>

    <div v-if="emlData.html" v-html="safeHtml"></div>
    <pre v-else>{{ emlData.text }}</pre>

    <div v-if="emlData.attachments?.length">
      <h3>Attachments</h3>
      <ul>
        <li v-for="att in emlData.attachments" :key="att.filename || att.contentId">
          {{ att.filename || 'unnamed attachment' }} ({{ att.mimeType }})
        </li>
      </ul>
    </div>
  </div>
</template>

<i18n lang="yaml" scope="local">
en:
  page:
    title: "Message: {subject}"
  from: "From"
  to: "To"
  date: "Date"
  download: "Download EML"
  submit:
    error: "Could not load message."
</i18n>

