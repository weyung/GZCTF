import { forwardRef } from 'react'
import { Group, Title, createStyles, GroupProps } from '@mantine/core'
import MainIcon from './icon/MainIcon'

const useStyles = createStyles((theme) => ({
  brand: {
    color:
      theme.colorScheme === 'dark'
        ? theme.colors[theme.primaryColor][4]
        : theme.colors[theme.primaryColor][6],
    display: 'inline-block',
    transform: 'translateY(-.12ch)',
  },
  title: {
    color: theme.colorScheme === 'dark' ? theme.colors.white[0] : theme.colors.gray[6],
    marginLeft: '-20px',
  },
}))

export const LogoHeader = forwardRef<HTMLDivElement, GroupProps>((props, ref) => {
  const { classes } = useStyles()
  return (
    <Group ref={ref} noWrap {...props}>
      <MainIcon style={{ maxWidth: 60, height: 'auto' }} />
      <Title className={classes.title}>
        GZ<span className={classes.brand}>::</span>CTF
      </Title>
    </Group>
  )
})

export default LogoHeader
