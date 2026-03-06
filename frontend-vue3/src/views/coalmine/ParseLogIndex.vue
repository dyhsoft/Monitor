<template>
  <div>
    <n-card title="历史文件查询" :bordered="false" class="proCard">
      <n-space vertical :size="16">
        <!-- 搜索栏 -->
        <n-space :size="12">
          <n-select
            v-model:value="searchParams.mineId"
            placeholder="选择煤矿"
            clearable
            :options="mineOptions"
            style="width: 200px"
          />
          <n-select
            v-model:value="searchParams.fileType"
            placeholder="文件类型"
            clearable
            :options="fileTypeOptions"
            style="width: 150px"
          />
          <n-select
            v-model:value="searchParams.status"
            placeholder="处理状态"
            clearable
            :options="statusOptions"
            style="width: 120px"
          />
          <n-date-picker
            v-model:formatted-value="searchParams.startTime"
            type="datetime"
            placeholder="开始时间"
            style="width: 200px"
          />
          <n-date-picker
            v-model:formatted-value="searchParams.endTime"
            type="datetime"
            placeholder="结束时间"
            style="width: 200px"
          />
          <n-button type="primary" @click="loadData">搜索</n-button>
          <n-button @click="handleReset">重置</n-button>
        </n-space>

        <!-- 数据表格 -->
        <n-data-table
          :columns="columns"
          :data="dataList"
          :loading="loading"
          :pagination="pagination"
          :row-key="(row) => row.id"
          :row-class-name="getRowClassName"
        />
      </n-space>
    </n-card>

    <!-- 详情弹窗 -->
    <n-modal v-model:show="detailVisible" preset="card" title="文件详情" style="width: 900px">
      <n-tabs type="line" animated>
        <!-- 基本信息 -->
        <n-tab-pane name="info" tab="基本信息">
          <n-descriptions :column="2" bordered label-placement="left">
            <n-descriptions-item label="文件名">{{ currentRecord.fileName }}</n-descriptions-item>
            <n-descriptions-item label="煤矿">{{ currentRecord.mineName }}</n-descriptions-item>
            <n-descriptions-item label="煤矿编号">{{ currentRecord.mineCode }}</n-descriptions-item>
            <n-descriptions-item label="文件类型">{{ currentRecord.fileTypeName }}</n-descriptions-item>
            <n-descriptions-item label="文件编码">{{ currentRecord.encoding }}</n-descriptions-item>
            <n-descriptions-item label="文件大小">{{ formatFileSize(currentRecord.fileSize) }}</n-descriptions-item>
            <n-descriptions-item label="解析记录数">{{ currentRecord.recordCount }}</n-descriptions-item>
            <n-descriptions-item label="成功数">{{ currentRecord.successCount }}</n-descriptions-item>
            <n-descriptions-item label="错误数">{{ currentRecord.errorCount }}</n-descriptions-item>
            <n-descriptions-item label="解析耗时">{{ currentRecord.parseTime }}ms</n-descriptions-item>
            <n-descriptions-item label="处理时间">{{ currentRecord.createTime }}</n-descriptions-item>
            <n-descriptions-item label="状态">
              <n-tag :type="currentRecord.status === 1 ? 'success' : 'error'" size="small">
                {{ currentRecord.status === 1 ? '成功' : '失败' }}
              </n-tag>
            </n-descriptions-item>
            <n-descriptions-item label="错误信息" v-if="currentRecord.errorMessage">
              <n-text type="error">{{ currentRecord.errorMessage }}</n-text>
            </n-descriptions-item>
          </n-descriptions>
        </n-tab-pane>

        <!-- 源文件内容 -->
        <n-tab-pane name="source" tab="源文件内容">
          <n-space vertical>
            <n-alert type="info" v-if="currentRecord.sourceContent">
              文件内容已保存，可用于追溯和重新解析
            </n-alert>
            <n-input
              v-model:value="currentRecord.sourceContent"
              type="textarea"
              :autosize="{ minRows: 10, maxRows: 20 }"
              readonly
              style="font-family: monospace;"
            />
          </n-space>
        </n-tab-pane>

        <!-- 协议验证 -->
        <n-tab-pane name="validate" tab="协议验证">
          <n-space vertical>
            <n-space>
              <n-button type="primary" @click="validateFile" :loading="validating">
                重新验证
              </n-button>
              <n-tag :type="validationPass ? 'success' : 'warning'">
                {{ validationPass ? '验证通过' : '存在问题' }}
              </n-tag>
            </n-space>
            
            <n-list hoverable>
              <n-list-item v-for="(item, index) in validationResults" :key="index">
                <n-thing>
                  <template #header>
                    <n-space>
                      <n-icon :color="item.isValid ? '#18a058' : '#d03050'">
                        <CheckCircle v-if="item.isValid" />
                        <XCircle v-else />
                      </n-icon>
                      <span>{{ item.item }}</span>
                    </n-space>
                  </template>
                  <template #description>
                    <n-text :type="item.isValid ? 'success' : 'error'">
                      {{ item.message }}
                    </n-text>
                  </template>
                </n-thing>
              </n-list-item>
              <n-empty v-if="validationResults.length === 0" description="暂无验证结果" />
            </n-list>
          </n-space>
        </n-tab-pane>
      </n-tabs>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, h } from 'vue'
