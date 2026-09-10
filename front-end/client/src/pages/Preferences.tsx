import { Link } from 'react-router-dom'
"use client"
// import { Sidebar, SidebarContent, SidebarProvider, SidebarTrigger } from "@/components/ui/sidebar"
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion"
// import ChangePassword from '@/components/ui/preferenceforms/changeUserN';
import { Button } from "@/components/ui/button"
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"

import { Label } from "@/components/ui/label"
import ChangeUserName from '@/components/ui/preferenceforms/changeUserN';
import ChangePassword from '@/components/ui/preferenceforms/changePassword';
import ChangeEmail from '@/components/ui/preferenceforms/changeEmail';


import { zodResolver } from "@hookform/resolvers/zod"
import { useForm } from "react-hook-form"
import { z } from "zod"


import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form"
import { Input } from "@/components/ui/input"



  const formUserDetailsSchema = z.object({
    firstname: z.string().min(2, {
    message: "first name must be at least 2 characters.",
  }),
  lastname: z.string().min(2, {
    message: "last name must be at least 2 characters.",
  }),
  email: z.string().min(2, {
    message: "email must be at least 2 characters.",
  }),
})

// password

const passwordformSchema = z.object({
  password: z.string().min(8, {
    message: "password must be at least 8 characters.",
  }),
  rpassword: z.string().min(8, {
    message: "password must be at least 8 characters.",
  }),
})

export default function Preferences ()  {

    function onUserDetailsSubmit(values: z.infer<typeof formUserDetailsSchema>) {
      // Do something with the form values.
      // ✅ This will be type-safe and validated.
      console.log(values)
    }
    const userDetailsform = useForm<z.infer<typeof formUserDetailsSchema>>({
        resolver: zodResolver(formUserDetailsSchema),
        defaultValues: {
            // TODO : fill it with the information from the database
            firstname: "",
            lastname: "",
            email: "",
        },
      })


      // change password

      function passwordOnSubmit(values: z.infer<typeof passwordformSchema>) {
          // Do something with the form values.
          // ✅ This will be type-safe and validated.
          // TODO : submit to the database
          console.log(values)
        }
        const passwordform = useForm<z.infer<typeof passwordformSchema>>({
          resolver: zodResolver(passwordformSchema),
          defaultValues: {
            password: "",
            rpassword: "",
          },
        })

  return (
    <div className='  w-full flex-col gap-4'>
      <h1>Preference Page</h1>
     
    <Card className="w-full max-w-sm">
      <CardHeader>
        <CardTitle>Change User Details</CardTitle>
      </CardHeader>
      <CardContent>
         <Form {...userDetailsform}>
      <form onSubmit={userDetailsform.handleSubmit(onUserDetailsSubmit)} className="space-y-8">
        <FormField
          control={userDetailsform.control}
          name="firstname"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Username</FormLabel>
              <FormControl>
                <Input placeholder="firstname" {...field} />
              </FormControl>
              
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={userDetailsform.control}
          name="lastname"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Username</FormLabel>
              <FormControl>
                <Input placeholder="lastname" {...field} />
              </FormControl>
              
              <FormMessage />
            </FormItem>
          )}
        />
        <FormField
          control={userDetailsform.control}
          name="email"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Username</FormLabel>
              <FormControl>
                <Input placeholder="email" {...field} />
              </FormControl>
              
              <FormMessage />
            </FormItem>
          )}
        />
        <Button type="submit">Update Details</Button>
       
      </form>
    </Form>
      </CardContent>
      <CardFooter className="flex-col gap-2">
      </CardFooter>
    </Card>




<Card className="w-full max-w-sm">
      <CardHeader>
        <CardTitle>Change your password</CardTitle>
       
      </CardHeader>
      <CardContent>
         <Form {...passwordform}>
      <form onSubmit={passwordform.handleSubmit(passwordOnSubmit)} className="space-y-8">
        <FormField
          control={passwordform.control}
          name="password"
          render={({ field }) => (
            <FormItem>
              <FormLabel>password</FormLabel>
              <FormControl>
                <Input placeholder="password" {...field} />
              </FormControl>
              
              <FormMessage />
            </FormItem>
          )}
        />

         <FormField
          control={passwordform.control}
          name="rpassword"
          render={({ field }) => (
            <FormItem>
              <FormLabel>re-type password</FormLabel>
              <FormControl>
                <Input placeholder="retype password" {...field} />
              </FormControl>
              
              <FormMessage />
            </FormItem>
          )}
        />
            <Button type="submit">Change password</Button>
       
      </form>
    </Form>
      </CardContent>
      <CardFooter className="flex-col gap-2">
        <CardAction>
        </CardAction>
         
      </CardFooter>
    </Card>

    </div>
  )
}

