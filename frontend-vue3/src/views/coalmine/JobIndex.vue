<template>
  <div>
    <n-card title="定时任务管理" :bordered="false" class="proCard">
      <n-space vertical :size="16">
        <!-- 任务列表 -->
        <n-space>
          <n-button type="primary" @click="refreshJobs">
            <template #icon>
              <n-icon><Refresh /></n-icon>
            </template>
            刷新
          </n-button>
        </n-space>

        <n-grid :x-gap="16" :y-gap="16" cols="1 s:1 m:2 l:3" responsive="screen">
          <!-- 人员定位采集任务 -->
          <n-gi>
            <n-card title="人员定位数据采集" class="job-card">
              <n-space vertical :size="12">
                <n-descriptions :column="1" label-placement="left">
                  <n-descriptions-item label="任务名称">PersonLocationCollect</n-descriptions-item>
                  <n-descriptions-item label="采集间隔">60秒</n-descriptions-item>
                  <n-descriptions-item label="数据类型">RYSS (人员实时)</n-descriptions-item>
                  <n-descriptions-item label="状态">
                    <n-tag :type="personJobRunning ? 'success' : 'default'">
                      {{ personJobRunning ? '运行中' : '已停止' }}
                    </n-tag>
                  </n-descriptions-item>
                </n-descriptions>
                <n-space>
                  <n-button v-if="!personJobRunning" type="primary" @click="startPersonJob">
                    启动
                  </n-button>
                  <n-button v-else type="warning" @click="stopPersonJob">
                    停止
                  </n-button>
                  <n-button @click="executePersonJobOnce">
                    执行一次
                  </n-button>
                </n-space>
              </n-space>
            </n-card>
          </n-gi>

          <!-- 水害监测采集任务 -->
          <n-gi>
            <n-card title="水害监测数据采集" class="job-card">
              <n-space vertical :size="12">
                <n-descriptions :column="1" label-placement="left">
                  <n-descriptions-item label="任务名称">WaterMonitorCollect</n-descriptions-item>
                  <n-descriptions-item label="采集间隔">60秒</n-descriptions-item>
                  <n-descriptions-item label="数据类型">CGKCDSS/PSLCDSS</n-descriptions-item>
                  <n-descriptions-item label="状态">
                    <n-tag :type="waterJobRunning ? 'success' : 'default'">
                      {{ waterJobRunning ? '运行中' : '已停止' }}
                    </n-tag>
                  </n-descriptions-item>
                </n-descriptions>
                <n-space>
                  <n-button v-if="!waterJobRunning" type="primary" @click="startWaterJob">
                    启动
                  </n-button>
                  <n-button v-else type="warning" @click="stopWaterJob">
                    停止
                  </n-button>
                  <n-button @click="executeWaterJobOnce">
                    执行一次
                  </n-button>
                </n-space>
              </n-space>
            </n-card>
          </n-gi>

          <!-- 报警检测任务 -->
          <n-gi>
            <n-card title="报警检测任务" class="job-card">
              <n-space vertical :size="12">
                <n-descriptions :column="1" label-placement="left">
                  <n-descriptions-item label="任务名称">AlarmCheck</n-descriptions-item>
                  <n-descriptions-item label="检测间隔">30秒</n-descriptions-item>
                  <n-descriptions-item label="功能">安全监测报警检测</n-descriptions-item>
                  <n-descriptions-item label="状态">
                    <n-tag :type="alarmJobRunning ? 'success' : 'default'">
                      {{ alarmJobRunning ? '运行中' : '已停止' }}
                    </n-tag>
                  </n-descriptions-item>
                </n-descriptions>
                <n-space>
                  <n-button v-if="!alarmJobRunning" type="primary" @click="startAlarmJob">
                    启动
                  </n-button>
                  <n-button v-else type="warning" @click="stopAlarmJob">
                    停止
                  </n-button>
                  <n-button @click="executeAlarmJobOnce">
                    执行一次
                  </n-button>
                </n-space>
              </n-space>
            </n-card>
          </n-gi>

          <!-- 历史数据归档任务 -->
          <n-gi>
            <n-card title="历史数据归档任务" class="job-card">
              <n-space vertical :size="12">
                <n-descriptions :column="1" label-placement="left">
                  <n-descriptions-item label="任务名称">HistoryArchive</n-descriptions-item>
                  <n-descriptions-item label="执行策略">每小时执行</n-descriptions-item>
                  <n-descriptions-item label="功能">实时数据转历史归档</n-descriptions-item>
                  <n-descriptions-item label="状态">
                    <n-tag :type="archiveJobRunning ? 'success' : 'default'">
                      {{ archiveJobRunning ? '运行中' : '已停止' }}
                    </n-tag>
                  </n-descriptions-item>
                </n-descriptions>
                <n-space>
                  <n-button v-if="!archiveJobRunning" type="primary" @click="startArchiveJob">
                    启动
                  </n-button>
                  <n-button v-else type="warning" @click="stopArchiveJob">
                    停止
                  </n-button>
                  <n-button @click="executeArchiveJobOnce">
                    执行一次
                  </n-button>
                </n-space>
              </n-space>
            </n-card>
          </n-gi>
        </n-grid>

        <!-- 执行日志 -->
        <n-card title="执行日志" :bordered="false" class="proCard">
          <n-log :lines="logs" />
        </n-card>
      </n-space>
    </n-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { Refresh } from '@vicons/tabler'