import { CheckCircle, XCircle } from '@vicons/tabler'
import { getParseLogPage, getParseLog, validateParseLog } from '@/api/coalmine/parseLog'
import { getCoalMineList } from '@/api/coalmine/coal'
import { useMessage } from 'naive-ui'

const message = useMessage()

const loading = ref(false)
const validating = ref(false)
const dataList = ref([])
const detailVisible = ref(false)
const currentRecord = ref<any>({})
const validationResults = ref<any[]>([])
const validationPass = ref(true)

const pagination = reactive({
  page: 1,
  pageSize: 10,
  itemCount: 0,
  showSizePicker: true,
  pageSizes: [10, 20, 50],
  showQuickJumper: true
})

const searchParams = reactive({
  mineId: null,
  fileType: '',
  status: null,
  startTime: '',
  endTime: ''
})

// 选项
const mineOptions = ref([])
const fileTypeOptions = [
  { label: '安全监测(CDSS)', value: 'CDSS' },
  { label: '报警数据(CDDY)', value: 'CDDY' },
  { label: '人员定位(RYSS)', value: 'RYSS' },
  { label: '人员初始化(RYCS)', value: 'RYCS' },
  { label: '人员出勤(RYCY)', value: 'RYCY' },
  { label: '水害监测(CGKCDSS)', value: 'CGKCDSS' },
  { label: '矿压监测(KYCDSS)', value: 'KYCDSS' }
]
const statusOptions = [
  { label: '成功', value: 1 },
  { label: '失败', value: 2 }
]

// 表格列
const columns = [
  { title: 'ID', key: 'id', width: 60 },
  { title: '煤矿', key: 'mineName', width: 100 },
  { title: '煤矿编号', key: 'mineCode', width: 100 },
  { title: '文件类型', key: 'fileTypeName', width: 120 },
  { title: '编码', key: 'encoding', width: 80 },
  { title: '文件大小', key: 'fileSize', width: 100, render: (row: any) => formatFileSize(row.fileSize) },
  { title: '记录数', key: 'recordCount', width: 80 },
  { title: '成功', key: 'successCount', width: 60 },
  { title: '失败', key: 'errorCount', width: 60 },
  { title: '耗时', key: 'parseTime', width: 80, render: (row: any) => row.parseTime + 'ms' },
  { title: '处理时间', key: 'createTime', width: 160 },
  { title: '状态', key: 'status', width: 80, render: (row: any) => getStatusTag(row.status) },
  {
    title: '操作',
    key: 'actions',
    width: 100,
    render: (row: any) => {
      return h(NButton, { size: 'small', onClick: () => viewDetail(row) }, () => '查看')
    }
  }
]

function getStatusTag(status: number) {
  if (status === 1) {
    return h(NTag, { type: 'success', size: 'small' }, () => '成功')
  }
  return h(NTag, { type: 'error', size: 'small' }, () => '失败')
}

function formatFileSize(bytes: number) {
  if (!bytes) return '-'
  if (bytes < 1024) return bytes + ' B'
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(2) + ' KB'
  return (bytes / 1024 / 1024).toFixed(2) + ' MB'
}

function getRowClassName(row: any) {
  if (row.status === 2) return 'row-error'
  return ''
}

async function loadData() {
  loading.value = true
  try {
    const res = await getParseLogPage({
      current: pagination.page,
      size: pagination.pageSize,
      ...searchParams
    })
    dataList.value = res.data.rows || []
    pagination.itemCount = res.data.total || 0
  } finally {
    loading.value = false
  }
}

async function viewDetail(row: any) {
  const res = await getParseLog(row.id)
  currentRecord.value = res.data || {}
  validationResults.value = res.data.validationResults || []
  validationPass.value = validationResults.value.every((r: any) => r.isValid)
  detailVisible.value = true
}

async function validateFile() {
  validating.value = true
  try {
    const res = await validateParseLog(currentRecord.value.id)
    validationResults.value = res.data || []
    validationPass.value = validationResults.value.every((r: any) => r.isValid)
    message.success('验证完成')
  } finally {
    validating.value = false
  }
}

async function loadMineOptions() {
  try {
    const res = await getCoalMineList()
    mineOptions.value = (res.data || []).map((m: any) => ({ label: m.name, value: m.id }))
  } catch (e) {
    console.error('加载煤矿列表失败', e)
  }
}

function handleReset() {
  searchParams.mineId = null
  searchParams.fileType = ''
  searchParams.status = null
  searchParams.startTime = ''
  searchParams.endTime = ''
  loadData()
}

onMounted(() => {
  loadMineOptions()
  loadData()
})
</script>

<script lang="ts">
import { h } from 'vue'
import { NButton, NTag } from 'naive-ui'

export default {
  setup() {
    return {}
  }
}
</script>

<style scoped>
.proCard {
  background: #fff;
}

:deep(.row-error) {
  background-color: #fff1f0;
}
</style>
