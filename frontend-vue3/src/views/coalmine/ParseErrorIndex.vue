<template>
  <div>
    <n-card title="解析错误查询" :bordered="false" class="proCard">
      <n-space vertical :size="16">
        <!-- 统计卡片 -->
        <n-grid :x-gap="16" :y-gap="16" cols="4">
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="今日错误数" :value="statistics.todayError" :value-style="{ color: '#d03050' }" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="本周错误数" :value="statistics.weekError" :value-style="{ color: '#f0a020' }" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="本月错误数" :value="statistics.monthError" :value-style="{ color: '#2080f0' }" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="总错误数" :value="statistics.totalError" />
            </n-card>
          </n-gi>
        </n-grid>

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

    <!-- 错误详情弹窗 -->
    <n-modal v-model:show="detailVisible" preset="card" title="解析错误详情" style="width: 900px">
      <n-tabs type="line" animated>
        <!-- 错误信息 -->
        <n-tab-pane name="error" tab="错误信息">
          <n-alert type="error" :title="currentRecord.errorMessage" style="margin-bottom: 16px">
            <template #header>错误原因</template>
          </n-alert>
          
          <n-descriptions :column="2" bordered label-placement="left">
            <n-descriptions-item label="文件名">{{ currentRecord.fileName }}</n-descriptions-item>
            <n-descriptions-item label="煤矿">{{ currentRecord.mineName }}</n-descriptions-item>
            <n-descriptions-item label="文件类型">{{ currentRecord.fileTypeName }}</n-descriptions-item>
            <n-descriptions-item label="处理时间">{{ currentRecord.createTime }}</n-descriptions-item>
            <n-descriptions-item label="煤矿编号">{{ currentRecord.mineCode }}</n-descriptions-item>
            <n-descriptions-item label="文件编码">{{ currentRecord.encoding }}</n-descriptions-item>
          </n-descriptions>

          <!-- 错误原因分析 -->
          <n-card title="错误原因分析" style="margin-top: 16px">
            <n-list>
              <n-list-item>
                <n-thing>
                  <template #header>
                    <n-space>
                      <n-icon color="#d03050"><Warning /></n-icon>
                      <span>解析失败</span>
                    </n-space>
                  </template>
                  <template #description>
                    <n-text type="error">
                      {{ analyzeErrorReason(currentRecord.errorMessage, currentRecord.fileType) }}
                    </n-text>
                  </template>
                </n-thing>
              </n-list-item>
            </n-list>
          </n-card>

          <!-- 常见错误 -->
          <n-card title="常见错误原因" style="margin-top: 16px">
            <n-list>
              <n-list-item v-for="(item, index) in commonErrors" :key="index">
                <n-thing :title="item.title" :description="item.desc" />
              </n-list-item>
            </n-list>
          </n-card>
        </n-tab-pane>

        <!-- 源文件内容 -->
        <n-tab-pane name="source" tab="源文件内容">
          <n-alert type="info" style="margin-bottom: 16px">
            源文件内容仅供参考，解析失败的文件可在错误目录中找到原始文件
          </n-alert>
          <n-input
            v-model:value="currentRecord.sourceContent"
            type="textarea"
            :autosize="{ minRows: 10, maxRows: 20 }"
            readonly
            style="font-family: monospace;"
          />
        </n-tab-pane>

        <!-- 协议验证 -->
        <n-tab-pane name="validate" tab="协议验证">
          <n-space vertical>
            <n-button type="primary" @click="validateFile" :loading="validating">
              验证文件是否符合协议
            </n-button>
            
            <n-list hoverable v-if="validationResults.length > 0">
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
            </n-list>
          </n-space>
        </n-tab-pane>
      </n-tabs>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, h } from 'vue'
import { Warning, CheckCircle, XCircle } from '@vicons/tabler'
import { getParseErrorPage, getParseLog, validateParseLog, deleteParseLog } from '@/api/coalmine/parseLog'
import { getCoalMineList } from '@/api/coalmine/coal'
import { useMessage } from 'naive-ui'

const message = useMessage()

const loading = ref(false)
const validating = ref(false)
const dataList = ref([])
const detailVisible = ref(false)
const currentRecord = ref<any>({})
const validationResults = ref<any[]>([])

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
  startTime: '',
  endTime: ''
})

// 统计
const statistics = reactive({
  todayError: 0,
  weekError: 0,
  monthError: 0,
  totalError: 0
})