import {
  startPersonLocationJob,
  stopPersonLocationJob,
  executePersonLocationJobOnce,
  startWaterMonitorJob,
  stopWaterMonitorJob,
  executeWaterMonitorJobOnce,
  startAlarmCheckJob,
  stopAlarmCheckJob,
  executeAlarmCheckJobOnce,
  startHistoryArchiveJob,
  stopHistoryArchiveJob,
  executeHistoryArchiveJobOnce,
  getJobList
} from '@/api/coalmine/job'

const personJobRunning = ref(false)
const waterJobRunning = ref(false)
const alarmJobRunning = ref(false)
const archiveJobRunning = ref(false)

const logs = ref<string[]>([])

function addLog(msg: string) {
  const time = new Date().toLocaleString()
  logs.value.unshift(`[${time}] ${msg}`)
  // 保留最近100条
  if (logs.value.length > 100) {
    logs.value.pop()
  }
}

async function refreshJobs() {
  try {
    const res = await getJobList()
    const workers = res.data || []
    personJobRunning.value = workers.some((w: any) => w.workerName === 'PersonLocationCollect')
    waterJobRunning.value = workers.some((w: any) => w.workerName === 'WaterMonitorCollect')
    alarmJobRunning.value = workers.some((w: any) => w.workerName === 'AlarmCheck')
    archiveJobRunning.value = workers.some((w: any) => w.workerName === 'HistoryArchive')
    addLog('刷新任务状态成功')
  } catch (e: any) {
    addLog(`刷新失败: ${e.message}`)
  }
}

async function startPersonJob() {
  try {
    await startPersonLocationJob({ interval: 60 })
    personJobRunning.value = true
    addLog('人员定位采集任务已启动')
  } catch (e: any) {
    addLog(`启动失败: ${e.message}`)
  }
}

async function stopPersonJob() {
  try {
    await stopPersonLocationJob({})
    personJobRunning.value = false
    addLog('人员定位采集任务已停止')
  } catch (e: any) {
    addLog(`停止失败: ${e.message}`)
  }
}

async function executePersonJobOnce() {
  try {
    const count = await executePersonLocationJobOnce()
    addLog(`人员定位采集执行完成，处理 ${count?.data || 0} 条记录`)
  } catch (e: any) {
    addLog(`执行失败: ${e.message}`)
  }
}

async function startWaterJob() {
  try {
    await startWaterMonitorJob({ interval: 60 })
    waterJobRunning.value = true
    addLog('水害监测采集任务已启动')
  } catch (e: any) {
    addLog(`启动失败: ${e.message}`)
  }
}

async function stopWaterJob() {
  try {
    await stopWaterMonitorJob({})
    waterJobRunning.value = false
    addLog('水害监测采集任务已停止')
  } catch (e: any) {
    addLog(`停止失败: ${e.message}`)
  }
}

async function executeWaterJobOnce() {
  try {
    const count = await executeWaterMonitorJobOnce()
    addLog(`水害监测采集执行完成，处理 ${count?.data || 0} 条记录`)
  } catch (e: any) {
    addLog(`执行失败: ${e.message}`)
  }
}

async function startAlarmJob() {
  try {
    await startAlarmCheckJob({ interval: 30 })
    alarmJobRunning.value = true
    addLog('报警检测任务已启动')
  } catch (e: any) {
    addLog(`启动失败: ${e.message}`)
  }
}

async function stopAlarmJob() {
  try {
    await stopAlarmCheckJob({})
    alarmJobRunning.value = false
    addLog('报警检测任务已停止')
  } catch (e: any) {
    addLog(`停止失败: ${e.message}`)
  }
}

async function executeAlarmJobOnce() {
  try {
    const count = await executeAlarmCheckJobOnce()
    addLog(`报警检测执行完成，生成 ${count?.data || 0} 条报警`)
  } catch (e: any) {
    addLog(`执行失败: ${e.message}`)
  }
}

async function startArchiveJob() {
  try {
    await startHistoryArchiveJob({})
    archiveJobRunning.value = true
    addLog('历史归档任务已启动')
  } catch (e: any) {
    addLog(`启动失败: ${e.message}`)
  }
}

async function stopArchiveJob() {
  try {
    await stopHistoryArchiveJob({})
    archiveJobRunning.value = false
    addLog('历史归档任务已停止')
  } catch (e: any) {
    addLog(`停止失败: ${e.message}`)
  }
}

async function executeArchiveJobOnce() {
  try {
    const count = await executeHistoryArchiveJobOnce()
    addLog(`历史归档执行完成，归档 ${count?.data || 0} 条记录`)
  } catch (e: any) {
    addLog(`执行失败: ${e.message}`)
  }
}

onMounted(() => {
  addLog('定时任务管理页面已加载')
  refreshJobs()
})
</script>

<style scoped>
.job-card {
  height: 100%;
}

.proCard {
  background: #fff;
}
</style>
