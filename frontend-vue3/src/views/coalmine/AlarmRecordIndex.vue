<template>
  <div>
    <n-card title="报警记录" :bordered="false" class="proCard">
      <n-space vertical :size="16">
        <!-- 统计卡片 -->
        <n-grid :x-gap="16" :y-gap="16" cols="4">
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="今日报警" :value="statistics.todayCount" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="未处理" :value="statistics.unprocessedCount" :value-style="{ color: '#d03050' }" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="已确认" :value="statistics.confirmedCount" :value-style="{ color: '#f0a020' }" />
            </n-card>
          </n-gi>
          <n-gi>
            <n-card class="stat-card">
              <n-statistic label="已解决" :value="statistics.resolvedCount" :value-style="{ color: '#18a058' }" />
            </n-card>
          </n-gi>
        </n-grid>

        <!-- 操作栏 -->
        <n-space>
          <n-button type="primary" @click="loadData">
            <template #icon>
              <n-icon><Refresh /></n-icon>
            </template>
            刷新
          </n-button>
        </n-space>

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
            v-model:value="searchParams.alarmLevel"
            placeholder="报警级别"
            clearable
            :options="alarmLevelOptions"
            style="width: 120px"
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

    <!-- 处理弹窗 -->
    <n-modal v-model:show="handleVisible" preset="card" title="处理报警" style="width: 500px">
      <n-form ref="formRef" :model="handleForm" label-placement="left">
        <n-form-item label="处理方式">
          <n-radio-group v-model:value="handleForm.handleType">
            <n-space>
              <n-radio value="confirm">确认</n-radio>
              <n-radio value="resolve">解决</n-radio>
              <n-radio value="ignore">忽略</n-radio>
            </n-space>
          </n-radio-group>
        </n-form-item>
        <n-form-item label="处理备注">
          <n-input v-model:value="handleForm.remark" type="textarea" />
        </n-form-item>
      </n-form>
      <template #footer>
        <n-space justify="end">
          <n-button @click="handleVisible = false">取消</n-button>
          <n-button type="primary" @click="submitHandle">确定</n-button>
        </n-space>
      </template>
    </n-modal>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue'
import { Refresh } from '@vicons/tabler'
import { getAlarmRecordPage, confirmAlarm, resolveAlarm, getTodayStatistics } from '@/api/coalmine/alarm'
import { getCoalMineList } from '@/api/coalmine/coal'
import { useMessage } from 'naive-ui'

const message = useMessage()

const loading = ref(false)
const dataList = ref([])
const handleVisible = ref(false)

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
  alarmLevel: null,
  status: null,
  startTime: '',
  endTime: ''
})

// 统计
const statistics = reactive({
  todayCount: 0,
  unprocessedCount: 0,
  confirmedCount: 0,
  resolvedCount: 0
})

// 处理表单
const handleForm = reactive({
  id: null,
  handleType: 'confirm',
  remark: ''
})

// 选项
const mineOptions = ref([])
const alarmLevelOptions = [
  { label: '一般', value: 1 },
  { label: '重要', value: 2 },
  { label: '紧急', value: 3 },
  { label: '严重', value: 4 }
]
const statusOptions = [
  { label: '未处理', value: 0 },
  { label: '已确认', value: 1 },
  { label: '已解决', value: 2 },
  { label: '已忽略', value: 3 }
]

// 表格列
const columns = [
  { title: 'ID', key: 'id', width: 60 },
  { title: '煤矿', key: 'mineName', width: 120 },
  { title: '测点', key: 'sensorName', width: 140 },
  { title: '报警类型', key: 'alarmType', width: 100 },
  { title: '级别', key: 'alarmLevel', width: 80, render: (row: any) => getAlarmLevelName(row.alarmLevel) },
  { title: '报警值', key: 'alarmValue', width: 100 },
  { title: '阈值', key: 'thresholdValue', width: 100 },
  { title: '报警时间', key: 'alarmTime', width: 160 },
  { title: '状态', key: 'status', width: 80, render: (row: any) => getStatusName(row.status) },
  {
    title: '操作',
    key: 'actions',
    width: 150,
    render: (row: any) => {
      const btns = []
      if (row.status === 0) {
        btns.push(h(NButton, { size: 'small', onClick: () => openHandle(row) }, () => '处理'))
      }
      btns.push(h(NButton, { size: 'small', onClick: () => viewDetail(row) }, () => '详情'))
      return btns
    }
  }
]

function getAlarmLevelName(level: number) {
  const map = { 1: '一般', 2: '重要', 3: '紧急', 4: '严重' }
  const colors = { 1: 'default', 2: 'warning', 3: 'error', 4: 'error' }
  return h(NTag, { type: colors[level] as any, size: 'small' }, () => map[level] || '未知')
}

function getStatusName(status: number) {
  const map = { 0: '未处理', 1: '已确认', 2: '已解决', 3: '已忽略' }
  const colors = { 0: 'error', 1: 'warning', 2: 'success', 3: 'default' }
  return h(NTag, { type: colors[status] as any, size: 'small' }, () => map[status] || '未知')
}

function getRowClassName(row: any) {
  if (row.status === 0 && row.alarmLevel >= 3) return 'row-danger'
  if (row.status === 0) return 'row-warning'
  return ''
}

async function loadData() {
  loading.value = true
  try {
    const res = await getAlarmRecordPage({
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

async function loadStatistics() {
  try {
    const res = await getTodayStatistics(searchParams.mineId)
    Object.assign(statistics, res.data || {})
  } catch (e) {
    console.error('加载统计失败', e)
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
  searchParams.alarmLevel = null
  searchParams.status = null
  searchParams.startTime = ''
  searchParams.endTime = ''
  loadData()
}

function openHandle(row: any) {
  handleForm.id = row.id
  handleForm.handleType = 'confirm'
  handleForm.remark = ''
  handleVisible.value = true
}

function viewDetail(row: any) {
  message.info('查看详情: ' + row.sensorName)
}

async function submitHandle() {
  try {
    if (handleForm.handleType === 'confirm' || handleForm.handleType === 'resolve') {
      const api = handleForm.handleType === 'confirm' ? confirmAlarm : resolveAlarm
      await api({ id: handleForm.id, remark: handleForm.remark })
      message.success('处理成功')
    }
    handleVisible.value = false
    loadData()
    loadStatistics()
  } catch (e: any) {
    message.error(e.message || '处理失败')
  }
}

onMounted(() => {
  // 设置默认时间
  const now = new Date()
  const today = now.toISOString().slice(0, 10)
  searchParams.startTime = today + ' 00:00:00'
  searchParams.endTime = now.toISOString().slice(0, 19).replace('T', ' ')
  
  loadMineOptions()
  loadData()
  loadStatistics()
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

.stat-card {
  text-align: center;
}

:deep(.row-warning) {
  background-color: #fffbe6;
}

:deep(.row-danger) {
  background-color: #fff1f0;
}
</style>