// 常见错误列表
const commonErrors = [
  { title: '煤矿编号不存在', desc: '文件头中的煤矿编号未在系统中登记' },
  { title: '文件格式错误', desc: '文件格式不符合协议规范，如缺少字段定义行' },
  { title: '编码错误', desc: '文件编码不是支持的格式（UTF-8/GBK/GB2312）' },
  { title: '数据类型不支持', desc: '文件类型代码不在支持列表中' },
  { title: '数据解析失败', desc: '数据行字段数不足或格式错误' },
  { title: '数据库连接失败', desc: '写入数据库时发生错误' }
]

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

// 表格列
const columns = [
  { title: 'ID', key: 'id', width: 60 },
  { title: '煤矿', key: 'mineName', width: 100 },
  { title: '煤矿编号', key: 'mineCode', width: 100 },
  { title: '文件类型', key: 'fileTypeName', width: 120 },
  { title: '编码', key: 'encoding', width: 80 },
  { title: '文件大小', key: 'fileSize', width: 100, render: (row: any) => formatFileSize(row.fileSize) },
  { title: '处理时间', key: 'createTime', width: 160 },
  { title: '错误原因', key: 'errorMessage', ellipsis: { tooltip: true } },
  {
    title: '操作',
    key: 'actions',
    width: 150,
    render: (row: any) => {
      return h(NButtonGroup, null, () => [
        h(NButton, { size: 'small', onClick: () => viewDetail(row) }, () => '详情'),
        h(NButton, { size: 'small', onClick: () => reparseFile(row) }, () => '重新解析')
      ])
    }
  }
]

function formatFileSize(bytes: number) {
  if (!bytes) return '-'
  if (bytes < 1024) return bytes + ' B'
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(2) + ' KB'
  return (bytes / 1024 / 1024).toFixed(2) + ' MB'
}

function getRowClassName(row: any) {
  return 'row-error'
}

function analyzeErrorReason(errorMsg: string, fileType: string) {
  if (!errorMsg) return '未知错误'
  
  if (errorMsg.includes('煤矿编号不存在') || errorMsg.includes('MineCode')) {
    return '文件头中的煤矿编号未在系统中登记，请先在煤矿管理中添加该煤矿'
  }
  if (errorMsg.includes('格式') || errorMsg.includes('format')) {
    return '文件格式不符合协议规范，请检查文件头和数据行格式'
  }
  if (errorMsg.includes('编码') || errorMsg.includes('encoding')) {
    return '文件编码不支持，请保存为UTF-8或GBK编码'
  }
  if (errorMsg.includes('类型') || errorMsg.includes('type')) {
    return `不支持的文件类型: ${fileType}，请检查文件类型代码`
  }
  if (errorMsg.includes('数据库') || errorMsg.includes('database')) {
    return '数据库操作失败，请检查数据库连接和权限'
  }
  if (errorMsg.includes('空') || errorMsg.includes('empty')) {
    return '文件内容为空，请检查文件是否有数据'
  }
  
  return errorMsg
}

async function loadData() {
  loading.value = true
  try {
    const res = await getParseErrorPage({
      current: pagination.page,
      size: pagination.pageSize,
      ...searchParams
    })
    dataList.value = res.data.rows || []
    pagination.itemCount = res.data.total || 0
    
    // 统计
    statistics.totalError = res.data.total || 0
    statistics.todayError = Math.floor(Math.random() * 10) // 模拟数据
    statistics.weekError = Math.floor(Math.random() * 50)
    statistics.monthError = Math.floor(Math.random() * 200)
  } finally {
    loading.value = false
  }
}

async function viewDetail(row: any) {
  const res = await getParseLog(row.id)
  currentRecord.value = res.data || {}
  validationResults.value = []
  detailVisible.value = true
}

async function validateFile() {
  validating.value = true
  try {
    const res = await validateParseLog(currentRecord.value.id)
    validationResults.value = res.data || []
    message.success('验证完成')
  } finally {
    validating.value = false
  }
}

async function reparseFile(row: any) {
  message.info('重新解析功能开发中')
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
  searchParams.startTime = ''
  searchParams.endTime = ''
  loadData()
}

onMounted(() => {
  // 设置默认时间
  const now = new Date()
  const yesterday = new Date(now.getTime() - 24 * 60 * 60 * 1000)
  searchParams.endTime = now.toISOString().slice(0, 19).replace('T', ' ')
  searchParams.startTime = yesterday.toISOString().slice(0, 19).replace('T', ' ')
  
  loadMineOptions()
  loadData()
})
</script>

<script lang="ts">
import { h } from 'vue'
import { NButton, NButtonGroup, NTag } from 'naive-ui'

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

.stat-card {
  text-align: center;
}

:deep(.row-error) {
  background-color: #fff1f0;
}
</style>
